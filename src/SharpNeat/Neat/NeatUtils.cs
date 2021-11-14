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
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.Experiments;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Speciation;
using SharpNeat.NeuralNets;

namespace SharpNeat.Neat
{
    /// <summary>
    /// Utility methods for creating and correctly 'wiring up' instances of NeatEvolutionAlgorithm.
    /// </summary>
    public static class NeatUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Create a new instance of <see cref="NeatEvolutionAlgorithm{T}"/> for the given neat experiment, and neat population.
        /// </summary>
        /// <param name="neatExperiment">A neat experiment instance; this conveys everything required to create a new evolution algorithm instance that is ready to be run.</param>
        /// <param name="neatPop">A pre constructed/loaded neat population; this must be compatible with the provided neat experiment, otherwise an exception will be thrown.</param>
        /// <returns>A new instance of <see cref="NeatEvolutionAlgorithm{T}"/>.</returns>
        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm(
            INeatExperiment<double> neatExperiment,
            NeatPopulation<double> neatPop)
        {
            // Validate MetaNeatGenome and NeatExperiment are compatible; normally the former should have been created based on the latter, but this is not enforced.
            MetaNeatGenome<double> metaNeatGenome = neatPop.MetaNeatGenome;
            ValidateCompatible(neatExperiment, metaNeatGenome);

            // Create a genomeList evaluator based on the experiment's configuration settings.
            var genomeListEvaluator = CreateGenomeListEvaluator(neatExperiment);

            // Create a speciation strategy based on the experiment's configuration settings.
            var speciationStrategy = CreateSpeciationStrategy(neatExperiment);

            // Create an instance of the default connection weight mutation scheme.
            var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme(neatExperiment.ConnectionWeightScale);

            // Pull all of the parts together into an evolution algorithm instance.
            var ea = new NeatEvolutionAlgorithm<double>(
                neatExperiment.NeatEvolutionAlgorithmSettings,
                genomeListEvaluator,
                speciationStrategy,
                neatPop,
                neatExperiment.ComplexityRegulationStrategy,
                neatExperiment.ReproductionAsexualSettings,
                neatExperiment.ReproductionSexualSettings,
                weightMutationScheme);

            return ea;
        }

        /// <summary>
        /// Create a new instance of <see cref="NeatEvolutionAlgorithm{T}"/> for the given neat experiment.
        /// </summary>
        /// <param name="neatExperiment">A neat experiment instance; this conveys everything required to create a new evolution algorithm instance that is ready to be run.</param>
        /// <returns>A new instance of <see cref="NeatEvolutionAlgorithm{T}"/>.</returns>
        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm(
            INeatExperiment<double> neatExperiment)
        {
            // Create a genomeList evaluator based on the experiment's configuration settings.
            var genomeListEvaluator = CreateGenomeListEvaluator(neatExperiment);

            // Create a MetaNeatGenome.
            var metaNeatGenome = CreateMetaNeatGenome(neatExperiment);

            // Create an initial population of genomes.
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: neatExperiment.InitialInterconnectionsProportion,
                popSize: neatExperiment.PopulationSize);

            // Create a speciation strategy based on the experiment's configuration settings.
            var speciationStrategy = CreateSpeciationStrategy(neatExperiment);

            // Create an instance of the default connection weight mutation scheme.
            var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme(neatExperiment.ConnectionWeightScale);

            // Pull all of the parts together into an evolution algorithm instance.
            var ea = new NeatEvolutionAlgorithm<double>(
                neatExperiment.NeatEvolutionAlgorithmSettings,
                genomeListEvaluator,
                speciationStrategy,
                neatPop,
                neatExperiment.ComplexityRegulationStrategy,
                neatExperiment.ReproductionAsexualSettings,
                neatExperiment.ReproductionSexualSettings,
                weightMutationScheme);

