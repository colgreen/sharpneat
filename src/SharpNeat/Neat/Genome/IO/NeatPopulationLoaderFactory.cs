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

namespace SharpNeat.Neat.Genome.IO
{
    /// <summary>
    /// <see cref="NeatPopulationLoader{Double}"/> factory.
    /// </summary>
    public static class NeatPopulationLoaderFactory
    {
        /// <summary>
        /// Create a new instance of <see cref="NeatPopulationLoader{Double}"/>.
        /// </summary>
        /// <param name="metaNeatGenome">Meta neat genome.</param>
        /// <returns>A new instance of <see cref="NeatPopulationLoader{Double}"/>.</returns>
        public static NeatPopulationLoader<double> CreateLoaderDouble(
            MetaNeatGenome<double> metaNeatGenome)
        {
            NeatGenomeLoader<double> genomeLoader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
            return new NeatPopulationLoader<double>(genomeLoader);
        }
    }
}
