namespace SharpNeat.Windows.App
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gbxGenomePopulation = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInitialInterconnectionsProportion = new System.Windows.Forms.TextBox();
            this.txtPopulationSize = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtPopulationStatus = new System.Windows.Forms.TextBox();
            this.btnCreateRandomPop = new System.Windows.Forms.Button();
            this.gbxLogging = new System.Windows.Forms.GroupBox();
            this.txtFileLogBaseName = new System.Windows.Forms.TextBox();
            this.chkFileWriteLog = new System.Windows.Forms.CheckBox();
            this.txtFileBaseName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkFileSaveGenomeOnImprovement = new System.Windows.Forms.CheckBox();
            this.gbxCurrentStats = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtStatsInterspeciesOffspringCount = new System.Windows.Forms.TextBox();
            this.txtStatsAlternativeFitness = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtStatsCrossoverOffspringCount = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtStatsAsexualOffspringCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtStatsTotalOffspringCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtStatsMaxGenomeComplx = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSpecieChampMean = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txtSearchStatsMode = new System.Windows.Forms.TextBox();
            this.txtStatsEvalsPerSec = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtStatsMeanGenomeComplx = new System.Windows.Forms.TextBox();
            this.txtStatsBestGenomeComplx = new System.Windows.Forms.TextBox();
            this.txtStatsTotalEvals = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStatsGeneration = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStatsMean = new System.Windows.Forms.TextBox();
            this.txtStatsBest = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnSearchReset = new System.Windows.Forms.Button();
            this.btnSearchStop = new System.Windows.Forms.Button();
            this.btnSearchStart = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnResetExperimentParameters = new System.Windows.Forms.Button();
            this.btnExperimentInfo = new System.Windows.Forms.Button();
            this.cmbExperiments = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbxNeatGenomeParameters = new System.Windows.Forms.GroupBox();
            this.label42 = new System.Windows.Forms.Label();
            this.txtConnectionWeightMutationProbability = new System.Windows.Forms.TextBox();
            this.txtDeleteConnectionMutationProbability = new System.Windows.Forms.TextBox();
            this.txtAddConnectionMutationProbability = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.txtAddNodeMutationProbability = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.gbxEAParameters = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtSpeciesCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSelectionProportion = new System.Windows.Forms.TextBox();
            this.txtInterspeciesMatingProportion = new System.Windows.Forms.TextBox();
            this.txtElitismProportion = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtOffspringSexualProportion = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtOffspringAsexualProportion = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPopulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSeedGenomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSeedGenomesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.savePopulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBestGenomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestGenomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieChampGenomesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.problemDomainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeSeriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitnessBestMeansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.complexityBestMeansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluationsPerSecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rankPlotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieSizeByRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieChampFitnessByRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieChampComplexityByRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.genomeFitnessByRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genomeComplexityByRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distributionPlotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieSizeDistributionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieFitnessDistributionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specieComplexityDistributionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.genomeFitnessDistributionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genomeComplexityDistributionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCopyLogToClipboard = new System.Windows.Forms.Button();
            this.lbxLog = new System.Windows.Forms.ListBox();
            this.populationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbxGenomePopulation.SuspendLayout();
            this.gbxLogging.SuspendLayout();
            this.gbxCurrentStats.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbxNeatGenomeParameters.SuspendLayout();
            this.gbxEAParameters.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnCopyLogToClipboard);
            this.splitContainer1.Panel2.Controls.Add(this.lbxLog);
            this.splitContainer1.Size = new System.Drawing.Size(611, 778);
            this.splitContainer1.SplitterDistance = 530;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(611, 506);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gbxGenomePopulation);
            this.tabPage1.Controls.Add(this.gbxLogging);
            this.tabPage1.Controls.Add(this.gbxCurrentStats);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(603, 478);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Page 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gbxGenomePopulation
            // 
            this.gbxGenomePopulation.Controls.Add(this.label1);
            this.gbxGenomePopulation.Controls.Add(this.txtInitialInterconnectionsProportion);
            this.gbxGenomePopulation.Controls.Add(this.txtPopulationSize);
            this.gbxGenomePopulation.Controls.Add(this.label28);
            this.gbxGenomePopulation.Controls.Add(this.txtPopulationStatus);
            this.gbxGenomePopulation.Controls.Add(this.btnCreateRandomPop);
            this.gbxGenomePopulation.Location = new System.Drawing.Point(9, 93);
            this.gbxGenomePopulation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxGenomePopulation.Name = "gbxGenomePopulation";
            this.gbxGenomePopulation.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxGenomePopulation.Size = new System.Drawing.Size(273, 158);
            this.gbxGenomePopulation.TabIndex = 21;
            this.gbxGenomePopulation.TabStop = false;
            this.gbxGenomePopulation.Text = "Genome Population";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(105, 125);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 23);
            this.label1.TabIndex = 54;
            this.label1.Text = "Initial Connections Proportion";
            // 
            // txtInitialInterconnectionsProportion
            // 
            this.txtInitialInterconnectionsProportion.Location = new System.Drawing.Point(9, 121);
            this.txtInitialInterconnectionsProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtInitialInterconnectionsProportion.Name = "txtInitialInterconnectionsProportion";
            this.txtInitialInterconnectionsProportion.Size = new System.Drawing.Size(93, 23);
            this.txtInitialInterconnectionsProportion.TabIndex = 53;
            this.txtInitialInterconnectionsProportion.Text = "0.1";
            // 
            // txtPopulationSize
            // 
            this.txtPopulationSize.Location = new System.Drawing.Point(9, 91);
            this.txtPopulationSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPopulationSize.Name = "txtPopulationSize";
            this.txtPopulationSize.Size = new System.Drawing.Size(93, 23);
            this.txtPopulationSize.TabIndex = 51;
            this.txtPopulationSize.Text = "150";
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(105, 95);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(132, 18);
            this.label28.TabIndex = 52;
            this.label28.Text = "Population Size";
            // 
            // txtPopulationStatus
            // 
            this.txtPopulationStatus.BackColor = System.Drawing.Color.Red;
            this.txtPopulationStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPopulationStatus.ForeColor = System.Drawing.Color.Black;
            this.txtPopulationStatus.Location = new System.Drawing.Point(9, 22);
            this.txtPopulationStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPopulationStatus.Name = "txtPopulationStatus";
            this.txtPopulationStatus.ReadOnly = true;
            this.txtPopulationStatus.Size = new System.Drawing.Size(255, 20);
            this.txtPopulationStatus.TabIndex = 50;
            this.txtPopulationStatus.TabStop = false;
            this.txtPopulationStatus.Text = "Population not initialized";
            this.txtPopulationStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnCreateRandomPop
            // 
            this.btnCreateRandomPop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCreateRandomPop.Location = new System.Drawing.Point(8, 52);
            this.btnCreateRandomPop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCreateRandomPop.Name = "btnCreateRandomPop";
            this.btnCreateRandomPop.Size = new System.Drawing.Size(257, 28);
            this.btnCreateRandomPop.TabIndex = 49;
            this.btnCreateRandomPop.Text = "Create Random Population";
            this.btnCreateRandomPop.Click += new System.EventHandler(this.btnCreateRandomPop_Click);
            // 
            // gbxLogging
            // 
            this.gbxLogging.Controls.Add(this.txtFileLogBaseName);
            this.gbxLogging.Controls.Add(this.chkFileWriteLog);
            this.gbxLogging.Controls.Add(this.txtFileBaseName);
            this.gbxLogging.Controls.Add(this.label13);
            this.gbxLogging.Controls.Add(this.label8);
            this.gbxLogging.Controls.Add(this.chkFileSaveGenomeOnImprovement);
            this.gbxLogging.Location = new System.Drawing.Point(9, 339);
            this.gbxLogging.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxLogging.Name = "gbxLogging";
            this.gbxLogging.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxLogging.Size = new System.Drawing.Size(273, 134);
            this.gbxLogging.TabIndex = 20;
            this.gbxLogging.TabStop = false;
            this.gbxLogging.Text = "File";
            // 
            // txtFileLogBaseName
            // 
            this.txtFileLogBaseName.Location = new System.Drawing.Point(9, 102);
            this.txtFileLogBaseName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFileLogBaseName.Name = "txtFileLogBaseName";
            this.txtFileLogBaseName.Size = new System.Drawing.Size(206, 23);
            this.txtFileLogBaseName.TabIndex = 25;
            this.txtFileLogBaseName.Text = "sharpneat";
            // 
            // chkFileWriteLog
            // 
            this.chkFileWriteLog.Location = new System.Drawing.Point(9, 74);
            this.chkFileWriteLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkFileWriteLog.Name = "chkFileWriteLog";
            this.chkFileWriteLog.Size = new System.Drawing.Size(152, 28);
            this.chkFileWriteLog.TabIndex = 24;
            this.chkFileWriteLog.Text = "Write Log File (*.log)";
            // 
            // txtFileBaseName
            // 
            this.txtFileBaseName.Location = new System.Drawing.Point(9, 46);
            this.txtFileBaseName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFileBaseName.Name = "txtFileBaseName";
            this.txtFileBaseName.Size = new System.Drawing.Size(206, 23);
            this.txtFileBaseName.TabIndex = 1;
            this.txtFileBaseName.Text = "champ";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(211, 39);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 37);
            this.label13.TabIndex = 23;
            this.label13.Text = "Filename prefix";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(214, 93);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 37);
            this.label8.TabIndex = 28;
            this.label8.Text = "Filename prefix";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkFileSaveGenomeOnImprovement
            // 
            this.chkFileSaveGenomeOnImprovement.Location = new System.Drawing.Point(9, 18);
            this.chkFileSaveGenomeOnImprovement.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkFileSaveGenomeOnImprovement.Name = "chkFileSaveGenomeOnImprovement";
            this.chkFileSaveGenomeOnImprovement.Size = new System.Drawing.Size(272, 28);
            this.chkFileSaveGenomeOnImprovement.TabIndex = 0;
            this.chkFileSaveGenomeOnImprovement.Text = "Save Genome on Improvement (*.gnm.xml)";
            // 
            // gbxCurrentStats
            // 
            this.gbxCurrentStats.Controls.Add(this.label17);
            this.gbxCurrentStats.Controls.Add(this.label12);
            this.gbxCurrentStats.Controls.Add(this.txtStatsInterspeciesOffspringCount);
            this.gbxCurrentStats.Controls.Add(this.txtStatsAlternativeFitness);
            this.gbxCurrentStats.Controls.Add(this.label11);
            this.gbxCurrentStats.Controls.Add(this.txtStatsCrossoverOffspringCount);
            this.gbxCurrentStats.Controls.Add(this.label10);
            this.gbxCurrentStats.Controls.Add(this.txtStatsAsexualOffspringCount);
            this.gbxCurrentStats.Controls.Add(this.label9);
            this.gbxCurrentStats.Controls.Add(this.txtStatsTotalOffspringCount);
            this.gbxCurrentStats.Controls.Add(this.label7);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMaxGenomeComplx);
            this.gbxCurrentStats.Controls.Add(this.label6);
            this.gbxCurrentStats.Controls.Add(this.txtSpecieChampMean);
            this.gbxCurrentStats.Controls.Add(this.label38);
            this.gbxCurrentStats.Controls.Add(this.txtSearchStatsMode);
            this.gbxCurrentStats.Controls.Add(this.txtStatsEvalsPerSec);
            this.gbxCurrentStats.Controls.Add(this.label20);
            this.gbxCurrentStats.Controls.Add(this.label19);
            this.gbxCurrentStats.Controls.Add(this.label18);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMeanGenomeComplx);
            this.gbxCurrentStats.Controls.Add(this.txtStatsBestGenomeComplx);
            this.gbxCurrentStats.Controls.Add(this.txtStatsTotalEvals);
            this.gbxCurrentStats.Controls.Add(this.label27);
            this.gbxCurrentStats.Controls.Add(this.label5);
            this.gbxCurrentStats.Controls.Add(this.txtStatsGeneration);
            this.gbxCurrentStats.Controls.Add(this.label3);
            this.gbxCurrentStats.Controls.Add(this.label2);
            this.gbxCurrentStats.Controls.Add(this.txtStatsMean);
            this.gbxCurrentStats.Controls.Add(this.txtStatsBest);
            this.gbxCurrentStats.Location = new System.Drawing.Point(300, 7);
            this.gbxCurrentStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxCurrentStats.Name = "gbxCurrentStats";
            this.gbxCurrentStats.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxCurrentStats.Size = new System.Drawing.Size(292, 474);
            this.gbxCurrentStats.TabIndex = 19;
            this.gbxCurrentStats.TabStop = false;
            this.gbxCurrentStats.Text = "Current Stats";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(121, 117);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(164, 18);
            this.label17.TabIndex = 35;
            this.label17.Text = "Alternative Best Fitness";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(121, 448);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(187, 18);
            this.label12.TabIndex = 33;
            this.label12.Text = "Interspecies Offspring Count";
            // 
            // txtStatsInterspeciesOffspringCount
            // 
            this.txtStatsInterspeciesOffspringCount.Location = new System.Drawing.Point(7, 443);
            this.txtStatsInterspeciesOffspringCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsInterspeciesOffspringCount.Name = "txtStatsInterspeciesOffspringCount";
            this.txtStatsInterspeciesOffspringCount.ReadOnly = true;
            this.txtStatsInterspeciesOffspringCount.Size = new System.Drawing.Size(111, 23);
            this.txtStatsInterspeciesOffspringCount.TabIndex = 32;
            this.txtStatsInterspeciesOffspringCount.TabStop = false;
            // 
            // txtStatsAlternativeFitness
            // 
            this.txtStatsAlternativeFitness.Location = new System.Drawing.Point(7, 112);
            this.txtStatsAlternativeFitness.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsAlternativeFitness.Name = "txtStatsAlternativeFitness";
            this.txtStatsAlternativeFitness.ReadOnly = true;
            this.txtStatsAlternativeFitness.Size = new System.Drawing.Size(111, 23);
            this.txtStatsAlternativeFitness.TabIndex = 34;
            this.txtStatsAlternativeFitness.TabStop = false;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(121, 418);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 18);
            this.label11.TabIndex = 31;
            this.label11.Text = "Crossover Offspring Count";
            // 
            // txtStatsCrossoverOffspringCount
            // 
            this.txtStatsCrossoverOffspringCount.Location = new System.Drawing.Point(7, 413);
            this.txtStatsCrossoverOffspringCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsCrossoverOffspringCount.Name = "txtStatsCrossoverOffspringCount";
            this.txtStatsCrossoverOffspringCount.ReadOnly = true;
            this.txtStatsCrossoverOffspringCount.Size = new System.Drawing.Size(111, 23);
            this.txtStatsCrossoverOffspringCount.TabIndex = 30;
            this.txtStatsCrossoverOffspringCount.TabStop = false;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(121, 388);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(160, 18);
            this.label10.TabIndex = 29;
            this.label10.Text = "Asexual Offspring Count";
            // 
            // txtStatsAsexualOffspringCount
            // 
            this.txtStatsAsexualOffspringCount.Location = new System.Drawing.Point(7, 383);
            this.txtStatsAsexualOffspringCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsAsexualOffspringCount.Name = "txtStatsAsexualOffspringCount";
            this.txtStatsAsexualOffspringCount.ReadOnly = true;
            this.txtStatsAsexualOffspringCount.Size = new System.Drawing.Size(111, 23);
            this.txtStatsAsexualOffspringCount.TabIndex = 28;
            this.txtStatsAsexualOffspringCount.TabStop = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(121, 358);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 18);
            this.label9.TabIndex = 27;
            this.label9.Text = "Total Offspring Count";
            // 
            // txtStatsTotalOffspringCount
            // 
            this.txtStatsTotalOffspringCount.Location = new System.Drawing.Point(7, 353);
            this.txtStatsTotalOffspringCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsTotalOffspringCount.Name = "txtStatsTotalOffspringCount";
            this.txtStatsTotalOffspringCount.ReadOnly = true;
            this.txtStatsTotalOffspringCount.Size = new System.Drawing.Size(111, 23);
            this.txtStatsTotalOffspringCount.TabIndex = 26;
            this.txtStatsTotalOffspringCount.TabStop = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(121, 328);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 18);
            this.label7.TabIndex = 25;
            this.label7.Text = "Max Genome Complexity";
            // 
            // txtStatsMaxGenomeComplx
            // 
            this.txtStatsMaxGenomeComplx.Location = new System.Drawing.Point(7, 323);
            this.txtStatsMaxGenomeComplx.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsMaxGenomeComplx.Name = "txtStatsMaxGenomeComplx";
            this.txtStatsMaxGenomeComplx.ReadOnly = true;
            this.txtStatsMaxGenomeComplx.Size = new System.Drawing.Size(111, 23);
            this.txtStatsMaxGenomeComplx.TabIndex = 24;
            this.txtStatsMaxGenomeComplx.TabStop = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(121, 168);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 35);
            this.label6.TabIndex = 23;
            this.label6.Text = "Mean Fitness (specie champs)";
            // 
            // txtSpecieChampMean
            // 
            this.txtSpecieChampMean.Location = new System.Drawing.Point(7, 172);
            this.txtSpecieChampMean.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSpecieChampMean.Name = "txtSpecieChampMean";
            this.txtSpecieChampMean.ReadOnly = true;
            this.txtSpecieChampMean.Size = new System.Drawing.Size(111, 23);
            this.txtSpecieChampMean.TabIndex = 22;
            this.txtSpecieChampMean.TabStop = false;
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(121, 27);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(131, 18);
            this.label38.TabIndex = 21;
            this.label38.Text = "Current Search Mode";
            // 
            // txtSearchStatsMode
            // 
            this.txtSearchStatsMode.BackColor = System.Drawing.Color.LightSkyBlue;
            this.txtSearchStatsMode.Location = new System.Drawing.Point(7, 22);
            this.txtSearchStatsMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSearchStatsMode.Name = "txtSearchStatsMode";
            this.txtSearchStatsMode.ReadOnly = true;
            this.txtSearchStatsMode.Size = new System.Drawing.Size(111, 23);
            this.txtSearchStatsMode.TabIndex = 20;
            this.txtSearchStatsMode.TabStop = false;
            // 
            // txtStatsEvalsPerSec
            // 
            this.txtStatsEvalsPerSec.Location = new System.Drawing.Point(7, 233);
            this.txtStatsEvalsPerSec.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsEvalsPerSec.Name = "txtStatsEvalsPerSec";
            this.txtStatsEvalsPerSec.ReadOnly = true;
            this.txtStatsEvalsPerSec.Size = new System.Drawing.Size(111, 23);
            this.txtStatsEvalsPerSec.TabIndex = 18;
            this.txtStatsEvalsPerSec.TabStop = false;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(121, 237);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(121, 18);
            this.label20.TabIndex = 19;
            this.label20.Text = "Evaluations / Sec";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(121, 298);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(160, 18);
            this.label19.TabIndex = 17;
            this.label19.Text = "Mean Genome Complexity";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(121, 268);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(160, 18);
            this.label18.TabIndex = 16;
            this.label18.Text = "Best Genome\'s Complexity";
            // 
            // txtStatsMeanGenomeComplx
            // 
            this.txtStatsMeanGenomeComplx.Location = new System.Drawing.Point(7, 293);
            this.txtStatsMeanGenomeComplx.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsMeanGenomeComplx.Name = "txtStatsMeanGenomeComplx";
            this.txtStatsMeanGenomeComplx.ReadOnly = true;
            this.txtStatsMeanGenomeComplx.Size = new System.Drawing.Size(111, 23);
            this.txtStatsMeanGenomeComplx.TabIndex = 15;
            this.txtStatsMeanGenomeComplx.TabStop = false;
            // 
            // txtStatsBestGenomeComplx
            // 
            this.txtStatsBestGenomeComplx.Location = new System.Drawing.Point(7, 263);
            this.txtStatsBestGenomeComplx.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsBestGenomeComplx.Name = "txtStatsBestGenomeComplx";
            this.txtStatsBestGenomeComplx.ReadOnly = true;
            this.txtStatsBestGenomeComplx.Size = new System.Drawing.Size(111, 23);
            this.txtStatsBestGenomeComplx.TabIndex = 14;
            this.txtStatsBestGenomeComplx.TabStop = false;
            // 
            // txtStatsTotalEvals
            // 
            this.txtStatsTotalEvals.Location = new System.Drawing.Point(7, 203);
            this.txtStatsTotalEvals.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsTotalEvals.Name = "txtStatsTotalEvals";
            this.txtStatsTotalEvals.ReadOnly = true;
            this.txtStatsTotalEvals.Size = new System.Drawing.Size(111, 23);
            this.txtStatsTotalEvals.TabIndex = 12;
            this.txtStatsTotalEvals.TabStop = false;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(121, 208);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(121, 18);
            this.label27.TabIndex = 13;
            this.label27.Text = "Total Evaluations";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(121, 57);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 18);
            this.label5.TabIndex = 7;
            this.label5.Text = "Generation";
            // 
            // txtStatsGeneration
            // 
            this.txtStatsGeneration.Location = new System.Drawing.Point(7, 53);
            this.txtStatsGeneration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsGeneration.Name = "txtStatsGeneration";
            this.txtStatsGeneration.ReadOnly = true;
            this.txtStatsGeneration.Size = new System.Drawing.Size(111, 23);
            this.txtStatsGeneration.TabIndex = 6;
            this.txtStatsGeneration.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(121, 145);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mean Fitness";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(121, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Best Fitness";
            // 
            // txtStatsMean
            // 
            this.txtStatsMean.Location = new System.Drawing.Point(7, 142);
            this.txtStatsMean.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsMean.Name = "txtStatsMean";
            this.txtStatsMean.ReadOnly = true;
            this.txtStatsMean.Size = new System.Drawing.Size(111, 23);
            this.txtStatsMean.TabIndex = 1;
            this.txtStatsMean.TabStop = false;
            // 
            // txtStatsBest
            // 
            this.txtStatsBest.Location = new System.Drawing.Point(7, 83);
            this.txtStatsBest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStatsBest.Name = "txtStatsBest";
            this.txtStatsBest.ReadOnly = true;
            this.txtStatsBest.Size = new System.Drawing.Size(111, 23);
            this.txtStatsBest.TabIndex = 0;
            this.txtStatsBest.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnSearchReset);
            this.groupBox6.Controls.Add(this.btnSearchStop);
            this.groupBox6.Controls.Add(this.btnSearchStart);
            this.groupBox6.Location = new System.Drawing.Point(9, 258);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Size = new System.Drawing.Size(273, 74);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Search Control";
            // 
            // btnSearchReset
            // 
            this.btnSearchReset.Enabled = false;
            this.btnSearchReset.Location = new System.Drawing.Point(183, 22);
            this.btnSearchReset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSearchReset.Name = "btnSearchReset";
            this.btnSearchReset.Size = new System.Drawing.Size(82, 40);
            this.btnSearchReset.TabIndex = 2;
            this.btnSearchReset.Text = "Reset";
            // 
            // btnSearchStop
            // 
            this.btnSearchStop.Enabled = false;
            this.btnSearchStop.Location = new System.Drawing.Point(96, 22);
            this.btnSearchStop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSearchStop.Name = "btnSearchStop";
            this.btnSearchStop.Size = new System.Drawing.Size(82, 40);
            this.btnSearchStop.TabIndex = 1;
            this.btnSearchStop.Text = "Stop / Pause";
            // 
            // btnSearchStart
            // 
            this.btnSearchStart.Enabled = false;
            this.btnSearchStart.Location = new System.Drawing.Point(8, 22);
            this.btnSearchStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSearchStart.Name = "btnSearchStart";
            this.btnSearchStart.Size = new System.Drawing.Size(82, 40);
            this.btnSearchStart.TabIndex = 0;
            this.btnSearchStart.Text = "Start / Continue";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnResetExperimentParameters);
            this.groupBox5.Controls.Add(this.btnExperimentInfo);
            this.groupBox5.Controls.Add(this.cmbExperiments);
            this.groupBox5.Location = new System.Drawing.Point(9, 7);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox5.Size = new System.Drawing.Size(273, 80);
            this.groupBox5.TabIndex = 15;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Experiment / Task";
            // 
            // btnResetExperimentParameters
            // 
            this.btnResetExperimentParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnResetExperimentParameters.Location = new System.Drawing.Point(8, 46);
            this.btnResetExperimentParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnResetExperimentParameters.Name = "btnResetExperimentParameters";
            this.btnResetExperimentParameters.Size = new System.Drawing.Size(232, 28);
            this.btnResetExperimentParameters.TabIndex = 48;
            this.btnResetExperimentParameters.Text = "Reset Experiment Parameters";
            this.btnResetExperimentParameters.Click += new System.EventHandler(this.btnResetExperimentParameters_Click);
            // 
            // btnExperimentInfo
            // 
            this.btnExperimentInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExperimentInfo.Location = new System.Drawing.Point(244, 17);
            this.btnExperimentInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExperimentInfo.Name = "btnExperimentInfo";
            this.btnExperimentInfo.Size = new System.Drawing.Size(22, 27);
            this.btnExperimentInfo.TabIndex = 47;
            this.btnExperimentInfo.Text = "?";
            this.btnExperimentInfo.Click += new System.EventHandler(this.btnExperimentInfo_Click);
            // 
            // cmbExperiments
            // 
            this.cmbExperiments.DisplayMember = "Name";
            this.cmbExperiments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExperiments.DropDownWidth = 300;
            this.cmbExperiments.Location = new System.Drawing.Point(9, 18);
            this.cmbExperiments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbExperiments.Name = "cmbExperiments";
            this.cmbExperiments.Size = new System.Drawing.Size(230, 23);
            this.cmbExperiments.TabIndex = 36;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gbxNeatGenomeParameters);
            this.tabPage2.Controls.Add(this.gbxEAParameters);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(603, 478);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Page 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gbxNeatGenomeParameters
            // 
            this.gbxNeatGenomeParameters.Controls.Add(this.label42);
            this.gbxNeatGenomeParameters.Controls.Add(this.txtConnectionWeightMutationProbability);
            this.gbxNeatGenomeParameters.Controls.Add(this.txtDeleteConnectionMutationProbability);
            this.gbxNeatGenomeParameters.Controls.Add(this.txtAddConnectionMutationProbability);
            this.gbxNeatGenomeParameters.Controls.Add(this.label34);
            this.gbxNeatGenomeParameters.Controls.Add(this.label36);
            this.gbxNeatGenomeParameters.Controls.Add(this.txtAddNodeMutationProbability);
            this.gbxNeatGenomeParameters.Controls.Add(this.label35);
            this.gbxNeatGenomeParameters.Location = new System.Drawing.Point(288, 7);
            this.gbxNeatGenomeParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxNeatGenomeParameters.Name = "gbxNeatGenomeParameters";
            this.gbxNeatGenomeParameters.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxNeatGenomeParameters.Size = new System.Drawing.Size(306, 156);
            this.gbxNeatGenomeParameters.TabIndex = 52;
            this.gbxNeatGenomeParameters.TabStop = false;
            this.gbxNeatGenomeParameters.Text = "Asexual Reproduction Settings";
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(69, 115);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(177, 18);
            this.label42.TabIndex = 27;
            this.label42.Text = "Delete Connection (probability)";
            // 
            // txtConnectionWeightMutationProbability
            // 
            this.txtConnectionWeightMutationProbability.Location = new System.Drawing.Point(8, 22);
            this.txtConnectionWeightMutationProbability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtConnectionWeightMutationProbability.Name = "txtConnectionWeightMutationProbability";
            this.txtConnectionWeightMutationProbability.Size = new System.Drawing.Size(55, 23);
            this.txtConnectionWeightMutationProbability.TabIndex = 24;
            this.txtConnectionWeightMutationProbability.Text = "0.988";
            // 
            // txtDeleteConnectionMutationProbability
            // 
            this.txtDeleteConnectionMutationProbability.Location = new System.Drawing.Point(8, 112);
            this.txtDeleteConnectionMutationProbability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtDeleteConnectionMutationProbability.Name = "txtDeleteConnectionMutationProbability";
            this.txtDeleteConnectionMutationProbability.Size = new System.Drawing.Size(55, 23);
            this.txtDeleteConnectionMutationProbability.TabIndex = 26;
            this.txtDeleteConnectionMutationProbability.Text = "0.001";
            // 
            // txtAddConnectionMutationProbability
            // 
            this.txtAddConnectionMutationProbability.Location = new System.Drawing.Point(8, 82);
            this.txtAddConnectionMutationProbability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAddConnectionMutationProbability.Name = "txtAddConnectionMutationProbability";
            this.txtAddConnectionMutationProbability.Size = new System.Drawing.Size(55, 23);
            this.txtAddConnectionMutationProbability.TabIndex = 20;
            this.txtAddConnectionMutationProbability.Text = "0.01";
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(69, 85);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(159, 18);
            this.label34.TabIndex = 25;
            this.label34.Text = "Add Connection (probability)";
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(69, 25);
            this.label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(230, 18);
            this.label36.TabIndex = 21;
            this.label36.Text = "Mutate Weights (probability)";
            // 
            // txtAddNodeMutationProbability
            // 
            this.txtAddNodeMutationProbability.Location = new System.Drawing.Point(8, 52);
            this.txtAddNodeMutationProbability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAddNodeMutationProbability.Name = "txtAddNodeMutationProbability";
            this.txtAddNodeMutationProbability.Size = new System.Drawing.Size(55, 23);
            this.txtAddNodeMutationProbability.TabIndex = 22;
            this.txtAddNodeMutationProbability.Text = "0.001";
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(69, 55);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(159, 18);
            this.label35.TabIndex = 23;
            this.label35.Text = "Add Neuron (probability)";
            // 
            // gbxEAParameters
            // 
            this.gbxEAParameters.BackColor = System.Drawing.Color.Transparent;
            this.gbxEAParameters.Controls.Add(this.label21);
            this.gbxEAParameters.Controls.Add(this.txtSpeciesCount);
            this.gbxEAParameters.Controls.Add(this.label4);
            this.gbxEAParameters.Controls.Add(this.label15);
            this.gbxEAParameters.Controls.Add(this.label32);
            this.gbxEAParameters.Controls.Add(this.label14);
            this.gbxEAParameters.Controls.Add(this.txtSelectionProportion);
            this.gbxEAParameters.Controls.Add(this.txtInterspeciesMatingProportion);
            this.gbxEAParameters.Controls.Add(this.txtElitismProportion);
            this.gbxEAParameters.Controls.Add(this.label25);
            this.gbxEAParameters.Controls.Add(this.txtOffspringSexualProportion);
            this.gbxEAParameters.Controls.Add(this.label26);
            this.gbxEAParameters.Controls.Add(this.txtOffspringAsexualProportion);
            this.gbxEAParameters.Location = new System.Drawing.Point(9, 7);
            this.gbxEAParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxEAParameters.Name = "gbxEAParameters";
            this.gbxEAParameters.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbxEAParameters.Size = new System.Drawing.Size(262, 228);
            this.gbxEAParameters.TabIndex = 16;
            this.gbxEAParameters.TabStop = false;
            this.gbxEAParameters.Text = "Evolution Algorithm Settings";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(65, 29);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(107, 15);
            this.label21.TabIndex = 57;
            this.label21.Text = "Number of Species";
            // 
            // txtSpeciesCount
            // 
            this.txtSpeciesCount.Location = new System.Drawing.Point(7, 25);
            this.txtSpeciesCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSpeciesCount.Name = "txtSpeciesCount";
            this.txtSpeciesCount.Size = new System.Drawing.Size(55, 23);
            this.txtSpeciesCount.TabIndex = 56;
            this.txtSpeciesCount.Text = "10";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 15);
            this.label4.TabIndex = 55;
            this.label4.Text = "Elitism Proportion";
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(7, 190);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(233, 1);
            this.label15.TabIndex = 54;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(65, 89);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(140, 18);
            this.label32.TabIndex = 24;
            this.label32.Text = "Selection Proportion";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(65, 198);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(175, 18);
            this.label14.TabIndex = 53;
            this.label14.Text = "Interspecies Mating Probability";
            // 
            // txtSelectionProportion
            // 
            this.txtSelectionProportion.Location = new System.Drawing.Point(7, 85);
            this.txtSelectionProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSelectionProportion.Name = "txtSelectionProportion";
            this.txtSelectionProportion.Size = new System.Drawing.Size(55, 23);
            this.txtSelectionProportion.TabIndex = 23;
            this.txtSelectionProportion.Text = "0.2";
            // 
            // txtInterspeciesMatingProportion
            // 
            this.txtInterspeciesMatingProportion.Location = new System.Drawing.Point(7, 195);
            this.txtInterspeciesMatingProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtInterspeciesMatingProportion.Name = "txtInterspeciesMatingProportion";
            this.txtInterspeciesMatingProportion.Size = new System.Drawing.Size(55, 23);
            this.txtInterspeciesMatingProportion.TabIndex = 52;
            this.txtInterspeciesMatingProportion.Text = "0.01";
            // 
            // txtElitismProportion
            // 
            this.txtElitismProportion.Location = new System.Drawing.Point(7, 55);
            this.txtElitismProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtElitismProportion.Name = "txtElitismProportion";
            this.txtElitismProportion.Size = new System.Drawing.Size(55, 23);
            this.txtElitismProportion.TabIndex = 21;
            this.txtElitismProportion.Text = "0.2";
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(65, 166);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(175, 18);
            this.label25.TabIndex = 51;
            this.label25.Text = "Sexual Offspring Proportion";
            // 
            // txtOffspringSexualProportion
            // 
            this.txtOffspringSexualProportion.Location = new System.Drawing.Point(7, 163);
            this.txtOffspringSexualProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtOffspringSexualProportion.Name = "txtOffspringSexualProportion";
            this.txtOffspringSexualProportion.Size = new System.Drawing.Size(55, 23);
            this.txtOffspringSexualProportion.TabIndex = 50;
            this.txtOffspringSexualProportion.Text = "0.5";
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(65, 136);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(175, 18);
            this.label26.TabIndex = 49;
            this.label26.Text = "Asexual Offspring Proportion";
            // 
            // txtOffspringAsexualProportion
            // 
            this.txtOffspringAsexualProportion.Location = new System.Drawing.Point(7, 133);
            this.txtOffspringAsexualProportion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtOffspringAsexualProportion.Name = "txtOffspringAsexualProportion";
            this.txtOffspringAsexualProportion.Size = new System.Drawing.Size(55, 23);
            this.txtOffspringAsexualProportion.TabIndex = 48;
            this.txtOffspringAsexualProportion.Text = "0.5";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(611, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPopulationToolStripMenuItem,
            this.loadSeedGenomeToolStripMenuItem,
            this.loadSeedGenomesToolStripMenuItem,
            this.toolStripSeparator,
            this.savePopulationToolStripMenuItem,
            this.saveBestGenomeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadPopulationToolStripMenuItem
            // 
            this.loadPopulationToolStripMenuItem.Name = "loadPopulationToolStripMenuItem";
            this.loadPopulationToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadPopulationToolStripMenuItem.Text = "Load Population";
            // 
            // loadSeedGenomeToolStripMenuItem
            // 
            this.loadSeedGenomeToolStripMenuItem.Name = "loadSeedGenomeToolStripMenuItem";
            this.loadSeedGenomeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadSeedGenomeToolStripMenuItem.Text = "Load Seed Genome";
            // 
            // loadSeedGenomesToolStripMenuItem
            // 
            this.loadSeedGenomesToolStripMenuItem.Name = "loadSeedGenomesToolStripMenuItem";
            this.loadSeedGenomesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadSeedGenomesToolStripMenuItem.Text = "Load Seed Genomes";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(178, 6);
            // 
            // savePopulationToolStripMenuItem
            // 
            this.savePopulationToolStripMenuItem.Enabled = false;
            this.savePopulationToolStripMenuItem.Name = "savePopulationToolStripMenuItem";
            this.savePopulationToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.savePopulationToolStripMenuItem.Text = "Save Population";
            // 
            // saveBestGenomeToolStripMenuItem
            // 
            this.saveBestGenomeToolStripMenuItem.Enabled = false;
            this.saveBestGenomeToolStripMenuItem.Name = "saveBestGenomeToolStripMenuItem";
            this.saveBestGenomeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveBestGenomeToolStripMenuItem.Text = "Save Best Genome";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bestGenomeToolStripMenuItem,
            this.specieChampGenomesToolStripMenuItem,
            this.problemDomainToolStripMenuItem,
            this.graphsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // bestGenomeToolStripMenuItem
            // 
            this.bestGenomeToolStripMenuItem.Name = "bestGenomeToolStripMenuItem";
            this.bestGenomeToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.bestGenomeToolStripMenuItem.Text = "Best Genome";
            // 
            // specieChampGenomesToolStripMenuItem
            // 
            this.specieChampGenomesToolStripMenuItem.Name = "specieChampGenomesToolStripMenuItem";
            this.specieChampGenomesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.specieChampGenomesToolStripMenuItem.Text = "Specie Champ Genomes";
            this.specieChampGenomesToolStripMenuItem.Visible = false;
            // 
            // problemDomainToolStripMenuItem
            // 
            this.problemDomainToolStripMenuItem.Name = "problemDomainToolStripMenuItem";
            this.problemDomainToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.problemDomainToolStripMenuItem.Text = "Problem Domain";
            // 
            // graphsToolStripMenuItem
            // 
            this.graphsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeSeriesToolStripMenuItem,
            this.rankPlotsToolStripMenuItem,
            this.distributionPlotsToolStripMenuItem});
            this.graphsToolStripMenuItem.Name = "graphsToolStripMenuItem";
            this.graphsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.graphsToolStripMenuItem.Text = "Graphs";
            // 
            // timeSeriesToolStripMenuItem
            // 
            this.timeSeriesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fitnessBestMeansToolStripMenuItem,
            this.complexityBestMeansToolStripMenuItem,
            this.evaluationsPerSecToolStripMenuItem});
            this.timeSeriesToolStripMenuItem.Name = "timeSeriesToolStripMenuItem";
            this.timeSeriesToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.timeSeriesToolStripMenuItem.Text = "Time Series";
            // 
            // fitnessBestMeansToolStripMenuItem
            // 
            this.fitnessBestMeansToolStripMenuItem.Name = "fitnessBestMeansToolStripMenuItem";
            this.fitnessBestMeansToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.fitnessBestMeansToolStripMenuItem.Text = "Fitness (Best && Means)";
            // 
            // complexityBestMeansToolStripMenuItem
            // 
            this.complexityBestMeansToolStripMenuItem.Name = "complexityBestMeansToolStripMenuItem";
            this.complexityBestMeansToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.complexityBestMeansToolStripMenuItem.Text = "Complexity (Best && Means)";
            // 
            // evaluationsPerSecToolStripMenuItem
            // 
            this.evaluationsPerSecToolStripMenuItem.Name = "evaluationsPerSecToolStripMenuItem";
            this.evaluationsPerSecToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.evaluationsPerSecToolStripMenuItem.Text = "Evaluations per Sec";
            // 
            // rankPlotsToolStripMenuItem
            // 
            this.rankPlotsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.specieSizeByRankToolStripMenuItem,
            this.specieChampFitnessByRankToolStripMenuItem,
            this.specieChampComplexityByRankToolStripMenuItem,
            this.toolStripSeparator3,
            this.genomeFitnessByRankToolStripMenuItem,
            this.genomeComplexityByRankToolStripMenuItem});
            this.rankPlotsToolStripMenuItem.Name = "rankPlotsToolStripMenuItem";
            this.rankPlotsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.rankPlotsToolStripMenuItem.Text = "Rank Plots";
            // 
            // specieSizeByRankToolStripMenuItem
            // 
            this.specieSizeByRankToolStripMenuItem.Name = "specieSizeByRankToolStripMenuItem";
            this.specieSizeByRankToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.specieSizeByRankToolStripMenuItem.Text = "Specie Size by Rank";
            // 
            // specieChampFitnessByRankToolStripMenuItem
            // 
            this.specieChampFitnessByRankToolStripMenuItem.Name = "specieChampFitnessByRankToolStripMenuItem";
            this.specieChampFitnessByRankToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.specieChampFitnessByRankToolStripMenuItem.Text = "Specie Fitness by Rank (Champs && Mean)";
            // 
            // specieChampComplexityByRankToolStripMenuItem
            // 
            this.specieChampComplexityByRankToolStripMenuItem.Name = "specieChampComplexityByRankToolStripMenuItem";
            this.specieChampComplexityByRankToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.specieChampComplexityByRankToolStripMenuItem.Text = "Specie Complexity by Rank (Champs && Mean)";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(315, 6);
            // 
            // genomeFitnessByRankToolStripMenuItem
            // 
            this.genomeFitnessByRankToolStripMenuItem.Name = "genomeFitnessByRankToolStripMenuItem";
            this.genomeFitnessByRankToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.genomeFitnessByRankToolStripMenuItem.Text = "Genome Fitness by Rank";
            // 
            // genomeComplexityByRankToolStripMenuItem
            // 
            this.genomeComplexityByRankToolStripMenuItem.Name = "genomeComplexityByRankToolStripMenuItem";
            this.genomeComplexityByRankToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.genomeComplexityByRankToolStripMenuItem.Text = "Genome Complexity by Rank";
            // 
            // distributionPlotsToolStripMenuItem
            // 
            this.distributionPlotsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.specieSizeDistributionToolStripMenuItem,
            this.specieFitnessDistributionsToolStripMenuItem,
            this.specieComplexityDistributionsToolStripMenuItem,
            this.toolStripSeparator1,
            this.genomeFitnessDistributionToolStripMenuItem,
            this.genomeComplexityDistributionToolStripMenuItem});
            this.distributionPlotsToolStripMenuItem.Name = "distributionPlotsToolStripMenuItem";
            this.distributionPlotsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.distributionPlotsToolStripMenuItem.Text = "Distribution Plots";
            // 
            // specieSizeDistributionToolStripMenuItem
            // 
            this.specieSizeDistributionToolStripMenuItem.Name = "specieSizeDistributionToolStripMenuItem";
            this.specieSizeDistributionToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.specieSizeDistributionToolStripMenuItem.Text = "Specie Size Distribution";
            // 
            // specieFitnessDistributionsToolStripMenuItem
            // 
            this.specieFitnessDistributionsToolStripMenuItem.Name = "specieFitnessDistributionsToolStripMenuItem";
            this.specieFitnessDistributionsToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.specieFitnessDistributionsToolStripMenuItem.Text = "Specie Fitness Distributions (Champ && Mean)";
            // 
            // specieComplexityDistributionsToolStripMenuItem
            // 
            this.specieComplexityDistributionsToolStripMenuItem.Name = "specieComplexityDistributionsToolStripMenuItem";
            this.specieComplexityDistributionsToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.specieComplexityDistributionsToolStripMenuItem.Text = "Specie Complexity Distributions (Champ && Mean)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(335, 6);
            // 
            // genomeFitnessDistributionToolStripMenuItem
            // 
            this.genomeFitnessDistributionToolStripMenuItem.Name = "genomeFitnessDistributionToolStripMenuItem";
            this.genomeFitnessDistributionToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.genomeFitnessDistributionToolStripMenuItem.Text = "Genome Fitness Distribution";
            // 
            // genomeComplexityDistributionToolStripMenuItem
            // 
            this.genomeComplexityDistributionToolStripMenuItem.Name = "genomeComplexityDistributionToolStripMenuItem";
            this.genomeComplexityDistributionToolStripMenuItem.Size = new System.Drawing.Size(338, 22);
            this.genomeComplexityDistributionToolStripMenuItem.Text = "Genome Complexity Distribution";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnCopyLogToClipboard
            // 
            this.btnCopyLogToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyLogToClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopyLogToClipboard.Location = new System.Drawing.Point(494, 0);
            this.btnCopyLogToClipboard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCopyLogToClipboard.Name = "btnCopyLogToClipboard";
            this.btnCopyLogToClipboard.Size = new System.Drawing.Size(117, 25);
            this.btnCopyLogToClipboard.TabIndex = 1;
            this.btnCopyLogToClipboard.Text = "Copy to clipboard";
            this.btnCopyLogToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyLogToClipboard.Click += new System.EventHandler(this.btnCopyLogToClipboard_Click);
            // 
            // lbxLog
            // 
            this.lbxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxLog.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbxLog.FormattingEnabled = true;
            this.lbxLog.ItemHeight = 15;
            this.lbxLog.Location = new System.Drawing.Point(0, 0);
            this.lbxLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbxLog.Name = "lbxLog";
            this.lbxLog.Size = new System.Drawing.Size(611, 241);
            this.lbxLog.TabIndex = 0;
            // 
            // populationToolStripMenuItem
            // 
            this.populationToolStripMenuItem.Name = "populationToolStripMenuItem";
            this.populationToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 778);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "SharpNEAT";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.gbxGenomePopulation.ResumeLayout(false);
            this.gbxGenomePopulation.PerformLayout();
            this.gbxLogging.ResumeLayout(false);
            this.gbxLogging.PerformLayout();
            this.gbxCurrentStats.ResumeLayout(false);
            this.gbxCurrentStats.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.gbxNeatGenomeParameters.ResumeLayout(false);
            this.gbxNeatGenomeParameters.PerformLayout();
            this.gbxEAParameters.ResumeLayout(false);
            this.gbxEAParameters.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbxLog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnSearchReset;
        private System.Windows.Forms.Button btnSearchStop;
        private System.Windows.Forms.Button btnSearchStart;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnExperimentInfo;
        private System.Windows.Forms.ComboBox cmbExperiments;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox gbxLogging;
        private System.Windows.Forms.TextBox txtFileLogBaseName;
        private System.Windows.Forms.CheckBox chkFileWriteLog;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtFileBaseName;
        private System.Windows.Forms.CheckBox chkFileSaveGenomeOnImprovement;
        private System.Windows.Forms.GroupBox gbxCurrentStats;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox txtSearchStatsMode;
        private System.Windows.Forms.TextBox txtStatsEvalsPerSec;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtStatsMeanGenomeComplx;
        private System.Windows.Forms.TextBox txtStatsBestGenomeComplx;
        private System.Windows.Forms.TextBox txtStatsTotalEvals;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStatsGeneration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStatsMean;
        private System.Windows.Forms.TextBox txtStatsBest;
        private System.Windows.Forms.GroupBox gbxEAParameters;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox txtSelectionProportion;
        private System.Windows.Forms.TextBox txtElitismProportion;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox txtDeleteConnectionMutationProbability;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox txtConnectionWeightMutationProbability;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txtAddNodeMutationProbability;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox txtAddConnectionMutationProbability;
        private System.Windows.Forms.GroupBox gbxNeatGenomeParameters;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSpecieChampMean;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtStatsMaxGenomeComplx;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtStatsInterspeciesOffspringCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtStatsCrossoverOffspringCount;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtStatsAsexualOffspringCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtStatsTotalOffspringCount;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtInterspeciesMatingProportion;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtOffspringSexualProportion;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtOffspringAsexualProportion;
        private System.Windows.Forms.GroupBox gbxGenomePopulation;
        private System.Windows.Forms.TextBox txtPopulationSize;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox txtPopulationStatus;
        private System.Windows.Forms.Button btnCreateRandomPop;
        private System.Windows.Forms.ToolStripMenuItem populationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInitialInterconnectionsProportion;
        private System.Windows.Forms.Button btnCopyLogToClipboard;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPopulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSeedGenomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSeedGenomesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveBestGenomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePopulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestGenomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieChampGenomesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem problemDomainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeSeriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitnessBestMeansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem complexityBestMeansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluationsPerSecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rankPlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieSizeByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieChampFitnessByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieChampComplexityByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem genomeFitnessByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genomeComplexityByRankToolStripMenuItem;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtStatsAlternativeFitness;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtSpeciesCount;
        private System.Windows.Forms.ToolStripMenuItem distributionPlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieSizeDistributionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieFitnessDistributionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specieComplexityDistributionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem genomeFitnessDistributionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genomeComplexityDistributionToolStripMenuItem;
        private System.Windows.Forms.Button btnResetExperiment;
        private System.Windows.Forms.Button btnResetExperimentParameters;
    }
}