/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using log4net;
using log4net.Config;

using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeatGUI
{
    /// <summary>
    /// SharpNEAT main GUI window.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly ILog __log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Instance Fields [General]

        IGuiNeatExperiment _selectedExperiment;
        IGenomeFactory<NeatGenome> _genomeFactory;
        List<NeatGenome> _genomeList;
        NeatEvolutionAlgorithm<NeatGenome> _ea;
        StreamWriter _logFileWriter = null;
        /// <summary>Number format for building filename when saving champ genomes.</summary>
        NumberFormatInfo _filenameNumberFormatter;
        /// <summary>XmlWriter settings for champ genome saving (format the XML to make it human readable).</summary>
        XmlWriterSettings _xwSettings;
        /// <summary>Tracks the best champ fitness observed so far.</summary>
        double _champGenomeFitness;

        #endregion

        #region Instance Fields [Views]

        GenomeForm _bestGenomeForm;
        ProblemDomainForm _domainForm;
        List<TimeSeriesGraphForm> _timeSeriesGraphFormList = new List<TimeSeriesGraphForm>();
        List<SummaryGraphForm> _summaryGraphFormList = new List<SummaryGraphForm>();

        // Working storage space for graph views.
        static int[] _specieDataArrayInt;
        static Point2DDouble[] _specieDataPointArrayInt;
        
        static double[] _specieDataArray;
        static Point2DDouble[] _specieDataPointArray;

        static double[] _genomeDataArray;
        static Point2DDouble[] _genomeDataPointArray;

        #endregion

        #region Form Constructor / Initialisation

        /// <summary>
        /// Construct and initialize the form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Logger.SetListBox(lbxLog);
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));
            InitProblemDomainList();

            _filenameNumberFormatter = new NumberFormatInfo();
            _filenameNumberFormatter.NumberDecimalSeparator = ",";

            _xwSettings = new XmlWriterSettings();
            _xwSettings.Indent = true;
        }

        /// <summary>
        /// Initialise the problem domain combobox. The list of problem domains is read from an XML file; this 
        /// allows changes to be made and new domains to be plugged-in without recompiling binaries.
        /// </summary>
        private void InitProblemDomainList()
        {
            // Find all experiment config data files in the current directory (*.experiments.xml)
            foreach(string filename in Directory.EnumerateFiles(".", "*.experiments.xml"))
            {
                List<ExperimentInfo> expInfoList = ExperimentInfo.ReadExperimentXml(filename);
                foreach(ExperimentInfo expInfo in expInfoList) {
                    cmbExperiments.Items.Add(new ListItem(string.Empty, expInfo.Name, expInfo));
                }
            }
            // Pre-select first item.
            cmbExperiments.SelectedIndex = 0;
        }

        #endregion

        #region GUI Wiring [Experiment Selection + Default Param Loading]

        private void cmbExperiments_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Nullify this variable. We get the selected experiment via GetSelectedExperiment(). That method will instantiate 
            // _selectedExperiment with the currently selected experiment (on-demand instantiation).
            _selectedExperiment = null;

            // Close any experiment specific forms that remain open.
            if(null != _bestGenomeForm) {
                _bestGenomeForm.Close();
                _bestGenomeForm = null;
            }

            if(null != _domainForm) {
                _domainForm.Close();
                _domainForm = null;
            }
        }

        private void btnExperimentInfo_Click(object sender, EventArgs e)
        {
            if(null == cmbExperiments.SelectedItem) {
                return;
            }

            INeatExperiment experiment = GetSelectedExperiment();
            if(null != experiment) {
                MessageBox.Show(experiment.Description);
            }
        }

        private void btnLoadDomainDefaults_Click(object sender, EventArgs e)
        {
            // Dump the experiment's default parameters into the GUI.
            INeatExperiment experiment = GetSelectedExperiment();
            txtParamPopulationSize.Text = experiment.DefaultPopulationSize.ToString();

            NeatEvolutionAlgorithmParameters eaParams = experiment.NeatEvolutionAlgorithmParameters;
            NeatGenomeParameters ngParams = experiment.NeatGenomeParameters;
            txtParamInitialConnectionProportion.Text = ngParams.InitialInterconnectionsProportion.ToString();
            txtParamNumberOfSpecies.Text = eaParams.SpecieCount.ToString();
            txtParamElitismProportion.Text = eaParams.ElitismProportion.ToString();
            txtParamSelectionProportion.Text = eaParams.SelectionProportion.ToString();
            txtParamOffspringAsexual.Text = eaParams.OffspringAsexualProportion.ToString();
            txtParamOffspringCrossover.Text = eaParams.OffspringSexualProportion.ToString();
            txtParamInterspeciesMating.Text = eaParams.InterspeciesMatingProportion.ToString();
            txtParamConnectionWeightRange.Text = ngParams.ConnectionWeightRange.ToString();
            txtParamMutateConnectionWeights.Text = ngParams.ConnectionWeightMutationProbability.ToString();
            txtParamMutateAddNode.Text = ngParams.AddNodeMutationProbability.ToString();
            txtParamMutateAddConnection.Text = ngParams.AddConnectionMutationProbability.ToString();
            txtParamMutateDeleteConnection.Text = ngParams.DeleteConnectionMutationProbability.ToString();
        }

        private IGuiNeatExperiment GetSelectedExperiment()
        {
            if(null == _selectedExperiment && null != cmbExperiments.SelectedItem)
            {
                ExperimentInfo expInfo = (ExperimentInfo)(((ListItem)cmbExperiments.SelectedItem).Data);

                Assembly assembly = Assembly.LoadFrom(expInfo.AssemblyPath);
                // TODO: Handle non-gui experiments.
                _selectedExperiment = assembly.CreateInstance(expInfo.ClassName) as IGuiNeatExperiment;
                _selectedExperiment.Initialize(expInfo.Name, expInfo.XmlConfig);
            }
            return _selectedExperiment;
        }

        #endregion

        #region GUI Wiring [Population Init]

        private void btnCreateRandomPop_Click(object sender, EventArgs e)
        {
            // Parse population size and interconnection proportion from GUI fields.
            int? popSize = ParseInt(txtParamPopulationSize);
            if(null == popSize) {
                return;
            }

            double? initConnProportion = ParseDouble(txtParamInitialConnectionProportion);
            if(null == initConnProportion) {
                return;
            }

            INeatExperiment experiment = GetSelectedExperiment();
            experiment.NeatGenomeParameters.InitialInterconnectionsProportion = initConnProportion.Value;

            // Create a genome factory appropriate for the experiment.
            IGenomeFactory<NeatGenome> genomeFactory = experiment.CreateGenomeFactory();
                
            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(popSize.Value, 0u);

            // Assign population to form variables & update GUI appropriately.
            _genomeFactory = genomeFactory;
            _genomeList = genomeList;
            UpdateGuiState();
        }

        #endregion

        #region GUI Wiring [Algorithm Init/Start/Stop]

        private void btnSearchStart_Click(object sender, EventArgs e)
        {
            if(null != _ea)
            {   // Resume existing EA & update GUI state.
                _ea.StartContinue();
                UpdateGuiState();
                return;
            }

            // Initialise and start a new evolution algorithm.
            ReadAndUpdateExperimentParams();

            // Check number of species is <= the number of the genomes.
            if(_genomeList.Count < _selectedExperiment.NeatEvolutionAlgorithmParameters.SpecieCount) {
                __log.ErrorFormat("Genome count must be >= specie count. Genomes=[{0}] Species=[{1}]",
                                    _selectedExperiment.NeatEvolutionAlgorithmParameters.SpecieCount, _genomeList.Count);
                return;
            }

            // Create evolution algorithm.
            _ea = _selectedExperiment.CreateEvolutionAlgorithm(_genomeFactory, _genomeList);

            // Attach update event listener.
            _ea.UpdateEvent += new EventHandler(_ea_UpdateEvent);
            _ea.PausedEvent += new EventHandler(_ea_PausedEvent);

            // Notify any open views.
            if(null != _bestGenomeForm) { _bestGenomeForm.Reconnect(_ea); }
            if(null != _domainForm) { _domainForm.Reconnect(_ea); }
            foreach(TimeSeriesGraphForm graphForm in _timeSeriesGraphFormList) {
                graphForm.Reconnect(_ea);
            }
            foreach(SummaryGraphForm graphForm in _summaryGraphFormList) {
                graphForm.Reconnect(_ea);
            }

            // Create/open log file if the option is selected.
            if(chkFileWriteLog.Checked && null==_logFileWriter)
            {
                string filename = txtFileLogBaseName.Text + '_' + DateTime.Now.ToString("yyyyMMdd") + ".log";
                _logFileWriter = new StreamWriter(filename, true);
                _logFileWriter.WriteLine("ClockTime,Gen,BestFitness,MeanFitness,MeanSpecieChampFitness,ChampComplexity,MeanComplexity,MaxComplexity,TotalEvaluationCount,EvaluationsPerSec,SearchMode");
            }

            // Start the algorithm & update GUI state.
            _ea.StartContinue();
            UpdateGuiState();
        }

        private void btnSearchStop_Click(object sender, EventArgs e)
        {
            _ea.RequestPause();

            if(null != _logFileWriter)
            {
                // Null _logFileWriter prior to closing the writer. This much reduced the chance of attempt to write to the stream after it has closed.
                StreamWriter sw = _logFileWriter;
                _logFileWriter = null;
                sw.Close();
            }
        }

        private void btnSearchReset_Click(object sender, EventArgs e)
        {
            _genomeFactory = null;
            _genomeList = null;
            // TODO: Proper cleanup of EA - e.g. main worker thread termination.
            _ea = null;
            _champGenomeFitness = 0.0;
            Logger.Clear();
            UpdateGuiState_ResetStats();
            UpdateGuiState();
        }

        /// <summary>
        /// Read experimental parameters from the GUI and update _selectedExperiment with the read values.
        /// </summary>
        private void ReadAndUpdateExperimentParams()
        {
            NeatEvolutionAlgorithmParameters eaParams = _selectedExperiment.NeatEvolutionAlgorithmParameters;
            eaParams.SpecieCount = ParseInt(txtParamNumberOfSpecies, eaParams.SpecieCount);
            eaParams.ElitismProportion = ParseDouble(txtParamElitismProportion, eaParams.ElitismProportion);
            eaParams.SelectionProportion = ParseDouble(txtParamSelectionProportion, eaParams.SelectionProportion);
            eaParams.OffspringAsexualProportion = ParseDouble(txtParamOffspringAsexual, eaParams.OffspringAsexualProportion);
            eaParams.OffspringSexualProportion = ParseDouble(txtParamOffspringCrossover, eaParams.OffspringSexualProportion);
            eaParams.InterspeciesMatingProportion = ParseDouble(txtParamInterspeciesMating, eaParams.InterspeciesMatingProportion);

            NeatGenomeParameters ngParams = _selectedExperiment.NeatGenomeParameters;
            ngParams.ConnectionWeightRange = ParseDouble(txtParamConnectionWeightRange, ngParams.ConnectionWeightRange);
            ngParams.ConnectionWeightMutationProbability = ParseDouble(txtParamMutateConnectionWeights, ngParams.ConnectionWeightMutationProbability);
            ngParams.AddNodeMutationProbability = ParseDouble(txtParamMutateAddNode, ngParams.AddNodeMutationProbability);
            ngParams.AddConnectionMutationProbability = ParseDouble(txtParamMutateAddConnection, ngParams.AddConnectionMutationProbability);
            ngParams.DeleteConnectionMutationProbability = ParseDouble(txtParamMutateDeleteConnection, ngParams.DeleteConnectionMutationProbability);
        }

        #endregion

        #region GUI Wiring [GUI State Updating]

        private void UpdateGuiState()
        {
            if(null == _ea)
            {
                if(null == _genomeList) {
                    UpdateGuiState_NoPopulation();
                }
                else {
                    UpdateGuiState_PopulationReady();
                }
            }
            else
            {
                switch(_ea.RunState)
                {
                    case RunState.Ready:
                    case RunState.Paused:
                        UpdateGuiState_EaReadyPaused();
                        break;
                    case RunState.Running:
                        UpdateGuiState_EaRunning();
                        break;
                    default:
                        throw new ApplicationException(string.Format("Unexpected RunState [{0}]", _ea.RunState));
                }
            }
        }

        private void UpdateGuiState_NoPopulation()
        {
            // Enable experiment selection and initialization buttons.
            cmbExperiments.Enabled = true;
            btnLoadDomainDefaults.Enabled = true;
            btnCreateRandomPop.Enabled = true;

            // Display population statuc (empty).
            txtPopulationStatus.Text = "Population not initialized";
            txtPopulationStatus.BackColor = Color.Red;

            // Disable search control buttons.
            btnSearchStart.Enabled = false;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = false;

            // Parameter fields enabled.
            txtParamPopulationSize.Enabled = true;
            txtParamInitialConnectionProportion.Enabled = true;
            txtParamElitismProportion.Enabled = true;
            txtParamSelectionProportion.Enabled = true;
            txtParamOffspringAsexual.Enabled = true;
            txtParamOffspringCrossover.Enabled = true;
            txtParamInterspeciesMating.Enabled = true;
            txtParamConnectionWeightRange.Enabled = true;
            txtParamMutateConnectionWeights.Enabled = true;
            txtParamMutateAddNode.Enabled = true;
            txtParamMutateAddConnection.Enabled = true;
            txtParamMutateDeleteConnection.Enabled = true;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar (file).
            loadPopulationToolStripMenuItem.Enabled = true;
            loadSeedGenomesToolStripMenuItem.Enabled = true;
            loadSeedGenomeToolStripMenuItem.Enabled = true;
            savePopulationToolStripMenuItem.Enabled = false;
            saveBestGenomeToolStripMenuItem.Enabled = false;
        }

        private void UpdateGuiState_PopulationReady()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnLoadDomainDefaults.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = string.Format("{0:D0} genomes ready", _genomeList.Count);
            txtPopulationStatus.BackColor = Color.Orange;

            // Enable search control buttons.
            btnSearchStart.Enabled = true;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = true;

            // Parameter fields enabled (apart from population creation params)
            txtParamPopulationSize.Enabled = false;
            txtParamInitialConnectionProportion.Enabled = false;
            txtParamElitismProportion.Enabled = true;
            txtParamSelectionProportion.Enabled = true;
            txtParamOffspringAsexual.Enabled = true;
            txtParamOffspringCrossover.Enabled = true;
            txtParamInterspeciesMating.Enabled = true;
            txtParamConnectionWeightRange.Enabled = true;
            txtParamMutateConnectionWeights.Enabled = true;
            txtParamMutateAddNode.Enabled = true;
            txtParamMutateAddConnection.Enabled = true;
            txtParamMutateDeleteConnection.Enabled = true;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar.
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = true;
            saveBestGenomeToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Evolution algorithm is ready/paused.
        /// </summary>
        private void UpdateGuiState_EaReadyPaused()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnLoadDomainDefaults.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = string.Format("{0:D0} genomes paused.", _genomeList.Count);
            txtPopulationStatus.BackColor = Color.Orange;

            // Search control buttons.
            btnSearchStart.Enabled = true;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = true;

            // Parameter fields (disable).
            txtParamPopulationSize.Enabled = false;
            txtParamInitialConnectionProportion.Enabled = false;
            txtParamElitismProportion.Enabled = false;
            txtParamSelectionProportion.Enabled = false;
            txtParamOffspringAsexual.Enabled = false;
            txtParamOffspringCrossover.Enabled = false;
            txtParamInterspeciesMating.Enabled = false;
            txtParamConnectionWeightRange.Enabled = false;
            txtParamMutateConnectionWeights.Enabled = false;
            txtParamMutateAddNode.Enabled = false;
            txtParamMutateAddConnection.Enabled = false;
            txtParamMutateDeleteConnection.Enabled = false;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar.
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = true;
            saveBestGenomeToolStripMenuItem.Enabled = (_ea.CurrentChampGenome != null);
        }

        /// <summary>
        /// Evolution algorithm is running.
        /// </summary>
        private void UpdateGuiState_EaRunning()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnLoadDomainDefaults.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = string.Format("{0:D0} genomes running", _genomeList.Count);
            txtPopulationStatus.BackColor = Color.LightGreen;

            // Search control buttons.
            btnSearchStart.Enabled = false;
            btnSearchStop.Enabled = true;
            btnSearchReset.Enabled = false;

            // Parameter fields (disable).
            txtParamPopulationSize.Enabled = false;
            txtParamInitialConnectionProportion.Enabled = false;
            txtParamElitismProportion.Enabled = false;
            txtParamSelectionProportion.Enabled = false;
            txtParamOffspringAsexual.Enabled = false;
            txtParamOffspringCrossover.Enabled = false;
            txtParamInterspeciesMating.Enabled = false;
            txtParamConnectionWeightRange.Enabled = false;
            txtParamMutateConnectionWeights.Enabled = false;
            txtParamMutateAddNode.Enabled = false;
            txtParamMutateAddConnection.Enabled = false;
            txtParamMutateDeleteConnection.Enabled = false;

            // Logging to file.
            gbxLogging.Enabled = false;

            // Menu bar.
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = false;
            saveBestGenomeToolStripMenuItem.Enabled = false;
        }

        private void UpdateGuiState_EaStats()
        {
            NeatAlgorithmStats stats = _ea.Statistics;
            txtSearchStatsMode.Text = _ea.ComplexityRegulationMode.ToString();
            switch( _ea.ComplexityRegulationMode)
            {
                case ComplexityRegulationMode.Complexifying:
                    txtSearchStatsMode.BackColor = Color.LightSkyBlue;
                    break;
                case ComplexityRegulationMode.Simplifying:
                    txtSearchStatsMode.BackColor = Color.Tomato;
                    break;
            }

            txtStatsGeneration.Text = _ea.CurrentGeneration.ToString("N0");
            txtStatsBest.Text = stats._maxFitness.ToString();
            txtStatsAlternativeFitness.Text = _ea.CurrentChampGenome.EvaluationInfo.AlternativeFitness.ToString("#.######");
            txtStatsMean.Text = stats._meanFitness.ToString("#.######");
            txtSpecieChampMean.Text = stats._meanSpecieChampFitness.ToString("#.######");
            txtStatsTotalEvals.Text = stats._totalEvaluationCount.ToString("N0");
            txtStatsEvalsPerSec.Text = stats._evaluationsPerSec.ToString("##,#.##");
            txtStatsBestGenomeComplx.Text = _ea.CurrentChampGenome.Complexity.ToString("N0");
            txtStatsMeanGenomeComplx.Text = stats._meanComplexity.ToString("#.##");
            txtStatsMaxGenomeComplx.Text = stats._maxComplexity.ToString("N0");

            ulong totalOffspringCount = stats._totalOffspringCount;
            txtStatsTotalOffspringCount.Text = totalOffspringCount.ToString("N0");
            txtStatsAsexualOffspringCount.Text = string.Format("{0:N0} ({1:P})", stats._asexualOffspringCount, ((double)stats._asexualOffspringCount/(double)totalOffspringCount));
            txtStatsCrossoverOffspringCount.Text = string.Format("{0:N0} ({1:P})", stats._sexualOffspringCount, ((double)stats._sexualOffspringCount/(double)totalOffspringCount));
            txtStatsInterspeciesOffspringCount.Text = string.Format("{0:N0} ({1:P})", stats._interspeciesOffspringCount, ((double)stats._interspeciesOffspringCount/(double)totalOffspringCount));
        }

        private void UpdateGuiState_ResetStats()
        {
            txtSearchStatsMode.Text = string.Empty;
            txtSearchStatsMode.BackColor = Color.LightSkyBlue;
            txtStatsGeneration.Text = string.Empty;
            txtStatsBest.Text = string.Empty;
            txtStatsAlternativeFitness.Text= string.Empty;
            txtStatsMean.Text = string.Empty;
            txtSpecieChampMean.Text = string.Empty;
            txtStatsTotalEvals.Text = string.Empty;
            txtStatsEvalsPerSec.Text = string.Empty;
            txtStatsBestGenomeComplx.Text =string.Empty;
            txtStatsMeanGenomeComplx.Text = string.Empty;
            txtStatsMaxGenomeComplx.Text = string.Empty;            
            txtStatsTotalOffspringCount.Text = string.Empty;
            txtStatsAsexualOffspringCount.Text = string.Empty;
            txtStatsCrossoverOffspringCount.Text = string.Empty;
            txtStatsInterspeciesOffspringCount.Text = string.Empty;
        }

        #endregion

        #region GUI Wiring [Menu Bar - Population & Genome Loading/Saving]

        private void loadPopulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string popFilePath = SelectFileToOpen("Load population", "pop.xml", "(*.pop.xml)|*.pop.xml");
            if(string.IsNullOrEmpty(popFilePath)) {
                return;
            }

            try
            {
                // Get the currently selected experiment.
                INeatExperiment experiment = GetSelectedExperiment();

                // Load population of genomes from file.
                List<NeatGenome> genomeList;
                using(XmlReader xr = XmlReader.Create(popFilePath)) 
                {
                    genomeList = experiment.LoadPopulation(xr);
                }

                if(genomeList.Count == 0) {
                    __log.WarnFormat("No genomes loaded from population file [{0}]", popFilePath);
                    return;
                }

                // Assign genome list and factory to class variables and update GUI.
                _genomeFactory = genomeList[0].GenomeFactory;
                _genomeList = genomeList;
                UpdateGuiState();
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error loading population. Error message [{0}]", ex.Message);
            }
        }

        private void loadSeedGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = SelectFileToOpen("Load seed genome", "gnm.xml", "(*.gnm.xml)|*.gnm.xml");
            if(string.IsNullOrEmpty(filePath)) {
                return;
            }

            // Parse population size from GUI field.
            int? popSize = ParseInt(txtParamPopulationSize);
            if(null == popSize) {
                return;
            }

            try
            {
                // Get the currently selected experiment.
                INeatExperiment experiment = GetSelectedExperiment();

                // Load genome from file.
                List<NeatGenome> genomeList;
                using(XmlReader xr = XmlReader.Create(filePath)) 
                {
                    genomeList = experiment.LoadPopulation(xr);
                }

                if(genomeList.Count == 0) {
                    __log.WarnFormat("No genome loaded from file [{0}]", filePath);
                    return;
                }

                // Create genome list from seed, assign to local variables and update GUI.
                _genomeFactory = genomeList[0].GenomeFactory;
                _genomeList = _genomeFactory.CreateGenomeList(popSize.Value, 0u, genomeList[0]);
                UpdateGuiState();
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error loading seed genome. Error message [{0}]", ex.Message);
            }
        }

        private void loadSeedGenomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string popFilePath = SelectFileToOpen("Load seed genomes", "pop.xml", "(*.pop.xml)|*.pop.xml");
            if(string.IsNullOrEmpty(popFilePath)) {
                return;
            }

            // Parse population size from GUI field.
            int? popSize = ParseInt(txtParamPopulationSize);
            if(null == popSize) {
                return;
            }

            try
            {
                // Get the currently selected experiment.
                INeatExperiment experiment = GetSelectedExperiment();

                // Load genome from file.
                List<NeatGenome> genomeList;
                using(XmlReader xr = XmlReader.Create(popFilePath)) 
                {
                    genomeList = experiment.LoadPopulation(xr);
                }

                if(genomeList.Count == 0) {
                    __log.WarnFormat("No seed genomes loaded from file [{0}]", popFilePath);
                    return;
                }

                // Create genome list from seed genomes, assign to local variables and update GUI.
                _genomeFactory = genomeList[0].GenomeFactory;
                _genomeList = _genomeFactory.CreateGenomeList(popSize.Value, 0u, genomeList);
                UpdateGuiState();
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error loading seed genomes. Error message [{0}]", ex.Message);
            }
        }

        private void savePopulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string popFilePath = SelectFileToSave("Save population", "pop.xml", "(*.pop.xml)|*.pop.xml");
            if(string.IsNullOrEmpty(popFilePath)) {
                return;
            }

            try
            {
                // Get the currently selected experiment.
                INeatExperiment experiment = GetSelectedExperiment();

                // Save genomes to xml file.
                using(XmlWriter xw = XmlWriter.Create(popFilePath, _xwSettings))
                {
                    experiment.SavePopulation(xw, _genomeList);
                }
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error saving population. Error message [{0}]", ex.Message);
            }
        }

        private void saveBestGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = SelectFileToSave("Save champion genome", "gnm.xml", "(*.gnm.xml)|*.gnm.xml");
            if(string.IsNullOrEmpty(filePath)) {
                return;
            }

            try
            {
                // Get the currently selected experiment.
                INeatExperiment experiment = GetSelectedExperiment();

                // Save genome to xml file.
                using(XmlWriter xw = XmlWriter.Create(filePath, _xwSettings))
                {
                    experiment.SavePopulation(xw, new NeatGenome[] {_ea.CurrentChampGenome});
                }
            }
            catch(Exception ex)
            {
                __log.ErrorFormat("Error saving genome. Error message [{0}]", ex.Message);
            }
        }

        #endregion

        #region GUI Wiring [Menu Bar - Views]

        private void bestGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGuiNeatExperiment experiment = GetSelectedExperiment();
            AbstractGenomeView genomeView = experiment.CreateGenomeView();
            if(null == genomeView) {
                return;
            }

            // Create form.
            _bestGenomeForm = new GenomeForm("Best Genome", genomeView, _ea);

            // Attach a event handler to update this main form when the genom form is closed.
            _bestGenomeForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _bestGenomeForm = null;
                bestGenomeToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            bestGenomeToolStripMenuItem.Enabled = false;

            // Show the form.
            _bestGenomeForm.Show(this);
            _bestGenomeForm.RefreshView();
        }

        private void problemDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGuiNeatExperiment experiment = GetSelectedExperiment();
            AbstractDomainView domainView = experiment.CreateDomainView();
            if(null == domainView) {
                return;
            }

            // Create form.
            _domainForm = new ProblemDomainForm(experiment.Name, domainView, _ea);

            // Attach a event handler to update this main form when the domain form is closed.
            _domainForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _domainForm = null;
                problemDomainToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            problemDomainToolStripMenuItem.Enabled = false;

            // Show the form.
            _domainForm.Show(this);
            _domainForm.RefreshView();
        }

        #endregion

        #region GUI Wiring [Menu Bar - Views - Graphs]

        private void fitnessBestMeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create data sources.
            TimeSeriesDataSource dsBestFitness = new TimeSeriesDataSource("Best", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Red, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._maxFitness);
                                                            });

            TimeSeriesDataSource dsMeanFitness = new TimeSeriesDataSource("Mean", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Black, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._meanFitness);
                                                            });

            TimeSeriesDataSource dsBestFitnessMA = new TimeSeriesDataSource("Best (Moving Average)", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Orange, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._bestFitnessMA.Mean);
                                                            });

            // Create form.
            TimeSeriesGraphForm graphForm = new TimeSeriesGraphForm("Fitness (Best and Mean)", "Generation", "Fitness", string.Empty,
                                                 new TimeSeriesDataSource[] {dsBestFitness, dsMeanFitness, dsBestFitnessMA}, _ea);
            _timeSeriesGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _timeSeriesGraphFormList.Remove(senderObj as TimeSeriesGraphForm);
                fitnessBestMeansToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            fitnessBestMeansToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }        

        private void complexityBestMeansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create data sources.
            TimeSeriesDataSource dsBestCmplx = new TimeSeriesDataSource("Best", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Red, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.CurrentChampGenome.Complexity);
                                                            });

            TimeSeriesDataSource dsMeanCmplx = new TimeSeriesDataSource("Mean", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Black, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._meanComplexity);
                                                            });

            TimeSeriesDataSource dsMeanCmplxMA = new TimeSeriesDataSource("Mean (Moving Average)", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Orange, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._complexityMA.Mean);
                                                            });

            // Create form.
            TimeSeriesGraphForm graphForm = new TimeSeriesGraphForm("Complexity (Best and Mean)", "Generation", "Complexity", string.Empty,
                                                 new TimeSeriesDataSource[] {dsBestCmplx, dsMeanCmplx, dsMeanCmplxMA}, _ea);
            _timeSeriesGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _timeSeriesGraphFormList.Remove(senderObj as TimeSeriesGraphForm);
                complexityBestMeansToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            complexityBestMeansToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void evaluationsPerSecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create data sources.
            TimeSeriesDataSource dsEvalsPerSec= new TimeSeriesDataSource("Evals Per Sec", TimeSeriesDataSource.DefaultHistoryLength, 0, Color.Black, delegate() 
                                                            {
                                                                return new Point2DDouble(_ea.CurrentGeneration, _ea.Statistics._evaluationsPerSec);
                                                            });
            // Create form.
            TimeSeriesGraphForm graphForm = new TimeSeriesGraphForm("Evaluations Per Second", "Generation", "Evaluations", string.Empty,
                                                 new TimeSeriesDataSource[] {dsEvalsPerSec}, _ea);
            _timeSeriesGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _timeSeriesGraphFormList.Remove(senderObj as TimeSeriesGraphForm);
                evaluationsPerSecToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            evaluationsPerSecToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieSizeByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieSizeRank = new SummaryDataSource("Specie Size", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArrayInt || _specieDataArrayInt.Length != specieCount) {
                                            _specieDataArrayInt = new int[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArrayInt[i] = _ea.SpecieList[i].GenomeList.Count;
                                        }

                                        // Build/create _specieSizePointArray from the _specieSizeArray.
                                        UpdateRankedDataPoints(_specieDataArrayInt, ref _specieDataPointArrayInt);

                                        // Return plot points.
                                        return _specieDataPointArrayInt;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Size by Rank", "Species (largest to smallest)", "Size", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieSizeRank}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieSizeByRankToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieSizeByRankToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieChampFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieChampFitnessRank = new SummaryDataSource("Specie Fitness (Champs)", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie fitnesses into the data array.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].GenomeList[0].EvaluationInfo.Fitness;
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_specieDataArray, ref _specieDataPointArray);

                                        // Return plot points.
                                        return _specieDataPointArray;
                                    });

            SummaryDataSource dsSpecieMeanFitnessRank = new SummaryDataSource("Specie Fitness (Means)", 0, Color.Black, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie fitnesses into the data array.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].CalcMeanFitness();
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_specieDataArray, ref _specieDataPointArray);

                                        // Return plot points.
                                        return _specieDataPointArray;
                                    });


            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Fitness by Rank", "Species", "Fitness", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieChampFitnessRank, dsSpecieMeanFitnessRank}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieChampFitnessByRankToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieChampFitnessByRankToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieChampComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieChampComplexityRank = new SummaryDataSource("Specie Complexity (Champs)", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie complexity values into the data array.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].GenomeList[0].Complexity;
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_specieDataArray, ref _specieDataPointArray);

                                        // Return plot points.
                                        return _specieDataPointArray;
                                    });

            SummaryDataSource dsSpecieMeanComplexityRank = new SummaryDataSource("Specie Complexity (Means)", 0, Color.Black, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie complexity values into the data array.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].CalcMeanComplexity();
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_specieDataArray, ref _specieDataPointArray);

                                        // Return plot points.
                                        return _specieDataPointArray;
                                    });


            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Complexity by Rank", "Species", "Complexity", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieChampComplexityRank, dsSpecieMeanComplexityRank}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieChampComplexityByRankToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieChampComplexityByRankToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void genomeFitnessByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsGenomeFitnessRank = new SummaryDataSource("Genome Fitness", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int genomeCount = _ea.GenomeList.Count;
                                        if(null == _genomeDataArray || _genomeDataArray.Length != genomeCount) {
                                            _genomeDataArray = new double[genomeCount];
                                        }

                                        // Copy genome fitness values into the data array.
                                        for(int i=0; i<genomeCount; i++) {
                                            _genomeDataArray[i] = _ea.GenomeList[i].EvaluationInfo.Fitness;
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_genomeDataArray, ref _genomeDataPointArray);

                                        // Return plot points.
                                        return _genomeDataPointArray;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Genome Fitness by Rank", "Genomes", "Fitness", string.Empty,
                                                 new SummaryDataSource[] {dsGenomeFitnessRank}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                genomeFitnessByRankToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            genomeFitnessByRankToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void genomeComplexityByRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsGenomeComplexityRank = new SummaryDataSource("Genome Complexity", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int genomeCount = _ea.GenomeList.Count;
                                        if(null == _genomeDataArray || _genomeDataArray.Length != genomeCount) {
                                            _genomeDataArray = new double[genomeCount];
                                        }

                                        // Copy genome complexity values into the data array.
                                        for(int i=0; i<genomeCount; i++) {
                                            _genomeDataArray[i] = _ea.GenomeList[i].Complexity;
                                        }

                                        // Build/create point array.
                                        UpdateRankedDataPoints(_genomeDataArray, ref _genomeDataPointArray);

                                        // Return plot points.
                                        return _genomeDataPointArray;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Genome Complexity by Rank", "Genomes", "Complexity", string.Empty,
                                                 new SummaryDataSource[] {dsGenomeComplexityRank}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                genomeComplexityByRankToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            genomeComplexityByRankToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieSizeDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieSizeDist = new SummaryDataSource("Specie Size Distribution", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].GenomeList.Count;
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_specieDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Size Frequency Distribution", "Species Size", "Frequency", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieSizeDist}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieSizeDistributionToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieSizeDistributionToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieFitnessDistributionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieChampFitnessDist = new SummaryDataSource("Specie Fitness Distribution (Champ)", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].GenomeList[0].EvaluationInfo.Fitness;
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_specieDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });

            SummaryDataSource dsSpecieMeanFitnessDist = new SummaryDataSource("Specie Fitness Distribution (Mean)", 0, Color.Black, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].CalcMeanFitness();
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_specieDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Fitness Distribution", "Fitness", "Frequency", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieChampFitnessDist, dsSpecieMeanFitnessDist}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieFitnessDistributionsToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieFitnessDistributionsToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void specieComplexityDistributionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsSpecieChampComplexityDist = new SummaryDataSource("Specie Complexity Distribution (Champ)", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].GenomeList[0].Complexity;
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_specieDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });

            SummaryDataSource dsSpecieMeanComplexityDist = new SummaryDataSource("Specie Complexity Distribution (Mean)", 0, Color.Black, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int specieCount = _ea.SpecieList.Count;
                                        if(null == _specieDataArray || _specieDataArray.Length != specieCount) {
                                            _specieDataArray = new double[specieCount];
                                        }

                                        // Copy specie sizes into _specieSizeArray.
                                        for(int i=0; i<specieCount; i++) {
                                            _specieDataArray[i] = _ea.SpecieList[i].CalcMeanComplexity();
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_specieDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Specie Complexity Distribution", "Complexity", "Frequency", string.Empty,
                                                 new SummaryDataSource[] {dsSpecieChampComplexityDist, dsSpecieMeanComplexityDist}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                specieFitnessDistributionsToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            specieFitnessDistributionsToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void genomeFitnessDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsGenomeFitnessDist = new SummaryDataSource("Genome Fitness Distribution", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int genomeCount = _ea.GenomeList.Count;
                                        if(null == _genomeDataArray || _genomeDataArray.Length != genomeCount) {
                                            _genomeDataArray = new double[genomeCount];
                                        }

                                        // Copy genome fitness values into the data array.
                                        for(int i=0; i<genomeCount; i++) {
                                            _genomeDataArray[i] = _ea.GenomeList[i].EvaluationInfo.Fitness;
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_genomeDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Genome Fitness Distribution", "Fitness", "Frequency", string.Empty,
                                                 new SummaryDataSource[] {dsGenomeFitnessDist}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                genomeFitnessDistributionToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            genomeFitnessDistributionToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        private void genomeComplexityDistributionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryDataSource dsGenomeComplexityDist = new SummaryDataSource("Genome Complexity Distribution", 0, Color.Red, delegate()
                                    {
                                        // Ensure temp working storage is ready.
                                        int genomeCount = _ea.GenomeList.Count;
                                        if(null == _genomeDataArray || _genomeDataArray.Length != genomeCount) {
                                            _genomeDataArray = new double[genomeCount];
                                        }

                                        // Copy genome fitness values into the data array.
                                        for(int i=0; i<genomeCount; i++) {
                                            _genomeDataArray[i] = _ea.GenomeList[i].Complexity;
                                        }

                                        // Calculate a frequency distribution and retrieve it as an array of plottable points.
                                        Point2DDouble[] pointArr = CalcDistributionDataPoints(_genomeDataArray);

                                        // Return plot points.
                                        return pointArr;
                                    });
            // Create form.
            SummaryGraphForm graphForm = new SummaryGraphForm("Genome Complexity Distribution", "Complexity", "Frequency", string.Empty,
                                                 new SummaryDataSource[] {dsGenomeComplexityDist}, _ea);
            _summaryGraphFormList.Add(graphForm);

            // Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _summaryGraphFormList.Remove(senderObj as SummaryGraphForm);
                genomeComplexityDistributionToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            genomeComplexityDistributionToolStripMenuItem.Enabled = false;

            // Show the form.
            graphForm.Show(this);
        }

        #endregion

        #region GUI Wiring [Misc Menu Bar & Button Event Handlers]

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAboutBox = new AboutForm();
            frmAboutBox.ShowDialog(this);
        }

        private void btnCopyLogToClipboard_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Logger.LogItem item in lbxLog.Items) {
                sb.AppendLine(item.Message);
            }

            if(sb.Length > 0) {
                Clipboard.SetText(sb.ToString());
            }
        }

        #endregion

        #region GUI Wiring [Update/Pause Event Handlers + Form Close Handler]

        void _ea_UpdateEvent(object sender, EventArgs e)
        {
            // Handle writing to log window. Switch execution to GUI thread if necessary.
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate() 
                {
                    // Update stats on screen.
                    UpdateGuiState_EaStats();

                    // Write entry to log window.
                    __log.Info(string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, _ea.Statistics._maxFitness));

                    // Check if we should save the champ genome to a file.
                    NeatGenome champGenome = _ea.CurrentChampGenome;
                    if(chkFileSaveGenomeOnImprovement.Checked && champGenome.EvaluationInfo.Fitness > _champGenomeFitness) 
                    {
                        _champGenomeFitness = champGenome.EvaluationInfo.Fitness;
                        string filename = string.Format(_filenameNumberFormatter, "{0}_{1:0.00}_{2:yyyyMMdd_HHmmss}.gnm.xml",
                                                        txtFileBaseName.Text, _champGenomeFitness, DateTime.Now);

                        // Get the currently selected experiment.
                        INeatExperiment experiment = GetSelectedExperiment();

                        // Save genome to xml file.
                        using(XmlWriter xw = XmlWriter.Create(filename, _xwSettings))
                        {
                            experiment.SavePopulation(xw, new NeatGenome[] {champGenome});
                        }
                    }
                }));
            }

            // Handle writing to log file.
            if(null != _logFileWriter)
            {
                NeatAlgorithmStats stats = _ea.Statistics;
                _logFileWriter.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                        DateTime.Now,
                                                        stats._generation,
                                                        stats._maxFitness,
                                                        stats._meanFitness,
                                                        stats._meanSpecieChampFitness,
                                                        _ea.CurrentChampGenome.Complexity,
                                                        stats._meanComplexity,
                                                        stats._maxComplexity,
                                                        stats._totalEvaluationCount,
                                                        stats._evaluationsPerSec,
                                                        _ea.ComplexityRegulationMode));
                _logFileWriter.Flush();
            }
        }

        void _ea_PausedEvent(object sender, EventArgs e)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate() 
                {
                    UpdateGuiState();
                }));
            }
        }

        /// <summary>
        /// Gracefully handle application exit request.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(null != _ea && _ea.RunState == RunState.Running)
            {
                DialogResult result = MessageBox.Show("Evolution algorithm is still running. Exit anyway?", "Exit?", MessageBoxButtons.YesNo);
                if(result == DialogResult.No)
                {   // Cancel closing of application.
                    e.Cancel = true;
                    return;
                }
            }

            if(null != _ea)
            {
                // Detach event handlers to prevent logging attempts to GUI as it is being torn down.
                _ea.UpdateEvent -= new EventHandler(_ea_UpdateEvent);
                _ea.PausedEvent -= new EventHandler(_ea_PausedEvent);

                if(_ea.RunState == RunState.Running)
                {
                    // Request algorithm to stop but don't wait.
                    _ea.RequestPause();
                }

                // Close log file.
                if(null != _logFileWriter)
                {
                    _logFileWriter.Close();
                    _logFileWriter = null;
                }
            }
        }

        #endregion

        #region Private Methods [Misc Helper Methods]

		/// <summary>
		/// Ask the user for a filename / path.
		/// </summary>
		private string SelectFileToOpen(string dialogTitle, string fileExtension, string filter)
		{
			OpenFileDialog oDialog = new OpenFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = fileExtension;
            oDialog.Filter = filter;
			oDialog.Title = dialogTitle;
			oDialog.RestoreDirectory = true;

            // Show dialog and block until user selects a file.
			if(oDialog.ShowDialog() == DialogResult.OK) {
				return oDialog.FileName;
            } 
            // No selection.
            return null;
		}

		/// <summary>
		/// Ask the user for a filename / path.
		/// </summary>
		private string SelectFileToSave(string dialogTitle, string fileExtension, string filter)
		{
			SaveFileDialog oDialog = new SaveFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = fileExtension;
            oDialog.Filter = filter;
			oDialog.Title = dialogTitle;
			oDialog.RestoreDirectory = true;

            // Show dialog and block until user selects a file.
			if(oDialog.ShowDialog() == DialogResult.OK) {
				return oDialog.FileName;
            } 
            // No selection.
            return null;
		}

        private int? ParseInt(TextBox txtBox)
        {
            int val;
            if(int.TryParse(txtBox.Text, out val))
            {
                return val;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return null;
        }

        private int ParseInt(TextBox txtBox, int defaultVal)
        {
            int val;
            if(int.TryParse(txtBox.Text, out val))
            {
                return val;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }

        private double? ParseDouble(TextBox txtBox)
        {
            double val;
            if(double.TryParse(txtBox.Text, out val))
            {
                return val;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return null;
        }

        private double ParseDouble(TextBox txtBox, double defaultVal)
        {
            double val;
            if(double.TryParse(txtBox.Text, out val))
            {
                return val;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }

        /// <summary>
        /// Updates an Point2DDouble array by sorting an array of values and copying the sorted values over the existing values in pointArr.
        /// Optionally creates the Point2DDouble array if it is null or is the wrong size.
        /// </summary>
        private void UpdateRankedDataPoints(int[] valArr, ref Point2DDouble[] pointArr)
        {

            // Sort values (largest first).
            Array.Sort(valArr, delegate(int x, int y)
            {
                if(x > y) {
                    return -1;
                }
                if(x < y) {
                    return 1;
                }
                return 0;
            });

            // Ensure point cache is ready.
            if(null == pointArr || pointArr.Length != valArr.Length)
            {
                pointArr = new Point2DDouble[valArr.Length];
                for(int i=0; i<valArr.Length; i++)  
                {
                    pointArr[i].X = i;
                    pointArr[i].Y = valArr[i];
                }
            }
            else
            {   // Copy sorted values into _specieSizePointArray.
                for(int i=0; i<valArr.Length; i++) {
                    pointArr[i].Y = valArr[i];
                }
            }
        }

        /// <summary>
        /// Updates an Point2DDouble array by sorting an array of values and copying the sorted values over the existing values in pointArr.
        /// Optionally creates the Point2DDouble array if it is null or is the wrong size.
        /// </summary>
        private void UpdateRankedDataPoints(double[] valArr, ref Point2DDouble[] pointArr)
        {

            // Sort values (largest first).
            Array.Sort(valArr, delegate(double x, double y)
            {
                if(x > y) {
                    return -1;
                }
                if(x < y) {
                    return 1;
                }
                return 0;
            });

            // Ensure point cache is ready.
            if(null == pointArr || pointArr.Length != valArr.Length)
            {
                pointArr = new Point2DDouble[valArr.Length];
                for(int i=0; i<valArr.Length; i++)  
                {
                    pointArr[i].X = i;
                    pointArr[i].Y = valArr[i];
                }
            }
            else
            {   // Copy sorted values into _specieSizePointArray.
                for(int i=0; i<valArr.Length; i++) {
                    pointArr[i].Y = valArr[i];
                }
            }
        }









        private Point2DDouble[] CalcDistributionDataPoints(double[] valArr)
        {
            // Square root is a fairly good choice for automatically determining the category count based on number of values being analysed.
            // See http://en.wikipedia.org/wiki/Histogram (section: Number of bins and width).
            int categoryCount = (int)Math.Sqrt(valArr.Length);
            FrequencyDistributionData fdd = Utilities.CalculateDistribution(valArr, categoryCount);

            // Create array of distribution plot points.
            Point2DDouble[] pointArr = new Point2DDouble[fdd.FrequencyArray.Length];
            double incr = fdd.Increment;
            double x = fdd.Min + (incr/2.0);

            for(int i=0; i<fdd.FrequencyArray.Length; i++, x+=incr)
            {
                pointArr[i].X = x;
                pointArr[i].Y = fdd.FrequencyArray[i];
            }
            return pointArr;
        }

        #endregion
    }
}
