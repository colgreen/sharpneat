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

namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// An enumeration of update schemes, e.g. Fire an update event the per some time duration or some number of generations.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Raise an update event at regular time intervals.
        /// </summary>
        Timespan,
        /// <summary>
        /// Raise an update event at regular generation intervals. (Every N generations).
        /// </summary>
        Generational
    }
}
