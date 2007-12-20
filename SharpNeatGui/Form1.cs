using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Xml;
using System.Globalization;
using System.Runtime.Remoting;

using SharpNeatLib;
using SharpNeatLib.AppConfig;
using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using SharpNeatLib.Xml;

namespace SharpNeat
{
	public class Form1 : System.Windows.Forms.Form
	{
		delegate void MessageDelegate(string message);

		#region Enumerations

		enum SearchStateEnum
		{
			Reset,
			Paused,
			Running
		}

		#endregion

		#region Class Variables

		SearchStateEnum searchState = SearchStateEnum.Reset;
		
		ExperimentConfigInfo[] experimentConfigInfoArray = null;
		IExperiment selectedExperiment=null;
		ExperimentConfigInfo selectedExperimentConfigInfo = null;
		
		IActivationFunction selectedActivationFunction=null;
		EvolutionAlgorithm ea;
		Population pop;
		Thread searchThread;
		long ticksAtSearchStart;
		NumberFormatInfo nfi;

		int evaluationsPerSec;

		bool stopSearchSignal;


		BestGenomeForm bestGenomeForm=null;
		SpeciesForm speciesForm=null;
		AbstractExperimentView experimentView=null;
		ProgressForm progressForm=null;
		ActivationFunctionForm activationFunctionForm=null;

		StreamWriter logWriter = null;

		object guiThreadLockObject = new object();

		
		/// <summary>
		/// Update Frequency (Generations).
		/// </summary>
		ulong updateFreqGens=1;

		/// <summary>
		/// Update Frequency (100 nanosecond ticks).
		/// </summary>
		long updateFreqTicks=10000000; // 1 second.

		/// <summary>
		/// Update mode. false=generations, true=seconds.
		/// </summary>
		bool updateMode = true;

		#endregion

		#region Windows Form Designer Variables

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox gbxLog;
		private System.Windows.Forms.TextBox txtLogWindow;
		private System.Windows.Forms.GroupBox gbxCurrentStats;
		private System.Windows.Forms.TextBox txtStatsBest;
		private System.Windows.Forms.TextBox txtStatsMean;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtStatsGeneration;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtStatsSpeciesCount;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtStatsCompatibilityThreshold;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtStatsTotalEvaluations;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtDomainOutputNeuronCount;
		private System.Windows.Forms.TextBox txtDomainInputNeuronCount;
		private System.Windows.Forms.ComboBox cmbDomain;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.TextBox txtParamSpeciesDropoffAge;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.TextBox txtParamTargetSpeciesCountMax;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.TextBox txtParamTargetSpeciesCountMin;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.TextBox txtParamSelectionProportion;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.TextBox txtParamElitismProportion;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.TextBox txtParamMutateConnectionWeights;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.TextBox txtParamMutateAddNode;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.TextBox txtParamMutateAddConnection;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtParamCompatDisjointCoeff;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.TextBox txtParamCompatExcessCoeff;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.TextBox txtParamCompatWeightDeltaCoeff;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TextBox txtParamCompatThreshold;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtParamOffspringCrossover;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox txtParamOffspringMutation;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.TextBox txtParamPopulationSize;
		private System.Windows.Forms.GroupBox gbxSearchParameters;
		private System.Windows.Forms.GroupBox gbxFile;
		private System.Windows.Forms.TextBox txtFileBaseName;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Button btnSearchStart;
		private System.Windows.Forms.Button btnSearchStop;
		private System.Windows.Forms.Button btnSearchReset;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtParamInterspeciesMating;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.MenuItem mnuFileSaveBestAsNetwork;
		private System.Windows.Forms.MenuItem mnuFileSaveBestAsGenome;
		private System.Windows.Forms.MenuItem mnuFileSavePopulation;
		private System.Windows.Forms.MenuItem mnuInitPopLoad;
		private System.Windows.Forms.MenuItem mnuInitPopLoadPopulation;
		private System.Windows.Forms.MenuItem mnuInitPopLoadSeedGenome;
		private System.Windows.Forms.MenuItem mnuInitPopLoadSeedPopulation;
		private System.Windows.Forms.MenuItem mnuInitPop;
		private System.Windows.Forms.MenuItem mnuInitPopAutoGenerate;
		private System.Windows.Forms.Button btnExperimentInfo;
		private System.Windows.Forms.Button btnLoadDefaults;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txtParamConnectionWeightMutationSigma;
		private System.Windows.Forms.TextBox txtParamConnectionWeightRange;
		private System.Windows.Forms.TextBox txtStatsBestGenomeLength;
		private System.Windows.Forms.TextBox txtStatsMeanGenomeLength;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox chkFileSaveGenomeOnImprovement;
		private System.Windows.Forms.TextBox txtStatsEvaluationsPerSec;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.MenuItem mnuFileSave;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.ComboBox cmbExperimentActivationFn;
		private System.Windows.Forms.TextBox txtExperimentActivationFn;
		private System.Windows.Forms.MenuItem mnuVisualization;
		private System.Windows.Forms.MenuItem mnuVisualizationBest;
		private System.Windows.Forms.MenuItem mnuVisualizationSpecies;
		private System.Windows.Forms.MenuItem mnuVisualizationExperiment;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.TextBox txtFileLogBaseName;
		private System.Windows.Forms.CheckBox chkFileWriteLog;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.TextBox txtStatsMode;
		private System.Windows.Forms.CheckBox chkParamPruningModeEnabled;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.CheckBox chkParamEnableConnectionWeightFixing;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.TextBox txtParamPruningBeginFitnessStagnationThreshold;
		private System.Windows.Forms.TextBox txtParamPruningBeginComplexityThreshold;
		private System.Windows.Forms.TextBox txtParamPruningEndComplexityStagnationThreshold;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.TextBox txtParamMutateDeleteConnection;
		private System.Windows.Forms.TextBox txtParamMutateDeleteNeuron;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency1Sec;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency2Sec;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency5Sec;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency10Sec;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency1Gen;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency2Gen;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency5Gen;
		private System.Windows.Forms.MenuItem mnuViewUpdateFrequency10Gen;
		private System.Windows.Forms.TextBox txtStatsEvaluatorStateMsg;
		private System.Windows.Forms.MenuItem mnuVisualizationProgressGraph;
        private System.Windows.Forms.Button btnPlotFunction;

		#endregion
        private CheckBox chkParamPruningModeAlwaysOn;
        private IContainer components;

