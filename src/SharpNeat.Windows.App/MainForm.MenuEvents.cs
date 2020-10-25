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
using SharpNeat.Experiments.Windows;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.Windows.App.Forms;
using SharpNeat.Windows.App.Forms.TimeSeries;
using static SharpNeat.Windows.App.AppUtils;

namespace SharpNeat.Windows.App
{
    partial class MainForm
    {
        #region File Menu Items

        private void saveBestGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the current best genome.
            NeatGenome<double> bestGenome = _neatPop?.BestGenome;
            if(bestGenome is null) {
                return;
            }

            // Ask the user to select a file path and name to save to.
            string filepath = SelectFileToSave("Save best genome", "genome", "(*.genome)|*.genome");
            if(string.IsNullOrEmpty(filepath)) {
                return;
            }

            // Save the genome.
            try
            { 
                NeatGenomeSaver<double>.Save(bestGenome, filepath);
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error saving genome; [{0}]", ex.Message);
            }
        }

        #endregion

        #region View Menu Items

        private void bestGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IExperimentUI experimentUI = GetExperimentUI();
            if(experimentUI is null) {
                return;
            }

            GenomeControl genomeCtrl = experimentUI.CreateGenomeControl();
            if(experimentUI is null) {
                return;
            }

            // Create form.
            _bestGenomeForm = new GenomeForm("Best Genome", genomeCtrl);

            // Attach an event handler to update this main form when the genome form is closed.
            _bestGenomeForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _bestGenomeForm = null;
                bestGenomeToolStripMenuItem.Enabled = true;
            });

            // Prevent creation of more then one instance of the genome form.
            bestGenomeToolStripMenuItem.Enabled = false;

            // Get the current best genome.
            NeatGenome<double> bestGenome = _neatPop?.BestGenome;
            if(bestGenome is object)
            {
                // Set the form's current genome. If the EA is running it will be set shortly anyway, but this ensures we 
                // see a genome right away, regardless of whether the EA is running or not.
                _bestGenomeForm.Genome = bestGenome;
            }

            // Show the form.
            _bestGenomeForm.Show(this);
        }

        #endregion

        #region View -> Charts Items

        private void fitnessBestMeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            _fitnessTimeSeriesForm = new FitnessTimeSeriesForm();

            // Prevent creating more then one instance of the form.
            fitnessBestMeansToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            _fitnessTimeSeriesForm.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    fitnessBestMeansToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            _fitnessTimeSeriesForm.Show(this);
        }  

        private void complexityBestMeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            _complexityTimeSeriesForm = new ComplexityTimeSeriesForm();

            // Prevent creating more then one instance of the form.
            complexityBestMeansToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            _complexityTimeSeriesForm.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    complexityBestMeansToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            _complexityTimeSeriesForm.Show(this);
        }

        private void evaluationsPerSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            _evalsPerSecTimeSeriesForm = new EvalsPerSecTimeSeriesForm();

            // Prevent creating more then one instance of the form.
            evaluationsPerSecToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            _evalsPerSecTimeSeriesForm.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    evaluationsPerSecToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            _evalsPerSecTimeSeriesForm.Show(this);
        }  

        private void specieSizeByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            _speciesSizeRankForm = new RankGraphForm("Species Size by Rank", "Rank", "Size", null);

            // Prevent creating more then one instance of the form.
            specieSizeByRankToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            _speciesSizeRankForm.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    specieSizeByRankToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            _speciesSizeRankForm.Show(this);
        }

        private void genomeFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            _genomeFitnessRankForm = new RankGraphForm("Genome Fitness by Rank", "Rank", "Fitness", null);

            // Prevent creating more then one instance of the form.
            genomeFitnessByRankToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            _genomeFitnessRankForm.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    genomeFitnessByRankToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            _genomeFitnessRankForm.Show(this);
        }

        #endregion

        #region About Menu Item

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAboutBox = new AboutForm();
            frmAboutBox.ShowDialog(this);
        }

        #endregion
    }
}