            return ea;
        }

        /// <summary>
        /// Create a <see cref="MetaNeatGenome{T}"/> based on the parameters supplied by an <see cref="INeatExperiment{T}"/>.
        /// </summary>
        /// <param name="neatExperiment">The neat experiment.</param>
        /// <returns>A new instance of <see cref="MetaNeatGenome{T}"/>.</returns>
        public static MetaNeatGenome<double> CreateMetaNeatGenome(INeatExperiment<double> neatExperiment)
        {
            // Resolve the configured activation function name to an activation function instance.
            var actFnFactory = new DefaultActivationFunctionFactory<double>(neatExperiment.EnableHardwareAcceleratedActivationFunctions);
            var activationFn = actFnFactory.GetActivationFunction(neatExperiment.ActivationFnName);

            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: neatExperiment.EvaluationScheme.InputCount,
                outputNodeCount: neatExperiment.EvaluationScheme.OutputCount,
                isAcyclic: neatExperiment.IsAcyclic,
                activationFn: activationFn,
                connectionWeightScale: neatExperiment.ConnectionWeightScale);

            return metaNeatGenome;
        }

        #endregion

        #region Private Static Methods

        // TODO: Creation of an IGenomeListEvaluator needs to be the responsibility of INeatExperimentFactory (or the evaluation scheme),
        // to allow for tasks that require the entire population to be evaluated as a whole, e.g. simulated life/worlds.
        // Furthermore, a new interface IPhenomeListEvaluator will be needed to allow the code for those types of task to be abstracted away from the type of genome in use.
        private static IGenomeListEvaluator<NeatGenome<double>> CreateGenomeListEvaluator(
            INeatExperiment<double> neatExperiment)
        {
            // Create a genome decoder based on experiment config settings.
            var genomeDecoder = CreateGenomeDecoder(neatExperiment);

            // Resolve degreeOfParallelism (-1 is allowed in config, but must be resolved here to an actual degree).
            int degreeOfParallelismResolved = ResolveDegreeOfParallelism(neatExperiment);

            // Create a genomeList evaluator, and return.
            var genomeListEvaluator = GenomeListEvaluatorFactory.CreateEvaluator(
                genomeDecoder,
                neatExperiment.EvaluationScheme,
                degreeOfParallelismResolved);

            return genomeListEvaluator;
        }

        private static IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> CreateGenomeDecoder(
            INeatExperiment<double> neatExperiment)
        {
            if(neatExperiment.IsAcyclic)
            {
                return NeatGenomeDecoderFactory.CreateGenomeDecoderAcyclic(
                    neatExperiment.EnableHardwareAcceleratedNeuralNets);
            }
            return NeatGenomeDecoderFactory.CreateGenomeDecoderCyclic(
                neatExperiment.CyclesPerActivation,
                neatExperiment.EnableHardwareAcceleratedNeuralNets);
        }

        private static ISpeciationStrategy<NeatGenome<double>, double> CreateSpeciationStrategy(
            INeatExperiment<double> neatExperiment)
        {
            // Resolve a degreeOfParallelism (-1 is allowed in config, but must be resolved here to an actual degree).
            int degreeOfParallelismResolved = ResolveDegreeOfParallelism(neatExperiment);

            // Define a distance metric to use for k-means speciation; this is the default from sharpneat 2.x.
            IDistanceMetric<double> distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);

            // Use k-means speciation strategy; this is the default from sharpneat 2.x.
            // Create a serial (single threaded) strategy if degreeOfParallelism is one.
            if(degreeOfParallelismResolved == 1) {
                return new Speciation.GeneticKMeans.GeneticKMeansSpeciationStrategy<double>(distanceMetric, 5);
            }

            // Create a parallel (multi-threaded) strategy for degreeOfParallelism > 1.
            return new Speciation.GeneticKMeans.Parallelized.GeneticKMeansSpeciationStrategy<double>(distanceMetric, 5, degreeOfParallelismResolved);
        }

        #endregion

        #region Private Static Methods [Low Level Helper Methods]

        private static void ValidateCompatible(INeatExperiment<double> neatExperiment, MetaNeatGenome<double> metaNeatGenome)
        {
            // Confirm that neatExperiment and metaNeatGenome are compatible with each other.
            if(neatExperiment.EvaluationScheme.InputCount != metaNeatGenome.InputNodeCount) throw new ArgumentException("InputNodeCount does not match INeatExperiment.", nameof(metaNeatGenome));
            if(neatExperiment.EvaluationScheme.OutputCount != metaNeatGenome.OutputNodeCount) throw new ArgumentException("OutputNodeCount does not match INeatExperiment.", nameof(metaNeatGenome));
            if(neatExperiment.IsAcyclic != metaNeatGenome.IsAcyclic) throw new ArgumentException("IsAcyclic does not match INeatExperiment.", nameof(metaNeatGenome));
            if(neatExperiment.ConnectionWeightScale != metaNeatGenome.ConnectionWeightScale) throw new ArgumentException("ConnectionWeightScale does not match INeatExperiment.", nameof(metaNeatGenome));
            // Note. neatExperiment.ActivationFnName is not being checked against metaNeatGenome.ActivationFn, as the name information is not present on the ActivationFn object.
        }

        private static int ResolveDegreeOfParallelism(INeatExperiment<double> neatExperiment)
        {
            int degreeOfParallelism = neatExperiment.DegreeOfParallelism;

            // Resolve special value of -1 to the number of logical CPU cores.
            if(degreeOfParallelism == -1) {
                degreeOfParallelism = Environment.ProcessorCount;
            }
            else if(degreeOfParallelism < 1) {
                throw new ArgumentException(nameof(degreeOfParallelism));
            }

            return degreeOfParallelism;
        }

        #endregion
    }
}
