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
using System.Text.Json;

namespace SharpNeat.Experiments.Windows
{
    /// <summary>
    /// Represents a factory of <see cref="IExperimentUI"/>.
    /// </summary>
    public interface IExperimentUIFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IExperimentUI"/> using the experiment
        /// configuration settings from the provided json object model.
        /// </summary>
        /// <param name="configElem">Experiment config in presented as json object model.</param>
        /// <returns>A new instance of <see cref="IExperimentUI{T}"/>.</returns>
        IExperimentUI CreateExperimentUI(JsonElement configElem);
    }
}
