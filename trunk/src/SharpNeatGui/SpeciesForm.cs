using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using SharpNeatLib.NetworkVisualization;
using SharpNeatLib.Evolution;


namespace SharpNeat
{
	public class SpeciesForm : System.Windows.Forms.Form
	{
		EvolutionAlgorithm ea=null;

		// Store the SpeciesMonitorControls in a table so we can check which species are 
		// already being monitored. Also place them in a list so that we can always 
		// display them in the same order on the screen.
		Hashtable speciesToMonitorTable = new Hashtable();
		ArrayList speciesToMonitorList = new ArrayList();

		int numberOfBestSpeciesToMonitor=4;
		SpeciesComparer speciesComparer = new SpeciesComparer();

		int updateMilliseconds=2000;
		uint updateGenerations=1;

		#region Windows FormDesigner variables

		private System.Windows.Forms.Panel pnlControl;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.ListView lvwSpecies;
		private System.Windows.Forms.ColumnHeader colId;
		private System.Windows.Forms.ColumnHeader colBestFitness;
		private System.Windows.Forms.ColumnHeader colMeanFitness;
		private System.Windows.Forms.ColumnHeader colSize;
		private System.Windows.Forms.ColumnHeader colAge;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkAutoMonitorBest;
		private System.Windows.Forms.TextBox txtAutoMonitorCount;
		private System.Windows.Forms.Button btnClearDeadSpecies;
		private System.Windows.Forms.ColumnHeader colAgeImprovement;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public SpeciesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.pnlControl = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnClearDeadSpecies = new System.Windows.Forms.Button();
			this.txtAutoMonitorCount = new System.Windows.Forms.TextBox();
			this.chkAutoMonitorBest = new System.Windows.Forms.CheckBox();
			this.lvwSpecies = new System.Windows.Forms.ListView();
			this.colId = new System.Windows.Forms.ColumnHeader();
			this.colBestFitness = new System.Windows.Forms.ColumnHeader();
			this.colMeanFitness = new System.Windows.Forms.ColumnHeader();
			this.colSize = new System.Windows.Forms.ColumnHeader();
			this.colAge = new System.Windows.Forms.ColumnHeader();
			this.colAgeImprovement = new System.Windows.Forms.ColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.pnlControl.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlControl
			// 
			this.pnlControl.Controls.Add(this.label1);
			this.pnlControl.Controls.Add(this.groupBox1);
			this.pnlControl.Controls.Add(this.lvwSpecies);
			this.pnlControl.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlControl.Location = new System.Drawing.Point(0, 0);
			this.pnlControl.Name = "pnlControl";
			this.pnlControl.Size = new System.Drawing.Size(344, 710);
			this.pnlControl.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(312, 32);
			this.label1.TabIndex = 2;
			this.label1.Text = "Double-click a species to monitor its best network. Double-click on the actual ne" +
				"twork to hide it again.";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnClearDeadSpecies);
			this.groupBox1.Controls.Add(this.txtAutoMonitorCount);
			this.groupBox1.Controls.Add(this.chkAutoMonitorBest);
			this.groupBox1.Location = new System.Drawing.Point(8, 656);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328, 48);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// btnClearDeadSpecies
			// 
			this.btnClearDeadSpecies.Location = new System.Drawing.Point(192, 16);
			this.btnClearDeadSpecies.Name = "btnClearDeadSpecies";
			this.btnClearDeadSpecies.Size = new System.Drawing.Size(120, 24);
			this.btnClearDeadSpecies.TabIndex = 8;
			this.btnClearDeadSpecies.Text = "Clear Dead Species";
			this.btnClearDeadSpecies.Click += new System.EventHandler(this.btnClearDeadSpecies_Click);
			// 
			// txtAutoMonitorCount
			// 
			this.txtAutoMonitorCount.Location = new System.Drawing.Point(8, 16);
			this.txtAutoMonitorCount.Name = "txtAutoMonitorCount";
			this.txtAutoMonitorCount.Size = new System.Drawing.Size(56, 20);
			this.txtAutoMonitorCount.TabIndex = 1;
			this.txtAutoMonitorCount.Text = "4";
			this.txtAutoMonitorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtAutoMonitorCount.Leave += new System.EventHandler(this.txtAutoMonitorCount_Leave);
			// 
			// chkAutoMonitorBest
			// 
			this.chkAutoMonitorBest.Location = new System.Drawing.Point(72, 10);
			this.chkAutoMonitorBest.Name = "chkAutoMonitorBest";
			this.chkAutoMonitorBest.Size = new System.Drawing.Size(104, 32);
			this.chkAutoMonitorBest.TabIndex = 0;
			this.chkAutoMonitorBest.Text = "Auto-Monitor best N species";
			this.chkAutoMonitorBest.CheckedChanged += new System.EventHandler(this.chkAutoMonitorBest_CheckedChanged);
			// 
			// lvwSpecies
			// 
			this.lvwSpecies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvwSpecies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.colId,
																						 this.colBestFitness,
																						 this.colMeanFitness,
																						 this.colSize,
																						 this.colAge,
																						 this.colAgeImprovement});
			this.lvwSpecies.Location = new System.Drawing.Point(8, 40);
			this.lvwSpecies.Name = "lvwSpecies";
			this.lvwSpecies.Size = new System.Drawing.Size(336, 616);
			this.lvwSpecies.TabIndex = 0;
			this.lvwSpecies.View = System.Windows.Forms.View.Details;
			this.lvwSpecies.DoubleClick += new System.EventHandler(this.lvwSpecies_DoubleClick);
			// 
			// colId
			// 
			this.colId.Text = "ID";
			this.colId.Width = 24;
			// 
			// colBestFitness
			// 
			this.colBestFitness.Text = "Best Fitness";
			this.colBestFitness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colBestFitness.Width = 69;
			// 
			// colMeanFitness
			// 
			this.colMeanFitness.Text = "Avg. Fitness";
			this.colMeanFitness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colMeanFitness.Width = 72;
			// 
			// colSize
			// 
			this.colSize.Text = "# of Genomes";
			this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colSize.Width = 79;
			// 
			// colAge
			// 
			this.colAge.Text = "Age";
			this.colAge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colAge.Width = 40;
			// 
			// colAgeImprovement
			// 
			this.colAgeImprovement.Text = "Gens since Improvement";
			this.colAgeImprovement.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colAgeImprovement.Width = 50;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(344, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 710);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// pnlMain
			// 
			this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(352, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(432, 710);
			this.pnlMain.TabIndex = 2;
			this.pnlMain.Resize += new System.EventHandler(this.pnlMain_Resize);
			// 
			// SpeciesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(784, 710);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.pnlControl);
			this.Name = "SpeciesForm";
			this.Text = "SharpNEAT [Species]";
			this.pnlControl.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Methods

		int lastUpdateTickCount=0;
		uint lastUpdateGeneration=0;
		public void Update(EvolutionAlgorithm ea)
		{
			this.ea = ea;

			RefreshSpeciesListView();

			if(chkAutoMonitorBest.Checked)
			{
				ShowBestNSpecies(numberOfBestSpeciesToMonitor);
			}
			else
			{	// Keep monitoring whatever the user has selected.
				foreach(SpeciesMonitorControl scm in speciesToMonitorList)
				{
					if(!scm.Dead)
					{	// Check if it is dead yet!
						if(!ea.Population.SpeciesTable.Contains(scm.SpeciesId))
						{	// This species is dead! display as such.
							scm.Dead = true;
						}
						else
						{
							scm.UpdateControl();
						}
					}
					
				}
			}
		}

		public void Reset()
		{
			RemoveAllSpecies();
			ea = null;
			lvwSpecies.Items.Clear();
		}

		#endregion
		
		#region Private Methods
		
		private void RefreshSpeciesListView()
		{
			lvwSpecies.Items.Clear();

			foreach(Species species in ea.Population.SpeciesTable.Values)
			{
				ListViewItem item = new ListViewItem(species.SpeciesId.ToString());
				item.SubItems.Add(species.Members[0].Fitness.ToString("0.000"));
				item.SubItems.Add(species.MeanFitness.ToString("0.000"));
				item.SubItems.Add(species.Members.Count.ToString());
				item.SubItems.Add(species.SpeciesAge.ToString());
				item.SubItems.Add((species.SpeciesAge-species.AgeAtLastImprovement).ToString());
				item.Tag = species.SpeciesId;
				lvwSpecies.Items.Add(item);
			}
		}

		private void LayoutSpeciesControls()
		{
			// Calculate some layout measurements.
			int count = speciesToMonitorList.Count;
			if(count==0)
				return;

			double widthHeightRatio = (double)pnlMain.Width / (double)pnlMain.Height;
			int colCount = Math.Min(count,(int)Math.Max(1.0, Math.Round(Math.Sqrt((double)count) * widthHeightRatio)));
			int rowCount = (int)Math.Ceiling((double)count / (double)colCount);
			
			// NOTE RJM: Patch. Ocasionally attempted to divide by zero.   Might be useful to raise a message.
			if(colCount == 0 || rowCount == 0)
			    return;
			
			int xIncrement = pnlMain.Width / colCount;
			int yIncrement = pnlMain.Height / rowCount;
			Size controlSize = new Size(xIncrement-1, yIncrement-1);

			int xCurrent=0, yCurrent=0;

			// Layout the controls.
			int col=0, row=0;
			foreach(SpeciesMonitorControl smc in speciesToMonitorList)
			{
				smc.Left = xCurrent;
				smc.Top = yCurrent;
				smc.Size = controlSize;
				smc.UpdateControl();
				smc.Visible = true;

				if(++col == colCount)
				{	// Next row.
					col=0;
					row++;

					xCurrent=0;
					yCurrent+=yIncrement;
				}
				else
				{
					xCurrent+=xIncrement;
				}
			}
		}

		private void AddSpecies(int speciesId)
		{
		//----- Create a new SpeciesMonitorControl.
			SpeciesMonitorControl smc = new SpeciesMonitorControl();
			// Hide it until it passed through the layout code.
			smc.Visible = false;
			smc.SetSpecies((Species)ea.Population.SpeciesTable[speciesId]);

			// Add the control to the relevant structures.
			speciesToMonitorTable[speciesId] = smc;
			speciesToMonitorList.Add(smc);
			pnlMain.Controls.Add(smc);

			smc.DoubleClick+=new SpeciesSelectionHandler(SpeciesMonitorControl_DoubleClick);
		}

		private void RemoveSpecies(int speciesId)
		{
			SpeciesMonitorControl smc = (SpeciesMonitorControl)speciesToMonitorTable[speciesId];
			if(smc==null)
				throw new ApplicationException("SpeciesForm:RemoveSpecies - species does not exist to remove.");

			pnlMain.Controls.Remove(smc);
			speciesToMonitorTable.Remove(speciesId);
			speciesToMonitorList.Remove(smc);
			smc.DoubleClick-=new SpeciesSelectionHandler(SpeciesMonitorControl_DoubleClick);
			smc.Dispose();
		}

		private void RemoveAllSpecies()
		{
			for(int i=speciesToMonitorList.Count-1; i>=0; i--)
			{
				SpeciesMonitorControl scm = (SpeciesMonitorControl)speciesToMonitorList[i];
				RemoveSpecies(scm.SpeciesId);
			}
		}

		private void ClearDeadSpecies()
		{
			ArrayList listOfTheDead = new ArrayList();
			
			foreach(SpeciesMonitorControl scm in speciesToMonitorList)
			{
				if(scm.Dead)
				{	// Just store them in a list. Otherwise we will be modifying the list we are iterating.
					listOfTheDead.Add(scm.SpeciesId);
				}
			}

			foreach(int speciesId in listOfTheDead)
			{
				RemoveSpecies(speciesId);
			}
		}

		private void ShowBestNSpecies(int N)
		{
			bool changeFlag=false;
			Hashtable bestSpeciesTable = GetNBestSpecies(N);

			//----- Remove any species that are no longer in the top N.
			ArrayList speciesToRemove = new ArrayList();
			foreach(SpeciesMonitorControl scm in speciesToMonitorList)
			{
				if(!bestSpeciesTable.Contains(scm.SpeciesId))
				{
					speciesToRemove.Add(scm.SpeciesId);
					changeFlag = true;
				}
			}

			foreach(int speciesId in speciesToRemove)
				RemoveSpecies(speciesId);

			//----- Ensure the top N are in the monitor list.
			foreach(Species species in bestSpeciesTable.Values)
			{
				if(!speciesToMonitorTable.Contains(species.SpeciesId))
				{
					AddSpecies(species.SpeciesId);
					changeFlag = true;
				}
			}

			//----- If something changed then re-layout the controls.
			if(changeFlag)
			{
				LayoutSpeciesControls();
			}
			else
			{	// Just update the network controls.
				foreach(SpeciesMonitorControl scm in speciesToMonitorList)
					scm.UpdateControl();
			}
		}

		private Hashtable GetNBestSpecies(int N)
		{
			// Put all species into a list and sort them on bestfitness
			ArrayList speciesList = new ArrayList();
			foreach(Species species in ea.Population.SpeciesTable.Values)
				speciesList.Add(species);

			speciesList.Sort(speciesComparer);

			// Put the best N into a table keyed on ID and return it.
			N = Math.Min(N, speciesList.Count);
			Hashtable speciesTable = new Hashtable(N);

			for(int i=0; i<N; i++)
			{
				Species species = (Species)speciesList[i];
				speciesTable.Add(species.SpeciesId, species);
			}

			return speciesTable;
		}


		#endregion

		#region Event Handlers

		private void lvwSpecies_DoubleClick(object sender, System.EventArgs e)
		{
			if(lvwSpecies.SelectedItems.Count==0)
				return;

			foreach(ListViewItem item in lvwSpecies.SelectedItems)
			{
				int speciesId = (int)item.Tag;

				if(speciesToMonitorTable.Contains(speciesId))
				{	// Already being monitored.
					continue;
				}
				AddSpecies(speciesId);
			}

			LayoutSpeciesControls();
		}

		private void pnlMain_Resize(object sender, System.EventArgs e)
		{
			LayoutSpeciesControls();
		}

		private void btnClearDeadSpecies_Click(object sender, System.EventArgs e)
		{
			ClearDeadSpecies();
		}

		private void chkAutoMonitorBest_CheckedChanged(object sender, System.EventArgs e)
		{
			if(chkAutoMonitorBest.Checked)
			{
				if(ea == null)
					return;

				// Remove any species that are currently being monitored.
				RemoveAllSpecies();
				ShowBestNSpecies(numberOfBestSpeciesToMonitor);
			}
			//			else // Just leave the same set of species being monitored.
		}

		private void txtAutoMonitorCount_Leave(object sender, System.EventArgs e)
		{
			try
			{
				numberOfBestSpeciesToMonitor = int.Parse(txtAutoMonitorCount.Text);
			}
			catch(Exception)
			{
				txtAutoMonitorCount.Text = numberOfBestSpeciesToMonitor.ToString();
			}
		}
		
		private void SpeciesMonitorControl_DoubleClick(object sender, SpeciesSelectionEventArgs e)
		{
			if(chkAutoMonitorBest.Checked)
				return;

			RemoveSpecies(e.SpeciesId);
			LayoutSpeciesControls();
		}

		#endregion


	}

	#region SpeciesComparer

	class SpeciesComparer : IComparer
	{
		#region IComparer Members

		/// <summary>
		/// Implemented in contravention of the spec to place fittest species first (descending order).
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			Species a = (Species)x;
			Species b = (Species)y;

			return b.Members[0].Fitness.CompareTo(a.Members[0].Fitness);
		}

		#endregion

	}

	#endregion

}
