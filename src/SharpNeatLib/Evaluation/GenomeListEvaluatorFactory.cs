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
using System;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// Static factory class for creating instances of <see cref="IGenomeListEvaluator{TGenome}"/>.
    /// </summary>
    public static class GenomeListEvaluatorFactory
    {
        /// <summary>
        /// Create a new genome list evaluator.
        /// </summary>
        /// <typeparam name="TGenome">Genome type.</typeparam>
        /// <typeparam name="TPhenome">Phenome type.</typeparam>
        /// <param name="genomeDecoder">Genome decoder, for decoding a genome to a phenome.</param>
        /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
        /// <param name="createConcurrentEvaluator">If true a evaluator that runs on multiple threads will be created.</param>
        /// <returns>A new instance of <see cref="IGenomeListEvaluator{TGenome}"/></returns>
        public static IGenomeListEvaluator<TGenome> CreateEvaluator<TGenome,TPhenome>(
            IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
            IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
            bool createConcurrentEvaluator)
            where TGenome : IGenome
            where TPhenome : class
        {
            if(!createConcurrentEvaluator) {
                return new SerialGenomeListEvaluator<TGenome, TPhenome>(genomeDecoder, phenomeEvaluationScheme);
            }

            // Create a parallelised evaluator.
            if(phenomeEvaluationScheme.EvaluatorsHaveState)
            {
                // TODO: ParallelGenomeListEvaluator with evaluator pool.
                throw new NotImplementedException();
            }

            // else
            return new ParallelGenomeListEvaluatorStateless<TGenome,TPhenome>(genomeDecoder, phenomeEvaluationScheme);
        }
    }
}
