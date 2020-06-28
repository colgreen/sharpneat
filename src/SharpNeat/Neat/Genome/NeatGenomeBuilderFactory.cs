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

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// Static factory class for creating instances of <see cref="INeatGenomeBuilder{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class NeatGenomeBuilderFactory<T>
        where T : struct
    {
        /// <summary>
        /// Create a new instance of <see cref="INeatGenomeBuilder{T}"/>.
        /// </summary>
        /// <param name="metaNeatGenome">Neat genome metadata.</param>
        /// <returns>A new instance of <see cref="INeatGenomeBuilder{T}"/>.</returns>
        public static INeatGenomeBuilder<T> Create(
            MetaNeatGenome<T> metaNeatGenome)
        {
            if(metaNeatGenome.IsAcyclic)
            {
                return new NeatGenomeBuilderAcyclic<T>(metaNeatGenome);
            }
            // else
            return new NeatGenomeBuilderCyclic<T>(metaNeatGenome);
        }
    }
}
