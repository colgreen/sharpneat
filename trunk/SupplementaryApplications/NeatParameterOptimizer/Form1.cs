using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Xml;

using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

using NeatParameterOptimizer.Xml;

namespace NeatParameterOptimizer
{
	public class Form1 : System.Windows.Forms.Form
	{
		#region Enumerations

		enum SearchStateEnum
		{
			Reset,
			Paused,
			Running
		}

		enum FileOutputType
		{
			BestOnly,
			All
		}

		#endregion

		#region Class Variables

		NeatParametersWrapperList npwList=null;
		MetaEvolutionAlgorithm mea=null;
		IExperiment selectedExperiment=null;
		IActivationFunction selectedActivationFunction=null;
		EAStoppingCondition stoppingCondition=null;

		Thread searchThread;
		bool stopSearchSignal;
		SearchStateEnum searchState = SearchStateEnum.Reset;

		#endregion

		#region Windows Form Designer variables

		private System.Windows.Forms.Button btnSearchStart;
		private System.Windows.Forms.Button btnSearchStop;
		private System.Windows.Forms.Button btnSearchReset;
		private System.Windows.Forms.GroupBox gbxSearchControl;
		private System.Windows.Forms.ComboBox cmbDomain;
		private System.Windows.Forms.GroupBox gbxSearchParams;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.ComboBox cmbExperimentActivationFn;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.TextBox txtParamPruningEndComplexityStagnationThreshold;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.TextBox txtParamPruningBeginFitnessStagnationThreshold;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.TextBox txtParamPruningBeginComplexityThreshold;
		private System.Windows.Forms.CheckBox chkParamEnableConnectionWeightFixing;
		private System.Windows.Forms.CheckBox chkParamPruningModeEnabled;
		private System.Windows.Forms.TextBox txtParamPopulationSize;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txtParamConnectionWeightMutationSigma;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtParamConnectionWeightRange;
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
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.TextBox txtParamMutateDeleteNeuron;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.TextBox txtParamMutateDeleteConnection;
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
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txtParamInterspeciesMating;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtParamOffspringCrossover;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox txtParamOffspringMutation;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button btnLoadDefaults;
		private System.Windows.Forms.CheckBox chkPopulationSize;
		private System.Windows.Forms.CheckBox chkPInitialPopulationInterconnections;
		private System.Windows.Forms.CheckBox chkPOffspringAsexual_Sexual;
		private System.Windows.Forms.CheckBox chkPInterspeciesMating;
		private System.Windows.Forms.CheckBox chkPDisjointExcessGenesRecombined;
		private System.Windows.Forms.CheckBox chkPMutateType;
		private System.Windows.Forms.CheckBox chkConnectionMutationParameterGroupList;
		private System.Windows.Forms.CheckBox chkCompatibilityThreshold;
		private System.Windows.Forms.CheckBox chkCompatibilityDisjointCoeff;
		private System.Windows.Forms.CheckBox chkCompatibilityExcessCoeff;
		private System.Windows.Forms.CheckBox chkCompatibilityWeightDeltaCoeff;
		private System.Windows.Forms.CheckBox chkElitismProportion;
		private System.Windows.Forms.CheckBox chkSelectionProportion;
		private System.Windows.Forms.CheckBox chkTargetSpeciesCountWindow;
		private System.Windows.Forms.CheckBox chkSpeciesDropoffAge;
		private System.Windows.Forms.CheckBox chkPruningPhaseBeginComplexityThreshold;
		private System.Windows.Forms.CheckBox chkPruningPhaseBeginFitnessStagnationThreshold;
		private System.Windows.Forms.CheckBox chkPruningPhaseEndComplexityStagnationThreshold;
		private System.Windows.Forms.CheckBox chkConnectionWeightRange;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnSearchTerminate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbEAStoppingCondition;
		private System.Windows.Forms.TextBox txtEaMaxGenerations;
		private System.Windows.Forms.TextBox txtEaMaxDuration;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox gbxFile;
		private System.Windows.Forms.ComboBox cmbFileOutputType;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtFileOutputPrefix;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox gbxNeatParameters;
		private System.Windows.Forms.GroupBox gbxMetaNeatParameters;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.GroupBox gbxLog;
		private System.Windows.Forms.TextBox txtLogWindow;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtEaRunsPerMetaEval;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtStatsGeneration;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtStatsMean;
		private System.Windows.Forms.TextBox txtStatsBest;
		private System.Windows.Forms.Button btnCalcAvgPerformance;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

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
			this.btnSearchStart = new System.Windows.Forms.Button();
			this.btnSearchStop = new System.Windows.Forms.Button();
			this.btnSearchReset = new System.Windows.Forms.Button();
			this.gbxSearchControl = new System.Windows.Forms.GroupBox();
			this.btnSearchTerminate = new System.Windows.Forms.Button();
			this.cmbDomain = new System.Windows.Forms.ComboBox();
			this.gbxSearchParams = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtEaRunsPerMetaEval = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtEaMaxDuration = new System.Windows.Forms.TextBox();
			this.txtEaMaxGenerations = new System.Windows.Forms.TextBox();
			this.cmbEAStoppingCondition = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnLoadDefaults = new System.Windows.Forms.Button();
			this.label21 = new System.Windows.Forms.Label();
			this.cmbExperimentActivationFn = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.btnCalcAvgPerformance = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtStatsGeneration = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.txtStatsMean = new System.Windows.Forms.TextBox();
			this.txtStatsBest = new System.Windows.Forms.TextBox();
			this.gbxFile = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtFileOutputPrefix = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cmbFileOutputType = new System.Windows.Forms.ComboBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.gbxNeatParameters = new System.Windows.Forms.GroupBox();
			this.txtParamConnectionWeightMutationSigma = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.txtParamConnectionWeightRange = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.txtParamInterspeciesMating = new System.Windows.Forms.TextBox();
			this.label25 = new System.Windows.Forms.Label();
			this.txtParamOffspringCrossover = new System.Windows.Forms.TextBox();
			this.label26 = new System.Windows.Forms.Label();
			this.txtParamOffspringMutation = new System.Windows.Forms.TextBox();
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
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.label41 = new System.Windows.Forms.Label();
			this.txtParamPruningEndComplexityStagnationThreshold = new System.Windows.Forms.TextBox();
			this.label40 = new System.Windows.Forms.Label();
			this.txtParamPruningBeginFitnessStagnationThreshold = new System.Windows.Forms.TextBox();
			this.label39 = new System.Windows.Forms.Label();
			this.txtParamPruningBeginComplexityThreshold = new System.Windows.Forms.TextBox();
			this.chkParamEnableConnectionWeightFixing = new System.Windows.Forms.CheckBox();
			this.chkParamPruningModeEnabled = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtParamCompatDisjointCoeff = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.txtParamCompatExcessCoeff = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.txtParamCompatWeightDeltaCoeff = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.txtParamCompatThreshold = new System.Windows.Forms.TextBox();
			this.label28 = new System.Windows.Forms.Label();
			this.txtParamPopulationSize = new System.Windows.Forms.TextBox();
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
			this.label17 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.gbxMetaNeatParameters = new System.Windows.Forms.GroupBox();
			this.chkCompatibilityThreshold = new System.Windows.Forms.CheckBox();
			this.chkConnectionMutationParameterGroupList = new System.Windows.Forms.CheckBox();
			this.chkPMutateType = new System.Windows.Forms.CheckBox();
			this.chkPDisjointExcessGenesRecombined = new System.Windows.Forms.CheckBox();
			this.chkPInterspeciesMating = new System.Windows.Forms.CheckBox();
			this.chkPOffspringAsexual_Sexual = new System.Windows.Forms.CheckBox();
			this.chkPInitialPopulationInterconnections = new System.Windows.Forms.CheckBox();
			this.chkPopulationSize = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkConnectionWeightRange = new System.Windows.Forms.CheckBox();
			this.chkPruningPhaseEndComplexityStagnationThreshold = new System.Windows.Forms.CheckBox();
			this.chkPruningPhaseBeginFitnessStagnationThreshold = new System.Windows.Forms.CheckBox();
			this.chkPruningPhaseBeginComplexityThreshold = new System.Windows.Forms.CheckBox();
			this.chkSpeciesDropoffAge = new System.Windows.Forms.CheckBox();
			this.chkTargetSpeciesCountWindow = new System.Windows.Forms.CheckBox();
			this.chkSelectionProportion = new System.Windows.Forms.CheckBox();
			this.chkElitismProportion = new System.Windows.Forms.CheckBox();
			this.chkCompatibilityWeightDeltaCoeff = new System.Windows.Forms.CheckBox();
			this.chkCompatibilityExcessCoeff = new System.Windows.Forms.CheckBox();
			this.chkCompatibilityDisjointCoeff = new System.Windows.Forms.CheckBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.gbxLog = new System.Windows.Forms.GroupBox();
			this.txtLogWindow = new System.Windows.Forms.TextBox();
			this.gbxSearchControl.SuspendLayout();
			this.gbxSearchParams.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.gbxFile.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.gbxNeatParameters.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.gbxMetaNeatParameters.SuspendLayout();
			this.gbxLog.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSearchStart
			// 
			this.btnSearchStart.Location = new System.Drawing.Point(8, 16);
			this.btnSearchStart.Name = "btnSearchStart";
			this.btnSearchStart.Size = new System.Drawing.Size(64, 32);
			this.btnSearchStart.TabIndex = 0;
			this.btnSearchStart.Text = "Start / Continue";
			this.btnSearchStart.Click += new System.EventHandler(this.btnSearchStart_Click);
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
			// btnSearchReset
			// 
			this.btnSearchReset.Location = new System.Drawing.Point(152, 16);
			this.btnSearchReset.Name = "btnSearchReset";
			this.btnSearchReset.Size = new System.Drawing.Size(64, 32);
			this.btnSearchReset.TabIndex = 2;
			this.btnSearchReset.Text = "Reset";
			this.btnSearchReset.Click += new System.EventHandler(this.btnSearchReset_Click);
			// 
			// gbxSearchControl
			// 
			this.gbxSearchControl.Controls.Add(this.btnSearchTerminate);
			this.gbxSearchControl.Controls.Add(this.btnSearchReset);
			this.gbxSearchControl.Controls.Add(this.btnSearchStop);
			this.gbxSearchControl.Controls.Add(this.btnSearchStart);
			this.gbxSearchControl.Location = new System.Drawing.Point(8, 8);
			this.gbxSearchControl.Name = "gbxSearchControl";
			this.gbxSearchControl.Size = new System.Drawing.Size(312, 56);
			this.gbxSearchControl.TabIndex = 3;
			this.gbxSearchControl.TabStop = false;
			this.gbxSearchControl.Text = "Search Control";
			// 
			// btnSearchTerminate
			// 
			this.btnSearchTerminate.Location = new System.Drawing.Point(240, 16);
			this.btnSearchTerminate.Name = "btnSearchTerminate";
			this.btnSearchTerminate.Size = new System.Drawing.Size(64, 32);
			this.btnSearchTerminate.TabIndex = 3;
			this.btnSearchTerminate.Text = "Terminate";
			this.btnSearchTerminate.Click += new System.EventHandler(this.btnSearchTerminate_Click);
			// 
			// cmbDomain
			// 
			this.cmbDomain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDomain.Location = new System.Drawing.Point(8, 40);
			this.cmbDomain.Name = "cmbDomain";
			this.cmbDomain.Size = new System.Drawing.Size(192, 21);
			this.cmbDomain.TabIndex = 37;
			this.cmbDomain.SelectedIndexChanged += new System.EventHandler(this.cmbDomain_SelectedIndexChanged);
			// 
			// gbxSearchParams
			// 
			this.gbxSearchParams.Controls.Add(this.label8);
			this.gbxSearchParams.Controls.Add(this.txtEaRunsPerMetaEval);
			this.gbxSearchParams.Controls.Add(this.label5);
			this.gbxSearchParams.Controls.Add(this.label4);
			this.gbxSearchParams.Controls.Add(this.txtEaMaxDuration);
			this.gbxSearchParams.Controls.Add(this.txtEaMaxGenerations);
			this.gbxSearchParams.Controls.Add(this.cmbEAStoppingCondition);
			this.gbxSearchParams.Controls.Add(this.label3);
			this.gbxSearchParams.Controls.Add(this.btnLoadDefaults);
			this.gbxSearchParams.Controls.Add(this.label21);
			this.gbxSearchParams.Controls.Add(this.cmbExperimentActivationFn);
			this.gbxSearchParams.Controls.Add(this.label1);
			this.gbxSearchParams.Controls.Add(this.cmbDomain);
			this.gbxSearchParams.Location = new System.Drawing.Point(8, 72);
			this.gbxSearchParams.Name = "gbxSearchParams";
			this.gbxSearchParams.Size = new System.Drawing.Size(424, 184);
			this.gbxSearchParams.TabIndex = 38;
			this.gbxSearchParams.TabStop = false;
			this.gbxSearchParams.Text = "Search Parameters";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(200, 16);
			this.label8.TabIndex = 61;
			this.label8.Text = "EvolutionAlgorithm runs per meta-eval";
			// 
			// txtEaRunsPerMetaEval
			// 
			this.txtEaRunsPerMetaEval.Location = new System.Drawing.Point(8, 152);
			this.txtEaRunsPerMetaEval.Name = "txtEaRunsPerMetaEval";
			this.txtEaRunsPerMetaEval.Size = new System.Drawing.Size(104, 20);
			this.txtEaRunsPerMetaEval.TabIndex = 60;
			this.txtEaRunsPerMetaEval.Text = "16";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(216, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 16);
			this.label5.TabIndex = 59;
			this.label5.Text = "Max Generations";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(216, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(144, 16);
			this.label4.TabIndex = 58;
			this.label4.Text = "Max Duration (Secs)";
			// 
			// txtEaMaxDuration
			// 
			this.txtEaMaxDuration.Location = new System.Drawing.Point(216, 72);
			this.txtEaMaxDuration.Name = "txtEaMaxDuration";
			this.txtEaMaxDuration.Size = new System.Drawing.Size(104, 20);
			this.txtEaMaxDuration.TabIndex = 57;
			this.txtEaMaxDuration.Text = "10";
			// 
			// txtEaMaxGenerations
			// 
			this.txtEaMaxGenerations.Location = new System.Drawing.Point(216, 112);
			this.txtEaMaxGenerations.Name = "txtEaMaxGenerations";
			this.txtEaMaxGenerations.Size = new System.Drawing.Size(104, 20);
			this.txtEaMaxGenerations.TabIndex = 56;
			this.txtEaMaxGenerations.Text = "60000";
			// 
			// cmbEAStoppingCondition
			// 
			this.cmbEAStoppingCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbEAStoppingCondition.Location = new System.Drawing.Point(216, 32);
			this.cmbEAStoppingCondition.Name = "cmbEAStoppingCondition";
			this.cmbEAStoppingCondition.Size = new System.Drawing.Size(192, 21);
			this.cmbEAStoppingCondition.TabIndex = 55;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(216, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(200, 16);
			this.label3.TabIndex = 54;
			this.label3.Text = "EvolutionAlgorithm Stopping Condition";
			// 
			// btnLoadDefaults
			// 
			this.btnLoadDefaults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnLoadDefaults.Location = new System.Drawing.Point(8, 62);
			this.btnLoadDefaults.Name = "btnLoadDefaults";
			this.btnLoadDefaults.Size = new System.Drawing.Size(192, 24);
			this.btnLoadDefaults.TabIndex = 53;
			this.btnLoadDefaults.Text = "Load Default Seed Parameters";
			this.btnLoadDefaults.Click += new System.EventHandler(this.btnLoadDefaults_Click);
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(8, 88);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(112, 16);
			this.label21.TabIndex = 52;
			this.label21.Text = "Activation Fn";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbExperimentActivationFn
			// 
			this.cmbExperimentActivationFn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbExperimentActivationFn.Location = new System.Drawing.Point(8, 104);
			this.cmbExperimentActivationFn.Name = "cmbExperimentActivationFn";
			this.cmbExperimentActivationFn.Size = new System.Drawing.Size(192, 21);
			this.cmbExperimentActivationFn.TabIndex = 51;
			this.cmbExperimentActivationFn.SelectedIndexChanged += new System.EventHandler(this.cmbExperimentActivationFn_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(168, 16);
			this.label1.TabIndex = 38;
			this.label1.Text = "Domain / Experiment";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(456, 424);
			this.tabControl1.TabIndex = 39;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.btnCalcAvgPerformance);
			this.tabPage1.Controls.Add(this.groupBox5);
			this.tabPage1.Controls.Add(this.gbxFile);
			this.tabPage1.Controls.Add(this.gbxSearchControl);
			this.tabPage1.Controls.Add(this.gbxSearchParams);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(448, 398);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Search Control";
			// 
			// btnCalcAvgPerformance
			// 
			this.btnCalcAvgPerformance.Location = new System.Drawing.Point(336, 16);
			this.btnCalcAvgPerformance.Name = "btnCalcAvgPerformance";
			this.btnCalcAvgPerformance.Size = new System.Drawing.Size(96, 32);
			this.btnCalcAvgPerformance.TabIndex = 41;
			this.btnCalcAvgPerformance.Text = "Calc. Avg. Performance";
			this.btnCalcAvgPerformance.Click += new System.EventHandler(this.btnCalcAvgPerformance_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label9);
			this.groupBox5.Controls.Add(this.txtStatsGeneration);
			this.groupBox5.Controls.Add(this.label10);
			this.groupBox5.Controls.Add(this.label11);
			this.groupBox5.Controls.Add(this.txtStatsMean);
			this.groupBox5.Controls.Add(this.txtStatsBest);
			this.groupBox5.Location = new System.Drawing.Point(224, 264);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(176, 112);
			this.groupBox5.TabIndex = 40;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Statistics";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(88, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 16);
			this.label9.TabIndex = 13;
			this.label9.Text = "Generation";
			// 
			// txtStatsGeneration
			// 
			this.txtStatsGeneration.Location = new System.Drawing.Point(8, 24);
			this.txtStatsGeneration.Name = "txtStatsGeneration";
			this.txtStatsGeneration.ReadOnly = true;
			this.txtStatsGeneration.Size = new System.Drawing.Size(80, 20);
			this.txtStatsGeneration.TabIndex = 12;
			this.txtStatsGeneration.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(88, 72);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(40, 16);
			this.label10.TabIndex = 11;
			this.label10.Text = "Mean";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(88, 48);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(40, 16);
			this.label11.TabIndex = 10;
			this.label11.Text = "Best";
			// 
			// txtStatsMean
			// 
			this.txtStatsMean.Location = new System.Drawing.Point(8, 72);
			this.txtStatsMean.Name = "txtStatsMean";
			this.txtStatsMean.ReadOnly = true;
			this.txtStatsMean.Size = new System.Drawing.Size(80, 20);
			this.txtStatsMean.TabIndex = 9;
			this.txtStatsMean.Text = "";
			// 
			// txtStatsBest
			// 
			this.txtStatsBest.Location = new System.Drawing.Point(8, 48);
			this.txtStatsBest.Name = "txtStatsBest";
			this.txtStatsBest.ReadOnly = true;
			this.txtStatsBest.Size = new System.Drawing.Size(80, 20);
			this.txtStatsBest.TabIndex = 8;
			this.txtStatsBest.Text = "";
			// 
			// gbxFile
			// 
			this.gbxFile.Controls.Add(this.label7);
			this.gbxFile.Controls.Add(this.txtFileOutputPrefix);
			this.gbxFile.Controls.Add(this.label6);
			this.gbxFile.Controls.Add(this.cmbFileOutputType);
			this.gbxFile.Location = new System.Drawing.Point(8, 264);
			this.gbxFile.Name = "gbxFile";
			this.gbxFile.Size = new System.Drawing.Size(208, 112);
			this.gbxFile.TabIndex = 39;
			this.gbxFile.TabStop = false;
			this.gbxFile.Text = "File Output";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 64);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(112, 16);
			this.label7.TabIndex = 59;
			this.label7.Text = "File output prefix";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtFileOutputPrefix
			// 
			this.txtFileOutputPrefix.Location = new System.Drawing.Point(8, 80);
			this.txtFileOutputPrefix.Name = "txtFileOutputPrefix";
			this.txtFileOutputPrefix.Size = new System.Drawing.Size(192, 20);
			this.txtFileOutputPrefix.TabIndex = 58;
			this.txtFileOutputPrefix.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 24);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 16);
			this.label6.TabIndex = 57;
			this.label6.Text = "Output Type";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbFileOutputType
			// 
			this.cmbFileOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFileOutputType.Location = new System.Drawing.Point(8, 40);
			this.cmbFileOutputType.Name = "cmbFileOutputType";
			this.cmbFileOutputType.Size = new System.Drawing.Size(192, 21);
			this.cmbFileOutputType.TabIndex = 56;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.gbxNeatParameters);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(448, 398);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Seed Parameters";
			// 
			// gbxNeatParameters
			// 
			this.gbxNeatParameters.Controls.Add(this.txtParamConnectionWeightMutationSigma);
			this.gbxNeatParameters.Controls.Add(this.label16);
			this.gbxNeatParameters.Controls.Add(this.txtParamConnectionWeightRange);
			this.gbxNeatParameters.Controls.Add(this.groupBox1);
			this.gbxNeatParameters.Controls.Add(this.groupBox4);
			this.gbxNeatParameters.Controls.Add(this.groupBox7);
			this.gbxNeatParameters.Controls.Add(this.groupBox3);
			this.gbxNeatParameters.Controls.Add(this.label28);
			this.gbxNeatParameters.Controls.Add(this.txtParamPopulationSize);
			this.gbxNeatParameters.Controls.Add(this.groupBox2);
			this.gbxNeatParameters.Controls.Add(this.label17);
			this.gbxNeatParameters.Location = new System.Drawing.Point(8, 8);
			this.gbxNeatParameters.Name = "gbxNeatParameters";
			this.gbxNeatParameters.Size = new System.Drawing.Size(440, 384);
			this.gbxNeatParameters.TabIndex = 66;
			this.gbxNeatParameters.TabStop = false;
			this.gbxNeatParameters.Text = "Neat Search Parameters";
			// 
			// txtParamConnectionWeightMutationSigma
			// 
			this.txtParamConnectionWeightMutationSigma.Enabled = false;
			this.txtParamConnectionWeightMutationSigma.Location = new System.Drawing.Point(8, 352);
			this.txtParamConnectionWeightMutationSigma.Name = "txtParamConnectionWeightMutationSigma";
			this.txtParamConnectionWeightMutationSigma.Size = new System.Drawing.Size(48, 20);
			this.txtParamConnectionWeightMutationSigma.TabIndex = 63;
			this.txtParamConnectionWeightMutationSigma.Text = "";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(48, 328);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(136, 16);
			this.label16.TabIndex = 62;
			this.label16.Text = "Connection Weight Range";
			// 
			// txtParamConnectionWeightRange
			// 
			this.txtParamConnectionWeightRange.Location = new System.Drawing.Point(8, 328);
			this.txtParamConnectionWeightRange.Name = "txtParamConnectionWeightRange";
			this.txtParamConnectionWeightRange.Size = new System.Drawing.Size(48, 20);
			this.txtParamConnectionWeightRange.TabIndex = 61;
			this.txtParamConnectionWeightRange.Text = "";
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
			this.groupBox1.Location = new System.Drawing.Point(216, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(216, 80);
			this.groupBox1.TabIndex = 57;
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
			this.txtParamInterspeciesMating.Text = "";
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
			this.txtParamOffspringCrossover.Text = "";
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
			this.txtParamOffspringMutation.Text = "";
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
			this.groupBox4.Location = new System.Drawing.Point(0, 48);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(216, 128);
			this.groupBox4.TabIndex = 60;
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
			this.txtParamSpeciesDropoffAge.Text = "";
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
			this.txtParamTargetSpeciesCountMax.Text = "";
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
			this.txtParamTargetSpeciesCountMin.Text = "";
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
			this.txtParamSelectionProportion.Text = "";
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
			this.txtParamElitismProportion.Text = "";
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.label41);
			this.groupBox7.Controls.Add(this.txtParamPruningEndComplexityStagnationThreshold);
			this.groupBox7.Controls.Add(this.label40);
			this.groupBox7.Controls.Add(this.txtParamPruningBeginFitnessStagnationThreshold);
			this.groupBox7.Controls.Add(this.label39);
			this.groupBox7.Controls.Add(this.txtParamPruningBeginComplexityThreshold);
			this.groupBox7.Controls.Add(this.chkParamEnableConnectionWeightFixing);
			this.groupBox7.Controls.Add(this.chkParamPruningModeEnabled);
			this.groupBox7.Location = new System.Drawing.Point(0, 176);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(216, 144);
			this.groupBox7.TabIndex = 65;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Pruning Phase Parameters";
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
			this.txtParamPruningEndComplexityStagnationThreshold.Text = "";
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
			this.txtParamPruningBeginFitnessStagnationThreshold.Text = "";
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
			this.txtParamPruningBeginComplexityThreshold.Text = "";
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
			this.chkParamPruningModeEnabled.Location = new System.Drawing.Point(8, 16);
			this.chkParamPruningModeEnabled.Name = "chkParamPruningModeEnabled";
			this.chkParamPruningModeEnabled.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkParamPruningModeEnabled.Size = new System.Drawing.Size(104, 16);
			this.chkParamPruningModeEnabled.TabIndex = 52;
			this.chkParamPruningModeEnabled.Text = "Enable Pruning";
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
			this.groupBox3.Location = new System.Drawing.Point(216, 224);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(216, 112);
			this.groupBox3.TabIndex = 58;
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
			this.txtParamCompatDisjointCoeff.Text = "";
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
			this.txtParamCompatExcessCoeff.Text = "";
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
			this.txtParamCompatWeightDeltaCoeff.Text = "";
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
			this.txtParamCompatThreshold.Text = "";
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(56, 24);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(64, 16);
			this.label28.TabIndex = 56;
			this.label28.Text = "Pop Size";
			// 
			// txtParamPopulationSize
			// 
			this.txtParamPopulationSize.Location = new System.Drawing.Point(8, 24);
			this.txtParamPopulationSize.Name = "txtParamPopulationSize";
			this.txtParamPopulationSize.Size = new System.Drawing.Size(48, 20);
			this.txtParamPopulationSize.TabIndex = 55;
			this.txtParamPopulationSize.Text = "";
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
			this.groupBox2.Location = new System.Drawing.Point(216, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(216, 128);
			this.groupBox2.TabIndex = 59;
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
			this.txtParamMutateDeleteNeuron.Text = "";
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
			this.txtParamMutateDeleteConnection.Text = "";
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
			this.txtParamMutateConnectionWeights.Text = "";
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
			this.txtParamMutateAddNode.Text = "";
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
			this.txtParamMutateAddConnection.Text = "";
			// 
			// label17
			// 
			this.label17.Enabled = false;
			this.label17.Location = new System.Drawing.Point(48, 352);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(160, 16);
			this.label17.TabIndex = 64;
			this.label17.Text = "Conn. Weight Mutation Sigma";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.gbxMetaNeatParameters);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(440, 398);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Meta Parameters";
			// 
			// gbxMetaNeatParameters
			// 
			this.gbxMetaNeatParameters.Controls.Add(this.chkCompatibilityThreshold);
			this.gbxMetaNeatParameters.Controls.Add(this.chkConnectionMutationParameterGroupList);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPMutateType);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPDisjointExcessGenesRecombined);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPInterspeciesMating);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPOffspringAsexual_Sexual);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPInitialPopulationInterconnections);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPopulationSize);
			this.gbxMetaNeatParameters.Controls.Add(this.label2);
			this.gbxMetaNeatParameters.Controls.Add(this.chkConnectionWeightRange);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPruningPhaseEndComplexityStagnationThreshold);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPruningPhaseBeginFitnessStagnationThreshold);
			this.gbxMetaNeatParameters.Controls.Add(this.chkPruningPhaseBeginComplexityThreshold);
			this.gbxMetaNeatParameters.Controls.Add(this.chkSpeciesDropoffAge);
			this.gbxMetaNeatParameters.Controls.Add(this.chkTargetSpeciesCountWindow);
			this.gbxMetaNeatParameters.Controls.Add(this.chkSelectionProportion);
			this.gbxMetaNeatParameters.Controls.Add(this.chkElitismProportion);
			this.gbxMetaNeatParameters.Controls.Add(this.chkCompatibilityWeightDeltaCoeff);
			this.gbxMetaNeatParameters.Controls.Add(this.chkCompatibilityExcessCoeff);
			this.gbxMetaNeatParameters.Controls.Add(this.chkCompatibilityDisjointCoeff);
			this.gbxMetaNeatParameters.Location = new System.Drawing.Point(8, 8);
			this.gbxMetaNeatParameters.Name = "gbxMetaNeatParameters";
			this.gbxMetaNeatParameters.Size = new System.Drawing.Size(416, 376);
			this.gbxMetaNeatParameters.TabIndex = 20;
			this.gbxMetaNeatParameters.TabStop = false;
			this.gbxMetaNeatParameters.Text = "Meta-NeatParameters";
			// 
			// chkCompatibilityThreshold
			// 
			this.chkCompatibilityThreshold.Location = new System.Drawing.Point(8, 152);
			this.chkCompatibilityThreshold.Name = "chkCompatibilityThreshold";
			this.chkCompatibilityThreshold.Size = new System.Drawing.Size(152, 16);
			this.chkCompatibilityThreshold.TabIndex = 7;
			this.chkCompatibilityThreshold.Text = "CompatibilityThreshold";
			// 
			// chkConnectionMutationParameterGroupList
			// 
			this.chkConnectionMutationParameterGroupList.Checked = true;
			this.chkConnectionMutationParameterGroupList.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkConnectionMutationParameterGroupList.Location = new System.Drawing.Point(8, 136);
			this.chkConnectionMutationParameterGroupList.Name = "chkConnectionMutationParameterGroupList";
			this.chkConnectionMutationParameterGroupList.Size = new System.Drawing.Size(232, 16);
			this.chkConnectionMutationParameterGroupList.TabIndex = 6;
			this.chkConnectionMutationParameterGroupList.Text = "ConnectionMutationParameterGroupList";
			// 
			// chkPMutateType
			// 
			this.chkPMutateType.Location = new System.Drawing.Point(8, 120);
			this.chkPMutateType.Name = "chkPMutateType";
			this.chkPMutateType.Size = new System.Drawing.Size(144, 16);
			this.chkPMutateType.TabIndex = 5;
			this.chkPMutateType.Text = "pMutateType";
			// 
			// chkPDisjointExcessGenesRecombined
			// 
			this.chkPDisjointExcessGenesRecombined.Location = new System.Drawing.Point(8, 104);
			this.chkPDisjointExcessGenesRecombined.Name = "chkPDisjointExcessGenesRecombined";
			this.chkPDisjointExcessGenesRecombined.Size = new System.Drawing.Size(144, 16);
			this.chkPDisjointExcessGenesRecombined.TabIndex = 4;
			this.chkPDisjointExcessGenesRecombined.Text = "pDisjointExcessGenesRecombined";
			// 
			// chkPInterspeciesMating
			// 
			this.chkPInterspeciesMating.Location = new System.Drawing.Point(8, 88);
			this.chkPInterspeciesMating.Name = "chkPInterspeciesMating";
			this.chkPInterspeciesMating.Size = new System.Drawing.Size(144, 16);
			this.chkPInterspeciesMating.TabIndex = 3;
			this.chkPInterspeciesMating.Text = "pInterspeciesMating";
			// 
			// chkPOffspringAsexual_Sexual
			// 
			this.chkPOffspringAsexual_Sexual.Location = new System.Drawing.Point(8, 72);
			this.chkPOffspringAsexual_Sexual.Name = "chkPOffspringAsexual_Sexual";
			this.chkPOffspringAsexual_Sexual.Size = new System.Drawing.Size(144, 16);
			this.chkPOffspringAsexual_Sexual.TabIndex = 2;
			this.chkPOffspringAsexual_Sexual.Text = "pOffspringAsexual_Sexual";
			// 
			// chkPInitialPopulationInterconnections
			// 
			this.chkPInitialPopulationInterconnections.Location = new System.Drawing.Point(8, 56);
			this.chkPInitialPopulationInterconnections.Name = "chkPInitialPopulationInterconnections";
			this.chkPInitialPopulationInterconnections.Size = new System.Drawing.Size(192, 16);
			this.chkPInitialPopulationInterconnections.TabIndex = 1;
			this.chkPInitialPopulationInterconnections.Text = "pInitialPopulationInterconnections";
			// 
			// chkPopulationSize
			// 
			this.chkPopulationSize.Location = new System.Drawing.Point(8, 40);
			this.chkPopulationSize.Name = "chkPopulationSize";
			this.chkPopulationSize.Size = new System.Drawing.Size(144, 16);
			this.chkPopulationSize.TabIndex = 0;
			this.chkPopulationSize.Text = "populationSize";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(208, 16);
			this.label2.TabIndex = 19;
			this.label2.Text = "Mutation / Search enable flags";
			// 
			// chkConnectionWeightRange
			// 
			this.chkConnectionWeightRange.Location = new System.Drawing.Point(8, 328);
			this.chkConnectionWeightRange.Name = "chkConnectionWeightRange";
			this.chkConnectionWeightRange.Size = new System.Drawing.Size(160, 16);
			this.chkConnectionWeightRange.TabIndex = 18;
			this.chkConnectionWeightRange.Text = "ConnectionWeightRange";
			// 
			// chkPruningPhaseEndComplexityStagnationThreshold
			// 
			this.chkPruningPhaseEndComplexityStagnationThreshold.Location = new System.Drawing.Point(8, 312);
			this.chkPruningPhaseEndComplexityStagnationThreshold.Name = "chkPruningPhaseEndComplexityStagnationThreshold";
			this.chkPruningPhaseEndComplexityStagnationThreshold.Size = new System.Drawing.Size(296, 16);
			this.chkPruningPhaseEndComplexityStagnationThreshold.TabIndex = 17;
			this.chkPruningPhaseEndComplexityStagnationThreshold.Text = "PruningPhaseEndComplexityStagnationThreshold";
			// 
			// chkPruningPhaseBeginFitnessStagnationThreshold
			// 
			this.chkPruningPhaseBeginFitnessStagnationThreshold.Location = new System.Drawing.Point(8, 296);
			this.chkPruningPhaseBeginFitnessStagnationThreshold.Name = "chkPruningPhaseBeginFitnessStagnationThreshold";
			this.chkPruningPhaseBeginFitnessStagnationThreshold.Size = new System.Drawing.Size(272, 16);
			this.chkPruningPhaseBeginFitnessStagnationThreshold.TabIndex = 16;
			this.chkPruningPhaseBeginFitnessStagnationThreshold.Text = "PruningPhaseBeginFitnessStagnationThreshold";
			// 
			// chkPruningPhaseBeginComplexityThreshold
			// 
			this.chkPruningPhaseBeginComplexityThreshold.Location = new System.Drawing.Point(8, 280);
			this.chkPruningPhaseBeginComplexityThreshold.Name = "chkPruningPhaseBeginComplexityThreshold";
			this.chkPruningPhaseBeginComplexityThreshold.Size = new System.Drawing.Size(264, 16);
			this.chkPruningPhaseBeginComplexityThreshold.TabIndex = 15;
			this.chkPruningPhaseBeginComplexityThreshold.Text = "PruningPhaseBeginComplexityThreshold";
			// 
			// chkSpeciesDropoffAge
			// 
			this.chkSpeciesDropoffAge.Location = new System.Drawing.Point(8, 264);
			this.chkSpeciesDropoffAge.Name = "chkSpeciesDropoffAge";
			this.chkSpeciesDropoffAge.Size = new System.Drawing.Size(144, 16);
			this.chkSpeciesDropoffAge.TabIndex = 14;
			this.chkSpeciesDropoffAge.Text = "SpeciesDropoffAge";
			// 
			// chkTargetSpeciesCountWindow
			// 
			this.chkTargetSpeciesCountWindow.Location = new System.Drawing.Point(8, 248);
			this.chkTargetSpeciesCountWindow.Name = "chkTargetSpeciesCountWindow";
			this.chkTargetSpeciesCountWindow.Size = new System.Drawing.Size(192, 16);
			this.chkTargetSpeciesCountWindow.TabIndex = 13;
			this.chkTargetSpeciesCountWindow.Text = "TargetSpeciesCountWindow";
			// 
			// chkSelectionProportion
			// 
			this.chkSelectionProportion.Location = new System.Drawing.Point(8, 232);
			this.chkSelectionProportion.Name = "chkSelectionProportion";
			this.chkSelectionProportion.Size = new System.Drawing.Size(144, 16);
			this.chkSelectionProportion.TabIndex = 12;
			this.chkSelectionProportion.Text = "SelectionProportion";
			// 
			// chkElitismProportion
			// 
			this.chkElitismProportion.Location = new System.Drawing.Point(8, 216);
			this.chkElitismProportion.Name = "chkElitismProportion";
			this.chkElitismProportion.Size = new System.Drawing.Size(144, 16);
			this.chkElitismProportion.TabIndex = 11;
			this.chkElitismProportion.Text = "ElitismProportion";
			// 
			// chkCompatibilityWeightDeltaCoeff
			// 
			this.chkCompatibilityWeightDeltaCoeff.Location = new System.Drawing.Point(8, 200);
			this.chkCompatibilityWeightDeltaCoeff.Name = "chkCompatibilityWeightDeltaCoeff";
			this.chkCompatibilityWeightDeltaCoeff.Size = new System.Drawing.Size(144, 16);
			this.chkCompatibilityWeightDeltaCoeff.TabIndex = 10;
			this.chkCompatibilityWeightDeltaCoeff.Text = "CompatibilityWeightDeltaCoeff";
			// 
			// chkCompatibilityExcessCoeff
			// 
			this.chkCompatibilityExcessCoeff.Location = new System.Drawing.Point(8, 184);
			this.chkCompatibilityExcessCoeff.Name = "chkCompatibilityExcessCoeff";
			this.chkCompatibilityExcessCoeff.Size = new System.Drawing.Size(160, 16);
			this.chkCompatibilityExcessCoeff.TabIndex = 9;
			this.chkCompatibilityExcessCoeff.Text = "CompatibilityExcessCoeff";
			// 
			// chkCompatibilityDisjointCoeff
			// 
			this.chkCompatibilityDisjointCoeff.Location = new System.Drawing.Point(8, 168);
			this.chkCompatibilityDisjointCoeff.Name = "chkCompatibilityDisjointCoeff";
			this.chkCompatibilityDisjointCoeff.Size = new System.Drawing.Size(160, 16);
			this.chkCompatibilityDisjointCoeff.TabIndex = 8;
			this.chkCompatibilityDisjointCoeff.Text = "CompatibilityDisjointCoeff";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 424);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(456, 8);
			this.splitter1.TabIndex = 40;
			this.splitter1.TabStop = false;
			// 
			// gbxLog
			// 
			this.gbxLog.Controls.Add(this.txtLogWindow);
			this.gbxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbxLog.Location = new System.Drawing.Point(0, 432);
			this.gbxLog.Name = "gbxLog";
			this.gbxLog.Size = new System.Drawing.Size(456, 238);
			this.gbxLog.TabIndex = 41;
			this.gbxLog.TabStop = false;
			this.gbxLog.Text = "Log";
			// 
			// txtLogWindow
			// 
			this.txtLogWindow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLogWindow.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtLogWindow.Location = new System.Drawing.Point(3, 16);
			this.txtLogWindow.Multiline = true;
			this.txtLogWindow.Name = "txtLogWindow";
			this.txtLogWindow.ReadOnly = true;
			this.txtLogWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLogWindow.Size = new System.Drawing.Size(450, 219);
			this.txtLogWindow.TabIndex = 5;
			this.txtLogWindow.Text = "";
			this.txtLogWindow.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 670);
			this.Controls.Add(this.gbxLog);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tabControl1);
			this.Name = "Form1";
			this.Text = "NeatParameter Optimizer";
			this.gbxSearchControl.ResumeLayout(false);
			this.gbxSearchParams.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.gbxFile.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.gbxNeatParameters.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.gbxMetaNeatParameters.ResumeLayout(false);
			this.gbxLog.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Main entry point routine

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		#endregion

		#region MetaSearchThreadMethod

		private void MetaSearchThreadMethod()
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

				mea.PerformOneGeneration(selectedExperiment, stoppingCondition);
				UpdateStats();
				WriteDataToFile();
			}
		}

		private void PerformanceTestThreadMethod()
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

				mea.RunPerformanceTest(selectedExperiment, stoppingCondition);
				UpdateStats();
				WriteDataToFile();
			}
		}

		#endregion

		#region Event Handlers

		private void btnSearchStart_Click(object sender, System.EventArgs e)
		{
			searchState = SearchStateEnum.Running;
			UpdateGuiState();

			// Get the seed NEatParameter values from the gui.
			NeatParameters np = GetUserNeatParameters();

			// Wrap the NeatParameters in a NeatParametersWrapper - this provides the functionality for the meta GA.
			NeatParametersWrapper npw = new NeatParametersWrapper(np);

			// Create a MetaNeatParameters that will drive the meta-GA.
			MetaNeatParameters mnp = GetUserMetaNeatParameters();

			// Construct a list of npw's from our seed.
			npwList = NeatParametersWrapperFactory.CreateNeatParametersWrapperList(npw, mnp, 50);

			// Create a stopping condition.
			stoppingCondition = GetEAStoppingCondition();

			// Create a MeatEvolutionAlgorithm and invoke it.
			mea = new MetaEvolutionAlgorithm();
			mea.Initialise(npwList, mnp, selectedActivationFunction);

			//
			stopSearchSignal=false; // reset this signal.
			searchThread = new Thread(new ThreadStart(MetaSearchThreadMethod));
			searchThread.IsBackground = true;
			searchThread.Priority = ThreadPriority.BelowNormal;
			searchThread.Start();
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
				if((searchThread.ThreadState & ThreadState.Suspended) ==0)
				{	// User must stop thread first.
					return;
				}
				// Bug in .Net requires call to Resume before Abort.!
				searchThread.Resume();
				searchThread.Abort(); 
			}

			searchThread = null;
			npwList = null;

			searchState = SearchStateEnum.Reset;
			UpdateGuiState();
		}

		private void btnSearchTerminate_Click(object sender, System.EventArgs e)
		{
			if(searchThread!=null)
			{
				// Bug in .Net requires call to Resume before Abort.!
				//searchThread.Resume();
				searchThread.Abort(); 
			}

			searchThread = null;
			npwList = null;

			searchState = SearchStateEnum.Reset;
			UpdateGuiState();
		}

		private void cmbExperimentActivationFn_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Store a reference to the selected activation function.
			ListItem listItem = (ListItem)cmbExperimentActivationFn.SelectedItem;
			selectedActivationFunction = (IActivationFunction)listItem.Data;
		}

		private void cmbDomain_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Store a reference to the selected experiment.
			ListItem listItem = (ListItem)cmbDomain.SelectedItem;
			selectedExperiment = (IExperiment)listItem.Data;

			SelectActivationFunction(selectedExperiment.SuggestedActivationFunction);
		}

		private void btnLoadDefaults_Click(object sender, System.EventArgs e)
		{
			LoadNeatParameters(selectedExperiment.DefaultNeatParameters);
		}

		#endregion

		#region Private Methods [Initialisation]

		private void InitialiseForm()
		{
			PopulateDomainCombo();
			PopulateActivationFunctionCombo();
			PopulateEAStoppingConditionCombo();
			PopulateFileOutputTypeCombo();

			cmbDomain.SelectedIndex=0;
			cmbEAStoppingCondition.SelectedIndex=2;
			cmbFileOutputType.SelectedIndex=0;

			// Load some default NeatParameters into the GUI.
			LoadNeatParameters(new NeatParameters());
		}

		private void PopulateDomainCombo()
		{
			cmbDomain.Items.Add(new ListItem("", "XOR", new XorExperiment()));
			cmbDomain.Items.Add(new ListItem("", "Prey Capture", new PreyCaptureExperiment()));
			cmbDomain.Items.Add(new ListItem("", "Pole Balancing(Single)", new SinglePoleBalancingExperiment() ));
			cmbDomain.Items.Add(new ListItem("", "Pole Balancing(Double)", new DoublePoleBalancingExperiment() ));
			cmbDomain.Items.Add(new ListItem("", "Pole Balancing(Double-NV)", new NvDoublePoleBalancingExperiment() ));
			cmbDomain.Items.Add(new ListItem("", "Pole Balancing(Double-NV-AntiWiggle)", new NvAntiWiggleDoublePoleBalancingExperiment() ));
//			cmbDomain.Items.Add(new ListItem("", "Tic-Tac-Toe(Original)", new TicTacToeExperiment()));
//			cmbDomain.Items.Add(new ListItem("", "Tic-Tac-Toe(Modified)", new TicTacToeExExperiment()));
			cmbDomain.Items.Add(new ListItem("", "Simple-OCR", new SimpleOcrExperiment()));
//			cmbDomain.Items.Add(new ListItem("", "Vector Cross Product", new CrossProductExperiment()));
			cmbDomain.Items.Add(new ListItem("", "Multiplication", new MultiplicationExperiment()));
//			cmbDomain.Items.Add(new ListItem("", "IEX Test1", new IexTest1Experiment()));
			cmbDomain.Items.Add(new ListItem("", "6-Multiplexer(Binary)", new BinarySixMultiplexerExperiment()));
		}

		private void PopulateActivationFunctionCombo()
		{
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Plain Sigmoid", new PlainSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Steepened Sigmoid", new SteepenedSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Inv-Abs [FAST]", new InverseAbsoluteSigmoid()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Stepped Approximation [FAST]", new SigmoidApproximation()));
			cmbExperimentActivationFn.Items.Add(new ListItem("", "Steepened Stepped Approx. [FAST]", new SteepenedSigmoidApproximation()));
			//			cmbExperimentActivationFn.Items.Add(new ListItem("", "Step Function", new StepFunction()));
		}

		private void PopulateEAStoppingConditionCombo()
		{
			cmbEAStoppingCondition.Items.Add(new ListItem("", "Max Duration", EAStoppingConditionType.MaxDuration));
			cmbEAStoppingCondition.Items.Add(new ListItem("", "Max Generations", EAStoppingConditionType.MaxGenerations));
			cmbEAStoppingCondition.Items.Add(new ListItem("", "Combined", EAStoppingConditionType.Combined));
		}

		private void PopulateFileOutputTypeCombo()
		{
			cmbFileOutputType.Items.Add(new ListItem("", "Save Best Only", FileOutputType.BestOnly));
			cmbFileOutputType.Items.Add(new ListItem("", "Save All", FileOutputType.All));
		}

		#endregion

		#region Private Methods

		private void UpdateGuiState()
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
					gbxNeatParameters.Enabled = true;
					gbxMetaNeatParameters.Enabled = false;
					gbxFile.Enabled=true;
					break;
					
				case SearchStateEnum.Paused:
					btnSearchStart.Enabled = true;
					btnSearchStop.Enabled = false;
					btnSearchReset.Enabled = true;

					cmbDomain.Enabled = false;
					cmbExperimentActivationFn.Enabled = false;
					btnLoadDefaults.Enabled = false;
					gbxNeatParameters.Enabled = false;
					gbxMetaNeatParameters.Enabled = false;
					gbxFile.Enabled=false;
					break;

				case SearchStateEnum.Running:
					btnSearchStart.Enabled = false;
					btnSearchStop.Enabled = true;
					btnSearchReset.Enabled = false;

					cmbDomain.Enabled = false;
					cmbExperimentActivationFn.Enabled = false;
					btnLoadDefaults.Enabled = false;
					gbxNeatParameters.Enabled = false;
					gbxMetaNeatParameters.Enabled = false;
					gbxFile.Enabled=false;
					break;
			}
		}

		private void UpdateStats()
		{
			LogMessage(System.DateTime.Now.ToShortDateString() +
				": gen=" + mea.Generation + 
				", bestFitness=" + mea.Best.Fitness.ToString("0.00") +
				", meanFitness=" + mea.MeanFitness.ToString("0.00"));

			txtStatsGeneration.Text = mea.Generation.ToString();
			txtStatsBest.Text = mea.Best.Fitness.ToString("0.00");
			txtStatsMean.Text = mea.MeanFitness.ToString("0.00");
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

		private void SelectActivationFunction(IActivationFunction activationFn)
		{
			foreach(ListItem listItem in cmbExperimentActivationFn.Items)
			{
				if(listItem.Data.GetType() == activationFn.GetType())
				{
					cmbExperimentActivationFn.SelectedItem = listItem;
					return;
				}
			}
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

		private NeatParameters GetUserNeatParameters()
		{
			NeatParameters np = selectedExperiment.DefaultNeatParameters;

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
			//			np.connectionMutationSigma = double.Parse(txtParamConnectionWeightMutationSigma.Text);
			
			return np;
		}

		private MetaNeatParameters GetUserMetaNeatParameters()
		{
			MetaNeatParameters mnp = new MetaNeatParameters();

			mnp.EaRunsPerMetaEvaluation = int.Parse(txtEaRunsPerMetaEval.Text);

			if(chkPopulationSize.Checked) 						mnp.ParameterFlags |= ParameterFlag.populationSize;
			if(chkPInitialPopulationInterconnections.Checked) 	mnp.ParameterFlags |= ParameterFlag.pInitialPopulationInterconnections;
			if(chkPOffspringAsexual_Sexual.Checked)				mnp.ParameterFlags |= ParameterFlag.pOffspringAsexual_Sexual;
			if(chkPInterspeciesMating.Checked)					mnp.ParameterFlags |= ParameterFlag.pInterspeciesMating;
			if(chkPDisjointExcessGenesRecombined.Checked)		mnp.ParameterFlags |= ParameterFlag.pDisjointExcessGenesRecombined;	
			if(chkPMutateType.Checked)							mnp.ParameterFlags |= ParameterFlag.pMutateType;
			if(chkConnectionMutationParameterGroupList.Checked)	mnp.ParameterFlags |= ParameterFlag.ConnectionMutationParameterGroupList;
			if(chkCompatibilityThreshold.Checked)				mnp.ParameterFlags |= ParameterFlag.compatibilityThreshold;
			if(chkCompatibilityDisjointCoeff.Checked)			mnp.ParameterFlags |= ParameterFlag.compatibilityDisjointCoeff;
			if(chkCompatibilityExcessCoeff.Checked)				mnp.ParameterFlags |= ParameterFlag.compatibilityExcessCoeff;
			if(chkCompatibilityWeightDeltaCoeff.Checked)		mnp.ParameterFlags |= ParameterFlag.compatibilityWeightDeltaCoeff;
			if(chkElitismProportion.Checked)					mnp.ParameterFlags |= ParameterFlag.elitismProportion;
			if(chkSelectionProportion.Checked)					mnp.ParameterFlags |= ParameterFlag.selectionProportion;
			if(chkTargetSpeciesCountWindow.Checked)				mnp.ParameterFlags |= ParameterFlag.targetSpeciesCountWindow;	
			if(chkSpeciesDropoffAge.Checked)					mnp.ParameterFlags |= ParameterFlag.speciesDropoffAge;
			if(chkPruningPhaseBeginComplexityThreshold.Checked)	mnp.ParameterFlags |= ParameterFlag.pruningPhaseBeginComplexityThreshold;
			if(chkPruningPhaseBeginFitnessStagnationThreshold.Checked)	mnp.ParameterFlags |= ParameterFlag.pruningPhaseBeginFitnessStagnationThreshold;
			if(chkPruningPhaseEndComplexityStagnationThreshold.Checked)	mnp.ParameterFlags |= ParameterFlag.pruningPhaseEndComplexityStagnationThreshold;
			if(chkConnectionWeightRange.Checked)				mnp.ParameterFlags |= ParameterFlag.connectionWeightRange;

			return mnp;
		}

		private EAStoppingCondition GetEAStoppingCondition()
		{
			ListItem listItem = (ListItem)cmbEAStoppingCondition.SelectedItem;
			EAStoppingConditionType type = (EAStoppingConditionType)listItem.Data;

			switch(type)
			{
				case EAStoppingConditionType.MaxDuration:
				{
					return EAStoppingCondition.CreateStoppingCondition_MaxDuration(int.Parse(txtEaMaxDuration.Text));
				}
				case EAStoppingConditionType.MaxGenerations:
				{
					return EAStoppingCondition.CreateStoppingCondition_MaxGenerations(int.Parse(txtEaMaxGenerations.Text));
				}
				default: //case EAStoppingConditionType.Combined:
				{
					return EAStoppingCondition.CreateStoppingCondition_Combined(int.Parse(txtEaMaxDuration.Text), int.Parse(txtEaMaxGenerations.Text));
				}
			}
		}

		private void WriteDataToFile()
		{
			string filename =  txtFileOutputPrefix.Text + '_'  
				+ DateTime.Now.ToString("yyMMdd_hhmmss") + '_'
				+ selectedExperiment.GetType().Name 
				+ ".xml";
			XmlDocument xmlDoc = new XmlDocument();

			// Determine the type of output.
			ListItem listItem  = (ListItem)cmbFileOutputType.SelectedItem;
			FileOutputType fileOutputType = (FileOutputType)listItem.Data;

			switch(fileOutputType)
			{
				case FileOutputType.BestOnly:
				{
					NeatParametersWrapperWriter.Write(xmlDoc, mea.Best);
					break;

				}
				default: // All.
				{
					NeatParametersWrapperWriter.Write(xmlDoc, mea.NeatParametersWrapperList);
					break;
				}
			}

			xmlDoc.Save(filename);
		}

		#endregion

		private void btnCalcAvgPerformance_Click(object sender, System.EventArgs e)
		{
			searchState = SearchStateEnum.Running;
			UpdateGuiState();

			// Get the seed NEatParameter values from the gui.
			NeatParameters np = GetUserNeatParameters();

			// Wrap the NeatParameters in a NeatParametersWrapper - this provides the functionality for the meta GA.
			NeatParametersWrapper npw = new NeatParametersWrapper(np);

			// Create a MetaNeatParameters that will drive the meta-GA.
			MetaNeatParameters mnp = GetUserMetaNeatParameters();
			
			// Construct a list of npw's from our seed.
			npwList = new NeatParametersWrapperList();
			npwList.Add(npw);

			// Create a stopping condition.
			stoppingCondition = GetEAStoppingCondition();

			// Create a MeatEvolutionAlgorithm and invoke it.
			mea = new MetaEvolutionAlgorithm();
			mea.Initialise(npwList, mnp, selectedActivationFunction);

			//
			stopSearchSignal=false; // reset this signal.
			searchThread = new Thread(new ThreadStart(PerformanceTestThreadMethod));
			searchThread.IsBackground = true;
			searchThread.Priority = ThreadPriority.BelowNormal;
			searchThread.Start();
		}
	}
}
