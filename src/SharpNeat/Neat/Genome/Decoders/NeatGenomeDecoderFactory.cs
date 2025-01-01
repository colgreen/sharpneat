// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat.Genome.Decoders;

/// <summary>
/// Static factory methods for creating new instances of <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.
/// </summary>
public static class NeatGenomeDecoderFactory
{
    /// <summary>
    /// Create a genome decoder that decodes a <see cref="NeatGenome{T}"/> to an appropriate neural network
    /// implementation.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="isAcyclic">Decode to an acyclic neural network.</param>
    /// <param name="enableHardwareAcceleration">Enable use of hardware accelerated black box (i.e neural network)
    /// implementations.</param>
    /// <returns>A new instance of <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.</returns>
    public static IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>> CreateGenomeDecoder<TScalar>(
        bool isAcyclic,
        bool enableHardwareAcceleration = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        if(isAcyclic)
        {
            // Decode to an acyclic neural net.
            if(enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                return new Vectorized.NeatGenomeDecoderAcyclic<TScalar>();
            else
                return new NeatGenomeDecoderAcyclic<TScalar>();
        }
        else
        {
            // Decode to a cyclic neural net.
            if(enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                return new Vectorized.NeatGenomeDecoderCyclic<TScalar>();
            else
                return new NeatGenomeDecoderCyclic<TScalar>();
        }
    }
}
