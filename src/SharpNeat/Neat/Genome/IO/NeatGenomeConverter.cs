// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using SharpNeat.Graphs;
using SharpNeat.IO.Models;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// Static methods for converting between <see cref="NeatGenome{T}"/> and <see cref="NetFileModel"/>.
/// </summary>
public static class NeatGenomeConverter
{
    /// <summary>
    /// Convert a <see cref="NeatGenome{T}"/> into a <see cref="NetFileModel"/> instance, suitable for saving to
    /// file.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    /// <param name="genome">The genome to convert.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel ToNetFileModel<T>(
        NeatGenome<T> genome)
        where T : struct
    {
        // Convert input and output counts, and cyclic/acyclic indicator.
        int inputCount = genome.MetaNeatGenome.InputNodeCount;
        int outputCount = genome.MetaNeatGenome.OutputNodeCount;
        bool isAcyclic = genome.MetaNeatGenome.IsAcyclic;

        // Convert connections.
        ConnectionGenes<T> connGenes = genome.ConnectionGenes;
        DirectedConnection[] connArr = connGenes._connArr;
        T[] weightArr = connGenes._weightArr;

        List<ConnectionLine> connList = new(connGenes.Length);

        for(int i=0; i < connGenes.Length; i++)
        {
            DirectedConnection conn = connArr[i];

            // Note. The neat genome may use 'double' or 'float' typed weights; whereas NetFileModel is uses
            // 'double' only, so we some conversion is required here.
            double weight = Convert.ToDouble(weightArr[i], CultureInfo.InvariantCulture);
            ConnectionLine connLine = new(conn.SourceId, conn.TargetId, weight);
            connList.Add(connLine);
        }

        // Convert activation function(s).
        // Note. By convention we use the activation function type short name as function code (e.g. "ReLU",
        // or "Logistic").
        ActivationFnLine actFnLine = new(0, genome.MetaNeatGenome.ActivationFn.GetType().Name);
        List<ActivationFnLine> actFnLines = new()
        {
            actFnLine
        };

        return new NetFileModel(
            inputCount, outputCount, isAcyclic,
            connList, actFnLines);
    }
}
