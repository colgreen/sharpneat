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
namespace SharpNeat.Core
{
    /// <summary>
    /// An enumeration of possible execution states for an IEvolutionAlgorithm.
    /// </summary>
    public enum RunState
    {
        /// <summary>
        /// Not yet initialized.
        /// </summary>
        NotReady,
        /// <summary>
        /// Initialized and ready to start.
        /// </summary>
        Ready,
        /// <summary>
        /// The algorithm is running.
        /// </summary>
        Running,
        /// <summary>
        /// The algorithm has been paused, either due to a user request or because a stop condition
        /// has been met. The algorithm can be restarted if the stop condition is no longer true.
        /// </summary>
        Paused,
        /// <summary>
        /// The algorithm thread has terminated. The algorithm cannot be restarted from this state, a new
        /// algorithm object must be created and started afresh.
        /// </summary>
        Terminated
    }
}
