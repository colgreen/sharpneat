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

using System;
using System.Windows.Forms;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace SharpNeatGUI
{
    /// <summary>
    /// Generic form for problem domain visualization. 
    /// </summary>
    public partial class ProblemDomainForm : Form
    {
        AbstractDomainView _domainViewControl;
        AbstractGenerationalAlgorithm<NeatGenome> _ea;

        #region Constructor

        /// <summary>
        /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
        /// from the evolution algorithm and cleanly detach from it when this form closes.
        /// </summary>
        public ProblemDomainForm(string title, AbstractDomainView domainViewControl, AbstractGenerationalAlgorithm<NeatGenome> ea)
        {
            InitializeComponent();
            this.Text = title;

            _domainViewControl = domainViewControl;
            domainViewControl.Dock = DockStyle.Fill;
            this.Controls.Add(domainViewControl);
            this.Size = domainViewControl.WindowSize;

            _ea = ea;
            if(null != ea) {
                _ea.UpdateEvent += new EventHandler(_ea_UpdateEvent);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when a new evolution algorithm is initialized. Clean up any existing event listeners and
        /// connect up to the new evolution algorithm.
        /// </summary>
        public void Reconnect(AbstractGenerationalAlgorithm<NeatGenome> ea)
        {
            // Clean up.
            if(null != _ea) {
                _ea.UpdateEvent -= new EventHandler(_ea_UpdateEvent);
            }

            // Reconnect.
            _ea = ea;
            _ea.UpdateEvent += new EventHandler(_ea_UpdateEvent);
        }

        /// <summary>
        /// Refresh view.
        /// </summary>
        public void RefreshView()
        {
            if(null != _ea && null != _ea.CurrentChampGenome) {
                _domainViewControl.RefreshView(_ea.CurrentChampGenome);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle update event from the evolution algorithm - update the view.
        /// </summary>
        public void _ea_UpdateEvent(object sender, EventArgs e)
        {
            // Switch execution to GUI thread if necessary.
            if(this.InvokeRequired)
            {
                // Use Invoke instead of BeginInvoke. This blocks the caller and thus makes the evolution algorithm wait for the view to refresh.
                // This prevent the GUI message queue from getting backlogged (if refreshing is slow) and thus prevents (or alleviates) GUI lock up / stickiness.
                this.Invoke(new MethodInvoker(delegate() 
                {
                    if(this.IsDisposed) {
                        return;
                    }
                    _domainViewControl.RefreshView(_ea.CurrentChampGenome);
                }));
            }
        }

        private void ProblemDomainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(null != _ea) {
                _ea.UpdateEvent -= new EventHandler(_ea_UpdateEvent);
            }
        }

        #endregion
    }
}
