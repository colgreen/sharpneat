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
        /// <param name="degreeOfParallelism">The number of CPU threads to distribute work to; Pass in a value of -1 to use a thread count that matches the number of logical CPU cores.</param>
        /// <returns>A new instance of <see cref="IGenomeListEvaluator{TGenome}"/></returns>
        public static IGenomeListEvaluator<TGenome> CreateEvaluator<TGenome,TPhenome>(
            IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
            IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
            int degreeOfParallelism)
            where TGenome : IGenome
            where TPhenome : class
        {
            // Resolve special value of -1 to the number of logical CPU cores.
            if(degreeOfParallelism == -1) {
                degreeOfParallelism = Environment.ProcessorCount;
            }
            else if(degreeOfParallelism < 1) {
                throw new ArgumentException(nameof(degreeOfParallelism));
            }

            if(degreeOfParallelism == 1) {
                return new SerialGenomeListEvaluator<TGenome, TPhenome>(genomeDecoder, phenomeEvaluationScheme);
            }

            // Create a parallelised evaluator.
            if(phenomeEvaluationScheme.EvaluatorsHaveState) {
                return new ParallelGenomeListEvaluator<TGenome,TPhenome>(genomeDecoder, phenomeEvaluationScheme, degreeOfParallelism);
            }

            // else
            return new ParallelGenomeListEvaluatorStateless<TGenome,TPhenome>(genomeDecoder, phenomeEvaluationScheme, degreeOfParallelism);
        }
    }
}