		#region Constructor / Disposal

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitialiseForm();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	
		#endregion
			
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnPlotFunction = new System.Windows.Forms.Button();
            this.txtExperimentActivationFn = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.cmbExperimentActivationFn = new System.Windows.Forms.ComboBox();
            this.btnLoadDefaults = new System.Windows.Forms.Button();
            this.btnExperimentInfo = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDomainOutputNeuronCount = new System.Windows.Forms.TextBox();
            this.txtDomainInputNeuronCount = new System.Windows.Forms.TextBox();
            this.cmbDomain = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnSearchReset = new System.Windows.Forms.Button();
            this.btnSearchStop = new System.Windows.Forms.Button();
            this.btnSearchStart = new System.Windows.Forms.Button();
            this.gbxFile = new System.Windows.Forms.GroupBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtFileLogBaseName = new System.Windows.Forms.TextBox();
            this.chkFileWriteLog = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtFileBaseName = new System.Windows.Forms.TextBox();
            this.chkFileSaveGenomeOnImprovement = new System.Windows.Forms.CheckBox();
            this.gbxCurrentStats = new System.Windows.Forms.GroupBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txtStatsMode = new System.Windows.Forms.TextBox();
            this.txtStatsEvaluationsPerSec = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtStatsMeanGenomeLength = new System.Windows.Forms.TextBox();
            this.txtStatsBestGenomeLength = new System.Windows.Forms.TextBox();
            this.txtStatsTotalEvaluations = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.txtStatsEvaluatorStateMsg = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtStatsCompatibilityThreshold = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStatsGeneration = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStatsSpeciesCount = new System.Windows.Forms.TextBox();
            this.txtStatsMean = new System.Windows.Forms.TextBox();
            this.txtStatsBest = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbxSearchParameters = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkParamPruningModeAlwaysOn = new System.Windows.Forms.CheckBox();
            this.label41 = new System.Windows.Forms.Label();
            this.txtParamPruningEndComplexityStagnationThreshold = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtParamPruningBeginFitnessStagnationThreshold = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.txtParamPruningBeginComplexityThreshold = new System.Windows.Forms.TextBox();
            this.chkParamEnableConnectionWeightFixing = new System.Windows.Forms.CheckBox();
            this.chkParamPruningModeEnabled = new System.Windows.Forms.CheckBox();
            this.txtParamPopulationSize = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtParamConnectionWeightMutationSigma = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtParamConnectionWeightRange = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label29 = new System.Windows.Forms.Label();
            this.txtParamSpeciesDropoffAge = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.txtParamTargetSpeciesCountMax = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txtParamTargetSpeciesCountMin = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtParamSelectionProportion = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtParamElitismProportion = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label43 = new System.Windows.Forms.Label();
            this.txtParamMutateDeleteNeuron = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.txtParamMutateDeleteConnection = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.txtParamMutateConnectionWeights = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txtParamMutateAddNode = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtParamMutateAddConnection = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtParamCompatDisjointCoeff = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtParamCompatExcessCoeff = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtParamCompatWeightDeltaCoeff = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtParamCompatThreshold = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtParamInterspeciesMating = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtParamOffspringCrossover = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtParamOffspringMutation = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gbxLog = new System.Windows.Forms.GroupBox();
            this.txtLogWindow = new System.Windows.Forms.TextBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuFileSave = new System.Windows.Forms.MenuItem();
            this.mnuFileSavePopulation = new System.Windows.Forms.MenuItem();
            this.mnuFileSaveBestAsNetwork = new System.Windows.Forms.MenuItem();
            this.mnuFileSaveBestAsGenome = new System.Windows.Forms.MenuItem();
            this.mnuInitPop = new System.Windows.Forms.MenuItem();
            this.mnuInitPopLoad = new System.Windows.Forms.MenuItem();
            this.mnuInitPopLoadPopulation = new System.Windows.Forms.MenuItem();
            this.mnuInitPopLoadSeedGenome = new System.Windows.Forms.MenuItem();
            this.mnuInitPopLoadSeedPopulation = new System.Windows.Forms.MenuItem();
            this.mnuInitPopAutoGenerate = new System.Windows.Forms.MenuItem();
            this.mnuView = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency1Sec = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency2Sec = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency5Sec = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency10Sec = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency1Gen = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency2Gen = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency5Gen = new System.Windows.Forms.MenuItem();
            this.mnuViewUpdateFrequency10Gen = new System.Windows.Forms.MenuItem();
            this.mnuVisualization = new System.Windows.Forms.MenuItem();
            this.mnuVisualizationProgressGraph = new System.Windows.Forms.MenuItem();
            this.mnuVisualizationBest = new System.Windows.Forms.MenuItem();
            this.mnuVisualizationSpecies = new System.Windows.Forms.MenuItem();
            this.mnuVisualizationExperiment = new System.Windows.Forms.MenuItem();
            this.mnuAbout = new System.Windows.Forms.MenuItem();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.gbxFile.SuspendLayout();
            this.gbxCurrentStats.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbxSearchParameters.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gbxLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 408);
            this.panel1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(464, 408);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.gbxFile);
            this.tabPage1.Controls.Add(this.gbxCurrentStats);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(456, 382);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Page 1";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnPlotFunction);
            this.groupBox5.Controls.Add(this.txtExperimentActivationFn);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.cmbExperimentActivationFn);
            this.groupBox5.Controls.Add(this.btnLoadDefaults);
            this.groupBox5.Controls.Add(this.btnExperimentInfo);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtDomainOutputNeuronCount);
            this.groupBox5.Controls.Add(this.txtDomainInputNeuronCount);
            this.groupBox5.Controls.Add(this.cmbDomain);
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(224, 240);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Domain / Experiment";
            // 
            // btnPlotFunction
            // 
            this.btnPlotFunction.Font = new System.Drawing.Font("Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPlotFunction.Location = new System.Drawing.Point(200, 88);
            this.btnPlotFunction.Name = "btnPlotFunction";
            this.btnPlotFunction.Size = new System.Drawing.Size(19, 21);
            this.btnPlotFunction.TabIndex = 52;
            this.btnPlotFunction.Text = "¼";
            this.btnPlotFunction.Click += new System.EventHandler(this.btnPlotFunction_Click);
            // 
            // txtExperimentActivationFn
            // 
            this.txtExperimentActivationFn.Location = new System.Drawing.Point(8, 120);
            this.txtExperimentActivationFn.Name = "txtExperimentActivationFn";
            this.txtExperimentActivationFn.ReadOnly = true;
            this.txtExperimentActivationFn.Size = new System.Drawing.Size(208, 20);
            this.txtExperimentActivationFn.TabIndex = 51;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(8, 72);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(120, 12);
            this.label21.TabIndex = 50;
            this.label21.Text = "Activation Fn";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbExperimentActivationFn
            // 
            this.cmbExperimentActivationFn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExperimentActivationFn.Location = new System.Drawing.Point(8, 88);
            this.cmbExperimentActivationFn.Name = "cmbExperimentActivationFn";
            this.cmbExperimentActivationFn.Size = new System.Drawing.Size(192, 21);
            this.cmbExperimentActivationFn.TabIndex = 49;
            this.cmbExperimentActivationFn.SelectedIndexChanged += new System.EventHandler(this.cmbExperimentActivationFn_SelectedIndexChanged);
            // 
            // btnLoadDefaults
            // 
            this.btnLoadDefaults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadDefaults.Location = new System.Drawing.Point(8, 40);
            this.btnLoadDefaults.Name = "btnLoadDefaults";
            this.btnLoadDefaults.Size = new System.Drawing.Size(192, 24);
            this.btnLoadDefaults.TabIndex = 48;
            this.btnLoadDefaults.Text = "Load Default Search Parameters";
            this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
            // 
            // btnExperimentInfo
            // 
            this.btnExperimentInfo.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExperimentInfo.Location = new System.Drawing.Point(200, 16);
            this.btnExperimentInfo.Name = "btnExperimentInfo";
            this.btnExperimentInfo.Size = new System.Drawing.Size(19, 21);
            this.btnExperimentInfo.TabIndex = 47;
            this.btnExperimentInfo.Text = "?";
            this.btnExperimentInfo.Click += new System.EventHandler(this.btnDomainExplanation_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(64, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 16);
            this.label8.TabIndex = 43;
            this.label8.Text = "Output Neurons";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 42;
            this.label1.Text = "Input Neurons";
            // 
            // txtDomainOutputNeuronCount
            // 
            this.txtDomainOutputNeuronCount.Location = new System.Drawing.Point(8, 168);
            this.txtDomainOutputNeuronCount.Name = "txtDomainOutputNeuronCount";
            this.txtDomainOutputNeuronCount.ReadOnly = true;
            this.txtDomainOutputNeuronCount.Size = new System.Drawing.Size(56, 20);
            this.txtDomainOutputNeuronCount.TabIndex = 38;
            // 
            // txtDomainInputNeuronCount
            // 
            this.txtDomainInputNeuronCount.Location = new System.Drawing.Point(8, 144);
            this.txtDomainInputNeuronCount.Name = "txtDomainInputNeuronCount";
            this.txtDomainInputNeuronCount.ReadOnly = true;
            this.txtDomainInputNeuronCount.Size = new System.Drawing.Size(56, 20);
            this.txtDomainInputNeuronCount.TabIndex = 37;
            // 
            // cmbDomain
            // 
            this.cmbDomain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDomain.Location = new System.Drawing.Point(8, 16);
            this.cmbDomain.Name = "cmbDomain";
            this.cmbDomain.Size = new System.Drawing.Size(192, 21);
            this.cmbDomain.TabIndex = 36;
            this.cmbDomain.SelectedIndexChanged += new System.EventHandler(this.cmbDomain_SelectedIndexChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnSearchReset);
            this.groupBox6.Controls.Add(this.btnSearchStop);
            this.groupBox6.Controls.Add(this.btnSearchStart);
            this.groupBox6.Location = new System.Drawing.Point(232, 0);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(224, 56);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Search Control";
            // 
            // btnSearchReset
            // 
            this.btnSearchReset.Location = new System.Drawing.Point(152, 16);
            this.btnSearchReset.Name = "btnSearchReset";
            this.btnSearchReset.Size = new System.Drawing.Size(64, 32);
            this.btnSearchReset.TabIndex = 2;
            this.btnSearchReset.Text = "Reset";
            this.btnSearchReset.Click += new System.EventHandler(this.btnSearchReset_Click);
            // 
            // btnSearchStop
            // 
            this.btnSearchStop.Location = new System.Drawing.Point(80, 16);
            this.btnSearchStop.Name = "btnSearchStop";
            this.btnSearchStop.Size = new System.Drawing.Size(64, 32);
            this.btnSearchStop.TabIndex = 1;
            this.btnSearchStop.Text = "Stop / Pause";
            this.btnSearchStop.Click += new System.EventHandler(this.btnSearchStop_Click);
            // 
            // btnSearchStart
            // 
            this.btnSearchStart.Enabled = false;
            this.btnSearchStart.Location = new System.Drawing.Point(8, 16);
            this.btnSearchStart.Name = "btnSearchStart";
            this.btnSearchStart.Size = new System.Drawing.Size(64, 32);
            this.btnSearchStart.TabIndex = 0;
            this.btnSearchStart.Text = "Start / Continue";
            this.btnSearchStart.Click += new System.EventHandler(this.btnSearchStart_Click);
            // 
            // gbxFile
            // 
            this.gbxFile.Controls.Add(this.label37);
            this.gbxFile.Controls.Add(this.txtFileLogBaseName);
            this.gbxFile.Controls.Add(this.chkFileWriteLog);
            this.gbxFile.Controls.Add(this.label13);
            this.gbxFile.Controls.Add(this.txtFileBaseName);
            this.gbxFile.Controls.Add(this.chkFileSaveGenomeOnImprovement);
            this.gbxFile.Location = new System.Drawing.Point(0, 240);
            this.gbxFile.Name = "gbxFile";
            this.gbxFile.Size = new System.Drawing.Size(224, 112);
            this.gbxFile.TabIndex = 16;
            this.gbxFile.TabStop = false;
            this.gbxFile.Text = "File";
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(128, 88);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(88, 16);
            this.label37.TabIndex = 26;
            this.label37.Text = "Filename prefix";
            // 
            // txtFileLogBaseName
            // 
            this.txtFileLogBaseName.Location = new System.Drawing.Point(8, 88);
            this.txtFileLogBaseName.Name = "txtFileLogBaseName";
            this.txtFileLogBaseName.Size = new System.Drawing.Size(120, 20);
            this.txtFileLogBaseName.TabIndex = 25;
            this.txtFileLogBaseName.Text = "log";
            // 
            // chkFileWriteLog
            // 
            this.chkFileWriteLog.Location = new System.Drawing.Point(8, 64);
            this.chkFileWriteLog.Name = "chkFileWriteLog";
            this.chkFileWriteLog.Size = new System.Drawing.Size(192, 24);
            this.chkFileWriteLog.TabIndex = 24;
            this.chkFileWriteLog.Text = "Write Log File";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(128, 44);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 16);
            this.label13.TabIndex = 23;
            this.label13.Text = "Filename prefix";
            // 
            // txtFileBaseName
            // 
            this.txtFileBaseName.Location = new System.Drawing.Point(8, 40);
            this.txtFileBaseName.Name = "txtFileBaseName";
            this.txtFileBaseName.Size = new System.Drawing.Size(120, 20);
            this.txtFileBaseName.TabIndex = 1;
            this.txtFileBaseName.Text = "genome";
            // 
            // chkFileSaveGenomeOnImprovement
            // 
            this.chkFileSaveGenomeOnImprovement.Location = new System.Drawing.Point(8, 16);
            this.chkFileSaveGenomeOnImprovement.Name = "chkFileSaveGenomeOnImprovement";
            this.chkFileSaveGenomeOnImprovement.Size = new System.Drawing.Size(192, 24);
            this.chkFileSaveGenomeOnImprovement.TabIndex = 0;
            this.chkFileSaveGenomeOnImprovement.Text = "Save Genome On Improvement";
            // 
            // gbxCurrentStats
            // 
            this.gbxCurrentStats.Controls.Add(this.label38);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMode);
            this.gbxCurrentStats.Controls.Add(this.txtStatsEvaluationsPerSec);
            this.gbxCurrentStats.Controls.Add(this.label20);
            this.gbxCurrentStats.Controls.Add(this.label19);
            this.gbxCurrentStats.Controls.Add(this.label18);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMeanGenomeLength);
            this.gbxCurrentStats.Controls.Add(this.txtStatsBestGenomeLength);
            this.gbxCurrentStats.Controls.Add(this.txtStatsTotalEvaluations);
            this.gbxCurrentStats.Controls.Add(this.label27);
            this.gbxCurrentStats.Controls.Add(this.txtStatsEvaluatorStateMsg);
            this.gbxCurrentStats.Controls.Add(this.label7);
            this.gbxCurrentStats.Controls.Add(this.txtStatsCompatibilityThreshold);
            this.gbxCurrentStats.Controls.Add(this.label5);
            this.gbxCurrentStats.Controls.Add(this.txtStatsGeneration);
            this.gbxCurrentStats.Controls.Add(this.label4);
            this.gbxCurrentStats.Controls.Add(this.label3);
            this.gbxCurrentStats.Controls.Add(this.label2);
            this.gbxCurrentStats.Controls.Add(this.txtStatsSpeciesCount);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMean);
            this.gbxCurrentStats.Controls.Add(this.txtStatsBest);
            this.gbxCurrentStats.Controls.Add(this.label6);
            this.gbxCurrentStats.Location = new System.Drawing.Point(232, 56);
            this.gbxCurrentStats.Name = "gbxCurrentStats";
            this.gbxCurrentStats.Size = new System.Drawing.Size(224, 296);
            this.gbxCurrentStats.TabIndex = 8;
            this.gbxCurrentStats.TabStop = false;
            this.gbxCurrentStats.Text = "Current Stats";
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(104, 256);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(112, 16);
            this.label38.TabIndex = 21;
            this.label38.Text = "Current Search Mode";
            // 
            // txtStatsMode
            // 
            this.txtStatsMode.Location = new System.Drawing.Point(8, 256);
            this.txtStatsMode.Name = "txtStatsMode";
            this.txtStatsMode.ReadOnly = true;
            this.txtStatsMode.Size = new System.Drawing.Size(96, 20);
            this.txtStatsMode.TabIndex = 20;
            // 
            // txtStatsEvaluationsPerSec
            // 
            this.txtStatsEvaluationsPerSec.Location = new System.Drawing.Point(8, 184);
            this.txtStatsEvaluationsPerSec.Name = "txtStatsEvaluationsPerSec";
            this.txtStatsEvaluationsPerSec.ReadOnly = true;
            this.txtStatsEvaluationsPerSec.Size = new System.Drawing.Size(80, 20);
            this.txtStatsEvaluationsPerSec.TabIndex = 18;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(88, 184);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(104, 16);
            this.label20.TabIndex = 19;
            this.label20.Text = "Evaluations / Sec";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(104, 232);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(112, 16);
            this.label19.TabIndex = 17;
            this.label19.Text = "Avg. Genome Length";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(104, 208);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(112, 16);
            this.label18.TabIndex = 16;
            this.label18.Text = "Best Genome Length";
            // 
            // txtStatsMeanGenomeLength
            // 
            this.txtStatsMeanGenomeLength.Location = new System.Drawing.Point(8, 232);
            this.txtStatsMeanGenomeLength.Name = "txtStatsMeanGenomeLength";
            this.txtStatsMeanGenomeLength.ReadOnly = true;
            this.txtStatsMeanGenomeLength.Size = new System.Drawing.Size(96, 20);
            this.txtStatsMeanGenomeLength.TabIndex = 15;
            // 
            // txtStatsBestGenomeLength
            // 
            this.txtStatsBestGenomeLength.Location = new System.Drawing.Point(8, 208);
            this.txtStatsBestGenomeLength.Name = "txtStatsBestGenomeLength";
            this.txtStatsBestGenomeLength.ReadOnly = true;
            this.txtStatsBestGenomeLength.Size = new System.Drawing.Size(96, 20);
            this.txtStatsBestGenomeLength.TabIndex = 14;
            // 
            // txtStatsTotalEvaluations
            // 
            this.txtStatsTotalEvaluations.Location = new System.Drawing.Point(8, 160);
            this.txtStatsTotalEvaluations.Name = "txtStatsTotalEvaluations";
            this.txtStatsTotalEvaluations.ReadOnly = true;
            this.txtStatsTotalEvaluations.Size = new System.Drawing.Size(80, 20);
            this.txtStatsTotalEvaluations.TabIndex = 12;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(88, 163);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(104, 16);
            this.label27.TabIndex = 13;
            this.label27.Text = "Total Evaluations";
            // 
            // txtStatsEvaluatorStateMsg
            // 
            this.txtStatsEvaluatorStateMsg.Location = new System.Drawing.Point(8, 136);
            this.txtStatsEvaluatorStateMsg.Name = "txtStatsEvaluatorStateMsg";
            this.txtStatsEvaluatorStateMsg.ReadOnly = true;
            this.txtStatsEvaluatorStateMsg.Size = new System.Drawing.Size(80, 20);
            this.txtStatsEvaluatorStateMsg.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(88, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 16);
            this.label7.TabIndex = 11;
            this.label7.Text = "Evaluator State Msg";
            // 
            // txtStatsCompatibilityThreshold
            // 
            this.txtStatsCompatibilityThreshold.Location = new System.Drawing.Point(8, 112);
            this.txtStatsCompatibilityThreshold.Name = "txtStatsCompatibilityThreshold";
            this.txtStatsCompatibilityThreshold.ReadOnly = true;
            this.txtStatsCompatibilityThreshold.Size = new System.Drawing.Size(80, 20);
            this.txtStatsCompatibilityThreshold.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(88, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Generation";
            // 
            // txtStatsGeneration
            // 
            this.txtStatsGeneration.Location = new System.Drawing.Point(8, 16);
            this.txtStatsGeneration.Name = "txtStatsGeneration";
            this.txtStatsGeneration.ReadOnly = true;
            this.txtStatsGeneration.Size = new System.Drawing.Size(80, 20);
            this.txtStatsGeneration.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(88, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "# of Species ";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(88, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mean";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(88, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Best";
            // 
            // txtStatsSpeciesCount
            // 
            this.txtStatsSpeciesCount.Location = new System.Drawing.Point(8, 88);
            this.txtStatsSpeciesCount.Name = "txtStatsSpeciesCount";
            this.txtStatsSpeciesCount.ReadOnly = true;
            this.txtStatsSpeciesCount.Size = new System.Drawing.Size(80, 20);
            this.txtStatsSpeciesCount.TabIndex = 2;
            // 
            // txtStatsMean
            // 
            this.txtStatsMean.Location = new System.Drawing.Point(8, 64);
            this.txtStatsMean.Name = "txtStatsMean";
            this.txtStatsMean.ReadOnly = true;
            this.txtStatsMean.Size = new System.Drawing.Size(80, 20);
            this.txtStatsMean.TabIndex = 1;
            // 
            // txtStatsBest
            // 
            this.txtStatsBest.Location = new System.Drawing.Point(8, 40);
            this.txtStatsBest.Name = "txtStatsBest";
            this.txtStatsBest.ReadOnly = true;
            this.txtStatsBest.Size = new System.Drawing.Size(80, 20);
            this.txtStatsBest.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(88, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Compatibility Threshold";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gbxSearchParameters);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(456, 382);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Page 2";
            // 
            // gbxSearchParameters
            // 
            this.gbxSearchParameters.Controls.Add(this.groupBox7);
            this.gbxSearchParameters.Controls.Add(this.txtParamPopulationSize);
            this.gbxSearchParameters.Controls.Add(this.label17);
            this.gbxSearchParameters.Controls.Add(this.txtParamConnectionWeightMutationSigma);
            this.gbxSearchParameters.Controls.Add(this.label16);
            this.gbxSearchParameters.Controls.Add(this.txtParamConnectionWeightRange);
            this.gbxSearchParameters.Controls.Add(this.groupBox4);
            this.gbxSearchParameters.Controls.Add(this.groupBox2);
            this.gbxSearchParameters.Controls.Add(this.groupBox3);
            this.gbxSearchParameters.Controls.Add(this.groupBox1);
            this.gbxSearchParameters.Controls.Add(this.label28);
            this.gbxSearchParameters.Location = new System.Drawing.Point(0, 0);
            this.gbxSearchParameters.Name = "gbxSearchParameters";
            this.gbxSearchParameters.Size = new System.Drawing.Size(456, 376);
            this.gbxSearchParameters.TabIndex = 15;
            this.gbxSearchParameters.TabStop = false;
            this.gbxSearchParameters.Text = "Search Parameters";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkParamPruningModeAlwaysOn);
            this.groupBox7.Controls.Add(this.label41);
            this.groupBox7.Controls.Add(this.txtParamPruningEndComplexityStagnationThreshold);
            this.groupBox7.Controls.Add(this.label40);
            this.groupBox7.Controls.Add(this.txtParamPruningBeginFitnessStagnationThreshold);
            this.groupBox7.Controls.Add(this.label39);
            this.groupBox7.Controls.Add(this.txtParamPruningBeginComplexityThreshold);
            this.groupBox7.Controls.Add(this.chkParamEnableConnectionWeightFixing);
            this.groupBox7.Controls.Add(this.chkParamPruningModeEnabled);
            this.groupBox7.Location = new System.Drawing.Point(8, 168);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(216, 144);
            this.groupBox7.TabIndex = 54;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Pruning Phase Parameters";
            // 
            // chkParamPruningModeAlwaysOn
            // 
            this.chkParamPruningModeAlwaysOn.Location = new System.Drawing.Point(114, 16);
            this.chkParamPruningModeAlwaysOn.Name = "chkParamPruningModeAlwaysOn";
            this.chkParamPruningModeAlwaysOn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkParamPruningModeAlwaysOn.Size = new System.Drawing.Size(104, 16);
            this.chkParamPruningModeAlwaysOn.TabIndex = 56;
            this.chkParamPruningModeAlwaysOn.Text = "Always On";
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(56, 109);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(152, 24);
            this.label41.TabIndex = 55;
            this.label41.Text = "End Complexity Stagnation Threshold";
            // 
            // txtParamPruningEndComplexityStagnationThreshold
            // 
            this.txtParamPruningEndComplexityStagnationThreshold.Location = new System.Drawing.Point(8, 111);
            this.txtParamPruningEndComplexityStagnationThreshold.Name = "txtParamPruningEndComplexityStagnationThreshold";
            this.txtParamPruningEndComplexityStagnationThreshold.Size = new System.Drawing.Size(48, 20);
            this.txtParamPruningEndComplexityStagnationThreshold.TabIndex = 54;
            // 
            // label40
            // 
            this.label40.Location = new System.Drawing.Point(56, 78);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(152, 31);
            this.label40.TabIndex = 34;
            this.label40.Text = "Begin Fitness Stagnation Threshold (gens)";
            // 
            // txtParamPruningBeginFitnessStagnationThreshold
            // 
            this.txtParamPruningBeginFitnessStagnationThreshold.Location = new System.Drawing.Point(8, 80);
            this.txtParamPruningBeginFitnessStagnationThreshold.Name = "txtParamPruningBeginFitnessStagnationThreshold";
            this.txtParamPruningBeginFitnessStagnationThreshold.Size = new System.Drawing.Size(48, 20);
            this.txtParamPruningBeginFitnessStagnationThreshold.TabIndex = 33;
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(56, 58);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(152, 16);
            this.label39.TabIndex = 32;
            this.label39.Text = "Begin Complexity Threshold";
            // 
            // txtParamPruningBeginComplexityThreshold
            // 
            this.txtParamPruningBeginComplexityThreshold.Location = new System.Drawing.Point(8, 56);
            this.txtParamPruningBeginComplexityThreshold.Name = "txtParamPruningBeginComplexityThreshold";
            this.txtParamPruningBeginComplexityThreshold.Size = new System.Drawing.Size(48, 20);
            this.txtParamPruningBeginComplexityThreshold.TabIndex = 31;
            // 
            // chkParamEnableConnectionWeightFixing
            // 
            this.chkParamEnableConnectionWeightFixing.Location = new System.Drawing.Point(8, 32);
            this.chkParamEnableConnectionWeightFixing.Name = "chkParamEnableConnectionWeightFixing";
            this.chkParamEnableConnectionWeightFixing.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkParamEnableConnectionWeightFixing.Size = new System.Drawing.Size(136, 16);
            this.chkParamEnableConnectionWeightFixing.TabIndex = 53;
            this.chkParamEnableConnectionWeightFixing.Text = "Enable Weight Fixing";
            // 
            // chkParamPruningModeEnabled
            // 
            this.chkParamPruningModeEnabled.Checked = true;
            this.chkParamPruningModeEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkParamPruningModeEnabled.Location = new System.Drawing.Point(8, 16);
            this.chkParamPruningModeEnabled.Name = "chkParamPruningModeEnabled";
            this.chkParamPruningModeEnabled.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkParamPruningModeEnabled.Size = new System.Drawing.Size(104, 16);
            this.chkParamPruningModeEnabled.TabIndex = 52;
            this.chkParamPruningModeEnabled.Text = "Enable Pruning";
            this.chkParamPruningModeEnabled.CheckedChanged += new System.EventHandler(this.chkParamPruningModeEnabled_CheckedChanged);
            // 
            // txtParamPopulationSize
            // 
            this.txtParamPopulationSize.Location = new System.Drawing.Point(16, 16);
            this.txtParamPopulationSize.Name = "txtParamPopulationSize";
            this.txtParamPopulationSize.Size = new System.Drawing.Size(48, 20);
            this.txtParamPopulationSize.TabIndex = 42;
            // 
            // label17
            // 
            this.label17.Enabled = false;
            this.label17.Location = new System.Drawing.Point(64, 344);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(160, 16);
            this.label17.TabIndex = 51;
            this.label17.Text = "Conn. Weight Mutation Sigma";
            // 
            // txtParamConnectionWeightMutationSigma
            // 
            this.txtParamConnectionWeightMutationSigma.Enabled = false;
            this.txtParamConnectionWeightMutationSigma.Location = new System.Drawing.Point(16, 344);
            this.txtParamConnectionWeightMutationSigma.Name = "txtParamConnectionWeightMutationSigma";
            this.txtParamConnectionWeightMutationSigma.Size = new System.Drawing.Size(48, 20);
            this.txtParamConnectionWeightMutationSigma.TabIndex = 50;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(64, 320);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(136, 16);
            this.label16.TabIndex = 49;
            this.label16.Text = "Connection Weight Range";
            // 
            // txtParamConnectionWeightRange
            // 
            this.txtParamConnectionWeightRange.Location = new System.Drawing.Point(16, 320);
            this.txtParamConnectionWeightRange.Name = "txtParamConnectionWeightRange";
            this.txtParamConnectionWeightRange.Size = new System.Drawing.Size(48, 20);
            this.txtParamConnectionWeightRange.TabIndex = 48;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.txtParamSpeciesDropoffAge);
            this.groupBox4.Controls.Add(this.label30);
            this.groupBox4.Controls.Add(this.txtParamTargetSpeciesCountMax);
            this.groupBox4.Controls.Add(this.label31);
            this.groupBox4.Controls.Add(this.txtParamTargetSpeciesCountMin);
            this.groupBox4.Controls.Add(this.label32);
            this.groupBox4.Controls.Add(this.txtParamSelectionProportion);
            this.groupBox4.Controls.Add(this.label33);
            this.groupBox4.Controls.Add(this.txtParamElitismProportion);
            this.groupBox4.Location = new System.Drawing.Point(8, 40);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(216, 128);
            this.groupBox4.TabIndex = 47;
            this.groupBox4.TabStop = false;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(56, 104);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(128, 16);
            this.label29.TabIndex = 30;
            this.label29.Text = "Species Dropoff Age";
            // 
            // txtParamSpeciesDropoffAge
            // 
            this.txtParamSpeciesDropoffAge.Location = new System.Drawing.Point(8, 104);
            this.txtParamSpeciesDropoffAge.Name = "txtParamSpeciesDropoffAge";
            this.txtParamSpeciesDropoffAge.Size = new System.Drawing.Size(48, 20);
            this.txtParamSpeciesDropoffAge.TabIndex = 29;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(56, 80);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(128, 16);
            this.label30.TabIndex = 28;
            this.label30.Text = "Max Species Threshold";
            // 
            // txtParamTargetSpeciesCountMax
            // 
            this.txtParamTargetSpeciesCountMax.Location = new System.Drawing.Point(8, 80);
            this.txtParamTargetSpeciesCountMax.Name = "txtParamTargetSpeciesCountMax";
            this.txtParamTargetSpeciesCountMax.Size = new System.Drawing.Size(48, 20);
            this.txtParamTargetSpeciesCountMax.TabIndex = 27;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(56, 56);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(128, 16);
            this.label31.TabIndex = 26;
            this.label31.Text = "Min Species Threshold";
            // 
            // txtParamTargetSpeciesCountMin
            // 
            this.txtParamTargetSpeciesCountMin.Location = new System.Drawing.Point(8, 56);
            this.txtParamTargetSpeciesCountMin.Name = "txtParamTargetSpeciesCountMin";
            this.txtParamTargetSpeciesCountMin.Size = new System.Drawing.Size(48, 20);
            this.txtParamTargetSpeciesCountMin.TabIndex = 25;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(56, 32);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(120, 16);
            this.label32.TabIndex = 24;
            this.label32.Text = "Selection Proportion";
            // 
            // txtParamSelectionProportion
            // 
            this.txtParamSelectionProportion.Location = new System.Drawing.Point(8, 32);
            this.txtParamSelectionProportion.Name = "txtParamSelectionProportion";
            this.txtParamSelectionProportion.Size = new System.Drawing.Size(48, 20);
            this.txtParamSelectionProportion.TabIndex = 23;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(56, 8);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(96, 16);
            this.label33.TabIndex = 22;
            this.label33.Text = "Elitism Proportion";
            // 
            // txtParamElitismProportion
            // 
            this.txtParamElitismProportion.Location = new System.Drawing.Point(8, 8);
            this.txtParamElitismProportion.Name = "txtParamElitismProportion";
            this.txtParamElitismProportion.Size = new System.Drawing.Size(48, 20);
            this.txtParamElitismProportion.TabIndex = 21;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label43);
            this.groupBox2.Controls.Add(this.txtParamMutateDeleteNeuron);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.txtParamMutateDeleteConnection);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Controls.Add(this.txtParamMutateConnectionWeights);
            this.groupBox2.Controls.Add(this.label35);
            this.groupBox2.Controls.Add(this.txtParamMutateAddNode);
            this.groupBox2.Controls.Add(this.label36);
            this.groupBox2.Controls.Add(this.txtParamMutateAddConnection);
            this.groupBox2.Location = new System.Drawing.Point(232, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(216, 128);
            this.groupBox2.TabIndex = 46;
            this.groupBox2.TabStop = false;
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(56, 80);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(144, 16);
            this.label43.TabIndex = 29;
            this.label43.Text = "p Mutate Delete Neuron";
            // 
            // txtParamMutateDeleteNeuron
            // 
            this.txtParamMutateDeleteNeuron.Location = new System.Drawing.Point(8, 80);
            this.txtParamMutateDeleteNeuron.Name = "txtParamMutateDeleteNeuron";
            this.txtParamMutateDeleteNeuron.Size = new System.Drawing.Size(48, 20);
            this.txtParamMutateDeleteNeuron.TabIndex = 28;
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(56, 104);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(152, 16);
            this.label42.TabIndex = 27;
            this.label42.Text = "p Mutate Delete Connection";
            // 
            // txtParamMutateDeleteConnection
            // 
            this.txtParamMutateDeleteConnection.Location = new System.Drawing.Point(8, 104);
            this.txtParamMutateDeleteConnection.Name = "txtParamMutateDeleteConnection";
            this.txtParamMutateDeleteConnection.Size = new System.Drawing.Size(48, 20);
            this.txtParamMutateDeleteConnection.TabIndex = 26;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(56, 56);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(136, 16);
            this.label34.TabIndex = 25;
            this.label34.Text = "p Mutate Add Connection";
            // 
            // txtParamMutateConnectionWeights
            // 
            this.txtParamMutateConnectionWeights.Location = new System.Drawing.Point(8, 8);
            this.txtParamMutateConnectionWeights.Name = "txtParamMutateConnectionWeights";
            this.txtParamMutateConnectionWeights.Size = new System.Drawing.Size(48, 20);
            this.txtParamMutateConnectionWeights.TabIndex = 24;
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(56, 32);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(136, 16);
            this.label35.TabIndex = 23;
            this.label35.Text = "p Mutate Add Neuron";
            // 
            // txtParamMutateAddNode
            // 
            this.txtParamMutateAddNode.Location = new System.Drawing.Point(8, 32);
            this.txtParamMutateAddNode.Name = "txtParamMutateAddNode";
            this.txtParamMutateAddNode.Size = new System.Drawing.Size(48, 20);
            this.txtParamMutateAddNode.TabIndex = 22;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(56, 8);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(152, 16);
            this.label36.TabIndex = 21;
            this.label36.Text = "p Mutate Connection Weights";
            // 
            // txtParamMutateAddConnection
            // 
            this.txtParamMutateAddConnection.Location = new System.Drawing.Point(8, 56);
            this.txtParamMutateAddConnection.Name = "txtParamMutateAddConnection";
            this.txtParamMutateAddConnection.Size = new System.Drawing.Size(48, 20);
            this.txtParamMutateAddConnection.TabIndex = 20;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txtParamCompatDisjointCoeff);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.txtParamCompatExcessCoeff);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.txtParamCompatWeightDeltaCoeff);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Controls.Add(this.txtParamCompatThreshold);
            this.groupBox3.Location = new System.Drawing.Point(232, 216);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(216, 112);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Speciation Parametes";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(56, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(136, 16);
            this.label12.TabIndex = 35;
            this.label12.Text = "Compat. Disjoint Coeff.";
            // 
            // txtParamCompatDisjointCoeff
            // 
            this.txtParamCompatDisjointCoeff.Location = new System.Drawing.Point(8, 40);
            this.txtParamCompatDisjointCoeff.Name = "txtParamCompatDisjointCoeff";
            this.txtParamCompatDisjointCoeff.Size = new System.Drawing.Size(48, 20);
            this.txtParamCompatDisjointCoeff.TabIndex = 34;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(56, 64);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(120, 16);
            this.label22.TabIndex = 33;
            this.label22.Text = "Compat. Excess Coeff.";
            // 
            // txtParamCompatExcessCoeff
            // 
            this.txtParamCompatExcessCoeff.Location = new System.Drawing.Point(8, 64);
            this.txtParamCompatExcessCoeff.Name = "txtParamCompatExcessCoeff";
            this.txtParamCompatExcessCoeff.Size = new System.Drawing.Size(48, 20);
            this.txtParamCompatExcessCoeff.TabIndex = 32;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(56, 88);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(152, 16);
            this.label23.TabIndex = 31;
            this.label23.Text = "Compat. Weight Delta Coeff.";
            // 
            // txtParamCompatWeightDeltaCoeff
            // 
            this.txtParamCompatWeightDeltaCoeff.Location = new System.Drawing.Point(8, 88);
            this.txtParamCompatWeightDeltaCoeff.Name = "txtParamCompatWeightDeltaCoeff";
            this.txtParamCompatWeightDeltaCoeff.Size = new System.Drawing.Size(48, 20);
            this.txtParamCompatWeightDeltaCoeff.TabIndex = 30;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(56, 16);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(152, 16);
            this.label24.TabIndex = 29;
            this.label24.Text = "Compat. Threshold Start Val";
            // 
            // txtParamCompatThreshold
            // 
            this.txtParamCompatThreshold.Location = new System.Drawing.Point(8, 16);
            this.txtParamCompatThreshold.Name = "txtParamCompatThreshold";
            this.txtParamCompatThreshold.Size = new System.Drawing.Size(48, 20);
            this.txtParamCompatThreshold.TabIndex = 28;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtParamInterspeciesMating);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.txtParamOffspringCrossover);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.txtParamOffspringMutation);
            this.groupBox1.Location = new System.Drawing.Point(232, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 80);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(8, 54);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(200, 1);
            this.label15.TabIndex = 24;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(56, 57);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(120, 16);
            this.label14.TabIndex = 23;
            this.label14.Text = "p Interspecies Mating";
            // 
            // txtParamInterspeciesMating
            // 
            this.txtParamInterspeciesMating.Location = new System.Drawing.Point(8, 56);
            this.txtParamInterspeciesMating.Name = "txtParamInterspeciesMating";
            this.txtParamInterspeciesMating.Size = new System.Drawing.Size(48, 20);
            this.txtParamInterspeciesMating.TabIndex = 22;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(56, 32);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(120, 16);
            this.label25.TabIndex = 21;
            this.label25.Text = "p Offspring Crossover";
            // 
            // txtParamOffspringCrossover
            // 
            this.txtParamOffspringCrossover.Location = new System.Drawing.Point(8, 32);
            this.txtParamOffspringCrossover.Name = "txtParamOffspringCrossover";
            this.txtParamOffspringCrossover.Size = new System.Drawing.Size(48, 20);
            this.txtParamOffspringCrossover.TabIndex = 20;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(56, 8);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(104, 16);
            this.label26.TabIndex = 19;
            this.label26.Text = "p Offspring Asexual";
            // 
            // txtParamOffspringMutation
            // 
            this.txtParamOffspringMutation.Location = new System.Drawing.Point(8, 8);
            this.txtParamOffspringMutation.Name = "txtParamOffspringMutation";
            this.txtParamOffspringMutation.Size = new System.Drawing.Size(48, 20);
            this.txtParamOffspringMutation.TabIndex = 18;
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(64, 16);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(64, 16);
            this.label28.TabIndex = 43;
            this.label28.Text = "Pop Size";
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 408);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(464, 8);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gbxLog);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 416);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(464, 105);
            this.panel2.TabIndex = 6;
            // 
            // gbxLog
            // 
            this.gbxLog.Controls.Add(this.txtLogWindow);
            this.gbxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxLog.Location = new System.Drawing.Point(0, 0);
            this.gbxLog.Name = "gbxLog";
            this.gbxLog.Size = new System.Drawing.Size(464, 105);
            this.gbxLog.TabIndex = 5;
            this.gbxLog.TabStop = false;
            this.gbxLog.Text = "Log";
            // 
            // txtLogWindow
            // 
            this.txtLogWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogWindow.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogWindow.Location = new System.Drawing.Point(3, 16);
            this.txtLogWindow.Multiline = true;
            this.txtLogWindow.Name = "txtLogWindow";
            this.txtLogWindow.ReadOnly = true;
            this.txtLogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogWindow.Size = new System.Drawing.Size(458, 86);
            this.txtLogWindow.TabIndex = 5;
            this.txtLogWindow.WordWrap = false;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.mnuInitPop,
            this.mnuView,
            this.mnuVisualization,
            this.mnuAbout});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileSave});
            this.menuItem1.Text = "File";
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Index = 0;
            this.mnuFileSave.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileSavePopulation,
            this.mnuFileSaveBestAsNetwork,
            this.mnuFileSaveBestAsGenome});
            this.mnuFileSave.Text = "Save";
            // 
            // mnuFileSavePopulation
            // 
            this.mnuFileSavePopulation.Index = 0;
            this.mnuFileSavePopulation.Text = "Save Population";
            this.mnuFileSavePopulation.Click += new System.EventHandler(this.mnuFileSavePopulation_Click);
            // 
            // mnuFileSaveBestAsNetwork
            // 
            this.mnuFileSaveBestAsNetwork.Index = 1;
            this.mnuFileSaveBestAsNetwork.Text = "Save Best As Network";
            this.mnuFileSaveBestAsNetwork.Click += new System.EventHandler(this.mnuFileSaveBestAsNetwork_Click);
            // 
            // mnuFileSaveBestAsGenome
            // 
            this.mnuFileSaveBestAsGenome.Index = 2;
            this.mnuFileSaveBestAsGenome.Text = "Save Best As Genome";
            this.mnuFileSaveBestAsGenome.Click += new System.EventHandler(this.mnuFileSaveBestAsGenome_Click);
            // 
            // mnuInitPop
            // 
            this.mnuInitPop.Index = 1;
            this.mnuInitPop.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuInitPopLoad,
            this.mnuInitPopAutoGenerate});
            this.mnuInitPop.Text = "Initialize Population";
            // 
            // mnuInitPopLoad
            // 
            this.mnuInitPopLoad.Index = 0;
            this.mnuInitPopLoad.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuInitPopLoadPopulation,
            this.mnuInitPopLoadSeedGenome,
            this.mnuInitPopLoadSeedPopulation});
            this.mnuInitPopLoad.Text = "Load";
            // 
            // mnuInitPopLoadPopulation
            // 
            this.mnuInitPopLoadPopulation.Index = 0;
            this.mnuInitPopLoadPopulation.Text = "Load Population";
            this.mnuInitPopLoadPopulation.Click += new System.EventHandler(this.mnuInitPopLoadPopulation_Click);
            // 
            // mnuInitPopLoadSeedGenome
            // 
            this.mnuInitPopLoadSeedGenome.Index = 1;
            this.mnuInitPopLoadSeedGenome.Text = "Load Seed Genome";
            this.mnuInitPopLoadSeedGenome.Click += new System.EventHandler(this.mnuInitPopLoadSeedGenome_Click);
            // 
            // mnuInitPopLoadSeedPopulation
            // 
            this.mnuInitPopLoadSeedPopulation.Index = 2;
            this.mnuInitPopLoadSeedPopulation.Text = "Load Seed Population";
            this.mnuInitPopLoadSeedPopulation.Click += new System.EventHandler(this.mnuInitPopLoadSeedPopulation_Click);
            // 
            // mnuInitPopAutoGenerate
            // 
            this.mnuInitPopAutoGenerate.Index = 1;
            this.mnuInitPopAutoGenerate.Text = "Auto Generate";
            this.mnuInitPopAutoGenerate.Click += new System.EventHandler(this.mnuInitPopAutoGenerate_Click);
            // 
            // mnuView
            // 
            this.mnuView.Index = 2;
            this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuViewUpdateFrequency});
            this.mnuView.Text = "View";
            // 
            // mnuViewUpdateFrequency
            // 
            this.mnuViewUpdateFrequency.Index = 0;
            this.mnuViewUpdateFrequency.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuViewUpdateFrequency1Sec,
            this.mnuViewUpdateFrequency2Sec,
            this.mnuViewUpdateFrequency5Sec,
            this.mnuViewUpdateFrequency10Sec,
            this.mnuViewUpdateFrequency1Gen,
            this.mnuViewUpdateFrequency2Gen,
            this.mnuViewUpdateFrequency5Gen,
            this.mnuViewUpdateFrequency10Gen});
            this.mnuViewUpdateFrequency.Text = "Update Frequency";
            // 
            // mnuViewUpdateFrequency1Sec
            // 
            this.mnuViewUpdateFrequency1Sec.Checked = true;
            this.mnuViewUpdateFrequency1Sec.Index = 0;
            this.mnuViewUpdateFrequency1Sec.RadioCheck = true;
            this.mnuViewUpdateFrequency1Sec.Text = "1 Second";
            this.mnuViewUpdateFrequency1Sec.Click += new System.EventHandler(this.mnuViewUpdateFrequency1Sec_Click);
            // 
            // mnuViewUpdateFrequency2Sec
            // 
            this.mnuViewUpdateFrequency2Sec.Index = 1;
            this.mnuViewUpdateFrequency2Sec.RadioCheck = true;
            this.mnuViewUpdateFrequency2Sec.Text = "2 Seconds";
            this.mnuViewUpdateFrequency2Sec.Click += new System.EventHandler(this.mnuViewUpdateFrequency2Sec_Click);
            // 
            // mnuViewUpdateFrequency5Sec
            // 
            this.mnuViewUpdateFrequency5Sec.Index = 2;
            this.mnuViewUpdateFrequency5Sec.RadioCheck = true;
            this.mnuViewUpdateFrequency5Sec.Text = "5 Seconds";
            this.mnuViewUpdateFrequency5Sec.Click += new System.EventHandler(this.mnuViewUpdateFrequency5Sec_Click);
            // 
            // mnuViewUpdateFrequency10Sec
            // 
            this.mnuViewUpdateFrequency10Sec.Index = 3;
            this.mnuViewUpdateFrequency10Sec.RadioCheck = true;
            this.mnuViewUpdateFrequency10Sec.Text = "10 Seconds";
            this.mnuViewUpdateFrequency10Sec.Click += new System.EventHandler(this.mnuViewUpdateFrequency10Sec_Click);
            // 
            // mnuViewUpdateFrequency1Gen
            // 
            this.mnuViewUpdateFrequency1Gen.Index = 4;
            this.mnuViewUpdateFrequency1Gen.RadioCheck = true;
            this.mnuViewUpdateFrequency1Gen.Text = "1 Generation";
            this.mnuViewUpdateFrequency1Gen.Click += new System.EventHandler(this.mnuViewUpdateFrequency1Gen_Click);
            // 
            // mnuViewUpdateFrequency2Gen
            // 
            this.mnuViewUpdateFrequency2Gen.Index = 5;
            this.mnuViewUpdateFrequency2Gen.RadioCheck = true;
            this.mnuViewUpdateFrequency2Gen.Text = "2 Generations";
            this.mnuViewUpdateFrequency2Gen.Click += new System.EventHandler(this.mnuViewUpdateFrequency2Gen_Click);
            // 
            // mnuViewUpdateFrequency5Gen
            // 
            this.mnuViewUpdateFrequency5Gen.Index = 6;
            this.mnuViewUpdateFrequency5Gen.RadioCheck = true;
            this.mnuViewUpdateFrequency5Gen.Text = "5 Generations";
            this.mnuViewUpdateFrequency5Gen.Click += new System.EventHandler(this.mnuViewUpdateFrequency5Gen_Click);
            // 
            // mnuViewUpdateFrequency10Gen
            // 
            this.mnuViewUpdateFrequency10Gen.Index = 7;
            this.mnuViewUpdateFrequency10Gen.RadioCheck = true;
            this.mnuViewUpdateFrequency10Gen.Text = "10 Generations";
            this.mnuViewUpdateFrequency10Gen.Click += new System.EventHandler(this.mnuViewUpdateFrequency10Gen_Click);
            // 
            // mnuVisualization
            // 
            this.mnuVisualization.Index = 3;
            this.mnuVisualization.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuVisualizationProgressGraph,
            this.mnuVisualizationBest,
            this.mnuVisualizationSpecies,
            this.mnuVisualizationExperiment});
            this.mnuVisualization.Text = "Visualization";
            // 
            // mnuVisualizationProgressGraph
            // 
            this.mnuVisualizationProgressGraph.Index = 0;
            this.mnuVisualizationProgressGraph.Text = "Progress Graphs";
            this.mnuVisualizationProgressGraph.Click += new System.EventHandler(this.mnuVisualizationProgressGraph_Click);
            // 
            // mnuVisualizationBest
            // 
            this.mnuVisualizationBest.Index = 1;
            this.mnuVisualizationBest.Text = "Best Genome";
            this.mnuVisualizationBest.Click += new System.EventHandler(this.mnuVisualizationBest_Click);
            // 
            // mnuVisualizationSpecies
            // 
            this.mnuVisualizationSpecies.Index = 2;
            this.mnuVisualizationSpecies.Text = "Species";
            this.mnuVisualizationSpecies.Click += new System.EventHandler(this.mnuVisualizationSpecies_Click);
            // 
            // mnuVisualizationExperiment
            // 
            this.mnuVisualizationExperiment.Index = 3;
            this.mnuVisualizationExperiment.Text = "Experiment";
            this.mnuVisualizationExperiment.Click += new System.EventHandler(this.mnuVisualizationExperiment_Click);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Index = 4;
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(464, 521);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "SharpNEAT";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.gbxFile.ResumeLayout(false);
            this.gbxFile.PerformLayout();
            this.gbxCurrentStats.ResumeLayout(false);
            this.gbxCurrentStats.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.gbxSearchParameters.ResumeLayout(false);
            this.gbxSearchParameters.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.gbxLog.ResumeLayout(false);
            this.gbxLog.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Main Method

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		#endregion
		
		#region Private Methods

		private void InitialiseForm()
		{
		    experimentConfigInfoArray = ExperimentConfigUtils.ReadExperimentConfigCatalog();
		
			nfi = new NumberFormatInfo();
			nfi.NumberDecimalSeparator = ",";

			txtLogWindow.BackColor = Color.White;
			PopulateDomainCombo();
			PopulateActivationFunctionCombo();
			cmbDomain.SelectedIndex=0;

			// Load default neatParameters.
			LoadNeatParameters(new NeatParameters());

			UpdateGuiState();

			// The progress graph form exists from this point on and is only
			// hidden/shown by the menu item. This allows the form to build up a history 
			// of data to show in the graphs.
            progressForm = new ProgressForm();
		}

		private void LoadNeatParameters(NeatParameters np)
		{
			txtParamPopulationSize.Text = np.populationSize.ToString();
			txtParamOffspringMutation.Text = np.pOffspringAsexual.ToString();
			txtParamOffspringCrossover.Text = np.pOffspringSexual.ToString();
			txtParamInterspeciesMating.Text = np.pInterspeciesMating.ToString();
			txtParamCompatThreshold.Text = np.compatibilityThreshold.ToString();
			txtParamCompatDisjointCoeff.Text = np.compatibilityDisjointCoeff.ToString();
			txtParamCompatExcessCoeff.Text = np.compatibilityExcessCoeff.ToString();
			txtParamCompatWeightDeltaCoeff.Text = np.compatibilityWeightDeltaCoeff.ToString();
			txtParamElitismProportion.Text = np.elitismProportion.ToString();
			txtParamSelectionProportion.Text = np.selectionProportion.ToString();
			txtParamTargetSpeciesCountMin.Text = np.targetSpeciesCountMin.ToString();
			txtParamTargetSpeciesCountMax.Text = np.targetSpeciesCountMax.ToString();
			txtParamSpeciesDropoffAge.Text = np.speciesDropoffAge.ToString();
			
			txtParamPruningBeginComplexityThreshold.Text = np.pruningPhaseBeginComplexityThreshold.ToString();
			txtParamPruningBeginFitnessStagnationThreshold.Text = np.pruningPhaseBeginFitnessStagnationThreshold.ToString();
			txtParamPruningEndComplexityStagnationThreshold.Text = np.pruningPhaseEndComplexityStagnationThreshold.ToString();

			txtParamMutateConnectionWeights.Text = np.pMutateConnectionWeights.ToString();
			txtParamMutateAddNode.Text = np.pMutateAddNode.ToString();
			txtParamMutateAddConnection.Text = np.pMutateAddConnection.ToString();
			txtParamMutateDeleteConnection.Text = np.pMutateDeleteConnection.ToString();
			txtParamMutateDeleteNeuron.Text = np.pMutateDeleteSimpleNeuron.ToString();

			txtParamConnectionWeightRange.Text = np.connectionWeightRange.ToString();
//			txtParamConnectionWeightMutationSigma.Text = np.connectionMutationSigma.ToString();
		}
	
		private void PopulateDomainCombo()
		{
		    foreach (ExperimentConfigInfo eci in experimentConfigInfoArray)
		    {
		        cmbDomain.Items.Add(new ListItem("", eci.Title, eci));
		    }
		}

		private void PopulateActivationFunctionCombo()
		{
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Reduced Sigmoid", new ReducedSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Plain Sigmoid", new PlainSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Steepened Sigmoid", new SteepenedSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Inv-Abs [FAST]", new InverseAbsoluteSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Sigmoid Approximation [FAST]", new SigmoidApproximation()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Steepened Sigmoid Approx. [FAST]", new SteepenedSigmoidApproximation()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Step Function", new StepFunction()));
            cmbExperimentActivationFn.Items.Add(new ListItem("", "Linear", new Linear()));
        }

		private void SearchThreadMethod()
		{
			// Keep track of when the last GUI update occured. And how many evalulations had occured at that time.
			long lastUpdateDateTimeTick=1;
			ulong lastUpdateGeneration=0;
			ulong lastUpdateEvaluationCount=0;

			double previousBestFitness=0.0;
			string previousEvaluatorStateMsg = string.Empty;

			try
			{
				for(;;)
				{
					if(stopSearchSignal)
					{
						searchState = SearchStateEnum.Paused;
						BeginInvoke(new MethodInvoker(UpdateGuiState));

						stopSearchSignal = false;	// reset flag.
						searchThread.Suspend();
					}

					// One generation.
					ea.PerformOneGeneration();

					//----- Determine if a GUI update is due.
					bool bUpdateDue=false;
					long tickDelta =  DateTime.Now.Ticks - lastUpdateDateTimeTick;
					if(updateMode)
					{	// Timebased updates.
						if(tickDelta > updateFreqTicks)
							bUpdateDue = true;
					}
					else
					{	// Generation based updates.
						if((ea.Generation - lastUpdateGeneration) >= updateFreqGens)
							bUpdateDue = true;
					}

					if(bUpdateDue)
					{
						// Calculate some stats and update the GUI to show them.
						float evaluationsDelta = ea.PopulationEvaluator.EvaluationCount - lastUpdateEvaluationCount;
						evaluationsPerSec = (int)(evaluationsDelta / ((float)tickDelta/10000000F));
						Invoke(new MethodInvoker(NotifyGuiUpdateRequired));

						// Write log entry if we are required to.
						if(logWriter!=null)
						{
							logWriter.WriteLine(ea.Generation.ToString() + ',' + 
								((DateTime.Now.Ticks-ticksAtSearchStart)*0.0000001).ToString("0.00000") + ',' + 
								ea.PopulationEvaluator.EvaluatorStateMessage + ',' + 
								ea.BestGenome.Fitness.ToString("0.00") + ',' +
								ea.Population.MeanFitness.ToString("0.00") + ',' +
								ea.Population.SpeciesTable.Count.ToString() + ',' +
								ea.NeatParameters.compatibilityThreshold.ToString("0.00") + ',' +
								((NeatGenome)ea.BestGenome).NeuronGeneList.Count.ToString() + ',' +
								((NeatGenome)ea.BestGenome).ConnectionGeneList.Count.ToString() + ',' +
								(ea.Population.TotalNeuronCount / ea.Population.GenomeList.Count).ToString("0.00") + ',' +
								(ea.Population.TotalConnectionCount / ea.Population.GenomeList.Count).ToString("0.00") + ',' +
								(ea.Population.AvgComplexity.ToString("0.00")));
						}

						// Store/update the lastUpdate* varaibles ready for the next loop.
						lastUpdateDateTimeTick = DateTime.Now.Ticks;
						lastUpdateGeneration = ea.Generation;
						lastUpdateEvaluationCount = ea.PopulationEvaluator.EvaluationCount;
					}

					//----- Check if we should save the best genome.
					if(		(ea.PopulationEvaluator.EvaluatorStateMessage != previousEvaluatorStateMsg)
						||	(ea.BestGenome.Fitness > previousBestFitness)
						||	(ea.PopulationEvaluator.BestIsIntermediateChampion))
					{
						//TODO: Technically this is not thread safe.
						if(chkFileSaveGenomeOnImprovement.Checked)
						{
							string filename = txtFileBaseName.Text + '_' + ea.PopulationEvaluator.EvaluatorStateMessage
								+ '_' + ea.BestGenome.Fitness.ToString("0.00", nfi)
								+ '_' + DateTime.Now.ToString("yyMMdd_hhmmss")
								+ (ea.PopulationEvaluator.BestIsIntermediateChampion ? "_champ" : "")
								+ ".xml";

							SaveBestGenome(filename);
						}

						previousBestFitness = ea.BestGenome.Fitness;
						previousEvaluatorStateMsg = ea.PopulationEvaluator.EvaluatorStateMessage;
					}
				}
			}
			catch(Exception ex)
			{	
				if(ex is ThreadAbortException)
				{	// This is expected.
					return;
				}

				// Something went wrong! Usually an error within a plugged-in evaluation scheme.
				// Write entry to the application log.
				EventLog.WriteEntry("SharpNeat.exe", ex.ToString());
				
				// Report the exception to the log window.
				SafeLogMessage("\r\n" + ex.ToString());

				// Update the GUI state so that the user can re-start an experiment or save the population to file, etc.
				searchState = SearchStateEnum.Paused;
				BeginInvoke(new MethodInvoker(UpdateGuiState));

				stopSearchSignal = false;	// reset flag.
				searchThread.Suspend();
			}
		}

		private void NotifyGuiUpdateRequired()
		{
			UpdateStats();
			NotifyNetworkVisualization();
			progressForm.Update(ea);
			
			if(experimentView != null)
			{
				experimentView.RefreshView(GenomeDecoder.DecodeToFloatFastConcurrentNetwork((NeatGenome)ea.BestGenome, selectedActivationFunction));
			}
		}

		
		private void NotifyNetworkVisualization()
		{
			if(bestGenomeForm!=null)
			{
				bestGenomeForm.SetBestGenome((NeatGenome)ea.BestGenome, ea.Generation);
			}
			if(speciesForm!=null)
			{
				speciesForm.Update(ea);
			}
		}

		private string storedLogMessage=null;
		private void SafeLogMessage(string msg)
		{
			if(this.InvokeRequired)
			{
				storedLogMessage = msg;
				this.BeginInvoke(new MethodInvoker(SafeLogMessage));
				return;
			}
			LogMessage(msg);
		}

		private void SafeLogMessage()
		{
			LogMessage(storedLogMessage);
		}

		private void LogMessage(string msg)
		{
			txtLogWindow.AppendText(msg + "\r\n");
	
			if(txtLogWindow.Text.Length > 20000)
			{
				txtLogWindow.Text = txtLogWindow.Text.Substring(txtLogWindow.Text.Length-18000, 18000);
				txtLogWindow.ScrollToCaret();
			}
		}

		private void UpdateStats()
		{
			LogMessage("gen=" + ea.Generation + 
				", bestFitness=" + ea.BestGenome.Fitness.ToString("0.00") +
				", meanFitness=" + pop.MeanFitness.ToString("0.00") +
				", species=" + pop.SpeciesTable.Count + 
				", evaluatorMsg=" + ea.PopulationEvaluator.EvaluatorStateMessage);

			txtStatsGeneration.Text = ea.Generation.ToString();
			txtStatsBest.Text = ea.BestGenome.Fitness.ToString("0.000000");
			txtStatsMean.Text = pop.MeanFitness.ToString("0.000000");
			txtStatsSpeciesCount.Text = pop.SpeciesTable.Count.ToString();
			txtStatsCompatibilityThreshold.Text = ea.NeatParameters.compatibilityThreshold.ToString("0.0");
			txtStatsEvaluatorStateMsg.Text = ea.PopulationEvaluator.EvaluatorStateMessage;
			txtStatsTotalEvaluations.Text = ea.PopulationEvaluator.EvaluationCount.ToString();
			txtStatsEvaluationsPerSec.Text = evaluationsPerSec.ToString();

			NeatGenome bestGenome = (NeatGenome)ea.BestGenome;
			txtStatsBestGenomeLength.Text = "N " + bestGenome.NeuronGeneList.Count.ToString() + " / " +
				"C " + bestGenome.ConnectionGeneList.Count.ToString();
	
			txtStatsMeanGenomeLength.Text = "N " +  ((double)ea.Population.TotalNeuronCount / (double)ea.Population.GenomeList.Count).ToString("0.00") + " / " +
				"C " + ((double)ea.Population.TotalConnectionCount / (double)ea.Population.GenomeList.Count).ToString("0.00");

			if(ea.IsInPruningMode)
			{
				txtStatsMode.Text = "Pruning";
				txtStatsMode.BackColor = Color.Red;
			}
			else
			{
				txtStatsMode.Text = "Complexifying";
				txtStatsMode.BackColor = Color.FromKnownColor(KnownColor.Control);
			}
		}

		/// <summary>
		/// Read NeatParameters from the UI.
		/// </summary>
		/// <returns></returns>
		private NeatParameters GetUserNeatParameters()
		{
			NeatParameters np = new NeatParameters();

			np.populationSize = int.Parse(txtParamPopulationSize.Text);
			np.pOffspringAsexual = double.Parse(txtParamOffspringMutation.Text);
			np.pOffspringSexual = double.Parse(txtParamOffspringCrossover.Text);
			np.pInterspeciesMating = double.Parse(txtParamInterspeciesMating.Text);
			np.compatibilityThreshold = double.Parse(txtParamCompatThreshold.Text);
			np.compatibilityDisjointCoeff = double.Parse(txtParamCompatDisjointCoeff.Text);
			np.compatibilityExcessCoeff = double.Parse(txtParamCompatExcessCoeff.Text);
			np.compatibilityWeightDeltaCoeff = double.Parse(txtParamCompatWeightDeltaCoeff.Text);
			np.elitismProportion = double.Parse(txtParamElitismProportion.Text);
			np.selectionProportion = double.Parse(txtParamSelectionProportion.Text);
			np.targetSpeciesCountMin = int.Parse(txtParamTargetSpeciesCountMin.Text);
			np.targetSpeciesCountMax = int.Parse(txtParamTargetSpeciesCountMax.Text);
			np.speciesDropoffAge = int.Parse(txtParamSpeciesDropoffAge.Text);

			np.pruningPhaseBeginComplexityThreshold = float.Parse(txtParamPruningBeginComplexityThreshold.Text);
			np.pruningPhaseBeginFitnessStagnationThreshold = int.Parse(txtParamPruningBeginFitnessStagnationThreshold.Text);
			np.pruningPhaseEndComplexityStagnationThreshold = int.Parse(txtParamPruningEndComplexityStagnationThreshold.Text);

			np.pMutateConnectionWeights = double.Parse(txtParamMutateConnectionWeights.Text);
			np.pMutateAddNode = double.Parse(txtParamMutateAddNode.Text);
			np.pMutateAddConnection = double.Parse(txtParamMutateAddConnection.Text);

			np.pMutateDeleteConnection = double.Parse(txtParamMutateDeleteConnection.Text);
			np.pMutateDeleteSimpleNeuron = double.Parse(txtParamMutateDeleteNeuron.Text);
			
			np.connectionWeightRange = double.Parse(txtParamConnectionWeightRange.Text);

			return np;
		}

		/// <summary>
		/// Checks if the provided file location can be written too.
		/// If the file already exists then we check it's ReadOnly flag. If the flag is set then ask the user
		/// if they wish to overwrite the file - if they select yes then reset the file's RO attribute and return true.
		/// If the file does not exist then return true - the filename specified can be written to.
		/// </summary>
		/// <param name="file">The fileInfo object representing the filename we wish to check.</param>
		/// <returns>True if the file location provided can be written too. Otherwise returns false.</returns>
		private bool IsWriteableFile(FileInfo file)
		{
			file.Refresh();
			if(file.Exists)
			{
				// If the file exists and it is readonly ask the user whether it should be overwritten
				if(file.Exists && ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
				{
					// Retrieve Title and Message from resource file
					string sMessageTitle = "Read-Only File";
					string sMessage = @"The current file is read-only:
  %1
Do you wish to overwrite the file?";
					sMessage = sMessage.Replace("%1", file.FullName);

					// Display message and respond to selection
					DialogResult oResult = MessageBox.Show(
						this,
						sMessage, 
						sMessageTitle, 
						MessageBoxButtons.YesNo, 
						MessageBoxIcon.Question, 
						MessageBoxDefaultButton.Button2);
					switch((int)oResult)
					{
						case (int) DialogResult.Yes:
						{
							// Reverse ReadOnly status
							file.Attributes = file.Attributes ^ FileAttributes.ReadOnly;
						
							// File exists and has been made writable
							return true;
						}
						case (int) DialogResult.No:
						{
							return false;
						}
						default:
						{
							return false;
						}
					}
				}
			}

			// The provided filename either does not exist or it exists and is read/write.
			return true;
		}


		/// <summary>
		/// Ask the user for a filename / path.
		/// </summary>
		private string SelectFileToSave(string dialogTitle)
		{
			//----- Save the XmlDocument to the file syatem.
			SaveFileDialog oDialog = new SaveFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = "xml";
			oDialog.Title = dialogTitle;
			oDialog.RestoreDirectory = true;

			// Show Open Dialog and Respond to OK
			if(oDialog.ShowDialog() == DialogResult.OK)
				return oDialog.FileName;
			else
				return string.Empty;
		}

		/// <summary>
		/// Ask the user for a filename / path.
		/// </summary>
		private string SelectFileToOpen(string dialogTitle)
		{
			//----- Save the XmlDocument to the file syatem.
			OpenFileDialog oDialog = new OpenFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = "xml";
			oDialog.Title = dialogTitle;
			oDialog.RestoreDirectory = true;

			// Show Open Dialog and Respond to OK
			if(oDialog.ShowDialog() == DialogResult.OK)
				return oDialog.FileName;
			else
				return string.Empty;
		}

		private void SaveBestNetwork()
		{
			string filename = SelectFileToSave("Save best genome as network XML");
			if(filename != string.Empty)
				SaveBestNetwork(filename);
		}

		private void SaveBestNetwork(string filename)
		{
			if(ea.BestGenome==null)
				return;

			//----- Determine the current experiment.
            /* NOTE RJM: Assumes that an experiment was already selected in event
               cmbDomain_SelectedIndexChanged
             */
            IExperiment experiment = selectedExperiment;

			//----- Write the genome to an XmlDocument.
			XmlDocument doc = new XmlDocument();
			XmlNetworkWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, selectedActivationFunction);
			FileInfo oFileInfo = new FileInfo(filename);

			if(IsWriteableFile(oFileInfo))
				doc.Save(oFileInfo.FullName);
			else
				return;
		}


		/// <summary>
		/// Ask the user for a filename / path.
		/// </summary>
		private void SaveBestGenome()
		{
			string filename = SelectFileToSave("Save best genome.");
			if(filename != string.Empty)
				SaveBestGenome(filename);
		}

		private void SaveBestGenome(string filename)
		{
			if(ea.BestGenome==null)
				return;

			//----- Determine the current experiment.
            /* NOTE RJM: Assumes that an experiment was already selected in event
               cmbDomain_SelectedIndexChanged
             */
            IExperiment experiment = selectedExperiment;

			//----- Write the genome to an XmlDocument.
			XmlDocument doc = new XmlDocument();
			XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, selectedActivationFunction);
			FileInfo oFileInfo = new FileInfo(filename);

			if(IsWriteableFile(oFileInfo))
				doc.Save(oFileInfo.FullName);
			else
				return;
		}

		private void SavePopulation()
		{
			string filename = SelectFileToSave("Save Population.");
			if(filename != string.Empty)
				SavePopulation(filename);
		}

		private void SavePopulation(string filename)
		{
			if(pop==null)
				return;

			//----- Determine the current experiment.
            /* NOTE RJM: Assumes that an experiment was already selected in event
               cmbDomain_SelectedIndexChanged
             */
            IExperiment experiment = selectedExperiment;

			//----- Write the population to an XmlDocument.
			XmlDocument doc = new XmlDocument();
			XmlPopulationWriter.Write(doc, pop, selectedActivationFunction);
			FileInfo oFileInfo = new FileInfo(filename);

			if(IsWriteableFile(oFileInfo))
				doc.Save(oFileInfo.FullName);
			else
				return;
		}

		private void LoadPopulation()
		{
			string filename = SelectFileToOpen("Load Population");
			if(filename == string.Empty)
				return;

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				IGenomeReader genomeReader = new XmlNeatGenomeReader();
				pop = XmlPopulationReader.Read(doc, genomeReader, new IdGeneratorFactory());
			}
			catch(Exception e)
			{
				MessageBox.Show("Problem loading population. \n" + e.Message);
				pop = null;
				return;
			}

			if(!pop.IsCompatibleWithNetwork(selectedExperiment.InputNeuronCount,
				selectedExperiment.OutputNeuronCount))
			{	
				MessageBox.Show(@"At least one genome in the population is incompatible with the currently selected experiment. Check the number of input/output neurons.");
				pop = null;
			}
			else
			{	// Population is OK.
				txtParamPopulationSize.Text = pop.GenomeList.Count.ToString();
			}
		}

		private void InititialisePopulationFromSeedGenome()
		{
			string filename = SelectFileToOpen("Load Seed Genome");
			if(filename == string.Empty)
				return;

			NeatGenome seedGenome=null;
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				seedGenome = XmlNeatGenomeReaderStatic.Read(doc);	
			}
			catch(Exception e)
			{
				MessageBox.Show("Problem loading genome. \n" + e.Message);
				pop = null;
				return;
			}

			if(!seedGenome.IsCompatibleWithNetwork(selectedExperiment.InputNeuronCount,
				selectedExperiment.OutputNeuronCount))
			{
				MessageBox.Show(@"The genome is incompatible with the currently selected experiment. Check the number of input/output neurons.");
				pop = null;
			}
			else
			{
				NeatParameters neatParameters = GetUserNeatParameters();
				IdGeneratorFactory idGeneratorFactory = new IdGeneratorFactory();
				IdGenerator idGenerator = idGeneratorFactory.CreateIdGenerator(seedGenome);
				GenomeList genomeList = GenomeFactory.CreateGenomeList(	seedGenome,
					neatParameters.populationSize,
					neatParameters,
					idGenerator);

				pop = new Population(idGenerator, genomeList);
			}
		}


		private void InititialisePopulationFromSeedPopulation()
		{
			string filename = SelectFileToOpen("Load Seed Population");
			if(filename == string.Empty)
				return;

			Population seedPopulation=null;
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				IGenomeReader genomeReader = new XmlNeatGenomeReader();
				seedPopulation = XmlPopulationReader.Read(doc, genomeReader, new IdGeneratorFactory());
			}
			catch(Exception e)
			{
				MessageBox.Show("Problem loading population. \n" + e.Message);
				pop = null;
				return;
			}

			if(!seedPopulation.IsCompatibleWithNetwork(selectedExperiment.InputNeuronCount,
				selectedExperiment.OutputNeuronCount))
			{	
				MessageBox.Show(@"At least one genome in the population is incompatible with the currently selected experiment. Check the number of input/output neurons.");
				pop = null;
			}
			else
			{
				NeatParameters neatParameters = GetUserNeatParameters();
				IdGeneratorFactory idGeneratorFactory = new IdGeneratorFactory();
				IdGenerator idGenerator = idGeneratorFactory.CreateIdGenerator(seedPopulation.GenomeList);
				GenomeList genomeList = GenomeFactory.CreateGenomeList(	seedPopulation,
					neatParameters.populationSize,
					neatParameters,
					idGenerator);

				pop = new Population(idGenerator, genomeList);
			}
		}

		private void UpdateGuiState()
		{
			if(pop==null)
			{	
				// Search cannot be started without a population.
				btnSearchStart.Enabled = false;
				btnSearchStop.Enabled = false;
				btnSearchReset.Enabled = false;

				cmbDomain.Enabled = true;
				cmbExperimentActivationFn.Enabled = true;
				btnLoadDefaults.Enabled = true;
				gbxSearchParameters.Enabled = true;
				gbxFile.Enabled=true;

				mnuFileSave.Enabled = false;
				mnuInitPop.Enabled = true;
			}
			else
			{	
				// There is a population. Check search state.
				switch(searchState)
				{
					case SearchStateEnum.Reset:
						btnSearchStart.Enabled = true;
						btnSearchStop.Enabled = false;
						btnSearchReset.Enabled = false;

						cmbDomain.Enabled = true;
						cmbExperimentActivationFn.Enabled = true;
						btnLoadDefaults.Enabled = true;
						gbxSearchParameters.Enabled = true;
						gbxFile.Enabled=true;

						mnuFileSave.Enabled = false;
						mnuInitPop.Enabled = false;
						break;
					
					case SearchStateEnum.Paused:
						btnSearchStart.Enabled = true;
						btnSearchStop.Enabled = false;
						btnSearchReset.Enabled = true;

						cmbDomain.Enabled = false;
						cmbExperimentActivationFn.Enabled = false;
						btnLoadDefaults.Enabled = false;
						gbxSearchParameters.Enabled = false;
						gbxFile.Enabled=false;

						mnuFileSave.Enabled = true;
						mnuInitPop.Enabled = false;
						break;

					case SearchStateEnum.Running:
						btnSearchStart.Enabled = false;
						btnSearchStop.Enabled = true;
						btnSearchReset.Enabled = false;

						cmbDomain.Enabled = false;
						cmbExperimentActivationFn.Enabled = false;
						btnLoadDefaults.Enabled = false;
						gbxSearchParameters.Enabled = false;
						gbxFile.Enabled=false;

						mnuFileSave.Enabled = false;
						mnuInitPop.Enabled = false;
						break;
				}
			}
			
		}

		private void FlushAndCloseLogFile()
		{
			// Ensure any log data is written to file before the application terminates.
			if(logWriter != null)
			{
				logWriter.Close();
				logWriter = null;
			}
		}

		private void ClearUpdateFreqMenus()
		{
			mnuViewUpdateFrequency1Sec.Checked = false;
			mnuViewUpdateFrequency2Sec.Checked = false;
			mnuViewUpdateFrequency5Sec.Checked = false;
			mnuViewUpdateFrequency10Sec.Checked = false;
			mnuViewUpdateFrequency1Gen.Checked = false;
			mnuViewUpdateFrequency2Gen.Checked = false;
			mnuViewUpdateFrequency5Gen.Checked = false;
			mnuViewUpdateFrequency10Gen.Checked = false;
		}

		#endregion

		#region Event Handlers
		
		#region File Menu

		private void mnuFileSaveBestAsNetwork_Click(object sender, System.EventArgs e)
		{
			SaveBestNetwork();
		}

		private void mnuFileSaveBestAsGenome_Click(object sender, System.EventArgs e)
		{
			SaveBestGenome();
		}


		private void mnuFileSavePopulation_Click(object sender, System.EventArgs e)
		{
			SavePopulation();
		}

		#endregion

		#region Initialize Population Menu

		private void mnuInitPopLoadPopulation_Click(object sender, System.EventArgs e)
		{
			LoadPopulation();
			UpdateGuiState();
		}

		private void mnuInitPopLoadSeedGenome_Click(object sender, System.EventArgs e)
		{
			InititialisePopulationFromSeedGenome();
			UpdateGuiState();
		}

		private void mnuInitPopLoadSeedPopulation_Click(object sender, System.EventArgs e)
		{
			InititialisePopulationFromSeedPopulation();
			UpdateGuiState();
		}

		private void mnuInitPopAutoGenerate_Click(object sender, System.EventArgs e)
		{
			ListItem listItem = (ListItem)cmbDomain.SelectedItem;
			/* NOTE RJM: Assumes that an experiment was already selected in event
			   cmbDomain_SelectedIndexChanged
			 */
			IExperiment experiment = selectedExperiment;

			NeatParameters neatParameters = GetUserNeatParameters();
			AutoGeneratePopulationForm form = new AutoGeneratePopulationForm(
												neatParameters, 
												selectedExperiment.InputNeuronCount,
												selectedExperiment.OutputNeuronCount);
			form.ShowDialog(this);
			if(form.DialogResult==DialogResult.OK)
			{
				pop = form.Population;
			}

			UpdateGuiState();
		}

		#endregion

		#region Search Buttons

		private void btnSearchStart_Click(object sender, System.EventArgs e)
		{
			searchState = SearchStateEnum.Running;
			UpdateGuiState();

			if(searchThread==null)
			{
                /* NOTE RJM: Assumes that an experiment was already selected in event
                   cmbDomain_SelectedIndexChanged
                 */
                IExperiment experiment = selectedExperiment;

				//--- Generate a new population.
				NeatParameters neatParameters = GetUserNeatParameters();
				IdGenerator idGenerator = new IdGenerator();

				//--- Create a new EvolutionAlgorithm.
				experiment.ResetEvaluator(selectedActivationFunction);
				ea = new EvolutionAlgorithm(pop, experiment.PopulationEvaluator, neatParameters);
				ea.IsPruningModeEnabled = chkParamPruningModeEnabled.Checked;
                ea.PruningModeAlwaysOn = chkParamPruningModeAlwaysOn.Checked;

				ea.IsConnectionWeightFixingEnabled = chkParamEnableConnectionWeightFixing.Checked;
			
				//--- Create a log file if necessary.
				if(chkFileWriteLog.Checked)
				{
					string filename = txtFileLogBaseName.Text + '_' + DateTime.Now.ToString("yyMMdd_hhmmss") + ".txt";
					logWriter = new StreamWriter(filename, true);
					logWriter.WriteLine("Gen, ClockTime, EvalStateMsg, BestFitness, MeanFitness, SpeciesCount, SpeciesCompatThreshold, BestGenomeNeuronCount, BestGenomeConnectionCount, PopMeanNeuronCount, PopMeanConnectionCount, MeanStructuresPerGenome");
				}

				stopSearchSignal=false; // reset this signal.
				searchThread = new Thread(new ThreadStart(SearchThreadMethod));
				searchThread.IsBackground = true;
				searchThread.Priority = ThreadPriority.BelowNormal;
				ticksAtSearchStart = DateTime.Now.Ticks;

				searchThread.Start();
			}
			else
			{
				searchThread.Resume();
			}
		}

		private void btnSearchStop_Click(object sender, System.EventArgs e)
		{
			// Don't stop the thread here. Send a signal to the thread to
			// stop when it is next convienient. This is primarily done
			// so that we always stop the search with a full population - during
			// PerformOneGeneration() the population size fluctuates.
			stopSearchSignal=true;
		}

		private void btnSearchReset_Click(object sender, System.EventArgs e)
		{
			if(searchThread!=null)
			{
				if((searchThread.ThreadState & System.Threading.ThreadState.Suspended) ==0)
				{	// User must stop thread first.
					return;
				}
				// Bug in .Net requires call to Resume before Abort.!
				searchThread.Resume();
				searchThread.Abort(); 

			}

			FlushAndCloseLogFile();

			searchThread = null;
			txtLogWindow.Text = string.Empty;
			pop = null;
			searchState = SearchStateEnum.Reset;
			UpdateGuiState();

			// Reset stats window.
			txtStatsGeneration.Text = string.Empty;
			txtStatsBest.Text = string.Empty;
			txtStatsMean.Text = string.Empty;
			txtStatsSpeciesCount.Text = string.Empty;
			txtStatsCompatibilityThreshold.Text = string.Empty;
			txtStatsEvaluatorStateMsg.Text = string.Empty;
			txtStatsTotalEvaluations.Text = string.Empty;

			// Reset visualization windows.
			if(progressForm!=null)
				progressForm.Reset();

			if(bestGenomeForm!=null)
				bestGenomeForm.Reset();

			if(speciesForm!=null)
				speciesForm.Reset();
		}

		#endregion

		#region Update Frequency Menu

		private void mnuViewUpdateFrequency1Sec_Click(object sender, System.EventArgs e)
		{
			updateFreqTicks=10000000; 
			updateMode=true;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency1Sec.Checked = true;
		}

		private void mnuViewUpdateFrequency2Sec_Click(object sender, System.EventArgs e)
		{
			updateFreqTicks=20000000; 
			updateMode=true;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency2Sec.Checked = true;
		}

		private void mnuViewUpdateFrequency5Sec_Click(object sender, System.EventArgs e)
		{
			updateFreqTicks=50000000; 
			updateMode=true;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency5Sec.Checked = true;
		}

		private void mnuViewUpdateFrequency10Sec_Click(object sender, System.EventArgs e)
		{
			updateFreqTicks=100000000; 
			updateMode=true;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency10Sec.Checked = true;
		}

		private void mnuViewUpdateFrequency1Gen_Click(object sender, System.EventArgs e)
		{
			updateFreqGens=1;
			updateMode=false;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency1Gen.Checked = true;
		}

		private void mnuViewUpdateFrequency2Gen_Click(object sender, System.EventArgs e)
		{
			updateFreqGens=2;
			updateMode=false;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency2Gen.Checked = true;
		}

		private void mnuViewUpdateFrequency5Gen_Click(object sender, System.EventArgs e)
		{
			updateFreqGens=5;
			updateMode=false;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency5Gen.Checked = true;
		}

		private void mnuViewUpdateFrequency10Gen_Click(object sender, System.EventArgs e)
		{
			updateFreqGens=10;
			updateMode=false;
			ClearUpdateFreqMenus();
			mnuViewUpdateFrequency10Gen.Checked = true;
		}

		#endregion

		#region About Menu

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			Form frmAboutBox = new AboutBox();
			frmAboutBox.ShowDialog(this);
		}

		#endregion

		#region Misc Buttons

		private void btnDomainExplanation_Click(object sender, System.EventArgs e)
		{
            /* NOTE RJM: Assumes that an experiment was already selected in event
               cmbDomain_SelectedIndexChanged
             */
            IExperiment experiment = selectedExperiment;

			MessageBox.Show(experiment.ExplanatoryText);
		}

		private void btnLoadDefaults_Click(object sender, System.EventArgs e)
		{
			// Load default neatParameters.
			LoadNeatParameters(selectedExperiment.DefaultNeatParameters);
		}

		private void btnPlotFunction_Click(object sender, System.EventArgs e)
		{
            if(activationFunctionForm==null)
            {
                activationFunctionForm = new ActivationFunctionForm();
                activationFunctionForm.ActivationFunction = selectedActivationFunction;
                activationFunctionForm.Closed+=new EventHandler(activationFunctionForm_Closed);
                activationFunctionForm.Show();
            }
		}

		#endregion

		#region Domain / Experiment GroupBox Controls
		
        private void cmbDomain_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Discard any existing population - its probably invalid for this problem (input/output neuron counts).
            pop = null;

            // Store a reference to the selected experiment.
            ListItem listItem = (ListItem)cmbDomain.SelectedItem;
            selectedExperimentConfigInfo = (ExperimentConfigInfo)listItem.Data;
			
            // This object will get re-created every time the user switches 
            // options, so here's to garbage collection working.
			ObjectHandle objectHandle = Activator.CreateInstanceFrom(selectedExperimentConfigInfo.AssemblyUrl, selectedExperimentConfigInfo.TypeName);
			selectedExperiment = (IExperiment)objectHandle.Unwrap();
			selectedExperiment.LoadExperimentParameters(selectedExperimentConfigInfo.ParameterTable);

