// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.IO.Compression;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Windows.App.Experiments;
using SharpNeat.Windows.App.Forms;
using SharpNeat.Windows.App.Forms.TimeSeries;
using SharpNeat.Windows.Experiments;
using static SharpNeat.Windows.App.AppUtils;

#pragma warning disable IDE1006 // Naming Styles. Allow naming convention for Windows.Forms event handlers.

namespace SharpNeat.Windows.App;

partial class MainForm
{
    #region File Menu Items

    private void loadPopulationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string filepath = SelectFileToOpen("Load population", "pop", "(*.pop)|*.pop");
        if(string.IsNullOrEmpty(filepath))
            return;

        INeatExperiment<double> neatExperiment = GetNeatExperiment();
        MetaNeatGenome<double> metaNeatGenome = NeatUtils.CreateMetaNeatGenome(neatExperiment);
        NeatPopulationLoader<double> popLoader = new(metaNeatGenome);
        
        try
        {
            List<NeatGenome<double>> genomeList = popLoader.LoadFromZipArchive(filepath);

            if(genomeList.Count == 0)
            {
                __log.WarnFormat("No genomes loaded from population file [{0}].", filepath);
                return;
            }

            INeatGenomeBuilder<double> genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            _neatPop = new NeatPopulation<double>(
                metaNeatGenome,
                genomeBuilder,
                neatExperiment.PopulationSize,
                genomeList);

            UpdateUIState();
        }
        catch(Exception ex)
        {
            __log.ErrorFormat("Error loading population. Error message [{0}]", ex.Message);
        }
    }

    private void loadSeedGenomeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string filepath = SelectFileToOpen("Load seed genome", "net", "(*.net)|*.net");
        if(string.IsNullOrEmpty(filepath))
            return;

        INeatExperiment<double> neatExperiment = GetNeatExperiment();
        MetaNeatGenome<double> metaNeatGenome = NeatUtils.CreateMetaNeatGenome(neatExperiment);

        try
        {
            // Load the seed genome.
            NeatGenome<double> seedGenome = NeatGenomeLoader.Load(filepath, metaNeatGenome, 0);

            // Create an instance of the default connection weight mutation scheme.
            var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme(
                neatExperiment.ConnectionWeightScale);

            _neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                neatExperiment.PopulationSize,
                seedGenome,
                neatExperiment.ReproductionAsexualSettings,
                weightMutationScheme);

            UpdateUIState();
        }
        catch(Exception ex)
        {
            __log.ErrorFormat("Error loading genome. Error message [{0}]", ex.Message);
        }
    }

    // TODO: Load Seed Genomes
    private void loadSeedGenomesToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void savePopulationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Ask the user to select a file path and name to save to.
        string filepath = SelectFileToSave("Save population", "pop", "(*.pop)|*.pop");
        if(string.IsNullOrEmpty(filepath))
            return;

        // Save the population.
        try
        {
            NeatPopulationSaver.SaveToZipArchive(
                _neatPop.GenomeList, filepath,
                CompressionLevel.Optimal);
        }
        catch(Exception ex)
        {
            __log.ErrorFormat("Error saving population; [{0}]", ex.Message);
        }
    }

    private void saveBestGenomeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Get the current best genome.
        NeatGenome<double> bestGenome = _neatPop?.BestGenome;
        if(bestGenome is null)
            return;

        // Ask the user to select a file path and name to save to.
        string filepath = SelectFileToSave("Save best genome", "net", "(*.net)|*.net");
        if(string.IsNullOrEmpty(filepath))
            return;

        // Save the genome.
        try
        {
            NeatGenomeSaver.Save(bestGenome, filepath);
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
        if(experimentUI is null)
            return;

        GenomeControl genomeCtrl = experimentUI.CreateGenomeControl();
        if(experimentUI is null)
            return;

        // Create form.
        _bestGenomeForm = new GenomeForm("Best Genome", genomeCtrl);

        // Attach an event handler to update this main form when the genome form is closed.
        _bestGenomeForm.FormClosed += new FormClosedEventHandler(delegate (object senderObj, FormClosedEventArgs eArgs)
        {
            _bestGenomeForm = null;
            bestGenomeToolStripMenuItem.Enabled = true;
        });

        // Prevent creation of more then one instance of the genome form.
        bestGenomeToolStripMenuItem.Enabled = false;

        // Get the current best genome.
        NeatGenome<double> bestGenome = _neatPop?.BestGenome;
        if(bestGenome is not null)
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
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
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
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
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
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                evaluationsPerSecToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _evalsPerSecTimeSeriesForm.Show(this);
    }

    private void specieSizeByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Create form.
        _speciesSizeRankForm = new RankGraphForm("Species Size by Rank", "Rank", "Size", "Species Size");

        // Prevent creating more then one instance of the form.
        specieSizeByRankToolStripMenuItem.Enabled = false;

        // Attach a event handler to update this main form when the child form is closed.
        _speciesSizeRankForm.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                specieSizeByRankToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _speciesSizeRankForm.Show(this);
    }

    private void specieFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Create form.
        _speciesFitnessRankForm = new RankPairGraphForm("Species Fitness by Rank (Best & Mean)", "Rank", "Fitness", "Best Fitness", "Mean Fitness");

        // Prevent creating more then one instance of the form.
        specieFitnessByRankToolStripMenuItem.Enabled = false;

        // Attach a event handler to update this main form when the child form is closed.
        _speciesFitnessRankForm.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                specieFitnessByRankToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _speciesFitnessRankForm.Show(this);
    }

    private void specieComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Create form.
        _speciesComplexityRankForm = new RankPairGraphForm("Species Complexity by Rank (Best & Mean)", "Rank", "Complexity", "Best Genome Complexity", "Mean Complexity");

        // Prevent creating more then one instance of the form.
        specieComplexityByRankToolStripMenuItem.Enabled = false;

        // Attach a event handler to update this main form when the child form is closed.
        _speciesComplexityRankForm.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                specieComplexityByRankToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _speciesComplexityRankForm.Show(this);
    }

    private void genomeFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Create form.
        _genomeFitnessRankForm = new RankGraphForm("Genome Fitness by Rank", "Rank", "Fitness", "Genome Fitness");

        // Prevent creating more then one instance of the form.
        genomeFitnessByRankToolStripMenuItem.Enabled = false;

        // Attach a event handler to update this main form when the child form is closed.
        _genomeFitnessRankForm.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                genomeFitnessByRankToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _genomeFitnessRankForm.Show(this);
    }

    private void genomeComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Create form.
        _genomeComplexityRankForm = new RankGraphForm("Genome Complexity by Rank", "Rank", "Complexity", "Genome Complexity");

        // Prevent creating more then one instance of the form.
        genomeComplexityByRankToolStripMenuItem.Enabled = false;

        // Attach a event handler to update this main form when the child form is closed.
        _genomeComplexityRankForm.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                genomeComplexityByRankToolStripMenuItem.Enabled = true;
            });

        // Show the form.
        _genomeComplexityRankForm.Show(this);
    }

    #endregion

    #region About Menu Item

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Form frmAboutBox = new AboutForm();
        frmAboutBox.ShowDialog(this);
    }

    #endregion

    #region Private Methods [Event Handler Subroutines

    private IExperimentUI GetExperimentUI()
    {
        // Create a new experiment instance if one has not already been created.
        _experimentUI ??= CreateAndConfigureExperimentUI((ExperimentInfo)cmbExperiments.SelectedItem);

        return _experimentUI;
    }

    #endregion
}
