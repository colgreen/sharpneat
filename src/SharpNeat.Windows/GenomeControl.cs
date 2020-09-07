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
using System.Windows.Forms;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows
{
    /// <summary>
    /// A user control that displays a genome in some form.
    /// This is used for displaying a genome's directly (e.g. a neural net structure), or some other type of view that uses the genome 
    /// (e.g. a task view that shows how the genome performs on the task).
    /// </summary>
    public partial class GenomeControl : UserControl
    {
        IGenome _genome;

        public GenomeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets of sets the genome object to display.
        /// </summary>
        public IGenome Genome 
        { 
            get => _genome;
            set
            {
                _genome = value;

                // TODO: Update/repaint the control.
                // Initial check.
                if(this.IsDisposed) {
                    return;
                }

                if(!this.InvokeRequired)
                {
                    Refresh();
                    return;
                }

                this.Invoke(new MethodInvoker(delegate()
                {
                    // Secondary check; the form could have been disposed after the first test of IsDisposed, and the call to Invoke().
                    if(this.IsDisposed) {
                        return;
                    }
                    Refresh();
                }));
            }
        }
    }
}
