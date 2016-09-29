/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Defines types of weight perturbation.
    /// </summary>
    public enum ConnectionPerturbanceType
    {
        /// <summary>
        /// Reset weight.
        /// </summary>
        Reset,
        /// <summary>
        /// Jiggle weight using deltas from a uniform distribution.
        /// </summary>
        JiggleUniform,
        /// <summary>
        /// Jiggle weight using deltas from a Gaussian distribution.
        /// </summary>
        JiggleGaussian

        // TODO: Replace Gaussian with a fatter tailed distribution.
    }
}
