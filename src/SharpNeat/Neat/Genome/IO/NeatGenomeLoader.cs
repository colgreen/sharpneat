/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome.IO
{
    /// <summary>
    /// For loading/deserializing instances of <see cref="NeatGenome{T}"/> from file, stream, etc.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class NeatGenomeLoader<T> where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly Func<string,(T,bool)> _tryParseWeight;
        readonly INeatGenomeBuilder<T> _genomeBuilder;

        readonly string _activationFnName;
        readonly List<DirectedConnection> _connList;
        readonly List<T> _weightList;
        readonly List<ActivationFunctionRow> _actFnList;

        StreamReader _sr;
        int _lineIdx;
        int _genomeId;

        #if DEBUG
        /// <summary>
        /// Indicates if a call to Load() is currently in progress. 
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new genome loader.
        /// </summary>
        /// <param name="metaNeatGenome">Neat genome meta data.</param>
        /// <param name="tryParseWeight">Connection weight parse function.</param>
        public NeatGenomeLoader(
            MetaNeatGenome<T> metaNeatGenome,
            Func<string,(T,bool)> tryParseWeight)
            : this(metaNeatGenome, tryParseWeight, 8)
        {}

        /// <summary>
        /// Construct a new genome loader.
        /// </summary>
        /// <param name="metaNeatGenome">Neat genome meta data.</param>
        /// <param name="tryParseWeight">Connection weight parse function.</param>
        /// <param name="connectionCapacity">The initial connection count to use for connection lists.</param>
        public NeatGenomeLoader(
            MetaNeatGenome<T> metaNeatGenome,
            Func<string,(T,bool)> tryParseWeight,
            int connectionCapacity)
        {
            _metaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
            _tryParseWeight = tryParseWeight ?? throw new ArgumentNullException(nameof(tryParseWeight));
            _genomeBuilder = NeatGenomeBuilderFactory<T>.Create(metaNeatGenome);

            _activationFnName = metaNeatGenome.ActivationFn.GetType().Name;
            _connList = new List<DirectedConnection>(connectionCapacity);
            _weightList = new List<T>(connectionCapacity);
            _actFnList = new List<ActivationFunctionRow>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a genome from the specified file.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        /// <returns>The loaded genome.</returns>
        public NeatGenome<T> Load(string path)
        {
            using(var sr = new StreamReader(path)) {
                return Load(sr);
            }
        }
        /// <summary>
        /// Load a genome from the given stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <returns>The loaded genome.</returns>
        /// <remarks>This method does not close the Stream.</remarks>
        public NeatGenome<T> Load(Stream stream)
        {
            if(stream is null) throw new ArgumentNullException(nameof(stream));

            using(var sr = new StreamReader(stream, Encoding.UTF8, true, 1024, true)) {
                return Load(sr);
            }
        }
        /// <summary>
        /// Load a genome from the given stream.
        /// </summary>
        /// <param name="sr"></param>
        /// <returns>The loaded genome.</returns>
        /// <remarks>This method does not close the StreamReader.</remarks>
        public NeatGenome<T> Load(StreamReader sr)
        {
            if(sr is null) throw new ArgumentNullException(nameof(sr));

            #if DEBUG
            // Check for attempts to re-enter this method.
            if(1 == Interlocked.CompareExchange(ref _reentranceFlag, 1, 0)) {
                throw new InvalidOperationException("Attempt to re-enter non-reentrant method.");
            }
            #endif

            try
            {
                _sr = sr;    
                return LoadInner();
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private NeatGenome<T> LoadInner()
        {
            // Reset line counter.
            _lineIdx = 0;

            // Read node counts.
            int inputCount = ReadInt32Line();
            int outputCount = ReadInt32Line();
            ReadEndOfSection();
            ValidateNodeCounts(inputCount, outputCount);

            // Read connections.
            ReadConnections(out DirectedConnection[] connArr, out T[] weightArr);

            // Read activation function(s).
            // Note. For a NeatGenome we expect a single line specifying a single activation function with ID 0,
            // and a function code that specifies the function. Additional lines in the section are ignored.
            ReadActivationFunctions();

            // Validate activation function(s).
            ValidateActivationFunctions();

            // Create a genome object and return.
            var connGenes = new ConnectionGenes<T>(connArr, weightArr);
            return _genomeBuilder.Create(_genomeId++, 0, connGenes);
        }

        private void ReadConnections(
            out DirectedConnection[] connArr,
            out T[] weightArr)
        {
            for(;;)
            {
                // Read a line.
                string line = ReadNextLine();

                // Stop reading connections if we detect the end of the section, or end of the file.
                if(string.IsNullOrEmpty(line)) {
                    break;
                }

                // Parse the connection fields.
                string[] fields = line.Split(' ', '\t');
                if(fields.Length != 3) {
                    throw new IOException($"Invalid connection. Line [{_lineIdx}].");
                }

                if(!TryParseInt32(fields[0], out int srcId)) {
                    throw new IOException($"Invalid connection source ID format. Line [{_lineIdx}].");
                }

                if(!TryParseInt32(fields[1], out int tgtId)) {
                    throw new IOException($"Invalid connection target ID format. Line [{_lineIdx}].");
                }

                if(!TryParseWeight(fields[2], out T weight)) {
                    throw new IOException($"Invalid connection weight format. Line [{_lineIdx}].");
                }

                // Further validation.
                // A target ID should never refer to an input node.
                if(tgtId < _metaNeatGenome.InputNodeCount) {
                    throw new IOException($"Invalid connection target ID. Refers to an input node. Line [{_lineIdx}].");
                }

                // Store the connection.
                _connList.Add(new DirectedConnection(srcId, tgtId));
                _weightList.Add(weight);
            }

            // Copy connection data into arrays, and sort into the correct order (for a NeatGenome).
            connArr = _connList.ToArray();
            weightArr = _weightList.ToArray();
            Array.Sort(connArr, weightArr);
        }

        private void ReadActivationFunctions()
        {
            for(int expectedFnId=0;; expectedFnId++)
            {
                // Read a line.
                string line = ReadNextLine();

                // Stop reading connections if we detect the end of the section, or end of the file.
                if(string.IsNullOrEmpty(line)) {
                    break;
                }

                string[] fields = line.Split(' ', '\t');
                if(fields.Length != 2) {
                    throw new IOException($"Invalid activation function line. Line [{_lineIdx}].");
                }

                if(!TryParseInt32(fields[0], out int fnId)) {
                    throw new IOException($"Invalid activation function ID format. Line [{_lineIdx}].");
                }

                // The function IDs are required to be in a continuous incrementing sequence, starting at zero.
                if(fnId != expectedFnId) {
                    throw new IOException($"Invalid activation function ID [{fnId}]; expected [{expectedFnId}]");
                }

                string fnCode = fields[1];
                _actFnList.Add(new ActivationFunctionRow(fnId, fnCode));
            }
        }

        private void Cleanup()
        {
            _connList.Clear();
            _weightList.Clear();
            _actFnList.Clear();

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion

        #region Private Methods [Validation]

        private void ValidateNodeCounts(
            int inputCount, int outputCount)
        {
            // Validate node counts.
            if(_metaNeatGenome.InputNodeCount != inputCount) {
                throw new IOException($"Incorrect input count. Line [{_lineIdx}].");
            }
            
            if(_metaNeatGenome.OutputNodeCount != outputCount) {
                throw new IOException($"Incorrect output count. Line [{_lineIdx}].");
            }
        }

        private void ValidateActivationFunctions()
        {
            // For NEAT each node uses the same activation function, and this is defined on MetaNeatGenome.
            if(0 == _actFnList.Count) {
                throw new IOException("No activation function defined for genome.");
            }

            // Test the code/name of the first defined activation function.
            string actFnCode = _actFnList[0].Code;
            if(string.CompareOrdinal(_activationFnName, actFnCode) != 0) {
                throw new IOException($"The activation function defined for the genome [{actFnCode}] does not match the expected activation function [{_activationFnName}]");
            }

            // Note. if more than on activation function is defined then the others are ignored.
        }

        #endregion

        #region Private Methods [Low Level]

        private int ReadInt32Line()
        {
            string line = ReadNonEmptyLine();
            if(!TryParseInt32(line, out int val)) {
                throw new IOException($"Invalid integer format. Line [{_lineIdx}].");
            }
            return val;
        }

        private void ReadEndOfSection()
        {
            string line = ReadNextLine();
            if(line != string.Empty) {
                throw new IOException($"End of section expected. Line [{_lineIdx}].");
            }
        }

        private string ReadNonEmptyLine()
        {
            string line = ReadNextLine();
            if(string.IsNullOrEmpty(line)) {
                throw new IOException($"Expected non-empty line. Line [{_lineIdx}].");
            }
            return line;
        }

        private string ReadNextLine()
        {
            for(;;)
            {
                // Read the next line.
                string line = _sr.ReadLine();
                _lineIdx++;

                // Return the line if it is empty (end of a section), null (end of file), 
                // or is not a comment line (starts with a hash character).
                if(string.IsNullOrEmpty(line) || line[0] != '#') {
                    return line;
                }
            }
        }

        private bool TryParseWeight(string str, out T weight)
        {
            (T, bool) result = _tryParseWeight(str);
            weight = result.Item1;
            return result.Item2;
        }

        #endregion

        #region Private Static Methods

        private static bool TryParseInt32(string str, out int val)
        {
            return int.TryParse(
                str,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out val);
        }

        #endregion
    }
}
