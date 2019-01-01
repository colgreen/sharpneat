/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Diagnostics;
using System.Numerics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.NeuralNet.Double;

namespace SharpNeat.Neat.Genome.Double
{
    public class NeatGenomeDecoder : IGenomeDecoder<NeatGenome<double>,IBlackBox<double>>
    {
        readonly int _activationCount;
        readonly bool _boundedOutput;

        #region Constructor

        /// <summary>
        /// Construct with the given decoding parameters.
        /// </summary>
        /// <param name="boundedOutput">Indicates whether the output values at the output nodes should be bounded to the interval [0,1]</param>
        /// <param name="activationCount">The number of cyclic neural net activation iterations per invocation of the neural net.</param>
        public NeatGenomeDecoder(
            int activationCount,
            bool boundedOutput)
        {
            _activationCount = activationCount;
            _boundedOutput = boundedOutput;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode a genome into a working neural network.
        /// </summary>
        /// <param name="genome">The genome to decode.</param>
        public IBlackBox<double> Decode(
            NeatGenome<double> genome)
        {
            // Note. In principle an acyclic net can be decoded to a cyclic network (but not the other way around), but standard sharpneat behaviour is not to support this.
            Debug.Assert(!genome.MetaNeatGenome.IsAcyclic);

            // Create a working neural net.
            IBlackBox<double> neuralNet;
            if(Vector.IsHardwareAccelerated)
            {
                neuralNet = new NeuralNet.Double.Vectorized.CyclicNeuralNet(
                    genome.DirectedGraph,
                    genome.ConnectionGenes._weightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn,
                    _activationCount, _boundedOutput);
            }
            else
            {
                neuralNet = new CyclicNeuralNet(
                    genome.DirectedGraph,
                    genome.ConnectionGenes._weightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn,
                    _activationCount, _boundedOutput);
            }

            return neuralNet;
        }

        #endregion
    }
}
