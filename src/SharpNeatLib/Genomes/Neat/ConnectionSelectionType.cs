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
    /// Different methods of determining which connection weights will be selected
    /// for mutation.
    /// </summary>
    public enum ConnectionSelectionType
    {
        // TODO: Remove as this probably isn't useful
        /// <summary>
        /// Select a proportion of the weights in a genome.
        /// </summary>
        Proportional,
        /// <summary>
        /// Select a fixed number of weights in a genome.
        /// </summary>
        FixedQuantity
    }
}
