// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Neat.Genome.Decoders.Vectorized;

/// <summary>
/// For decoding instances of <see cref="NeatGenome{Double}"/> to <see cref="IBlackBox{Double}"/>, specifically
/// cyclic neural network instances implemented by <see cref="NeuralNets.Vectorized.NeuralNetCyclic{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class NeatGenomeDecoderCyclic<TScalar> : IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Decodes a NEAT genome into a working neural network.
    /// </summary>
    /// <param name="genome">The genome to decode.</param>
    /// <returns>An <see cref="IBlackBox{T}"/>.</returns>
    public IBlackBox<TScalar> Decode(NeatGenome<TScalar> genome)
    {
        // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but standard sharpneat behaviour is not to support this.
        Debug.Assert(!genome.MetaNeatGenome.IsAcyclic);

        return new NeuralNets.Vectorized.NeuralNetCyclic<TScalar>(
                genome.DirectedGraph,
                genome.ConnectionGenes._weightArr,
                genome.MetaNeatGenome.ActivationFn.Fn,
                genome.MetaNeatGenome.CyclesPerActivation);
    }
}
