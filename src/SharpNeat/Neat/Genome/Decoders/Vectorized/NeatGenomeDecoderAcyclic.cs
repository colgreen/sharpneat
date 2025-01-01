// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Neat.Genome.Decoders.Vectorized;

/// <summary>
/// For decoding instances of <see cref="NeatGenome{T}"/> to <see cref="IBlackBox{T}"/>, specifically
/// acyclic neural network instances implemented by <see cref="NeuralNets.Vectorized.NeuralNetAcyclic{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class NeatGenomeDecoderAcyclic<TScalar> : IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Decodes a NEAT genome into a working neural network.
    /// </summary>
    /// <param name="genome">The genome to decode.</param>
    /// <returns>An <see cref="IBlackBox{T}"/>.</returns>
    public IBlackBox<TScalar> Decode(
        NeatGenome<TScalar> genome)
    {
        Debug.Assert(genome?.MetaNeatGenome?.IsAcyclic == true);
        Debug.Assert(genome?.ConnectionGenes is not null);
        Debug.Assert(genome.ConnectionGenes.Length == genome?.ConnectionIndexMap?.Length);
        Debug.Assert(genome.DirectedGraph is DirectedGraphAcyclic);

        // Get a neural net weight array.
        TScalar[] neuralNetWeightArr = genome.GetDigraphWeightArray();

        // Create a working neural net.
        return new NeuralNets.Vectorized.NeuralNetAcyclic<TScalar>(
                (DirectedGraphAcyclic)genome.DirectedGraph,
                neuralNetWeightArr,
                genome.MetaNeatGenome.ActivationFn.Fn);
    }
}
