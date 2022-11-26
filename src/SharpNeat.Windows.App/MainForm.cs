// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using System.Text.Json;
using log4net;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Windows.App.Experiments;
using SharpNeat.Windows.App.Forms;
using SharpNeat.Windows.App.Forms.TimeSeries;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.App;

/// <summary>
/// SharpNEAT main GUI window.
/// </summary>
internal sealed partial class MainForm : Form
{
    private static readonly ILog __log = LogManager.GetLogger(typeof(MainForm));

    // The current NEAT experiment.
    private INeatExperiment<double> _neatExperiment;
    private NeatPopulation<double> _neatPop;
    private EvolutionAlgorithmRunner _eaRunner;
    private IExperimentUI _experimentUI;
    private GenomeForm _bestGenomeForm;

    // Time series forms.
    private FitnessTimeSeriesForm _fitnessTimeSeriesForm;
    private ComplexityTimeSeriesForm _complexityTimeSeriesForm;
    private EvalsPerSecTimeSeriesForm _evalsPerSecTimeSeriesForm;

    // Rank chart forms.
    private RankGraphForm _speciesSizeRankForm;
    private RankPairGraphForm _speciesFitnessRankForm;
    private RankPairGraphForm _speciesComplexityRankForm;
    private RankGraphForm _genomeFitnessRankForm;
    private RankGraphForm _genomeComplexityRankForm;

    // Histogram chart forms.
    private HistogramGraphForm _genomeFitnessHistogramForm;
    private HistogramGraphForm _genomeComplexityHistogramForm;

    /// <summary>
    /// Construct and initialize the form.
    /// </summary>
    public MainForm()
    {
        // Set the default culture for all threads in the application to the Invariant culture.
        // This is a cheap way of ensuring that all form fields and data IO routines
        // read and write textual data in the same format, in particular the use of a dot as the
        // decimal separator (some culture use a comma).
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        InitializeComponent();
        Logger.SetListBox(lbxLog);

        // Populate the experiments combo-box (drop-down list) with experiment loaded from the experiments.json config file.
        InitExperimentList();
    }

    private void InitExperimentList()
    {
        // Load experiments.json from file.
        // Note. Use of ReadAllText() isn't ideal, but for a small file it's fine, and this avoids the complexities of dealign
        // with async code in a synchronous context.
        string experimentsJson = File.ReadAllText("config/experiments.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip };
        ExperimentRegistry registry = JsonSerializer.Deserialize<ExperimentRegistry>(experimentsJson, options);

        // Populate the combo box.
        foreach(ExperimentInfo expInfo in registry.Experiments)
        {
            cmbExperiments.Items.Add(expInfo);
        }

        // Pre-select first item.
        cmbExperiments.SelectedIndex = 0;
    }
}
