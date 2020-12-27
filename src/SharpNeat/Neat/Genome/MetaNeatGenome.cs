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
using SharpNeat.NeuralNets;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// NeatGenome metadata.
    /// Genome related values/settings that are consistent across all genomes for the lifetime of an evolutionary algorithm run.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class MetaNeatGenome<T> where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Input node count.
        /// </summary>
        public int InputNodeCount { get; }

        /// <summary>
        /// Output node count.
        /// </summary>
        public int OutputNodeCount { get; }

        /// <summary>
        /// Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic connection paths.
        /// </summary>
        public bool IsAcyclic { get; }

        /// <summary>
        /// The neuron activation function to use in evolved networks. NEAT uses the same activation
        /// function at each node.
        /// </summary>
        public IActivationFunction<T> ActivationFn { get; }

        /// <summary>
        /// Maximum connection weight scale/magnitude.
        /// E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightScale { get; } = 5.0;

        #endregion

        #region Properties

        /// <summary>
        /// The total number of input and output nodes.
        /// </summary>
        public int InputOutputNodeCount {
            get { return this.InputNodeCount + this.OutputNodeCount; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="inputNodeCount">Input node count.</param>
        /// <param name="outputNodeCount">Output node count.</param>
        /// <param name="isAcyclic">Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic connection paths.</param>
        /// <param name="activationFn">The neuron activation function to use in evolved networks. NEAT uses the same activation function at each node.</param>
        /// <param name="connectionWeightScale">Maximum connection weight scale/magnitude.</param>
        public MetaNeatGenome(
            int inputNodeCount, int outputNodeCount, bool isAcyclic,
            IActivationFunction<T> activationFn,
            double connectionWeightScale)
        {
            this.InputNodeCount = inputNodeCount;
            this.OutputNodeCount = outputNodeCount;
            this.IsAcyclic = isAcyclic;
            this.ActivationFn = activationFn;
            this.ConnectionWeightScale = connectionWeightScale;
        }

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="inputNodeCount">Input node count.</param>
        /// <param name="outputNodeCount">Output node count.</param>
        /// <param name="isAcyclic">Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic connection paths.</param>
        /// <param name="activationFn">The neuron activation function to use in evolved networks. NEAT uses the same activation function at each node.</param>
        public MetaNeatGenome(
            int inputNodeCount, int outputNodeCount, bool isAcyclic,
            IActivationFunction<T> activationFn)
        {
            this.InputNodeCount = inputNodeCount;
            this.OutputNodeCount = outputNodeCount;
            this.IsAcyclic = isAcyclic;
            this.ActivationFn = activationFn;
        }

        #endregion
    }
}
