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
using System.Windows.Forms;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Windows.App.Forms
{
    /// <summary>
    /// Form for genome visualization.
    /// This is used for displaying genome's directly (e.g. a neural net structure), or some other type of visualization
    /// that uses the genome, e.g. a task view that shows how the genome performs on some task.
    /// </summary>
    public partial class GenomeForm : Form
    {
        #region Constructor

        /// <summary>
        /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
        /// from the evolution algorithm and cleanly detach from it when this form closes.
        /// </summary>
        public GenomeForm(string title, GenomeControl genomeCtrl)
        {
            if(genomeCtrl is null) throw new ArgumentNullException(nameof(genomeCtrl));

            this.genomeCtrl = genomeCtrl;

            InitializeComponent();
            this.Text = title;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the genome to render.
        /// </summary>
        public IGenome Genome 
        { 
            get => this.genomeCtrl.Genome;
            set => this.genomeCtrl.Genome = value;
        }

        #endregion
    }
}
