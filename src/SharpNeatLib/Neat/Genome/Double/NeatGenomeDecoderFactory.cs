using System.Numerics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Neat.Genome.Double
{
    /// <summary>
    /// Static factory methods for creating new instances of <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.
    /// </summary>
    public static class NeatGenomeDecoderFactory
    {
        /// <summary>
        /// Create a genome decoder that decodes to a cyclic neural network implementation.
        /// </summary>
        /// <param name="cyclesPerActivation">The number of cyclic neural net activation iterations per invocation of the neural net.</param>
        /// <param name="suppressHardwareAcceleration">Suppress use of hardware accelerated black box (i.e neural network) implementations.</param>
        /// <returns>A new instance of <see cref="IGenomeDecoder{TGenome, TPhenome}"/></returns>
        public static IGenomeDecoder<NeatGenome<double>,IBlackBox<double>> CreateGenomeDecoderCyclic(
            int cyclesPerActivation,
            bool suppressHardwareAcceleration = false)
        {
            if(!suppressHardwareAcceleration && Vector.IsHardwareAccelerated)
            {
                return new Vectorized.NeatGenomeDecoderCyclic(cyclesPerActivation);    
            }
            // else
            return new NeatGenomeDecoderCyclic(cyclesPerActivation);
        }

        /// <summary>
        /// Create a genome decoder that decodes to an  acyclic neural network implementation.
        /// </summary>
        /// <param name="boundedOutput">Indicates whether the output values at the output nodes should be bounded to the interval [0,1]</param>
        /// <param name="suppressHardwareAcceleration">Suppress use of hardware accelerated black box (i.e neural network) implementations.</param>
        /// <returns>A new instance of <see cref="IGenomeDecoder{TGenome, TPhenome}"/></returns>
        public static IGenomeDecoder<NeatGenome<double>,IBlackBox<double>> CreateGenomeDecoderAcyclic(
            bool boundedOutput,
            bool suppressHardwareAcceleration = false)
        {
            if(!suppressHardwareAcceleration && Vector.IsHardwareAccelerated)
            {
                return new Vectorized.NeatGenomeDecoderAcyclic();    
            }
            // else
            return new NeatGenomeDecoderAcyclic();
        }
    }
}