//			selectedExperiment = (IExperiment)AppTools.RunSubApplication(selectedExperimentConfigInfo.ApplicationData);
//			selectedExperiment.LoadExperimentParameters(selectedExperimentConfigInfo.ParameterTable);

            txtDomainInputNeuronCount.Text = selectedExperiment.InputNeuronCount.ToString();
            txtDomainOutputNeuronCount.Text = selectedExperiment.OutputNeuronCount.ToString();

            SelectActivationFunction(selectedExperiment.SuggestedActivationFunction);

            // Close any open experiment view. It will likely be for a different experiment.
            if(experimentView!=null)
            {
                experimentView.Close();
            }
        }

		private void cmbExperimentActivationFn_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Store a reference to the selected activation function.
            // Store a reference to the selected activation function.
            ListItem listItem = (ListItem)cmbExperimentActivationFn.SelectedItem;
            selectedActivationFunction = (IActivationFunction)listItem.Data;
            txtExperimentActivationFn.Text = selectedActivationFunction.FunctionString;
        }

		private void SelectActivationFunction(IActivationFunction activationFn)
		{
		    // TODO RJM: See how this is affected by the recent change.
			foreach(ListItem listItem in cmbExperimentActivationFn.Items)
			{
				if(listItem.Data.GetType() == activationFn.GetType())
				{
					cmbExperimentActivationFn.SelectedItem = listItem;
					return;
				}
			}
		}

		#endregion

		#region Visualization Menu

		private void mnuVisualizationProgressGraph_Click(object sender, System.EventArgs e)
		{
			if(!progressForm.Visible)
			{
				progressForm.Show();
			}
		}

		private void mnuVisualizationBest_Click(object sender, System.EventArgs e)
		{
			if(bestGenomeForm==null)
			{
				bestGenomeForm = new BestGenomeForm();
				bestGenomeForm.Closed+=new EventHandler(bestGenomeForm_Closed);
				bestGenomeForm.Show();

				//TODO: Slightly dodgy. Chance of a threading problem?
				if(ea!=null && ea.BestGenome!=null)
					bestGenomeForm.SetBestGenome((NeatGenome)ea.BestGenome, ea.Generation);
			}
		}

		private void mnuVisualizationSpecies_Click(object sender, System.EventArgs e)
		{
			if(speciesForm==null)
			{
				speciesForm = new SpeciesForm();
				speciesForm.Closed+=new EventHandler(speciesForm_Closed);
				speciesForm.Show();

				if(ea!=null)
					speciesForm.Update(ea);
			}
		}

		private void mnuVisualizationExperiment_Click(object sender, System.EventArgs e)
		{
			// If there is already a view created then do nothing.
			if(experimentView!=null)
				return;

			// Just in case, test for null.
			if(selectedExperiment==null)
				return;

			// Ask the current experiment to create a view.
			experimentView = selectedExperiment.CreateExperimentView();

			// Some experiments may not have aview defined. Test for null.
			if(experimentView==null)
				return;

			// OK we have a view, so lets show it.
			experimentView.Closed +=new EventHandler(experimentView_Closed);
			experimentView.Show();
		}

		#endregion

		#region BestGenomeForm

		private void bestGenomeForm_Closed(object sender, EventArgs e)
		{
			bestGenomeForm=null;
		}

		private void speciesForm_Closed(object sender, EventArgs e)
		{
			speciesForm=null;
		}

        private void activationFunctionForm_Closed(object sender, EventArgs e)
        {
            activationFunctionForm=null;
        }
		
		#endregion

		#region ExperimentView

		private void experimentView_Closed(object sender, EventArgs e)
		{
			experimentView=null;
		}

		#endregion

		#region Search Parameters GroupBox Controls

		private void chkParamPruningModeEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
			// If pruning mode is disabled then connection weight fixing cannot occur.
			if(chkParamPruningModeEnabled.Checked)
			{
				chkParamEnableConnectionWeightFixing.Enabled = true;
			}
			else
			{
				chkParamEnableConnectionWeightFixing.Enabled = false;
				chkParamEnableConnectionWeightFixing.Checked = false;
			}
		}

		#endregion

		#region Form Events

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			FlushAndCloseLogFile();
		}

		#endregion

		#endregion
	}
}
