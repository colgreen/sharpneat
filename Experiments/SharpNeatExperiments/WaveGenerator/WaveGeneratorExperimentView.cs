using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RudyGraph;

namespace SharpNeatLib.Experiments
{

	public class WaveGeneratorExperimentView : AbstractExperimentView
	{
		#region Class Variables

		WaveGeneratorNetworkEvaluator networkEvaluator=null;
		PlotSet correctResponsePlotSet;
		PlotSet networkResponsePlotSet;
		double[] networkResponseArray;

		#endregion

		#region Form Designer variables

		private RudyGraph.RudyGraphControl rudyGraphControl1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public WaveGeneratorExperimentView(WaveGeneratorNetworkEvaluator networkEvaluator)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Keep a reference to the network evaluator. We need this to draw/refresh the graph.
			this.networkEvaluator = networkEvaluator;

			// Create some plot sets and bind them to the graph control.
			// Copy the correct response array to an array which is longer. This causes the correct responses to show at the same scale
			// as the network responses.
			double[] correctResponseArray = new double[networkEvaluator.ACTIVATIONS_EVAL*3];
			networkEvaluator.CorrectResponseArray.CopyTo(correctResponseArray, 0);
			correctResponsePlotSet = new PlotSet(correctResponseArray, LineType.Straight, PlotPointType.None, Color.Black, Color.Black);

			networkResponseArray = new double[networkEvaluator.ACTIVATIONS_EVAL*3];
			networkResponsePlotSet = new PlotSet(networkResponseArray, LineType.Straight, PlotPointType.None, Color.Red, Color.Red);

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
			this.Text = "SharpNEAT [Wave Generator Experiment View]";
			this.ResumeLayout(false);

		}
		#endregion

		#region AbstractExperimentView implementation

		public override void RefreshView(SharpNeatLib.NeuralNetwork.INetwork network)
		{
			// Fill our networkResponseArray with network responses.

			// Clear any existing network signals.
			network.ClearSignals();

			network.SetInputSignal(0, 1.0);

			// Activate a few times before we start taking readings.
			network.SingleStep();

			network.SetInputSignal(0, 0.0);

			// Loop over the sample points. There are more sample points than in the network evaluator class
			// so that we can see if the wavform is continous and stable.
			for(int i=0; i<networkResponseArray.Length; i++)
			{
				// Activate the network and take a reading.
				network.SingleStep();
				networkResponseArray[i] = network.GetOutputSignal(0);
			}
			
			networkResponsePlotSet.LoadValues(networkResponseArray);
		}

		#endregion
	}
}
