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
using System.Diagnostics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Neat.Genome.Double.Vectorized
{
    /// <summary>
    /// For decoding instances of <see cref="NeatGenome{Double}"/> to <see cref="IBlackBox{Double}"/>, specifically
    /// cyclic neural network instances implemented by <see cref="NeuralNets.Double.Vectorized.NeuralNetCyclic"/>.
    /// </summary>
    public sealed class NeatGenomeDecoderCyclic : IGenomeDecoder<NeatGenome<double>,IBlackBox<double>>
    {
        readonly int _cyclesPerActivation;

        #region Constructor

        /// <summary>
        /// Construct with the given decode arguments.
        /// </summary>
        /// <param name="cyclesPerActivation">The number of cyclic neural net activation iterations per invocation of the neural net.</param>
        public NeatGenomeDecoderCyclic(int cyclesPerActivation)
        {
            _cyclesPerActivation = cyclesPerActivation;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decodes a NEAT genome into a working neural network.
        /// </summary>
        /// <param name="genome">The genome to decode.</param>
        /// <returns>An <see cref="IBlackBox{T}"/>.</returns>
        public IBlackBox<double> Decode(NeatGenome<double> genome)
        {
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but standard sharpneat behaviour is not to support this.
            Debug.Assert(!genome.MetaNeatGenome.IsAcyclic);

            return new NeuralNets.Double.Vectorized.NeuralNetCyclic(
                    genome.DirectedGraph,
                    genome.ConnectionGenes._weightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn,
                    _cyclesPerActivation);
        }

        #endregion
    }
}
