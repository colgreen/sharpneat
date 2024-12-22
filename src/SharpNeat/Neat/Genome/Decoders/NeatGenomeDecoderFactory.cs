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
        where TScalar : unmanaged
    {
        Type scalarType = typeof(TScalar);

        if (scalarType == typeof(double))
        {
            return (IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>>)CreateGenomeDecoderDouble(isAcyclic, enableHardwareAcceleration);
        }

        throw new InvalidOperationException($"Unsupported scalar type '{scalarType}'.");
    }

    private static IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> CreateGenomeDecoderDouble(
        bool isAcyclic,
        bool enableHardwareAcceleration = false)
    {
        if (isAcyclic)
        {
            // Decode to an acyclic neural net.
            if (enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                return new Double.Vectorized.NeatGenomeDecoderAcyclic();
            else
                return new Double.NeatGenomeDecoderAcyclic();
        }
        else
        {
            // Decode to a cyclic neural net.
            if (enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                return new Double.Vectorized.NeatGenomeDecoderCyclic();
            else
                return new Double.NeatGenomeDecoderCyclic();
        }
    }
}
