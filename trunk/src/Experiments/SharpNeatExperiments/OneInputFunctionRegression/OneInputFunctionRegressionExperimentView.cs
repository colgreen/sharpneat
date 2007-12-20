using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RudyGraph;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// Summary description for OneInputFunctionRegressionExperimentView.
	/// </summary>
	public class OneInputFunctionRegressionExperimentView : AbstractExperimentView
	{
		#region Class Variables

		OneInputFunctionRegressionNetworkEvaluator networkEvaluator=null;
		PlotSet correctResponsePlotSet;
		PlotSet networkResponsePlotSet;

		#endregion

		#region Form Designer variables

		private RudyGraph.RudyGraphControl rudyGraphControl1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public OneInputFunctionRegressionExperimentView(OneInputFunctionRegressionNetworkEvaluator networkEvaluator)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Keep a reference to the network evaluator. We need this to draw/refresh the graph.
			this.networkEvaluator = networkEvaluator;

			// Create some plot sets and bind them to the graph control.
			correctResponsePlotSet = new PlotSet(networkEvaluator.CorrectResponseArray, LineType.Straight, PlotPointType.None, Color.Black, Color.Black);
			networkResponsePlotSet = new PlotSet(networkEvaluator.NetworkResponseArray, LineType.Straight, PlotPointType.None, Color.Red, Color.Red);

			rudyGraphControl1.PrimaryYAxis.Bind(correctResponsePlotSet);
			rudyGraphControl1.PrimaryYAxis.Bind(networkResponsePlotSet);
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
			this.rudyGraphControl1 = new RudyGraph.RudyGraphControl();
			this.SuspendLayout();
			// 
			// rudyGraphControl1
			// 
			this.rudyGraphControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rudyGraphControl1.Location = new System.Drawing.Point(8, 8);
			this.rudyGraphControl1.Name = "rudyGraphControl1";
			this.rudyGraphControl1.Size = new System.Drawing.Size(456, 264);
			this.rudyGraphControl1.TabIndex = 0;
			// 
			// OneInputFunctionRegressionExperimentView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 278);
			this.Controls.Add(this.rudyGraphControl1);
			this.Name = "OneInputFunctionRegressionExperimentView";
			this.Text = "SharpNEAT [Function Regression Experiment View]";
			this.ResumeLayout(false);

		}
		#endregion

		#region AbstractExperimentView implementation

		public override void RefreshView(SharpNeatLib.NeuralNetwork.INetwork network)
		{
			networkEvaluator.EvaluateNetwork(network);
			networkResponsePlotSet.LoadValues(networkEvaluator.NetworkResponseArray);
		}

		#endregion
	}
}
