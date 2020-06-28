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
using System;
using SharpNeat.Evaluation;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;

namespace SharpNeat.Experiments
{
    /// <summary>
    /// An aggregation of settings objects that make up a given experiment.
    /// </summary>
    /// <typeparam name="T">Black box numeric data type.</typeparam>
    public class NeatExperiment<T> : INeatExperiment<T>
        where T : struct
    {
        #region Auto Properties

        /// <summary>
        /// Experiment name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Experiment description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Experiment evaluation scheme object.
        /// </summary>
        public IBlackBoxEvaluationScheme<T> EvaluationScheme { get; }

        /// <summary>
        /// A boolean flag that indicates if the genomes that are evolved are acyclic,
        /// i.e. they should have no recurrent/cyclic connection paths.
        /// </summary>
        public bool IsAcyclic { get; set; } = false;

        /// <summary>
        /// For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> is false) this defines how many timesteps to run the neural net per call to Activate().
        /// </summary>
        public int CyclesPerActivation { get; set; } = 1;

        /// <summary>
        /// Name of the neuron activation function to use in evolved networks.
        /// </summary>
        public string ActivationFnName { get; set; } 

        /// <summary>
        /// The <see cref="NeatEvolutionAlgorithmSettings"/> to be used for the experiment.
        /// </summary>
        public NeatEvolutionAlgorithmSettings NeatEvolutionAlgorithmSettings { get; }

        /// <summary>
        /// The asexual reproduction settings to use for the experiment.
        /// </summary>
        public NeatReproductionAsexualSettings ReproductionAsexualSettings { get; }

        /// <summary>
        /// The sexual reproduction settings to use for the experiment.
        /// </summary>
        public NeatReproductionSexualSettings ReproductionSexualSettings { get; }

        /// <summary>
        /// The population size to use for the experiment.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// The initial interconnections proportion. This is the proportion of possible
        /// direct connections from the input nodes to the output nodes that are to be created in 
        /// each initial genome. The connections to create are selected at random (using a 
        /// select-without-replacement method).
        /// </summary>
        public double InitialInterconnectionsProportion { get; set; }

        /// <summary>
        /// The maximum connection weight scale/magnitude. 
        /// E.g. a value of 5 defines a weight range of -5 to 5.
        /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
        /// </summary>
        public double ConnectionWeightScale { get; set; }

        /// <summary>
        /// The complexity regulation strategy to use for the experiment.
        /// </summary>
        public IComplexityRegulationStrategy ComplexityRegulationStrategy { get; set; }
        
        /// <summary>
        /// The number of CPU threads to distribute work to. 
        /// Set to -1 to use a thread count that matches the number of logical CPU cores (this is the default).
        /// </summary>
        public int DegreeOfParallelism { get; set; } = -1;

        /// <summary>
        /// Enable use of hardware accelerated neural network implementations, i.e. alternate implementations that use 
        /// CPU SIMD/vector instructions.
        /// </summary>
        /// <remarks>
        /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
        /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
        /// of all vectorized code to rule out that code as the source of a problem/bug.
        /// </remarks>
        public bool EnableHardwareAcceleratedNeuralNets { get; set; } = false;

        /// <summary>
        /// Enable use of hardware accelerated neural network activation functions, i.e. alternate implementations that use
        /// CPU SIMD/vector instructions.
        /// </summary>
        /// <remarks>
        /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
        /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
        /// of all vectorized code so as to rule out that code as the source of a problem/bug.
        /// 
        /// Furthermore, enabling hardware acceleration has been observed to often result in slower execution speed,
        /// probably because NEAT deals with non-homogeneous, irregular neural network structures that are generally not 
        /// conducive to the application of vectorized code.
        /// </remarks>
        public bool EnableHardwareAcceleratedActivationFunctions { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs with the provided name and evaluation scheme, and default settings.
        /// </summary>
        /// <param name="name">Experiment name.</param>
        /// <param name="evalScheme">Experiment evaluation scheme object.</param>
        public NeatExperiment(
            string name,
            IBlackBoxEvaluationScheme<T> evalScheme)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.EvaluationScheme = evalScheme ?? throw new ArgumentNullException(nameof(evalScheme));

            // Assign a set of default settings.
            this.NeatEvolutionAlgorithmSettings = new NeatEvolutionAlgorithmSettings();
            this.ReproductionAsexualSettings = new NeatReproductionAsexualSettings();
            this.ReproductionSexualSettings = new NeatReproductionSexualSettings();
            this.PopulationSize = 400;
            this.InitialInterconnectionsProportion = 0.05;
            this.ConnectionWeightScale = 5.0;

            // Assign a default complexity regulation strategy.
            this.ComplexityRegulationStrategy = new NullComplexityRegulationStrategy();
        }

        #endregion
    }
}
