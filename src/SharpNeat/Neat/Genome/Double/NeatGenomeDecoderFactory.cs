// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat.Genome.Double;

/// <summary>
/// Static factory methods for creating new instances of <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.
/// </summary>
public static class NeatGenomeDecoderFactory
{
    /// <summary>
    /// Create a genome decoder that decodes a <see cref="NeatGenome{T}"/> to an appropriate neural network
    /// implementation.
    /// </summary>
    /// <param name="isAcyclic">Decode to an acyclic neural network.</param>
    /// <param name="enableHardwareAcceleration">Enable use of hardware accelerated black box (i.e neural network)
    /// implementations.</param>
    /// <returns>A new instance of <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.</returns>
    public static IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> CreateGenomeDecoder(
        bool isAcyclic,
        bool enableHardwareAcceleration = false)
    {
        IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> decoder;

        if (isAcyclic)
        {
            // Decode to an acyclic neural net.
            if(enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                decoder = new Vectorized.NeatGenomeDecoderAcyclic();
            else
                decoder = new NeatGenomeDecoderAcyclic();
        }
        else
        {
            // Decode to a cyclic neural net.
            if(enableHardwareAcceleration && Vector.IsHardwareAccelerated)
                decoder = new Vectorized.NeatGenomeDecoderCyclic();
            else
                decoder = new NeatGenomeDecoderCyclic();
        }

        return decoder;
    }
}
