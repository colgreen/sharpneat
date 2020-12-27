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
using System;

namespace SharpNeat.EvolutionAlgorithm.Runner
{
    /// <summary>
    /// Evolution algorithm update scheme.
    /// </summary>
    public sealed class UpdateScheme
    {
        #region Auto Properties

        /// <summary>
        /// Gets the update scheme's mode.
        /// </summary>
        public UpdateMode UpdateMode {  get; }

        /// <summary>
        /// Gets the number of generations between updates Applies to the generational update scheme only.
        /// </summary>
        public uint Generations { get; }

        /// <summary>
        /// Gets the timespan between updates. Applies to the timespan update scheme only.
        /// </summary>
        public TimeSpan TimeSpan { get; }

        #endregion

        #region Constructors

        private UpdateScheme(
            UpdateMode updateMode,
            uint generations,
            TimeSpan timespan)
        {
            this.UpdateMode = updateMode;
            this.Generations = generations;
            this.TimeSpan = timespan;
        }

        #endregion

        #region Public Static Factory Methods

        /// <summary>
        /// Create a 'no updates' update scheme.
        /// </summary>
        /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
        public static UpdateScheme CreateNoUpdateScheme()
        {
            return new UpdateScheme(UpdateMode.None, 0, default);
        }

        /// <summary>
        /// Create a generation based update scheme. I.e. the update event will trigger every N generations.
        /// </summary>
        /// <param name="generations">The number of generations between update events.</param>
        /// <returns>A new instance of <see cref="UpdateScheme"/>.</returns>
        public static UpdateScheme CreateGenerationalUpdateScheme(uint generations)
        {
            return new UpdateScheme(UpdateMode.Generational, generations, default);
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
