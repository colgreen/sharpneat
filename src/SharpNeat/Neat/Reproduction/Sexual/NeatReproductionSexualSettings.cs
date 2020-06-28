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

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Settings related to <see cref="NeatReproductionSexual{T}"/>.
    /// </summary>
    public class NeatReproductionSexualSettings
    {
        /// <summary>
        /// The probability that a gene that exists only on the secondary parent is copied into the child genome.
        /// </summary>
        public double SecondaryParentGeneProbability { get; set; } = 0.1;
    }
}
