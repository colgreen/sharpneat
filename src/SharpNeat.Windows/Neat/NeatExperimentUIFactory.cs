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
using SharpNeat.Experiments.Windows;

namespace SharpNeat.Windows.Neat
{
    /// <summary>
    /// Default implementation of <see cref="IExperimentUIFactory"/> for NEAT and NEAT genomes."/>
    /// </summary>
    public class NeatExperimentUIFactory : IExperimentUIFactory
    {
        public IExperimentUI CreateExperimentUI(JsonElement configElem)
        {
            return new NeatExperimentUI();
        }
    }
}
