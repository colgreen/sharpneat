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

namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// Represents an update scheme for an IEvolutionAlgorithm. e.g. update per some time duration or 
    /// some number of generations.
    /// </summary>
    public class UpdateScheme
    {
        readonly UpdateMode _updateMode;
        readonly uint _generations;
        readonly TimeSpan _timespan;

        #region Constructors

        private UpdateScheme(
            UpdateMode updateMode,
            uint generations,
            TimeSpan timespan)
        {
            _updateMode = updateMode;
            _generations = generations;
            _timespan = timespan;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the update scheme's mode.
        /// </summary>
        public UpdateMode UpdateMode
        {
            get { return _updateMode; }
        }

        /// <summary>
        /// Gets the number of generations between updates Applies to the generational update scheme only.
        /// </summary>
        public uint Generations
        {
            get { return _generations; }
        }

        /// <summary>
        /// Gets the timespan between updates. Applies to the timespan update scheme only.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return _timespan; }
        }

        #endregion

        #region Public Static Factory Methods

        /// <summary>
        /// Create a generation based update scheme. I.e. the update event will trigger every N generations.
        /// </summary>
        /// <param name="generations">The number of generations between update events.</param>
        /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
        public static UpdateScheme CreateGenerationalUpdateScheme(uint generations)
        {
            return new UpdateScheme(UpdateMode.Generational, generations, default(TimeSpan));
        }

        /// <summary>
        /// Create a clock time based update scheme. I.e. the update event will trigger periodically based on the specified clock time duration/timespan.
        /// </summary>
        /// <param name="timespan">The duration between update events.</param>
        /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
        public static UpdateScheme CreateTimeSpanUpdateScheme(TimeSpan timespan)
        {
            return new UpdateScheme(UpdateMode.Timespan, 0, timespan);
        }

        #endregion
    }
}
