using System;
using System.Windows.Forms;
using SharpNeat.Experiments.Windows;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
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

        #region View->Charts Items

        private void fitnessBestMeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create form.
            Form form = new TimeSeriesForm("Fitness (Best and Mean)", "Generation", "Fitness", "Y2");

            // Prevent creating more then one instance of the form.
            fitnessBestMeansToolStripMenuItem.Enabled = false;

            // Attach a event handler to update this main form when the child form is closed.
            form.FormClosed += new FormClosedEventHandler(
                delegate (object senderObj,FormClosedEventArgs eArgs) {
                    fitnessBestMeansToolStripMenuItem.Enabled = true;
                });

            // Show the form.
            form.Show(this);
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
