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
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Sexual.Strategy;
using SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Creation of offspring given two parents (sexual reproduction).
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public class NeatReproductionSexual<T> : ISexualReproductionStrategy<T>
        where T : struct
    {
        readonly NeatReproductionSexualSettings _settings;
        readonly ISexualReproductionStrategy<T> _strategy;

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="metaNeatGenome">NeatGenome metadata.</param>
        /// <param name="genomeBuilder">NeatGenome builder.</param>
        /// <param name="genomeIdSeq">Genome ID sequence; for obtaining new genome IDs.</param>
        /// <param name="generationSeq">Generation sequence; for obtaining the current generation number.</param>
        /// <param name="settings">Sexual reproduction settings.</param>
        public NeatReproductionSexual(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            Int32Sequence genomeIdSeq,
            Int32Sequence generationSeq,
            NeatReproductionSexualSettings settings)
        {
            _settings = settings;
            _strategy = new UniformCrossoverReproductionStrategy<T>(
                                metaNeatGenome.IsAcyclic,
                                settings.SecondaryParentGeneProbability,
                                genomeBuilder,
                                genomeIdSeq, generationSeq);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new child genome based on the genetic content of two parent genome.
        /// </summary>
        /// <param name="parent1">Parent 1.</param>
        /// <param name="parent2">Parent 2.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new child genome.</returns>
        public NeatGenome<T> CreateGenome(NeatGenome<T> parent1, NeatGenome<T> parent2, IRandomSource rng)
        {
            // Invoke the reproduction strategy.
            return _strategy.CreateGenome(parent1, parent2, rng);
        }

        #endregion
    }
}
