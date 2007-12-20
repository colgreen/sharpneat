using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;


using SharpNeatLib;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.Evolution;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

namespace PhysicalTSPSimulator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		INetwork network = null;
		Timer	timer=null;
		PhysicalTSPControllerNetworkEvaluator simulator;

		#region Windows designer variables

		private ptsp_entry.MonitorRouteViewport routeViewport1;
		private System.Windows.Forms.Button btnLoadGenome;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.TextBox txtTimestep;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtRouteIdx;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtVelocity1;
		private System.Windows.Forms.Button btnPlotPath;
		private System.Windows.Forms.Button btnPlotPathLimit;
		private System.Windows.Forms.Label label3;

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

			simulator = new PhysicalTSPControllerNetworkEvaluator();
			simulator.Initialise();
			routeViewport1.SetRoute(simulator.Route);
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
			this.routeViewport1 = new ptsp_entry.MonitorRouteViewport();
			this.btnLoadGenome = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.txtTimestep = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnPlotPath = new System.Windows.Forms.Button();
			this.btnPlotPathLimit = new System.Windows.Forms.Button();
			this.txtRouteIdx = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtVelocity1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// routeViewport1
			// 
			this.routeViewport1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.routeViewport1.Location = new System.Drawing.Point(8, 8);
			this.routeViewport1.Name = "routeViewport1";
			this.routeViewport1.Size = new System.Drawing.Size(696, 536);
			this.routeViewport1.TabIndex = 0;
			// 
			// btnLoadGenome
			// 
			this.btnLoadGenome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnLoadGenome.Location = new System.Drawing.Point(8, 552);
			this.btnLoadGenome.Name = "btnLoadGenome";
			this.btnLoadGenome.Size = new System.Drawing.Size(96, 24);
			this.btnLoadGenome.TabIndex = 1;
			this.btnLoadGenome.Text = "Load Genome";
			this.btnLoadGenome.Click += new System.EventHandler(this.btnLoadGenome_Click);
			// 
			// btnStart
			// 
			this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStart.Location = new System.Drawing.Point(112, 552);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(80, 24);
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStop.Location = new System.Drawing.Point(112, 576);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(80, 24);
			this.btnStop.TabIndex = 3;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// txtTimestep
			// 
			this.txtTimestep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtTimestep.Location = new System.Drawing.Point(632, 552);
			this.txtTimestep.Name = "txtTimestep";
			this.txtTimestep.ReadOnly = true;
			this.txtTimestep.Size = new System.Drawing.Size(72, 20);
			this.txtTimestep.TabIndex = 4;
			this.txtTimestep.Text = "";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(576, 552);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Timestep";
			// 
			// btnPlotPath
			// 
			this.btnPlotPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPlotPath.Location = new System.Drawing.Point(200, 552);
			this.btnPlotPath.Name = "btnPlotPath";
			this.btnPlotPath.Size = new System.Drawing.Size(168, 24);
			this.btnPlotPath.TabIndex = 6;
			this.btnPlotPath.Text = "Plot Path";
			this.btnPlotPath.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnPlotPathLimit
			// 
			this.btnPlotPathLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPlotPathLimit.Location = new System.Drawing.Point(200, 576);
			this.btnPlotPathLimit.Name = "btnPlotPathLimit";
			this.btnPlotPathLimit.Size = new System.Drawing.Size(168, 24);
			this.btnPlotPathLimit.TabIndex = 7;
			this.btnPlotPathLimit.Text = "Plot Path (w/ Stop Condition)";
			this.btnPlotPathLimit.Click += new System.EventHandler(this.button2_Click);
			// 
			// txtRouteIdx
			// 
			this.txtRouteIdx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtRouteIdx.Location = new System.Drawing.Point(480, 576);
			this.txtRouteIdx.Name = "txtRouteIdx";
			this.txtRouteIdx.ReadOnly = true;
			this.txtRouteIdx.Size = new System.Drawing.Size(32, 20);
			this.txtRouteIdx.TabIndex = 8;
			this.txtRouteIdx.Text = "30";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(576, 576);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 10;
			this.label2.Text = "Velocity";
			// 
			// txtVelocity1
			// 
			this.txtVelocity1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtVelocity1.Location = new System.Drawing.Point(632, 576);
			this.txtVelocity1.Name = "txtVelocity1";
			this.txtVelocity1.ReadOnly = true;
			this.txtVelocity1.Size = new System.Drawing.Size(72, 20);
			this.txtVelocity1.TabIndex = 11;
			this.txtVelocity1.Text = "";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(376, 576);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 12;
			this.label3.Text = "Waypoint Reached Index";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(712, 606);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtVelocity1);
			this.Controls.Add(this.txtRouteIdx);
			this.Controls.Add(this.txtTimestep);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnPlotPathLimit);
			this.Controls.Add(this.btnPlotPath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnLoadGenome);
			this.Controls.Add(this.routeViewport1);
			this.Name = "Form1";
			this.Text = "Physical Travelling Salesperson Simulator";
			this.ResumeLayout(false);

		}
		#endregion

		#region Main()

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		#endregion

		#region Event Handlers [Button Clicks]

		private void btnLoadGenome_Click(object sender, System.EventArgs e)
		{
			//----- Save the XmlDocument to the file syatem.
			OpenFileDialog oDialog = new OpenFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = "xml";
			oDialog.Title = "Load genome XML";
			oDialog.RestoreDirectory = true;

			// Show Open Dialog and Respond to OK
			if(oDialog.ShowDialog() != DialogResult.OK)
				return;

			//----- Load the file into an XmlDocument.
			XmlDocument doc = new XmlDocument();
			

			//----- Read the network structure from the XmlDocument.
			try
			{
				doc.Load(oDialog.FileName);
				NeatGenome genome = XmlNeatGenomeReaderStatic.Read(doc);
				network = GenomeDecoder.DecodeToConcurrentNetwork(genome, new SteepenedSigmoidApproximation());
				//network = GenomeDecoder.DecodeToFastConcurrentNetwork(genome, new SteepenedSigmoidApproximation());
			}

			catch(Exception ex)
			{
				MessageBox.Show(this, "Error loading network xml: \r\n" + ex.Message);
				network=null;
			}
		}

		private void btnStart_Click(object sender, System.EventArgs e)
		{
			simulator.Initialise();
			network.ClearSignals();
			routeViewport1.ResetPoints();
			if(timer==null)
			{
				timer = new Timer();
				timer.Tick += new EventHandler(OnTickEvent);
			}
			timer.Interval = 25;
			timer.Start();
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
			timer.Stop();
			timer = null;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			simulator.Initialise();
			network.ClearSignals();
			routeViewport1.ResetPoints();

			for(;;)
			{
				bool stop = simulator.PerformSingleStep(network);
				routeViewport1.AddPoint(routeViewport1.LogicalToViewport_Vector2d(simulator.Position));
				if(stop) break;
			}
			routeViewport1.Invalidate();

			txtTimestep.Text = simulator.Timestep.ToString();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			int max_route_idx = int.Parse(txtRouteIdx.Text);
			simulator.Initialise();
			network.ClearSignals();
			routeViewport1.ResetPoints();

			for(;;)
			{
				bool stop = simulator.PerformSingleStep(network);
				routeViewport1.AddPoint(routeViewport1.LogicalToViewport_Vector2d(simulator.Position));
				if(stop || simulator.RouteIdx == max_route_idx)
					break;

			}
			routeViewport1.Invalidate();

			txtTimestep.Text = simulator.Timestep.ToString();
		}

		#endregion

		#region Event Handlers [Other]

		private void OnTickEvent(Object myObject, EventArgs myEventArgs) 
		{
			bool stop = simulator.PerformSingleStep(network);
			routeViewport1.AddPoint(routeViewport1.LogicalToViewport_Vector2d(simulator.Position));
			//routeViewport1.Refresh();
			routeViewport1.Invalidate();

			txtTimestep.Text = simulator.Timestep.ToString();
			txtVelocity1.Text = simulator.Velocity.x.ToString() + "," + simulator.Velocity.y.ToString();

			if(stop)
			{
				timer.Stop();
			}


			//PeformSingleCycle();
		}

		#endregion






	}
}
