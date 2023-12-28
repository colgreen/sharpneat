using System.Diagnostics;

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
            Debug.WriteLine("MainForm.Dispose() enter");

            if (disposing)
            {
                _eaRunner?.Dispose();
                _bestGenomeForm?.Dispose();
                _taskForm?.Dispose();
                _fitnessTimeSeriesForm?.Dispose();
                _complexityTimeSeriesForm?.Dispose();
                _evalsPerSecTimeSeriesForm?.Dispose();
                _speciesSizeRankForm?.Dispose();
                _speciesFitnessRankForm?.Dispose();
                _speciesComplexityRankForm?.Dispose();
                _genomeFitnessRankForm?.Dispose();
                _genomeComplexityRankForm?.Dispose();
                _speciesSizeHistogramForm?.Dispose();
                _speciesMeanFitnessHistogramForm?.Dispose();
                _speciesMeanComplexityHistogramForm?.Dispose();
                _genomeFitnessHistogramForm?.Dispose();
                _genomeComplexityHistogramForm?.Dispose();

                components?.Dispose();
            }
            base.Dispose(disposing);

            Debug.WriteLine("MainForm.Dispose() exit");
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            splitContainer1 = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            gbxGenomePopulation = new GroupBox();
            label1 = new Label();
            txtInitialInterconnectionsProportion = new TextBox();
            txtPopulationSize = new TextBox();
            label28 = new Label();
            txtPopulationStatus = new TextBox();
            btnCreateRandomPop = new Button();
            gbxLogging = new GroupBox();
            label2 = new Label();
            txtFileLogBaseName = new TextBox();
            chkFileWriteLog = new CheckBox();
            txtFileBaseName = new TextBox();
            label13 = new Label();
            chkFileSaveGenomeOnImprovement = new CheckBox();
            gbxCurrentStats = new GroupBox();
            label23 = new Label();
            label22 = new Label();
            label20 = new Label();
            label19 = new Label();
            label18 = new Label();
            label17 = new Label();
            label16 = new Label();
            label12 = new Label();
            label9 = new Label();
            label11 = new Label();
            label10 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label3 = new Label();
            txtStatsInterspeciesOffspringCount = new TextBox();
            txtStatsAlternativeFitness = new TextBox();
            txtStatsCrossoverOffspringCount = new TextBox();
            txtStatsAsexualOffspringCount = new TextBox();
            txtStatsTotalOffspringCount = new TextBox();
            txtStatsMaxGenomeComplx = new TextBox();
            txtSpeciesChampsMean = new TextBox();
            txtSearchStatsMode = new TextBox();
            txtStatsEvalsPerSec = new TextBox();
            txtStatsMeanGenomeComplx = new TextBox();
            txtStatsBestGenomeComplx = new TextBox();
            txtStatsTotalEvals = new TextBox();
            txtStatsGeneration = new TextBox();
            txtStatsMean = new TextBox();
            txtStatsBest = new TextBox();
            groupBox6 = new GroupBox();
            btnSearchReset = new Button();
            btnSearchStop = new Button();
            btnSearchStart = new Button();
            groupBox5 = new GroupBox();
            btnLoadExperimentDefaultParameters = new Button();
            btnExperimentInfo = new Button();
            cmbExperiments = new ComboBox();
            tabPage2 = new TabPage();
            gbxNeatGenomeParameters = new GroupBox();
            label42 = new Label();
            txtConnectionWeightMutationProbability = new TextBox();
            txtDeleteConnectionMutationProbability = new TextBox();
            txtAddConnectionMutationProbability = new TextBox();
            label34 = new Label();
            label36 = new Label();
            txtAddNodeMutationProbability = new TextBox();
            label35 = new Label();
            gbxEAParameters = new GroupBox();
            label21 = new Label();
            txtSpeciesCount = new TextBox();
            label4 = new Label();
            label15 = new Label();
            label32 = new Label();
            label14 = new Label();
            txtSelectionProportion = new TextBox();
            txtInterspeciesMatingProportion = new TextBox();
            txtElitismProportion = new TextBox();
            label25 = new Label();
            txtOffspringRecombinationProportion = new TextBox();
            label26 = new Label();
            txtOffspringAsexualProportion = new TextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadPopulationToolStripMenuItem = new ToolStripMenuItem();
            loadSeedGenomeToolStripMenuItem = new ToolStripMenuItem();
            loadSeedGenomesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            savePopulationToolStripMenuItem = new ToolStripMenuItem();
            saveBestGenomeToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            bestGenomeToolStripMenuItem = new ToolStripMenuItem();
            specieChampGenomesToolStripMenuItem = new ToolStripMenuItem();
            taskToolStripMenuItem = new ToolStripMenuItem();
            graphsToolStripMenuItem = new ToolStripMenuItem();
            timeSeriesToolStripMenuItem = new ToolStripMenuItem();
            fitnessBestMeanToolStripMenuItem = new ToolStripMenuItem();
            complexityBestMeanToolStripMenuItem = new ToolStripMenuItem();
            evaluationsPerSecToolStripMenuItem = new ToolStripMenuItem();
            histogramsToolStripMenuItem = new ToolStripMenuItem();
            speciesSizeHistogramToolStripMenuItem = new ToolStripMenuItem();
            speciesMeanFitnessHistogramToolStripMenuItem = new ToolStripMenuItem();
            speciesMeanComplexityHistogramToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            genomeFitnessHistogramToolStripMenuItem = new ToolStripMenuItem();
            genomeComplexityHistogramToolStripMenuItem = new ToolStripMenuItem();
            rankPlotsToolStripMenuItem = new ToolStripMenuItem();
            speciesSizeByRankToolStripMenuItem = new ToolStripMenuItem();
            speciesFitnessByRankToolStripMenuItem = new ToolStripMenuItem();
            speciesComplexityByRankToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            genomeFitnessByRankToolStripMenuItem = new ToolStripMenuItem();
            genomeComplexityByRankToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnCopyLogToClipboard = new Button();
            lbxLog = new ListBox();
            populationToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            gbxGenomePopulation.SuspendLayout();
            gbxLogging.SuspendLayout();
            gbxCurrentStats.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox5.SuspendLayout();
            tabPage2.SuspendLayout();
            gbxNeatGenomeParameters.SuspendLayout();
            gbxEAParameters.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControl1);
            splitContainer1.Panel1.Controls.Add(menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnCopyLogToClipboard);
            splitContainer1.Panel2.Controls.Add(lbxLog);
            splitContainer1.Size = new Size(554, 693);
            splitContainer1.SplitterDistance = 496;
            splitContainer1.SplitterWidth = 7;
            splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 24);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(554, 472);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(gbxGenomePopulation);
            tabPage1.Controls.Add(gbxLogging);
            tabPage1.Controls.Add(gbxCurrentStats);
            tabPage1.Controls.Add(groupBox6);
            tabPage1.Controls.Add(groupBox5);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Margin = new Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4, 3, 4, 3);
            tabPage1.Size = new Size(546, 444);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Page 1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // gbxGenomePopulation
            // 
            gbxGenomePopulation.Controls.Add(label1);
            gbxGenomePopulation.Controls.Add(txtInitialInterconnectionsProportion);
            gbxGenomePopulation.Controls.Add(txtPopulationSize);
            gbxGenomePopulation.Controls.Add(label28);
            gbxGenomePopulation.Controls.Add(txtPopulationStatus);
            gbxGenomePopulation.Controls.Add(btnCreateRandomPop);
            gbxGenomePopulation.Location = new Point(9, 89);
            gbxGenomePopulation.Margin = new Padding(4, 3, 4, 3);
            gbxGenomePopulation.Name = "gbxGenomePopulation";
            gbxGenomePopulation.Padding = new Padding(4, 3, 4, 3);
            gbxGenomePopulation.Size = new Size(240, 135);
            gbxGenomePopulation.TabIndex = 21;
            gbxGenomePopulation.TabStop = false;
            gbxGenomePopulation.Text = "Genome Population";
            // 
            // label1
            // 
            label1.Location = new Point(105, 98);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(129, 31);
            label1.TabIndex = 54;
            label1.Text = "Initial Connections Proportion";
            // 
            // txtInitialInterconnectionsProportion
            // 
            txtInitialInterconnectionsProportion.Location = new Point(9, 104);
            txtInitialInterconnectionsProportion.Margin = new Padding(4, 3, 4, 3);
            txtInitialInterconnectionsProportion.Name = "txtInitialInterconnectionsProportion";
            txtInitialInterconnectionsProportion.Size = new Size(93, 23);
            txtInitialInterconnectionsProportion.TabIndex = 53;
            txtInitialInterconnectionsProportion.Text = "0.1";
            // 
            // txtPopulationSize
            // 
            txtPopulationSize.Location = new Point(9, 75);
            txtPopulationSize.Margin = new Padding(4, 3, 4, 3);
            txtPopulationSize.Name = "txtPopulationSize";
            txtPopulationSize.Size = new Size(93, 23);
            txtPopulationSize.TabIndex = 51;
            txtPopulationSize.Text = "150";
            // 
            // label28
            // 
            label28.Location = new Point(105, 76);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(132, 18);
            label28.TabIndex = 52;
            label28.Text = "Population Size";
            // 
            // txtPopulationStatus
            // 
            txtPopulationStatus.BackColor = Color.Red;
            txtPopulationStatus.Font = new Font("Microsoft Sans Serif", 8.25F);
            txtPopulationStatus.ForeColor = Color.Black;
            txtPopulationStatus.Location = new Point(9, 20);
            txtPopulationStatus.Margin = new Padding(4, 3, 4, 3);
            txtPopulationStatus.Name = "txtPopulationStatus";
            txtPopulationStatus.ReadOnly = true;
            txtPopulationStatus.Size = new Size(223, 20);
            txtPopulationStatus.TabIndex = 50;
            txtPopulationStatus.TabStop = false;
            txtPopulationStatus.Text = "Population not initialized";
            txtPopulationStatus.TextAlign = HorizontalAlignment.Center;
            // 
            // btnCreateRandomPop
            // 
            btnCreateRandomPop.Font = new Font("Microsoft Sans Serif", 8.25F);
            btnCreateRandomPop.Location = new Point(8, 45);
            btnCreateRandomPop.Margin = new Padding(4, 3, 4, 3);
            btnCreateRandomPop.Name = "btnCreateRandomPop";
            btnCreateRandomPop.Size = new Size(224, 24);
            btnCreateRandomPop.TabIndex = 49;
            btnCreateRandomPop.Text = "Create Random Population";
            btnCreateRandomPop.Click += btnCreateRandomPop_Click;
            // 
            // gbxLogging
            // 
            gbxLogging.Controls.Add(label2);
            gbxLogging.Controls.Add(txtFileLogBaseName);
            gbxLogging.Controls.Add(chkFileWriteLog);
            gbxLogging.Controls.Add(txtFileBaseName);
            gbxLogging.Controls.Add(label13);
            gbxLogging.Controls.Add(chkFileSaveGenomeOnImprovement);
            gbxLogging.Location = new Point(9, 306);
            gbxLogging.Margin = new Padding(4, 3, 4, 3);
            gbxLogging.Name = "gbxLogging";
            gbxLogging.Padding = new Padding(4, 3, 4, 3);
            gbxLogging.Size = new Size(240, 130);
            gbxLogging.TabIndex = 20;
            gbxLogging.TabStop = false;
            gbxLogging.Text = "File";
            // 
            // label2
            // 
            label2.Location = new Point(180, 87);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(54, 37);
            label2.TabIndex = 23;
            label2.Text = "filename prefix";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtFileLogBaseName
            // 
            txtFileLogBaseName.Location = new Point(9, 95);
            txtFileLogBaseName.Margin = new Padding(4, 3, 4, 3);
            txtFileLogBaseName.Name = "txtFileLogBaseName";
            txtFileLogBaseName.Size = new Size(169, 23);
            txtFileLogBaseName.TabIndex = 25;
            txtFileLogBaseName.Text = "sharpneat";
            // 
            // chkFileWriteLog
            // 
            chkFileWriteLog.Location = new Point(9, 69);
            chkFileWriteLog.Margin = new Padding(4, 3, 4, 3);
            chkFileWriteLog.Name = "chkFileWriteLog";
            chkFileWriteLog.Size = new Size(152, 28);
            chkFileWriteLog.TabIndex = 24;
            chkFileWriteLog.Text = "Write log file (*.log)";
            // 
            // txtFileBaseName
            // 
            txtFileBaseName.Location = new Point(9, 40);
            txtFileBaseName.Margin = new Padding(4, 3, 4, 3);
            txtFileBaseName.Name = "txtFileBaseName";
            txtFileBaseName.Size = new Size(169, 23);
            txtFileBaseName.TabIndex = 1;
            txtFileBaseName.Text = "champ";
            // 
            // label13
            // 
            label13.Location = new Point(180, 33);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(54, 37);
            label13.TabIndex = 23;
            label13.Text = "filename prefix";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // chkFileSaveGenomeOnImprovement
            // 
            chkFileSaveGenomeOnImprovement.Location = new Point(9, 14);
            chkFileSaveGenomeOnImprovement.Margin = new Padding(4, 3, 4, 3);
            chkFileSaveGenomeOnImprovement.Name = "chkFileSaveGenomeOnImprovement";
            chkFileSaveGenomeOnImprovement.Size = new Size(272, 28);
            chkFileSaveGenomeOnImprovement.TabIndex = 0;
            chkFileSaveGenomeOnImprovement.Text = "Save best genome (*.net)";
            // 
            // gbxCurrentStats
            // 
            gbxCurrentStats.Controls.Add(label23);
            gbxCurrentStats.Controls.Add(label22);
            gbxCurrentStats.Controls.Add(label20);
            gbxCurrentStats.Controls.Add(label19);
            gbxCurrentStats.Controls.Add(label18);
            gbxCurrentStats.Controls.Add(label17);
            gbxCurrentStats.Controls.Add(label16);
            gbxCurrentStats.Controls.Add(label12);
            gbxCurrentStats.Controls.Add(label9);
            gbxCurrentStats.Controls.Add(label11);
            gbxCurrentStats.Controls.Add(label10);
            gbxCurrentStats.Controls.Add(label8);
            gbxCurrentStats.Controls.Add(label7);
            gbxCurrentStats.Controls.Add(label6);
            gbxCurrentStats.Controls.Add(label5);
            gbxCurrentStats.Controls.Add(label3);
            gbxCurrentStats.Controls.Add(txtStatsInterspeciesOffspringCount);
            gbxCurrentStats.Controls.Add(txtStatsAlternativeFitness);
            gbxCurrentStats.Controls.Add(txtStatsCrossoverOffspringCount);
            gbxCurrentStats.Controls.Add(txtStatsAsexualOffspringCount);
            gbxCurrentStats.Controls.Add(txtStatsTotalOffspringCount);
            gbxCurrentStats.Controls.Add(txtStatsMaxGenomeComplx);
            gbxCurrentStats.Controls.Add(txtSpeciesChampsMean);
            gbxCurrentStats.Controls.Add(txtSearchStatsMode);
            gbxCurrentStats.Controls.Add(txtStatsEvalsPerSec);
            gbxCurrentStats.Controls.Add(txtStatsMeanGenomeComplx);
            gbxCurrentStats.Controls.Add(txtStatsBestGenomeComplx);
            gbxCurrentStats.Controls.Add(txtStatsTotalEvals);
            gbxCurrentStats.Controls.Add(txtStatsGeneration);
            gbxCurrentStats.Controls.Add(txtStatsMean);
            gbxCurrentStats.Controls.Add(txtStatsBest);
            gbxCurrentStats.Location = new Point(257, 7);
            gbxCurrentStats.Margin = new Padding(4, 3, 4, 3);
            gbxCurrentStats.Name = "gbxCurrentStats";
            gbxCurrentStats.Padding = new Padding(4, 3, 4, 3);
            gbxCurrentStats.Size = new Size(281, 429);
            gbxCurrentStats.TabIndex = 19;
            gbxCurrentStats.TabStop = false;
            gbxCurrentStats.Text = "Current Stats";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(116, 400);
            label23.Name = "label23";
            label23.Size = new Size(166, 15);
            label23.TabIndex = 35;
            label23.Text = "Offspring Count (interspecies)";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(116, 374);
            label22.Name = "label22";
            label22.Size = new Size(154, 15);
            label22.TabIndex = 35;
            label22.Text = "Offspring Count (crossover)";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(116, 347);
            label20.Name = "label20";
            label20.Size = new Size(143, 15);
            label20.TabIndex = 35;
            label20.Text = "Offspring Count (asexual)";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(116, 319);
            label19.Name = "label19";
            label19.Size = new Size(121, 15);
            label19.TabIndex = 35;
            label19.Text = "Total Offspring Count";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(116, 292);
            label18.Name = "label18";
            label18.Size = new Size(94, 15);
            label18.TabIndex = 35;
            label18.Text = "Max Complexity";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(116, 265);
            label17.Name = "label17";
            label17.Size = new Size(101, 15);
            label17.TabIndex = 35;
            label17.Text = "Mean Complexity";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(116, 238);
            label16.Name = "label16";
            label16.Size = new Size(141, 15);
            label16.TabIndex = 35;
            label16.Text = "Best Genome Complexity";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(116, 211);
            label12.Name = "label12";
            label12.Size = new Size(95, 15);
            label12.TabIndex = 35;
            label12.Text = "Evaluations / sec";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(116, 153);
            label9.Name = "label9";
            label9.RightToLeft = RightToLeft.No;
            label9.Size = new Size(76, 15);
            label9.TabIndex = 35;
            label9.Text = "Mean Fitness";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(116, 166);
            label11.Name = "label11";
            label11.RightToLeft = RightToLeft.No;
            label11.Size = new Size(101, 15);
            label11.TabIndex = 35;
            label11.Text = "(species' champs)";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(116, 186);
            label10.Name = "label10";
            label10.Size = new Size(95, 15);
            label10.TabIndex = 35;
            label10.Text = "Total Evaluations";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(116, 132);
            label8.Name = "label8";
            label8.Size = new Size(76, 15);
            label8.TabIndex = 35;
            label8.Text = "Mean Fitness";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(116, 104);
            label7.Name = "label7";
            label7.Size = new Size(128, 15);
            label7.TabIndex = 35;
            label7.Text = "Alternative Best Fitness";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(116, 77);
            label6.Name = "label6";
            label6.Size = new Size(68, 15);
            label6.TabIndex = 35;
            label6.Text = "Best Fitness";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(116, 49);
            label5.Name = "label5";
            label5.Size = new Size(65, 15);
            label5.TabIndex = 35;
            label5.Text = "Generation";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(116, 22);
            label3.Name = "label3";
            label3.Size = new Size(119, 15);
            label3.TabIndex = 35;
            label3.Text = "Current Search Mode";
            // 
            // txtStatsInterspeciesOffspringCount
            // 
            txtStatsInterspeciesOffspringCount.Location = new Point(6, 397);
            txtStatsInterspeciesOffspringCount.Margin = new Padding(4, 3, 4, 3);
            txtStatsInterspeciesOffspringCount.Name = "txtStatsInterspeciesOffspringCount";
            txtStatsInterspeciesOffspringCount.ReadOnly = true;
            txtStatsInterspeciesOffspringCount.Size = new Size(108, 23);
            txtStatsInterspeciesOffspringCount.TabIndex = 32;
            txtStatsInterspeciesOffspringCount.TabStop = false;
            // 
            // txtStatsAlternativeFitness
            // 
            txtStatsAlternativeFitness.Location = new Point(6, 100);
            txtStatsAlternativeFitness.Margin = new Padding(4, 3, 4, 3);
            txtStatsAlternativeFitness.Name = "txtStatsAlternativeFitness";
            txtStatsAlternativeFitness.ReadOnly = true;
            txtStatsAlternativeFitness.Size = new Size(108, 23);
            txtStatsAlternativeFitness.TabIndex = 34;
            txtStatsAlternativeFitness.TabStop = false;
            // 
            // txtStatsCrossoverOffspringCount
            // 
            txtStatsCrossoverOffspringCount.Location = new Point(6, 370);
            txtStatsCrossoverOffspringCount.Margin = new Padding(4, 3, 4, 3);
            txtStatsCrossoverOffspringCount.Name = "txtStatsCrossoverOffspringCount";
            txtStatsCrossoverOffspringCount.ReadOnly = true;
            txtStatsCrossoverOffspringCount.Size = new Size(108, 23);
            txtStatsCrossoverOffspringCount.TabIndex = 30;
            txtStatsCrossoverOffspringCount.TabStop = false;
            // 
            // txtStatsAsexualOffspringCount
            // 
            txtStatsAsexualOffspringCount.Location = new Point(6, 343);
            txtStatsAsexualOffspringCount.Margin = new Padding(4, 3, 4, 3);
            txtStatsAsexualOffspringCount.Name = "txtStatsAsexualOffspringCount";
            txtStatsAsexualOffspringCount.ReadOnly = true;
            txtStatsAsexualOffspringCount.Size = new Size(108, 23);
            txtStatsAsexualOffspringCount.TabIndex = 28;
            txtStatsAsexualOffspringCount.TabStop = false;
            // 
            // txtStatsTotalOffspringCount
            // 
            txtStatsTotalOffspringCount.Location = new Point(6, 316);
            txtStatsTotalOffspringCount.Margin = new Padding(4, 3, 4, 3);
            txtStatsTotalOffspringCount.Name = "txtStatsTotalOffspringCount";
            txtStatsTotalOffspringCount.ReadOnly = true;
            txtStatsTotalOffspringCount.Size = new Size(108, 23);
            txtStatsTotalOffspringCount.TabIndex = 26;
            txtStatsTotalOffspringCount.TabStop = false;
            // 
            // txtStatsMaxGenomeComplx
            // 
            txtStatsMaxGenomeComplx.Location = new Point(6, 289);
            txtStatsMaxGenomeComplx.Margin = new Padding(4, 3, 4, 3);
            txtStatsMaxGenomeComplx.Name = "txtStatsMaxGenomeComplx";
            txtStatsMaxGenomeComplx.ReadOnly = true;
            txtStatsMaxGenomeComplx.Size = new Size(108, 23);
            txtStatsMaxGenomeComplx.TabIndex = 24;
            txtStatsMaxGenomeComplx.TabStop = false;
            // 
            // txtSpeciesChampsMean
            // 
            txtSpeciesChampsMean.Location = new Point(6, 154);
            txtSpeciesChampsMean.Margin = new Padding(4, 3, 4, 3);
            txtSpeciesChampsMean.Name = "txtSpeciesChampsMean";
            txtSpeciesChampsMean.ReadOnly = true;
            txtSpeciesChampsMean.Size = new Size(108, 23);
            txtSpeciesChampsMean.TabIndex = 22;
            txtSpeciesChampsMean.TabStop = false;
            // 
            // txtSearchStatsMode
            // 
            txtSearchStatsMode.BackColor = Color.LightSkyBlue;
            txtSearchStatsMode.Location = new Point(6, 19);
            txtSearchStatsMode.Margin = new Padding(4, 3, 4, 3);
            txtSearchStatsMode.Name = "txtSearchStatsMode";
            txtSearchStatsMode.ReadOnly = true;
            txtSearchStatsMode.Size = new Size(108, 23);
            txtSearchStatsMode.TabIndex = 20;
            txtSearchStatsMode.TabStop = false;
            // 
            // txtStatsEvalsPerSec
            // 
            txtStatsEvalsPerSec.Location = new Point(6, 208);
            txtStatsEvalsPerSec.Margin = new Padding(4, 3, 4, 3);
            txtStatsEvalsPerSec.Name = "txtStatsEvalsPerSec";
            txtStatsEvalsPerSec.ReadOnly = true;
            txtStatsEvalsPerSec.Size = new Size(108, 23);
            txtStatsEvalsPerSec.TabIndex = 18;
            txtStatsEvalsPerSec.TabStop = false;
            // 
            // txtStatsMeanGenomeComplx
            // 
            txtStatsMeanGenomeComplx.Location = new Point(6, 262);
            txtStatsMeanGenomeComplx.Margin = new Padding(4, 3, 4, 3);
            txtStatsMeanGenomeComplx.Name = "txtStatsMeanGenomeComplx";
            txtStatsMeanGenomeComplx.ReadOnly = true;
            txtStatsMeanGenomeComplx.Size = new Size(108, 23);
            txtStatsMeanGenomeComplx.TabIndex = 15;
            txtStatsMeanGenomeComplx.TabStop = false;
            // 
            // txtStatsBestGenomeComplx
            // 
            txtStatsBestGenomeComplx.Location = new Point(6, 235);
            txtStatsBestGenomeComplx.Margin = new Padding(4, 3, 4, 3);
            txtStatsBestGenomeComplx.Name = "txtStatsBestGenomeComplx";
            txtStatsBestGenomeComplx.ReadOnly = true;
            txtStatsBestGenomeComplx.Size = new Size(108, 23);
            txtStatsBestGenomeComplx.TabIndex = 14;
            txtStatsBestGenomeComplx.TabStop = false;
            // 
            // txtStatsTotalEvals
            // 
            txtStatsTotalEvals.Location = new Point(6, 181);
            txtStatsTotalEvals.Margin = new Padding(4, 3, 4, 3);
            txtStatsTotalEvals.Name = "txtStatsTotalEvals";
            txtStatsTotalEvals.ReadOnly = true;
            txtStatsTotalEvals.Size = new Size(108, 23);
            txtStatsTotalEvals.TabIndex = 12;
            txtStatsTotalEvals.TabStop = false;
            // 
            // txtStatsGeneration
            // 
            txtStatsGeneration.Location = new Point(6, 46);
            txtStatsGeneration.Margin = new Padding(4, 3, 4, 3);
            txtStatsGeneration.Name = "txtStatsGeneration";
            txtStatsGeneration.ReadOnly = true;
            txtStatsGeneration.Size = new Size(108, 23);
            txtStatsGeneration.TabIndex = 6;
            txtStatsGeneration.TabStop = false;
            // 
            // txtStatsMean
            // 
            txtStatsMean.Location = new Point(6, 127);
            txtStatsMean.Margin = new Padding(4, 3, 4, 3);
            txtStatsMean.Name = "txtStatsMean";
            txtStatsMean.ReadOnly = true;
            txtStatsMean.Size = new Size(108, 23);
            txtStatsMean.TabIndex = 1;
            txtStatsMean.TabStop = false;
            // 
            // txtStatsBest
            // 
            txtStatsBest.Location = new Point(6, 73);
            txtStatsBest.Margin = new Padding(4, 3, 4, 3);
            txtStatsBest.Name = "txtStatsBest";
            txtStatsBest.ReadOnly = true;
            txtStatsBest.Size = new Size(108, 23);
            txtStatsBest.TabIndex = 0;
            txtStatsBest.TabStop = false;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(btnSearchReset);
            groupBox6.Controls.Add(btnSearchStop);
            groupBox6.Controls.Add(btnSearchStart);
            groupBox6.Location = new Point(9, 230);
            groupBox6.Margin = new Padding(4, 3, 4, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new Padding(4, 3, 4, 3);
            groupBox6.Size = new Size(240, 70);
            groupBox6.TabIndex = 18;
            groupBox6.TabStop = false;
            groupBox6.Text = "Search Control";
            // 
            // btnSearchReset
            // 
            btnSearchReset.Enabled = false;
            btnSearchReset.Location = new Point(164, 22);
            btnSearchReset.Margin = new Padding(4, 3, 4, 3);
            btnSearchReset.Name = "btnSearchReset";
            btnSearchReset.Size = new Size(70, 38);
            btnSearchReset.TabIndex = 2;
            btnSearchReset.Text = "Reset";
            btnSearchReset.Click += btnSearchReset_Click;
            // 
            // btnSearchStop
            // 
            btnSearchStop.Enabled = false;
            btnSearchStop.Location = new Point(86, 22);
            btnSearchStop.Margin = new Padding(4, 3, 4, 3);
            btnSearchStop.Name = "btnSearchStop";
            btnSearchStop.Size = new Size(70, 38);
            btnSearchStop.TabIndex = 1;
            btnSearchStop.Text = "Stop / Pause";
            btnSearchStop.Click += btnSearchStop_Click;
            // 
            // btnSearchStart
            // 
            btnSearchStart.Enabled = false;
            btnSearchStart.Location = new Point(8, 22);
            btnSearchStart.Margin = new Padding(4, 3, 4, 3);
            btnSearchStart.Name = "btnSearchStart";
            btnSearchStart.Size = new Size(70, 38);
            btnSearchStart.TabIndex = 0;
            btnSearchStart.Text = "Start / Continue";
            btnSearchStart.Click += btnSearchStart_Click;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btnLoadExperimentDefaultParameters);
            groupBox5.Controls.Add(btnExperimentInfo);
            groupBox5.Controls.Add(cmbExperiments);
            groupBox5.Location = new Point(9, 7);
            groupBox5.Margin = new Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new Padding(4, 3, 4, 3);
            groupBox5.Size = new Size(240, 76);
            groupBox5.TabIndex = 15;
            groupBox5.TabStop = false;
            groupBox5.Text = "Experiment / Task";
            // 
            // btnLoadExperimentDefaultParameters
            // 
            btnLoadExperimentDefaultParameters.Font = new Font("Microsoft Sans Serif", 8.25F);
            btnLoadExperimentDefaultParameters.Location = new Point(7, 46);
            btnLoadExperimentDefaultParameters.Margin = new Padding(4, 3, 4, 3);
            btnLoadExperimentDefaultParameters.Name = "btnLoadExperimentDefaultParameters";
            btnLoadExperimentDefaultParameters.Size = new Size(198, 24);
            btnLoadExperimentDefaultParameters.TabIndex = 48;
            btnLoadExperimentDefaultParameters.Text = "Load Experiment Default Parameters";
            btnLoadExperimentDefaultParameters.Click += btnLoadExperimentDefaultParameters_Click;
            // 
            // btnExperimentInfo
            // 
            btnExperimentInfo.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold);
            btnExperimentInfo.Location = new Point(212, 15);
            btnExperimentInfo.Margin = new Padding(4, 3, 4, 3);
            btnExperimentInfo.Name = "btnExperimentInfo";
            btnExperimentInfo.Size = new Size(22, 25);
            btnExperimentInfo.TabIndex = 47;
            btnExperimentInfo.Text = "?";
            btnExperimentInfo.Click += btnExperimentInfo_Click;
            // 
            // cmbExperiments
            // 
            cmbExperiments.DisplayMember = "Name";
            cmbExperiments.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbExperiments.DropDownWidth = 300;
            cmbExperiments.Location = new Point(8, 16);
            cmbExperiments.Margin = new Padding(4, 3, 4, 3);
            cmbExperiments.Name = "cmbExperiments";
            cmbExperiments.Size = new Size(199, 23);
            cmbExperiments.TabIndex = 36;
            cmbExperiments.SelectedIndexChanged += cmbExperiments_SelectedIndexChanged;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(gbxNeatGenomeParameters);
            tabPage2.Controls.Add(gbxEAParameters);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4, 3, 4, 3);
            tabPage2.Size = new Size(546, 444);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Page 2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // gbxNeatGenomeParameters
            // 
            gbxNeatGenomeParameters.Controls.Add(label42);
            gbxNeatGenomeParameters.Controls.Add(txtConnectionWeightMutationProbability);
            gbxNeatGenomeParameters.Controls.Add(txtDeleteConnectionMutationProbability);
            gbxNeatGenomeParameters.Controls.Add(txtAddConnectionMutationProbability);
            gbxNeatGenomeParameters.Controls.Add(label34);
            gbxNeatGenomeParameters.Controls.Add(label36);
            gbxNeatGenomeParameters.Controls.Add(txtAddNodeMutationProbability);
            gbxNeatGenomeParameters.Controls.Add(label35);
            gbxNeatGenomeParameters.Location = new Point(288, 7);
            gbxNeatGenomeParameters.Margin = new Padding(4, 3, 4, 3);
            gbxNeatGenomeParameters.Name = "gbxNeatGenomeParameters";
            gbxNeatGenomeParameters.Padding = new Padding(4, 3, 4, 3);
            gbxNeatGenomeParameters.Size = new Size(254, 156);
            gbxNeatGenomeParameters.TabIndex = 52;
            gbxNeatGenomeParameters.TabStop = false;
            gbxNeatGenomeParameters.Text = "Asexual Reproduction Settings";
            // 
            // label42
            // 
            label42.Location = new Point(69, 115);
            label42.Margin = new Padding(4, 0, 4, 0);
            label42.Name = "label42";
            label42.Size = new Size(177, 18);
            label42.TabIndex = 27;
            label42.Text = "Delete Connection (probability)";
            // 
            // txtConnectionWeightMutationProbability
            // 
            txtConnectionWeightMutationProbability.Location = new Point(8, 22);
            txtConnectionWeightMutationProbability.Margin = new Padding(4, 3, 4, 3);
            txtConnectionWeightMutationProbability.Name = "txtConnectionWeightMutationProbability";
            txtConnectionWeightMutationProbability.Size = new Size(55, 23);
            txtConnectionWeightMutationProbability.TabIndex = 24;
            txtConnectionWeightMutationProbability.Text = "0.988";
            // 
            // txtDeleteConnectionMutationProbability
            // 
            txtDeleteConnectionMutationProbability.Location = new Point(8, 112);
            txtDeleteConnectionMutationProbability.Margin = new Padding(4, 3, 4, 3);
            txtDeleteConnectionMutationProbability.Name = "txtDeleteConnectionMutationProbability";
            txtDeleteConnectionMutationProbability.Size = new Size(55, 23);
            txtDeleteConnectionMutationProbability.TabIndex = 26;
            txtDeleteConnectionMutationProbability.Text = "0.001";
            // 
            // txtAddConnectionMutationProbability
            // 
            txtAddConnectionMutationProbability.Location = new Point(8, 82);
            txtAddConnectionMutationProbability.Margin = new Padding(4, 3, 4, 3);
            txtAddConnectionMutationProbability.Name = "txtAddConnectionMutationProbability";
            txtAddConnectionMutationProbability.Size = new Size(55, 23);
            txtAddConnectionMutationProbability.TabIndex = 20;
            txtAddConnectionMutationProbability.Text = "0.01";
            // 
            // label34
            // 
            label34.Location = new Point(69, 85);
            label34.Margin = new Padding(4, 0, 4, 0);
            label34.Name = "label34";
            label34.Size = new Size(175, 18);
            label34.TabIndex = 25;
            label34.Text = "Add Connection (probability)";
            // 
            // label36
            // 
            label36.Location = new Point(69, 25);
            label36.Margin = new Padding(4, 0, 4, 0);
            label36.Name = "label36";
            label36.Size = new Size(230, 18);
            label36.TabIndex = 21;
            label36.Text = "Mutate Weights (probability)";
            // 
            // txtAddNodeMutationProbability
            // 
            txtAddNodeMutationProbability.Location = new Point(8, 52);
            txtAddNodeMutationProbability.Margin = new Padding(4, 3, 4, 3);
            txtAddNodeMutationProbability.Name = "txtAddNodeMutationProbability";
            txtAddNodeMutationProbability.Size = new Size(55, 23);
            txtAddNodeMutationProbability.TabIndex = 22;
            txtAddNodeMutationProbability.Text = "0.001";
            // 
            // label35
            // 
            label35.Location = new Point(69, 55);
            label35.Margin = new Padding(4, 0, 4, 0);
            label35.Name = "label35";
            label35.Size = new Size(159, 18);
            label35.TabIndex = 23;
            label35.Text = "Add Neuron (probability)";
            // 
            // gbxEAParameters
            // 
            gbxEAParameters.BackColor = Color.Transparent;
            gbxEAParameters.Controls.Add(label21);
            gbxEAParameters.Controls.Add(txtSpeciesCount);
            gbxEAParameters.Controls.Add(label4);
            gbxEAParameters.Controls.Add(label15);
            gbxEAParameters.Controls.Add(label32);
            gbxEAParameters.Controls.Add(label14);
            gbxEAParameters.Controls.Add(txtSelectionProportion);
            gbxEAParameters.Controls.Add(txtInterspeciesMatingProportion);
            gbxEAParameters.Controls.Add(txtElitismProportion);
            gbxEAParameters.Controls.Add(label25);
            gbxEAParameters.Controls.Add(txtOffspringRecombinationProportion);
            gbxEAParameters.Controls.Add(label26);
            gbxEAParameters.Controls.Add(txtOffspringAsexualProportion);
            gbxEAParameters.Location = new Point(9, 7);
            gbxEAParameters.Margin = new Padding(4, 3, 4, 3);
            gbxEAParameters.Name = "gbxEAParameters";
            gbxEAParameters.Padding = new Padding(4, 3, 4, 3);
            gbxEAParameters.Size = new Size(262, 228);
            gbxEAParameters.TabIndex = 16;
            gbxEAParameters.TabStop = false;
            gbxEAParameters.Text = "Evolution Algorithm Settings";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(65, 29);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(107, 15);
            label21.TabIndex = 57;
            label21.Text = "Number of Species";
            // 
            // txtSpeciesCount
            // 
            txtSpeciesCount.Location = new Point(7, 25);
            txtSpeciesCount.Margin = new Padding(4, 3, 4, 3);
            txtSpeciesCount.Name = "txtSpeciesCount";
            txtSpeciesCount.Size = new Size(55, 23);
            txtSpeciesCount.TabIndex = 56;
            txtSpeciesCount.Text = "10";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(65, 59);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(102, 15);
            label4.TabIndex = 55;
            label4.Text = "Elitism Proportion";
            // 
            // label15
            // 
            label15.BackColor = Color.Black;
            label15.Location = new Point(7, 190);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(233, 1);
            label15.TabIndex = 54;
            // 
            // label32
            // 
            label32.Location = new Point(65, 89);
            label32.Margin = new Padding(4, 0, 4, 0);
            label32.Name = "label32";
            label32.Size = new Size(140, 18);
            label32.TabIndex = 24;
            label32.Text = "Selection Proportion";
            // 
            // label14
            // 
            label14.Location = new Point(65, 198);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(175, 18);
            label14.TabIndex = 53;
            label14.Text = "Interspecies Mating Probability";
            // 
            // txtSelectionProportion
            // 
            txtSelectionProportion.Location = new Point(7, 85);
            txtSelectionProportion.Margin = new Padding(4, 3, 4, 3);
            txtSelectionProportion.Name = "txtSelectionProportion";
            txtSelectionProportion.Size = new Size(55, 23);
            txtSelectionProportion.TabIndex = 23;
            txtSelectionProportion.Text = "0.2";
            // 
            // txtInterspeciesMatingProportion
            // 
            txtInterspeciesMatingProportion.Location = new Point(7, 195);
            txtInterspeciesMatingProportion.Margin = new Padding(4, 3, 4, 3);
            txtInterspeciesMatingProportion.Name = "txtInterspeciesMatingProportion";
            txtInterspeciesMatingProportion.Size = new Size(55, 23);
            txtInterspeciesMatingProportion.TabIndex = 52;
            txtInterspeciesMatingProportion.Text = "0.01";
            // 
            // txtElitismProportion
            // 
            txtElitismProportion.Location = new Point(7, 55);
            txtElitismProportion.Margin = new Padding(4, 3, 4, 3);
            txtElitismProportion.Name = "txtElitismProportion";
            txtElitismProportion.Size = new Size(55, 23);
            txtElitismProportion.TabIndex = 21;
            txtElitismProportion.Text = "0.2";
            // 
            // label25
            // 
            label25.Location = new Point(65, 166);
            label25.Margin = new Padding(4, 0, 4, 0);
            label25.Name = "label25";
            label25.Size = new Size(175, 18);
            label25.TabIndex = 51;
            label25.Text = "Recombination Offspring Proportion";
            // 
            // txtOffspringRecombinationProportion
            // 
            txtOffspringRecombinationProportion.Location = new Point(7, 163);
            txtOffspringRecombinationProportion.Margin = new Padding(4, 3, 4, 3);
            txtOffspringRecombinationProportion.Name = "txtOffspringRecombinationProportion";
            txtOffspringRecombinationProportion.Size = new Size(55, 23);
            txtOffspringRecombinationProportion.TabIndex = 50;
            txtOffspringRecombinationProportion.Text = "0.5";
            // 
            // label26
            // 
            label26.Location = new Point(65, 136);
            label26.Margin = new Padding(4, 0, 4, 0);
            label26.Name = "label26";
            label26.Size = new Size(175, 18);
            label26.TabIndex = 49;
            label26.Text = "Asexual Offspring Proportion";
            // 
            // txtOffspringAsexualProportion
            // 
            txtOffspringAsexualProportion.Location = new Point(7, 133);
            txtOffspringAsexualProportion.Margin = new Padding(4, 3, 4, 3);
            txtOffspringAsexualProportion.Name = "txtOffspringAsexualProportion";
            txtOffspringAsexualProportion.Size = new Size(55, 23);
            txtOffspringAsexualProportion.TabIndex = 48;
            txtOffspringAsexualProportion.Text = "0.5";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(554, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadPopulationToolStripMenuItem, loadSeedGenomeToolStripMenuItem, loadSeedGenomesToolStripMenuItem, toolStripSeparator, savePopulationToolStripMenuItem, saveBestGenomeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadPopulationToolStripMenuItem
            // 
            loadPopulationToolStripMenuItem.Name = "loadPopulationToolStripMenuItem";
            loadPopulationToolStripMenuItem.Size = new Size(181, 22);
            loadPopulationToolStripMenuItem.Text = "Load Population";
            loadPopulationToolStripMenuItem.Click += loadPopulationToolStripMenuItem_Click;
            // 
            // loadSeedGenomeToolStripMenuItem
            // 
            loadSeedGenomeToolStripMenuItem.Name = "loadSeedGenomeToolStripMenuItem";
            loadSeedGenomeToolStripMenuItem.Size = new Size(181, 22);
            loadSeedGenomeToolStripMenuItem.Text = "Load Seed Genome";
            loadSeedGenomeToolStripMenuItem.Click += loadSeedGenomeToolStripMenuItem_Click;
            // 
            // loadSeedGenomesToolStripMenuItem
            // 
            loadSeedGenomesToolStripMenuItem.Name = "loadSeedGenomesToolStripMenuItem";
            loadSeedGenomesToolStripMenuItem.Size = new Size(181, 22);
            loadSeedGenomesToolStripMenuItem.Text = "Load Seed Genomes";
            loadSeedGenomesToolStripMenuItem.Click += loadSeedGenomesToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(178, 6);
            // 
            // savePopulationToolStripMenuItem
            // 
            savePopulationToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Name = "savePopulationToolStripMenuItem";
            savePopulationToolStripMenuItem.Size = new Size(181, 22);
            savePopulationToolStripMenuItem.Text = "Save Population";
            savePopulationToolStripMenuItem.Click += savePopulationToolStripMenuItem_Click;
            // 
            // saveBestGenomeToolStripMenuItem
            // 
            saveBestGenomeToolStripMenuItem.Enabled = false;
            saveBestGenomeToolStripMenuItem.Name = "saveBestGenomeToolStripMenuItem";
            saveBestGenomeToolStripMenuItem.Size = new Size(181, 22);
            saveBestGenomeToolStripMenuItem.Text = "Save Best Genome";
            saveBestGenomeToolStripMenuItem.Click += saveBestGenomeToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { bestGenomeToolStripMenuItem, specieChampGenomesToolStripMenuItem, taskToolStripMenuItem, graphsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // bestGenomeToolStripMenuItem
            // 
            bestGenomeToolStripMenuItem.Name = "bestGenomeToolStripMenuItem";
            bestGenomeToolStripMenuItem.Size = new Size(203, 22);
            bestGenomeToolStripMenuItem.Text = "Best Genome";
            bestGenomeToolStripMenuItem.Click += bestGenomeToolStripMenuItem_Click;
            // 
            // specieChampGenomesToolStripMenuItem
            // 
            specieChampGenomesToolStripMenuItem.Name = "specieChampGenomesToolStripMenuItem";
            specieChampGenomesToolStripMenuItem.Size = new Size(203, 22);
            specieChampGenomesToolStripMenuItem.Text = "Specie Champ Genomes";
            specieChampGenomesToolStripMenuItem.Visible = false;
            // 
            // taskToolStripMenuItem
            // 
            taskToolStripMenuItem.Name = "taskToolStripMenuItem";
            taskToolStripMenuItem.Size = new Size(203, 22);
            taskToolStripMenuItem.Text = "Task";
            taskToolStripMenuItem.Click += taskToolStripMenuItem_Click;
            // 
            // graphsToolStripMenuItem
            // 
            graphsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { timeSeriesToolStripMenuItem, histogramsToolStripMenuItem, rankPlotsToolStripMenuItem });
            graphsToolStripMenuItem.Name = "graphsToolStripMenuItem";
            graphsToolStripMenuItem.Size = new Size(203, 22);
            graphsToolStripMenuItem.Text = "Charts";
            // 
            // timeSeriesToolStripMenuItem
            // 
            timeSeriesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fitnessBestMeanToolStripMenuItem, complexityBestMeanToolStripMenuItem, evaluationsPerSecToolStripMenuItem });
            timeSeriesToolStripMenuItem.Name = "timeSeriesToolStripMenuItem";
            timeSeriesToolStripMenuItem.Size = new Size(135, 22);
            timeSeriesToolStripMenuItem.Text = "Time Series";
            // 
            // fitnessBestMeanToolStripMenuItem
            // 
            fitnessBestMeanToolStripMenuItem.Name = "fitnessBestMeanToolStripMenuItem";
            fitnessBestMeanToolStripMenuItem.Size = new Size(214, 22);
            fitnessBestMeanToolStripMenuItem.Text = "Fitness (Best && Mean)";
            fitnessBestMeanToolStripMenuItem.Click += fitnessBestMeanToolStripMenuItem_Click;
            // 
            // complexityBestMeanToolStripMenuItem
            // 
            complexityBestMeanToolStripMenuItem.Name = "complexityBestMeanToolStripMenuItem";
            complexityBestMeanToolStripMenuItem.Size = new Size(214, 22);
            complexityBestMeanToolStripMenuItem.Text = "Complexity (Best && Mean)";
            complexityBestMeanToolStripMenuItem.Click += complexityBestMeanToolStripMenuItem_Click;
            // 
            // evaluationsPerSecToolStripMenuItem
            // 
            evaluationsPerSecToolStripMenuItem.Name = "evaluationsPerSecToolStripMenuItem";
            evaluationsPerSecToolStripMenuItem.Size = new Size(214, 22);
            evaluationsPerSecToolStripMenuItem.Text = "Evaluations per second";
            evaluationsPerSecToolStripMenuItem.Click += evaluationsPerSecToolStripMenuItem_Click;
            // 
            // histogramsToolStripMenuItem
            // 
            histogramsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { speciesSizeHistogramToolStripMenuItem, speciesMeanFitnessHistogramToolStripMenuItem, speciesMeanComplexityHistogramToolStripMenuItem, toolStripSeparator1, genomeFitnessHistogramToolStripMenuItem, genomeComplexityHistogramToolStripMenuItem });
            histogramsToolStripMenuItem.Name = "histogramsToolStripMenuItem";
            histogramsToolStripMenuItem.Size = new Size(135, 22);
            histogramsToolStripMenuItem.Text = "Histograms";
            // 
            // speciesSizeHistogramToolStripMenuItem
            // 
            speciesSizeHistogramToolStripMenuItem.Name = "speciesSizeHistogramToolStripMenuItem";
            speciesSizeHistogramToolStripMenuItem.Size = new Size(269, 22);
            speciesSizeHistogramToolStripMenuItem.Text = "Species Size Histogram";
            speciesSizeHistogramToolStripMenuItem.Click += speciesSizeHistogramToolStripMenuItem_Click;
            // 
            // speciesMeanFitnessHistogramToolStripMenuItem
            // 
            speciesMeanFitnessHistogramToolStripMenuItem.Name = "speciesMeanFitnessHistogramToolStripMenuItem";
            speciesMeanFitnessHistogramToolStripMenuItem.Size = new Size(269, 22);
            speciesMeanFitnessHistogramToolStripMenuItem.Text = "Species Mean Fitness Histogram";
            speciesMeanFitnessHistogramToolStripMenuItem.Click += speciesMeanFitnessHistogramToolStripMenuItem_Click;
            // 
            // speciesMeanComplexityHistogramToolStripMenuItem
            // 
            speciesMeanComplexityHistogramToolStripMenuItem.Name = "speciesMeanComplexityHistogramToolStripMenuItem";
            speciesMeanComplexityHistogramToolStripMenuItem.Size = new Size(269, 22);
            speciesMeanComplexityHistogramToolStripMenuItem.Text = "Species Mean Complexity Histogram";
            speciesMeanComplexityHistogramToolStripMenuItem.Click += speciesMeanComplexityHistogramToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(266, 6);
            // 
            // genomeFitnessHistogramToolStripMenuItem
            // 
            genomeFitnessHistogramToolStripMenuItem.Name = "genomeFitnessHistogramToolStripMenuItem";
            genomeFitnessHistogramToolStripMenuItem.Size = new Size(269, 22);
            genomeFitnessHistogramToolStripMenuItem.Text = "Genome Fitness Histogram";
            genomeFitnessHistogramToolStripMenuItem.Click += genomeFitnessHistogramToolStripMenuItem_Click;
            // 
            // genomeComplexityHistogramToolStripMenuItem
            // 
            genomeComplexityHistogramToolStripMenuItem.Name = "genomeComplexityHistogramToolStripMenuItem";
            genomeComplexityHistogramToolStripMenuItem.Size = new Size(269, 22);
            genomeComplexityHistogramToolStripMenuItem.Text = "Genome Complexity Histogram";
            genomeComplexityHistogramToolStripMenuItem.Click += genomeComplexityHistogramToolStripMenuItem_Click;
            // 
            // rankPlotsToolStripMenuItem
            // 
            rankPlotsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { speciesSizeByRankToolStripMenuItem, speciesFitnessByRankToolStripMenuItem, speciesComplexityByRankToolStripMenuItem, toolStripSeparator3, genomeFitnessByRankToolStripMenuItem, genomeComplexityByRankToolStripMenuItem });
            rankPlotsToolStripMenuItem.Name = "rankPlotsToolStripMenuItem";
            rankPlotsToolStripMenuItem.Size = new Size(135, 22);
            rankPlotsToolStripMenuItem.Text = "Rank Plots";
            // 
            // speciesSizeByRankToolStripMenuItem
            // 
            speciesSizeByRankToolStripMenuItem.Name = "speciesSizeByRankToolStripMenuItem";
            speciesSizeByRankToolStripMenuItem.Size = new Size(301, 22);
            speciesSizeByRankToolStripMenuItem.Text = "Species Size by Rank";
            speciesSizeByRankToolStripMenuItem.Click += speciesSizeByRankToolStripMenuItem_Click;
            // 
            // speciesFitnessByRankToolStripMenuItem
            // 
            speciesFitnessByRankToolStripMenuItem.Name = "speciesFitnessByRankToolStripMenuItem";
            speciesFitnessByRankToolStripMenuItem.Size = new Size(301, 22);
            speciesFitnessByRankToolStripMenuItem.Text = "Species Fitness by Rank (Best && Mean)";
            speciesFitnessByRankToolStripMenuItem.Click += speciesFitnessByRankToolStripMenuItem_Click;
            // 
            // speciesComplexityByRankToolStripMenuItem
            // 
            speciesComplexityByRankToolStripMenuItem.Name = "speciesComplexityByRankToolStripMenuItem";
            speciesComplexityByRankToolStripMenuItem.Size = new Size(301, 22);
            speciesComplexityByRankToolStripMenuItem.Text = "Species Complexity by Rank (Best && Mean)";
            speciesComplexityByRankToolStripMenuItem.Click += speciesComplexityByRankToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(298, 6);
            // 
            // genomeFitnessByRankToolStripMenuItem
            // 
            genomeFitnessByRankToolStripMenuItem.Name = "genomeFitnessByRankToolStripMenuItem";
            genomeFitnessByRankToolStripMenuItem.Size = new Size(301, 22);
            genomeFitnessByRankToolStripMenuItem.Text = "Genome Fitness by Rank";
            genomeFitnessByRankToolStripMenuItem.Click += genomeFitnessByRankToolStripMenuItem_Click;
            // 
            // genomeComplexityByRankToolStripMenuItem
            // 
            genomeComplexityByRankToolStripMenuItem.Name = "genomeComplexityByRankToolStripMenuItem";
            genomeComplexityByRankToolStripMenuItem.Size = new Size(301, 22);
            genomeComplexityByRankToolStripMenuItem.Text = "Genome Complexity by Rank";
            genomeComplexityByRankToolStripMenuItem.Click += genomeComplexityByRankToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // btnCopyLogToClipboard
            // 
            btnCopyLogToClipboard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyLogToClipboard.FlatStyle = FlatStyle.Popup;
            btnCopyLogToClipboard.Location = new Point(419, 1);
            btnCopyLogToClipboard.Margin = new Padding(4, 3, 4, 3);
            btnCopyLogToClipboard.Name = "btnCopyLogToClipboard";
            btnCopyLogToClipboard.Size = new Size(117, 25);
            btnCopyLogToClipboard.TabIndex = 1;
            btnCopyLogToClipboard.Text = "Copy to clipboard";
            btnCopyLogToClipboard.UseVisualStyleBackColor = true;
            btnCopyLogToClipboard.Click += btnCopyLogToClipboard_Click;
            // 
            // lbxLog
            // 
            lbxLog.Dock = DockStyle.Fill;
            lbxLog.Font = new Font("Courier New", 9F);
            lbxLog.FormattingEnabled = true;
            lbxLog.ItemHeight = 15;
            lbxLog.Location = new Point(0, 0);
            lbxLog.Margin = new Padding(4, 3, 4, 3);
            lbxLog.Name = "lbxLog";
            lbxLog.Size = new Size(554, 190);
            lbxLog.TabIndex = 0;
            // 
            // populationToolStripMenuItem
            // 
            populationToolStripMenuItem.Name = "populationToolStripMenuItem";
            populationToolStripMenuItem.Size = new Size(171, 22);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 693);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "SharpNEAT";
            FormClosing += MainForm_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            gbxGenomePopulation.ResumeLayout(false);
            gbxGenomePopulation.PerformLayout();
            gbxLogging.ResumeLayout(false);
            gbxLogging.PerformLayout();
            gbxCurrentStats.ResumeLayout(false);
            gbxCurrentStats.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            gbxNeatGenomeParameters.ResumeLayout(false);
            gbxNeatGenomeParameters.PerformLayout();
            gbxEAParameters.ResumeLayout(false);
            gbxEAParameters.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtSearchStatsMode;
        private System.Windows.Forms.TextBox txtStatsEvalsPerSec;
        private System.Windows.Forms.TextBox txtStatsMeanGenomeComplx;
        private System.Windows.Forms.TextBox txtStatsBestGenomeComplx;
        private System.Windows.Forms.TextBox txtStatsTotalEvals;
        private System.Windows.Forms.TextBox txtStatsGeneration;
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
        private System.Windows.Forms.TextBox txtSpeciesChampsMean;
        private System.Windows.Forms.TextBox txtStatsMaxGenomeComplx;
        private System.Windows.Forms.TextBox txtStatsInterspeciesOffspringCount;
        private System.Windows.Forms.TextBox txtStatsCrossoverOffspringCount;
        private System.Windows.Forms.TextBox txtStatsAsexualOffspringCount;
        private System.Windows.Forms.TextBox txtStatsTotalOffspringCount;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtInterspeciesMatingProportion;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtOffspringRecombinationProportion;
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
        private System.Windows.Forms.ToolStripMenuItem taskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeSeriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitnessBestMeanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem complexityBestMeanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluationsPerSecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rankPlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesSizeByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesFitnessByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesComplexityByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem genomeFitnessByRankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genomeComplexityByRankToolStripMenuItem;
        private System.Windows.Forms.TextBox txtStatsAlternativeFitness;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtSpeciesCount;
        private System.Windows.Forms.ToolStripMenuItem histogramsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesSizeHistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesMeanFitnessHistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speciesMeanComplexityHistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem genomeFitnessHistogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genomeComplexityHistogramToolStripMenuItem;
        private System.Windows.Forms.Button btnLoadExperimentDefaultParameters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
    }
}