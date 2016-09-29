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
using System.Drawing;
using System.Windows.Forms;

namespace SharpNeat.Domains
{
    /// <summary>
    /// Abstract class for problem domain visualization.
    /// </summary>
    public abstract class AbstractDomainView : UserControl
    {
        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        abstract public void RefreshView(object genome);

        /// <summary>
        /// Gets the desired initial size for the view window.
        /// </summary>
        public virtual Size WindowSize
        {
            get { return new Size(356, 355); }
        }
    }
}
