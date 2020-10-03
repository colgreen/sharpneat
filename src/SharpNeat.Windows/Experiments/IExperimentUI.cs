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
using SharpNeat.Windows;

namespace SharpNeat.Experiments.Windows
{
    public interface IExperimentUI
    {
        /// <summary>
        /// Create a new Windows.Forms UI control for direct visualisation of the genome, i.e. the 
        /// actual neural net nodes and weights.
        /// </summary>
        /// <returns>A new instance of <see cref="GenomeControl"/>.</returns>
        /// <returns>A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a genome control.</returns>
        GenomeControl CreateGenomeControl();

        /// <summary>
        /// Create a new Windows.Forms UI control for visualisation of task. E.g. for a prey capture task
        /// we might have a control that show the prey capture world, and how the current best genome peforms
        /// in that world.
        /// </summary>
        /// <returns>A new instance of <see cref="GenomeControl"/>; or null if the experiment does not provide a task control.</returns>
        GenomeControl CreateTaskControl();
    }
}
