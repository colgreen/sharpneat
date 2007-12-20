using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using SharpNeatLib;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NetworkVisualization;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeat
{
	public class BestGenomeForm : System.Windows.Forms.Form
	{
		NeatGenome bestGenome;
		NetworkControl nc;
		GridLayoutManager layoutManager = new GridLayoutManager();

		int updateMilliseconds=2000;
		uint updateGenerations=1;

		#region Windows FormDesigner variables

		private System.Windows.Forms.Panel pnlNetwork;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public BestGenomeForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Create NetworkControl.
			nc = new NetworkControl();
			nc.Dock = DockStyle.Fill;
			pnlNetwork.Controls.Add(nc);
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
			this.pnlNetwork = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// pnlNetwork
			// 
			this.pnlNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlNetwork.Location = new System.Drawing.Point(0, 0);
			this.pnlNetwork.Name = "pnlNetwork";
			this.pnlNetwork.Size = new System.Drawing.Size(624, 560);
			this.pnlNetwork.TabIndex = 0;
			// 
			// BestGenomeForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(624, 566);
			this.Controls.Add(this.pnlNetwork);
			this.Name = "BestGenomeForm";
			this.Text = "SharpNEAT [Best Genome]";
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties

		int lastUpdateTickCount=0;
		uint lastUpdateGeneration=0;
		public void SetBestGenome(NeatGenome genome, uint generation)
		{
			bestGenome = genome;

			// Activation function isn;t important here. Use of PlainSigmoid is arbitrary.
			//NetworkModel nm = new NetworkModel((ConcurrentNetwork)bestGenome.Decode(activationFn));
			//NetworkModel nm = GenomeDecoder.DecodeToNetworkModel((ConcurrentNetwork)bestGenome.Decode(activationFn));
			NetworkModel nm = GenomeDecoder.DecodeToNetworkModel(bestGenome);

			layoutManager.Layout(nm, nc.Size);
			nc.NetworkModel = nm;

			lastUpdateTickCount = Environment.TickCount;
			lastUpdateGeneration = generation;
		}

		#endregion

		#region Public Methods

		public void Reset()
		{
			bestGenome = null;
		}

		#endregion


	}
}
