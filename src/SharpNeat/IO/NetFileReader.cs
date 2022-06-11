// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using SharpNeat.IO.Models;

namespace SharpNeat.IO;

/// <summary>
/// For reading of 'net' format files.
/// </summary>
internal class NetFileReader
{
    static readonly char[] __separatorChars = new char[] { ' ', '\t' };

    readonly StreamReader _sr;
    int _lineIdx;
    int _inputCount, _outputCount;

    #region Construction

    private NetFileReader(StreamReader sr)
    {
        _sr = sr ?? throw new ArgumentNullException(nameof(sr));
    }

    #endregion

    #region Private Methods

    private NetFileModel Read()
    {
        // Read node counts.
        ReadInputOutputCounts(out _inputCount, out _outputCount);

        // Read cyclic/acyclic indicator.
        ReadCyclicIndicator(out bool isAcyclic);

        // Read connections.
        ReadConnections(out List<ConnectionLine> connList);

        // Read activation function(s).
        // Note. Currently we expect a single line specifying a single activation function with ID 0,
        // and a function code that specifies the function. Additional lines in the section are ignored.
        ReadActivationFunctions(out List<ActivationFnLine> activationFnList);

        return new NetFileModel(
            _inputCount, _outputCount, isAcyclic,
            connList, activationFnList);
    }

    private void ReadInputOutputCounts(
        out int inputCount, out int outputCount)
    {
        string line = ReadNonEmptyLine();

        string[] fields = line.Split(__separatorChars, StringSplitOptions.RemoveEmptyEntries);
        if(fields.Length != 2)
            throw new FileFormatException($"Invalid input output count line. Line [{_lineIdx}].");

        if(!TryParseInt32(fields[0], out inputCount))
            throw new FileFormatException($"Invalid input count. Line [{_lineIdx}].");

        if(!TryParseInt32(fields[1], out outputCount))
            throw new FileFormatException($"Invalid output count. Line [{_lineIdx}].");

        // Validate node counts.
        // Note. Zero input nodes is allowed, but zero output nodes is nonsensical.
        if(inputCount < 0)
            throw new FileFormatException($"Invalid input count. Line [{_lineIdx}].");

        if(outputCount < 1)
            throw new FileFormatException($"Invalid output count. Line [{_lineIdx}].");

        ReadEndOfSection();
    }

    private void ReadCyclicIndicator(out bool isAcyclic)
    {
        string line = ReadNonEmptyLine();

        isAcyclic = line switch
        {
            "cyclic" => false,
            "acyclic" => true,
            _ => throw new FileFormatException($"Invalid cyclic/acyclic indicator [{line}].")
        };

        ReadEndOfSection();
    }

    private void ReadConnections(out List<ConnectionLine> connList)
    {
        connList = new();

        for(;;)
        {
            // Read a line.
            string? line = ReadNextLine();

            // Stop reading connections if we detect the end of the section, or end of the file.
            if(string.IsNullOrEmpty(line))
                break;

            // Parse the connection fields.
            string[] fields = line.Split(__separatorChars, StringSplitOptions.RemoveEmptyEntries);
            if(fields.Length != 3)
                throw new FileFormatException($"Invalid connection. Line [{_lineIdx}].");

            if(!TryParseInt32(fields[0], out int srcId))
                throw new FileFormatException($"Invalid connection source ID format. Line [{_lineIdx}].");

            if(!TryParseInt32(fields[1], out int tgtId))
                throw new FileFormatException($"Invalid connection target ID format. Line [{_lineIdx}].");

            if(!TryParseDouble(fields[2], out double weight))
                throw new FileFormatException($"Invalid connection weight format. Line [{_lineIdx}].");

            // Further validation.
            if(srcId < 0) throw new FileFormatException($"Invalid connection source ID; negative node ID. Line [{_lineIdx}].");
            if(tgtId < 0) throw new FileFormatException($"Invalid connection target ID; negative node ID. Line [{_lineIdx}].");

            // A target ID should never refer to an input node.
            if(tgtId < _inputCount)
                throw new IOException($"Invalid connection target ID; target cannot be an input node. Line [{_lineIdx}].");

            // Store the connection.
            connList.Add(new ConnectionLine(srcId, tgtId, weight));
        }
    }

    private void ReadActivationFunctions(out List<ActivationFnLine> activationFnList)
    {
        activationFnList = new();

        // Read activation function lines, until an empty line is reached.
        for(int expectedFnId = 0; ; expectedFnId++)
        {
            // Read a line.
            string? line = ReadNextLine();

            // Stop reading connections if we detect the end of the section, or end of the file.
            if(string.IsNullOrEmpty(line))
                break;

            string[] fields = line.Split(__separatorChars, StringSplitOptions.RemoveEmptyEntries);
            if(fields.Length != 2)
                throw new FileFormatException($"Invalid activation function line. Line [{_lineIdx}].");

            if(!TryParseInt32(fields[0], out int fnId))
                throw new FileFormatException($"Invalid activation function ID format. Line [{_lineIdx}].");

            // The function IDs are required to be in a continuous incrementing sequence, starting at zero.
            if(fnId != expectedFnId)
                throw new FileFormatException($"Invalid activation function ID [{fnId}]; expected [{expectedFnId}]");

            string fnCode = fields[1];
            activationFnList.Add(new ActivationFnLine(fnId, fnCode));
        }

        // Additional validation of the activation functions.
        // Note. For NEAT, each node uses the same activation function, therefore there must be at least one
        // activation function defined.
        if(activationFnList.Count == 0)
            throw new FileFormatException("No activation function defined for genome.");

        // Note. if more than one activation function is defined then all but the first are currently ignored.
    }

    #endregion

    #region Private Methods [Low Level Line Reading]

    private string? ReadNextLine()
    {
        for(;;)
        {
            // Read the next line.
            string? line = _sr!.ReadLine();
            _lineIdx++;

            // Return the line if it is empty (end of a section), null (end of file),
            // or is not a comment line (starts with a hash character).
            if(string.IsNullOrEmpty(line) || line[0] != '#')
                return line;
        }
    }

    private string ReadNonEmptyLine()
    {
        string? line = ReadNextLine();
        if(string.IsNullOrEmpty(line))
            throw new FileFormatException($"Expected non-empty line. Line [{_lineIdx}].");

        return line;
    }

    private void ReadEndOfSection()
    {
        string? line = ReadNextLine();
        if(!string.IsNullOrEmpty(line))
            throw new FileFormatException($"End of section expected. Line [{_lineIdx}].");
    }

    #endregion

    #region Private Static Methods

    private static bool TryParseInt32(string s, out int result)
    {
        return int.TryParse(
            s,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out result);
    }

    private static bool TryParseDouble(string s, out double result)
    {
        return double.TryParse(
            s,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture,
            out result);
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Read a 'net' format file from a stream reader.
    /// </summary>
    /// <param name="sr">The stream reader to read from.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel Read(StreamReader sr)
    {
        var loader = new NetFileReader(sr);
        return loader.Read();
    }

    #endregion
}
