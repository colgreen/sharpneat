using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.NetworkVisualization;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;

namespace SharpNeat
{
	public class SpeciesMonitorControl : System.Windows.Forms.UserControl
	{
		NetworkControl nc;
		Species species;
		GridLayoutManager layoutManager = new GridLayoutManager();

		#region Event Declarations

		public event SpeciesSelectionHandler DoubleClick;

		#endregion

		#region Component Designer variables

		private System.Windows.Forms.Label lblSpeciesId;
		private System.Windows.Forms.Panel pnlNetwork;
		private System.Windows.Forms.Label lblDead;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public SpeciesMonitorControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Create NetworkControl.
			nc = new NetworkControl();
			nc.Dock = DockStyle.Fill;
			nc.AutoScroll = false;
			pnlNetwork.Controls.Add(nc);
			nc.DoubleClick += new EventHandler(nc_DoubleClick);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlNetwork = new System.Windows.Forms.Panel();
			this.lblDead = new System.Windows.Forms.Label();
			this.lblSpeciesId = new System.Windows.Forms.Label();
			this.pnlNetwork.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlNetwork
			// 
			this.pnlNetwork.BackColor = System.Drawing.Color.Cornsilk;
			this.pnlNetwork.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlNetwork.Controls.Add(this.lblDead);
			this.pnlNetwork.Controls.Add(this.lblSpeciesId);
			this.pnlNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlNetwork.Location = new System.Drawing.Point(0, 0);
			this.pnlNetwork.Name = "pnlNetwork";
			this.pnlNetwork.Size = new System.Drawing.Size(296, 296);
			this.pnlNetwork.TabIndex = 0;
			// 
			// lblDead
			// 
			this.lblDead.BackColor = System.Drawing.Color.Cornsilk;
			this.lblDead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDead.ForeColor = System.Drawing.Color.Red;
			this.lblDead.Location = new System.Drawing.Point(0, 0);
			this.lblDead.Name = "lblDead";
			this.lblDead.Size = new System.Drawing.Size(112, 16);
			this.lblDead.TabIndex = 1;
			this.lblDead.Text = "DEAD SPECIES";
			this.lblDead.Visible = false;
			// 
			// lblSpeciesId
			// 
			this.lblSpeciesId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSpeciesId.BackColor = System.Drawing.Color.Cornsilk;
			this.lblSpeciesId.Location = new System.Drawing.Point(2, 278);
			this.lblSpeciesId.Name = "lblSpeciesId";
			this.lblSpeciesId.Size = new System.Drawing.Size(278, 16);
			this.lblSpeciesId.TabIndex = 0;
			this.lblSpeciesId.Text = "speciesId()";
			// 
			// SpeciesMonitorControl
			// 
			this.Controls.Add(this.pnlNetwork);
			this.Name = "SpeciesMonitorControl";
			this.Size = new System.Drawing.Size(296, 296);
			this.pnlNetwork.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties

		public int SpeciesId
		{
			get
			{
				return species.SpeciesId;
			}
		}

		public bool Dead
		{
			get
			{
				return lblDead.Visible;
			}
			set
			{
				lblDead.Visible = value;
			}
		}

		#endregion

		#region Public Methods

		public void SetSpecies(Species species)
		{
			this.species = species;
			UpdateControl();
//			// Member 0 is the fittest genome in this species.
//			NetworkModel nm = new NetworkModel((RealtimeNetwork)species.Members[0].Decode(activationFn));
//			layoutManager.Layout(nm, nc.Size);
//			nc.NetworkModel = nm;
//
//			lblSpeciesId.Text = "species(" + species.SpeciesId + ") bestfitness(" + genome.Fitness.ToString("0.00") + ") age(" + species.SpeciesAge.ToString() + ')';

		}

		//TODO: investigate if this should bean override!
		public void UpdateControl()
		{
			NeatGenome genome = (NeatGenome)species.Members[0];
			//NetworkModel nm = new NetworkModel((ConcurrentNetwork)genome.Decode(activationFn));
			//NetworkModel nm = GenomeDecoder.DecodeToNetworkModel((ConcurrentNetwork)genome.Decode(activationFn));
			NetworkModel nm = GenomeDecoder.DecodeToNetworkModel(genome);

			layoutManager.Layout(nm, nc.Size);
			nc.NetworkModel = nm;
		
			lblSpeciesId.Text = "species(" + species.SpeciesId + ") bestfitness(" + genome.Fitness.ToString("0.00") + ") age(" + species.SpeciesAge.ToString() + ')';
		}

		#endregion

		#region Event Handlers

		private void nc_DoubleClick(object sender, EventArgs e)
		{
			if(DoubleClick!=null)
			{
				DoubleClick(this, new SpeciesSelectionEventArgs(species.SpeciesId));
			}	
		}

		#endregion
	}

	#region SpeciesSelectionEventArgs

	public delegate void SpeciesSelectionHandler(object sender, SpeciesSelectionEventArgs e);
	public class SpeciesSelectionEventArgs : EventArgs
	{
		int speciesId;

		public SpeciesSelectionEventArgs(int speciesId)
		{
			this.speciesId = speciesId;
		}

		public int SpeciesId
		{
			get
			{
				return speciesId;
			}
		}
	}

	#endregion
}
