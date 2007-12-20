using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using SharpNeatLib.Evolution;
using RudyGraph;

namespace SharpNeat
{
	/// <summary>
	/// Shows a graph of progress. Incorporates graph lines showing best fitness, mean fitness
	/// number of species and average genome complexity.
	/// </summary>
	public class ProgressGraphForm : System.Windows.Forms.Form
	{
		#region Class Variables

		// The max number of points to show on screen befor ethe graph starts rolling old 
		// values of to the left. 900 gives us 15 mins of history witha 1 sec update rate.
		const int HISTORY_LENGTH = 900;

		RollingPlotSet plotSetBestFitness;
		RollingPlotSet plotSetMeanFitness;
		RollingPlotSet plotSetSpeciesCount;
		RollingPlotSet plotSetAvgComplexity;
		RollingPlotSet plotSetCompatibilityThreshold;

		#endregion

		#region Form designer variables

		private RudyGraph.RudyGraphControl graph1;
		private RudyGraph.RudyGraphControl graph2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public ProgressGraphForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			plotSetBestFitness = new RollingPlotSet(HISTORY_LENGTH, LineType.Straight, PlotPointType.None, Color.Black, Color.Black);
			plotSetMeanFitness = new RollingPlotSet(HISTORY_LENGTH, LineType.Straight, PlotPointType.None, Color.Blue, Color.Black);
			plotSetAvgComplexity = new RollingPlotSet(HISTORY_LENGTH, LineType.Straight, PlotPointType.None, Color.Green, Color.Black);

			plotSetSpeciesCount = new RollingPlotSet(HISTORY_LENGTH, LineType.Straight, PlotPointType.None, Color.Black, Color.Black);
			plotSetCompatibilityThreshold = new RollingPlotSet(HISTORY_LENGTH, LineType.Straight, PlotPointType.None, Color.Red, Color.Black);
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
			this.graph1 = new RudyGraph.RudyGraphControl();
			this.graph2 = new RudyGraph.RudyGraphControl();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// graph1
			// 
			this.graph1.Location = new System.Drawing.Point(8, 16);
			this.graph1.Name = "graph1";
			this.graph1.Size = new System.Drawing.Size(424, 200);
			this.graph1.TabIndex = 0;
			// 
			// graph2
			// 
			this.graph2.Location = new System.Drawing.Point(8, 240);
			this.graph2.Name = "graph2";
			this.graph2.Size = new System.Drawing.Size(424, 200);
			this.graph2.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Black;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(8, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Best Fitness";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Blue;
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(80, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Mean Fitness";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Green;
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(336, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Mean Complexity";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Black;
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(8, 224);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "Species Count";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Red;
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Location = new System.Drawing.Point(328, 224);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(104, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "Compat. Threshold";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ProgressGraphForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 448);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.graph2);
			this.Controls.Add(this.graph1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ProgressGraphForm";
			this.Text = "SharpNEAT [Progress Graphs]";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ProgressGraphForm_Closing);
			this.VisibleChanged += new System.EventHandler(this.ProgressGraphForm_VisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Methods

		public void Update(EvolutionAlgorithm ea)
		{
			Population p = ea.Population;

			bool visible = this.Visible;

			if(visible)
			{
				graph1.BeginUpdate();
				try
				{
					plotSetBestFitness.Enqueue(ea.BestGenome.Fitness);
					plotSetMeanFitness.Enqueue(p.MeanFitness);
					plotSetSpeciesCount.Enqueue(p.SpeciesTable.Values.Count);
				}
				finally
				{
					graph1.EndUpdate();
				}

				graph2.BeginUpdate();
				try
				{
					plotSetAvgComplexity.Enqueue(p.AvgComplexity);
					plotSetCompatibilityThreshold.Enqueue(ea.NeatParameters.compatibilityThreshold);
				}
				finally
				{
					graph2.EndUpdate();
				}
			}
			else
			{
				// Keep recording values anyway ready for when the form is made visible.
				plotSetBestFitness.Enqueue(ea.BestGenome.Fitness);
				plotSetMeanFitness.Enqueue(p.MeanFitness);
				plotSetSpeciesCount.Enqueue(p.SpeciesTable.Values.Count);
				plotSetAvgComplexity.Enqueue(p.AvgComplexity);
				plotSetCompatibilityThreshold.Enqueue(ea.NeatParameters.compatibilityThreshold);

			}
		}

		public void Reset()
		{
			graph1.BeginUpdate();
			try
			{
				plotSetBestFitness.Clear();
				plotSetMeanFitness.Clear();
				plotSetSpeciesCount.Clear();
			}
			finally
			{
				graph1.EndUpdate();
			}

			graph2.BeginUpdate();
			try
			{
				plotSetAvgComplexity.Clear();
				plotSetCompatibilityThreshold.Clear();
			}
			finally
			{
				graph2.EndUpdate();
			}
		}

		#endregion

		#region Private Methods

		private void BindPlotSets()
		{
			graph1.PrimaryYAxis.Bind(plotSetBestFitness);
			graph1.PrimaryYAxis.Bind(plotSetMeanFitness);
			graph1.SecondaryYAxis.Bind(plotSetAvgComplexity);

			graph2.PrimaryYAxis.Bind(plotSetSpeciesCount);
			graph2.SecondaryYAxis.Bind(plotSetCompatibilityThreshold);
		}

		private void UnbindPlotSets()
		{
			graph1.PrimaryYAxis.UnBind(plotSetBestFitness);
			graph1.PrimaryYAxis.UnBind(plotSetMeanFitness);
			graph1.SecondaryYAxis.UnBind(plotSetAvgComplexity);

			graph2.PrimaryYAxis.UnBind(plotSetSpeciesCount);
			graph2.SecondaryYAxis.UnBind(plotSetCompatibilityThreshold);
		}

		#endregion

		#region Event Handlers

		private void ProgressGraphForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Don't actualyl close the form. Just hide it so we can continue tracking.
			e.Cancel = true;
			this.Visible = false;	
		}

		private void ProgressGraphForm_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible)
			{
				BindPlotSets();
			}
			else
			{	// Unbind the plot sets from the graphs to prevent the graph controls from updating unnecessarily.
				UnbindPlotSets();
			}
		}

		#endregion
	}
}
