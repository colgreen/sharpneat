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
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Neat.Genome.Double.Vectorized
{
    /// <summary>
    /// For decoding instances of <see cref="NeatGenome{Double}"/> to <see cref="IBlackBox{Double}"/>, specifically
    /// acyclic neural network instances implemented by <see cref="NeuralNets.Double.Vectorized.NeuralNetAcyclic"/>.
    /// </summary>
    public sealed class NeatGenomeDecoderAcyclic : IGenomeDecoder<NeatGenome<double>,IBlackBox<double>>
    {
        #region Public Methods

        /// <summary>
        /// Decodes a NEAT genome into a working neural network.
        /// </summary>
        /// <param name="genome">The genome to decode.</param>
        /// <returns>An <see cref="IBlackBox{T}"/>.</returns>
        public IBlackBox<double> Decode(
            NeatGenome<double> genome)
        {
            Debug.Assert(genome?.MetaNeatGenome?.IsAcyclic == true);
            Debug.Assert(genome?.ConnectionGenes is object);
            Debug.Assert(genome.ConnectionGenes.Length == genome?.ConnectionIndexMap?.Length);
            Debug.Assert(genome.DirectedGraph is DirectedGraphAcyclic);

            // Get a neural net weight array.
            double[] neuralNetWeightArr = genome.GetDigraphWeightArray();

            // Create a working neural net.
            return new NeuralNets.Double.Vectorized.NeuralNetAcyclic(
                    (DirectedGraphAcyclic)genome.DirectedGraph,
                    neuralNetWeightArr,
                    genome.MetaNeatGenome.ActivationFn.Fn);
        }

        #endregion
    }
}
