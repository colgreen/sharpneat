// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.IO.Compression;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
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

            UpdateUiState();
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

            UpdateUiState();
        }
        catch(Exception ex)
        {
            __log.ErrorFormat("Error loading genome. Error message [{0}]", ex.Message);
        }
    }

    private void loadSeedGenomesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string filepath = SelectFileToOpen("Load seed population", "pop", "(*.pop)|*.pop");
        if(string.IsNullOrEmpty(filepath))
            return;

        INeatExperiment<double> neatExperiment = GetNeatExperiment();
        MetaNeatGenome<double> metaNeatGenome = NeatUtils.CreateMetaNeatGenome(neatExperiment);
        NeatPopulationLoader<double> popLoader = new(metaNeatGenome);

        try
        {
            // Load the seed genomes.
            List<NeatGenome<double>> seedGenomes = popLoader.LoadFromZipArchive(filepath);

            if(seedGenomes.Count == 0)
            {
                __log.WarnFormat("No genomes loaded from population file [{0}].", filepath);
                return;
            }

            // Create an instance of the default connection weight mutation scheme.
            var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme(
                neatExperiment.ConnectionWeightScale);

            _neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                neatExperiment.PopulationSize,
                seedGenomes,
                neatExperiment.ReproductionAsexualSettings,
                weightMutationScheme);

            UpdateUiState();
        }
        catch(Exception ex)
        {
            __log.ErrorFormat("Error loading seed population. Error message [{0}]", ex.Message);
        }
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
        IExperimentUi experimentUi = GetExperimentUi();
        if(experimentUi is null)
            return;

        GenomeControl genomeCtrl = experimentUi.CreateGenomeControl();
        if(genomeCtrl is null)
            return;

        // Create form, disable its associated menu item, and attach form-close event handlers.
        _bestGenomeForm = HandleFormOpening(
            new GenomeForm("Best Genome", genomeCtrl),
            bestGenomeToolStripMenuItem,
            () => _bestGenomeForm  = null);

        // Get the current best genome.
        NeatGenome<double> bestGenome = _neatPop?.BestGenome;
        if(bestGenome is not null)
        {
            // Set the form's current genome. If the EA is running it will be set shortly anyway, but this ensures we
            // see a genome right away, regardless of whether the EA is running or not.
            _bestGenomeForm.Genome = bestGenome;
        }
    }

    private void taskToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IExperimentUi experimentUi = GetExperimentUi();
        if(experimentUi is null)
            return;

        GenomeControl taskCtrl = experimentUi.CreateTaskControl();
        if(taskCtrl is null)
            return;

        // Create form, disable its associated menu item, and attach form-close event handlers.
        _taskForm = HandleFormOpening(
            new GenomeForm("Task View", taskCtrl),
            taskToolStripMenuItem,
            () => _taskForm  = null);

        // Get the current best genome.
        NeatGenome<double> bestGenome = _neatPop?.BestGenome;
        if(bestGenome is not null)
        {
            // Set the form's current genome. If the EA is running it will be set shortly anyway, but this ensures we
            // see a genome right away, regardless of whether the EA is running or not.
            _bestGenomeForm.Genome = bestGenome;
        }
    }

    #endregion

    #region View -> Charts Items

    private void fitnessBestMeanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _fitnessTimeSeriesForm = HandleFormOpening(
            new FitnessTimeSeriesForm(),
            fitnessBestMeanToolStripMenuItem,
            () => _fitnessTimeSeriesForm = null);
    }

    private void complexityBestMeanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _complexityTimeSeriesForm = HandleFormOpening(
            new ComplexityTimeSeriesForm(),
            complexityBestMeanToolStripMenuItem,
            () => _complexityTimeSeriesForm = null);
    }

    private void evaluationsPerSecToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _evalsPerSecTimeSeriesForm = HandleFormOpening(
            new EvalsPerSecTimeSeriesForm(),
            evaluationsPerSecToolStripMenuItem,
            () => _evalsPerSecTimeSeriesForm = null);
    }

    private void speciesSizeHistogramToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesSizeHistogramForm = HandleFormOpening(
            new HistogramGraphForm(
                "Species Size Histogram",
                "Species Size",
                "Frequency",
                null),
            speciesSizeHistogramToolStripMenuItem,
            () => _speciesSizeHistogramForm = null);
    }

    private void speciesMeanFitnessHistogramToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesMeanFitnessHistogramForm = HandleFormOpening(
            new HistogramGraphForm(
                "Species Mean Fitness Histogram",
                "Species Mean Fitness",
                "Frequency",
                null),
            speciesMeanFitnessHistogramToolStripMenuItem,
            () => _speciesMeanFitnessHistogramForm = null);
    }

    private void speciesMeanComplexityHistogramToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesMeanComplexityHistogramForm = HandleFormOpening(
            new HistogramGraphForm(
                "Species Mean Complexity Histogram",
                "Species Mean Complexity",
                "Frequency",
                null),
            speciesMeanComplexityHistogramToolStripMenuItem,
            () => _speciesMeanComplexityHistogramForm = null);
    }

    private void genomeFitnessHistogramToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _genomeFitnessHistogramForm = HandleFormOpening(
            new HistogramGraphForm(
                "Genome Fitness Histogram",
                "Fitness",
                "Frequency",
                null),
            genomeFitnessHistogramToolStripMenuItem,
            () => _genomeFitnessHistogramForm = null);
    }

    private void genomeComplexityHistogramToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _genomeComplexityHistogramForm = HandleFormOpening(
            new HistogramGraphForm(
                "Genome Complexity Histogram",
                "Complexity",
                "Frequency",
                null),
            genomeComplexityHistogramToolStripMenuItem,
            () => _genomeComplexityHistogramForm = null);
    }

    private void speciesSizeByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesSizeRankForm = HandleFormOpening(
            new RankGraphForm(
                "Species Size by Rank",
                "Rank",
                "Size",
                null),
            speciesSizeByRankToolStripMenuItem,
            () => _speciesSizeRankForm = null);
    }

    private void speciesFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesFitnessRankForm = HandleFormOpening(
            new RankPairGraphForm(
                "Species Fitness by Rank (Best & Mean)",
                "Rank", "Fitness",
                "Best Fitness",
                "Mean Fitness"),
            speciesFitnessByRankToolStripMenuItem,
            () => _speciesFitnessRankForm = null);
    }

    private void speciesComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _speciesComplexityRankForm = HandleFormOpening(
            new RankPairGraphForm(
                "Species Complexity by Rank (Best & Mean)",
                "Rank",
                "Complexity",
                "Best Genome Complexity",
                "Mean Complexity"),
            speciesComplexityByRankToolStripMenuItem,
            () => _speciesComplexityRankForm = null);
    }

    private void genomeFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _genomeFitnessRankForm = HandleFormOpening(
            new RankGraphForm(
                "Genome Fitness by Rank",
                "Rank",
                "Fitness",
                null),
            genomeFitnessByRankToolStripMenuItem,
            () => _genomeFitnessRankForm = null);
    }

    private void genomeComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _genomeComplexityRankForm = HandleFormOpening(
            new RankGraphForm(
                "Genome Complexity by Rank",
                "Rank",
                "Complexity",
                null),
            genomeComplexityByRankToolStripMenuItem,
            () => _genomeComplexityRankForm = null);
    }

    #endregion

    #region About Menu Item

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using Form frmAboutBox = new AboutForm();
        frmAboutBox.ShowDialog(this);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Shows the provided form instance, and disabled the menu item used for opening the form, to prevent users
    /// clicking on it when the form is already open.
    /// 
    /// Also attaches an FormClosed event handler to the form, that will re-enable the menu item when the form is
    /// closed.
    /// </summary>
    /// <param name="form">The form to be shown to the user.</param>
    /// <param name="toolStripMenuItem">Represents the menu item that was just clicked in order to open the form.</param>
    /// <returns>The form instance that was passed in via <paramref name="form"/>.</returns>
    private TForm HandleFormOpening<TForm>(
        TForm form,
        ToolStripMenuItem toolStripMenuItem,
        Action formCloseAction)
        where TForm : Form
    {
        // Disable the menu item for the form, to prevent creation of more than one instance of the form.
        toolStripMenuItem.Enabled = false;

        // Attach an event handler to re-enable the menu item when the new form is closed.
        form.FormClosed += new FormClosedEventHandler(
            delegate (object senderObj, FormClosedEventArgs eArgs)
            {
                toolStripMenuItem.Enabled = true;
                formCloseAction();
            });

        // Show the form.
        form.Show(this);

        return form;
    }

    #endregion
}
