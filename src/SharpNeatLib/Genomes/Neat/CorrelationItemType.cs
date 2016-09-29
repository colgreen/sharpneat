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
    /// An enumeration of connection gene correlation types.
    /// Mismatched genes can be disjoint or excess. This distinction is defined by original
    /// NEAT but is probably redundant for most genome comparison/distance metrics.
    /// </summary>
    public enum CorrelationItemType
    {
        /// <summary>
        /// A match between two connections in two distinct genomes.
        /// </summary>
        Match,
        /// <summary>
        /// A connection with no match in the other genome (that we are comparing with) and that has 
        /// an innovation ID less than the highest innovation ID in the other genome,
        /// </summary>
        Disjoint,
        /// <summary>
        /// A connection with no match in the other genome (that we are comparing with) and that has 
        /// an innovation ID higher than the highest innovation ID in the other genome.
        /// </summary>
        Excess
    }
}
