using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments.Views
{
	/// <summary>
	/// Summary description for BinarySixMultiplexerExperimentView.
	/// </summary>
	public class BinarySixMultiplexerExperimentView : AbstractExperimentView //System.Windows.Forms.Form
	{
		#region Form Designer variables

		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public BinarySixMultiplexerExperimentView()
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
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtOutput
			// 
			this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutput.Location = new System.Drawing.Point(8, 8);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.Size = new System.Drawing.Size(408, 352);
			this.txtOutput.TabIndex = 0;
			this.txtOutput.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(344, 368);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(72, 24);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// BinarySixMultiplexerExperimentView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 398);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtOutput);
			this.Name = "BinarySixMultiplexerExperimentView";
			this.Text = "Binary 6-Multiplexer Experiment View";
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Override Methods

		public override void RefreshView(INetwork network)
		{
			EvaluateNetwork(network);
		}

		#endregion

		#region Private Methods

		private void EvaluateNetwork(INetwork network)
		{
			StringBuilder sb = new StringBuilder();

			// 64 test cases.
			for(int i=0; i<64; i++)
			{
				// Apply bitmask to i and shift left to generate the input signals.
				// In addition we scale 0->1 to be 0.1->0.9.
				// Note. We can eliminate all the maths by pre-building a table of test signals. 
				// Using this approach instead makes the code easier to scale up to the 11 and 20 
				// multiplexer problems.
				int tmp=i;
				for(int j=0; j<6; j++) 
				{
					network.SetInputSignal(j, ((tmp&0x1)*0.8)+0.1);
					tmp >>= 1;
				}
								
				// Activate the network.
//				network.RelaxNetwork(10, 0.005);
				network.MultipleSteps(3);
			
				// Get network's answer.
				double output = network.GetOutputSignal(0);
				BuildSixMultiplexerTestCaseString(sb, i, output);
				network.ClearSignals();
			}
			txtOutput.Text = sb.ToString();
		}

		private void BuildSixMultiplexerTestCaseString(StringBuilder sb, int testCase, double networkResponse)
		{
			// Address lines (bits 0 and 1).
			int tmp=testCase;
			for(int j=0; j<2; j++) 
			{
				sb.Append((tmp&0x1).ToString());
				tmp >>=1;
			}

			// Data Lines.
			sb.Append("    ");
			for(int j=0; j<4; j++) 
			{
				sb.Append((tmp&0x1).ToString());
				tmp >>=1;
			}

			// Correct answer.
			bool correctAnswer = ((1<<(2+(testCase&0x3)))&testCase)!=0;
			sb.Append("    ");
			sb.Append(correctAnswer?'1':'0');

			// Network response.
			sb.Append("    ");
			sb.Append(networkResponse.ToString("0.00"));
			sb.Append(!(correctAnswer ^ (networkResponse>=0.5))?" CORRECT":" WRONG");
			sb.Append("\r\n");
		}

		#endregion

		#region Event Handlers

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		#endregion
	}
}
