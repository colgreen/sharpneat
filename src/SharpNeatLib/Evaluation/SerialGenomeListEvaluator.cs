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
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// An implementation of <see cref="IGenomeListEvaluator{TGenome}"/> that evaluates genomes in series on a single CPU thread.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
    /// <remarks>
    /// Single threaded evaluation can be useful in various scenarios e.g. when debugging.
    /// 
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// </remarks>
    public class SerialGenomeListEvaluator<TGenome,TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : IGenome
        where TPhenome : class
    {
        #region Instance Fields

        readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

        #endregion

        #region Constructor


        /// <summary>
        /// Construct with the provided <see cref="IGenomeDecoder{TGenome,TPhenome}"/> and <see cref="IPhenomeEvaluator{TPhenome}"/>.
        /// Phenome caching is enabled by default.
        /// </summary>
        public SerialGenomeListEvaluator(
            IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
            IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluationScheme = phenomeEvaluationScheme;

            // Note. SerialGenomeListEvaluator will only evaluate on one thread therefore only ever requires a single evaluator.
            _phenomeEvaluator = phenomeEvaluationScheme.CreateEvaluator();
        }

        #endregion

        #region IGenomeListEvaluator

        /// <summary>
        /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
        /// </summary>
        /// <remarks>
        /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation 
        /// for the same genome, such as scheme is non-deterministic.
        /// </remarks>
        public bool IsDeterministic => _phenomeEvaluationScheme.IsDeterministic;

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitenssInfo between any two.
        /// </remarks>
        public IComparer<FitnessInfo> FitnessComparer => _phenomeEvaluationScheme.FitnessComparer;

        /// <summary>
        /// Evaluates a collection of genomes and assigns fitness info to each.
        /// </summary>
        public void Evaluate(ICollection<TGenome> genomeList)
        {
            // Decode and evaluate each genome in turn.
            foreach(TGenome genome in genomeList)
            {
                // TODO: Implement phenome caching (to avoid decode cost when re-evaluating with a non-deterministic evaluation scheme).
                TPhenome phenome = _genomeDecoder.Decode(genome);
                if(null == phenome)
                {   // Non-viable genome.
                    genome.FitnessInfo = _phenomeEvaluationScheme.NullFitness;
                }
                else 
                {   
                    genome.FitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                }
            }
        }

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return _phenomeEvaluationScheme.TestForStopCondition(fitnessInfo);
        }

        #endregion
    }
}
