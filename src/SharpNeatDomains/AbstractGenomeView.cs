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
using System.Windows.Forms;

namespace SharpNeat.Domains
{
    /// <summary>
    /// Abstract class for genome visualization controls.
    /// </summary>
    public abstract class AbstractGenomeView : UserControl
    {
        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        abstract public void RefreshView(object genome);
    }
}
