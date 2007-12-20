using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.IO;

using SharpNeatLib;
using SharpNeatLib.Experiments;
using SharpNeatLib.Xml;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;

namespace SharpNeat.PoleBalancingSimulator
{
	public class Form1 : System.Windows.Forms.Form
	{
		Timer	timer=null;
		
		INetwork network = null;
		ISimulator simulator;

		#region Windows Form Designer Variables

		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.GroupBox gbxManual;
		private System.Windows.Forms.Button btnStep;
		private System.Windows.Forms.GroupBox gbxAutomatic;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.GroupBox gbxStatus;
		private System.Windows.Forms.TrackBar tbrSpeed;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkContinuous;
		private System.Windows.Forms.CheckBox chkDelay;
		private System.Windows.Forms.Button btnInitialiseRnd;
		private System.Windows.Forms.Panel pnlPoleBalancing;
		private System.Windows.Forms.TextBox txtStatusCartX;
		private System.Windows.Forms.TextBox txtStatusCartVelocity;
		private System.Windows.Forms.TextBox txtStatusRealtime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cmbSimulationType;
		private System.Windows.Forms.Button btnLoadGenome;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public Form1()
		{
			InitializeComponent();
			PopulateSimulationTypeCombo();
			cmbSimulationType.SelectedIndex=0;
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
			this.btnLoad = new System.Windows.Forms.Button();
			this.gbxManual = new System.Windows.Forms.GroupBox();
			this.btnStep = new System.Windows.Forms.Button();
			this.btnInitialiseRnd = new System.Windows.Forms.Button();
			this.gbxAutomatic = new System.Windows.Forms.GroupBox();
			this.chkDelay = new System.Windows.Forms.CheckBox();
			this.chkContinuous = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbrSpeed = new System.Windows.Forms.TrackBar();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnGo = new System.Windows.Forms.Button();
			this.gbxStatus = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtStatusRealtime = new System.Windows.Forms.TextBox();
			this.txtStatusCartVelocity = new System.Windows.Forms.TextBox();
			this.txtStatusCartX = new System.Windows.Forms.TextBox();
			this.pnlPoleBalancing = new System.Windows.Forms.Panel();
			this.cmbSimulationType = new System.Windows.Forms.ComboBox();
			this.btnLoadGenome = new System.Windows.Forms.Button();
			this.gbxManual.SuspendLayout();
			this.gbxAutomatic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbrSpeed)).BeginInit();
			this.gbxStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnLoad.Location = new System.Drawing.Point(160, 316);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(96, 18);
			this.btnLoad.TabIndex = 0;
			this.btnLoad.Text = "Load Network";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// gbxManual
			// 
			this.gbxManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.gbxManual.Controls.Add(this.btnStep);
			this.gbxManual.Controls.Add(this.btnInitialiseRnd);
			this.gbxManual.Location = new System.Drawing.Point(8, 352);
			this.gbxManual.Name = "gbxManual";
			this.gbxManual.Size = new System.Drawing.Size(248, 48);
			this.gbxManual.TabIndex = 2;
			this.gbxManual.TabStop = false;
			this.gbxManual.Text = "Manual Control";
			// 
			// btnStep
			// 
			this.btnStep.Enabled = false;
			this.btnStep.Location = new System.Drawing.Point(128, 16);
			this.btnStep.Name = "btnStep";
			this.btnStep.Size = new System.Drawing.Size(112, 24);
			this.btnStep.TabIndex = 1;
			this.btnStep.Text = "Step";
			this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
			// 
			// btnInitialiseRnd
			// 
			this.btnInitialiseRnd.Enabled = false;
			this.btnInitialiseRnd.Location = new System.Drawing.Point(8, 16);
			this.btnInitialiseRnd.Name = "btnInitialiseRnd";
			this.btnInitialiseRnd.Size = new System.Drawing.Size(112, 24);
			this.btnInitialiseRnd.TabIndex = 0;
			this.btnInitialiseRnd.Text = "Initialise [Random]";
			this.btnInitialiseRnd.Click += new System.EventHandler(this.btnInitialiseRnd_Click);
			// 
			// gbxAutomatic
			// 
			this.gbxAutomatic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.gbxAutomatic.Controls.Add(this.chkDelay);
			this.gbxAutomatic.Controls.Add(this.chkContinuous);
			this.gbxAutomatic.Controls.Add(this.label2);
			this.gbxAutomatic.Controls.Add(this.tbrSpeed);
			this.gbxAutomatic.Controls.Add(this.btnStop);
			this.gbxAutomatic.Controls.Add(this.btnGo);
			this.gbxAutomatic.Location = new System.Drawing.Point(264, 312);
			this.gbxAutomatic.Name = "gbxAutomatic";
			this.gbxAutomatic.Size = new System.Drawing.Size(304, 88);
			this.gbxAutomatic.TabIndex = 3;
			this.gbxAutomatic.TabStop = false;
			this.gbxAutomatic.Text = "Automatic Control";
			// 
			// chkDelay
			// 
			this.chkDelay.Checked = true;
			this.chkDelay.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDelay.Location = new System.Drawing.Point(8, 64);
			this.chkDelay.Name = "chkDelay";
			this.chkDelay.Size = new System.Drawing.Size(136, 16);
			this.chkDelay.TabIndex = 9;
			this.chkDelay.Text = "Delay Between Trials";
			// 
			// chkContinuous
			// 
			this.chkContinuous.Checked = true;
			this.chkContinuous.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkContinuous.Location = new System.Drawing.Point(8, 48);
			this.chkContinuous.Name = "chkContinuous";
			this.chkContinuous.Size = new System.Drawing.Size(136, 16);
			this.chkContinuous.TabIndex = 8;
			this.chkContinuous.Text = "Continuous Trials";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(160, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Simulation Speed";
			// 
			// tbrSpeed
			// 
			this.tbrSpeed.Location = new System.Drawing.Point(160, 32);
			this.tbrSpeed.Maximum = 50;
			this.tbrSpeed.Minimum = 1;
			this.tbrSpeed.Name = "tbrSpeed";
			this.tbrSpeed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbrSpeed.Size = new System.Drawing.Size(136, 45);
			this.tbrSpeed.SmallChange = 5;
			this.tbrSpeed.TabIndex = 6;
			this.tbrSpeed.TickFrequency = 5;
			this.tbrSpeed.Value = 10;
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(80, 16);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(64, 24);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnGo
			// 
			this.btnGo.Enabled = false;
			this.btnGo.Location = new System.Drawing.Point(8, 16);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(64, 24);
			this.btnGo.TabIndex = 0;
			this.btnGo.Text = "Go";
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// gbxStatus
			// 
			this.gbxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.gbxStatus.Controls.Add(this.label6);
			this.gbxStatus.Controls.Add(this.label3);
			this.gbxStatus.Controls.Add(this.label1);
			this.gbxStatus.Controls.Add(this.txtStatusRealtime);
			this.gbxStatus.Controls.Add(this.txtStatusCartVelocity);
			this.gbxStatus.Controls.Add(this.txtStatusCartX);
			this.gbxStatus.Location = new System.Drawing.Point(576, 312);
			this.gbxStatus.Name = "gbxStatus";
			this.gbxStatus.Size = new System.Drawing.Size(176, 88);
			this.gbxStatus.TabIndex = 4;
			this.gbxStatus.TabStop = false;
			this.gbxStatus.Text = "Status";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(56, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 16);
			this.label6.TabIndex = 9;
			this.label6.Text = "Realtime (secs)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(56, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Cart Velocity (m/s)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(56, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Cart X (meters)";
			// 
			// txtStatusRealtime
			// 
			this.txtStatusRealtime.Location = new System.Drawing.Point(8, 16);
			this.txtStatusRealtime.Name = "txtStatusRealtime";
			this.txtStatusRealtime.Size = new System.Drawing.Size(48, 20);
			this.txtStatusRealtime.TabIndex = 4;
			this.txtStatusRealtime.Text = "";
			// 
			// txtStatusCartVelocity
			// 
			this.txtStatusCartVelocity.Location = new System.Drawing.Point(8, 64);
			this.txtStatusCartVelocity.Name = "txtStatusCartVelocity";
			this.txtStatusCartVelocity.Size = new System.Drawing.Size(48, 20);
			this.txtStatusCartVelocity.TabIndex = 1;
			this.txtStatusCartVelocity.Text = "";
			// 
			// txtStatusCartX
			// 
			this.txtStatusCartX.Location = new System.Drawing.Point(8, 40);
			this.txtStatusCartX.Name = "txtStatusCartX";
			this.txtStatusCartX.Size = new System.Drawing.Size(48, 20);
			this.txtStatusCartX.TabIndex = 0;
			this.txtStatusCartX.Text = "";
			// 
			// pnlPoleBalancing
			// 
			this.pnlPoleBalancing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlPoleBalancing.Location = new System.Drawing.Point(8, 8);
			this.pnlPoleBalancing.Name = "pnlPoleBalancing";
			this.pnlPoleBalancing.Size = new System.Drawing.Size(744, 304);
			this.pnlPoleBalancing.TabIndex = 5;
			// 
			// cmbSimulationType
			// 
			this.cmbSimulationType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmbSimulationType.Location = new System.Drawing.Point(8, 320);
			this.cmbSimulationType.Name = "cmbSimulationType";
			this.cmbSimulationType.Size = new System.Drawing.Size(152, 21);
			this.cmbSimulationType.TabIndex = 6;
			this.cmbSimulationType.SelectedIndexChanged += new System.EventHandler(this.cmbSimulationType_SelectedIndexChanged);
			// 
			// btnLoadGenome
			// 
			this.btnLoadGenome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnLoadGenome.Location = new System.Drawing.Point(160, 336);
			this.btnLoadGenome.Name = "btnLoadGenome";
			this.btnLoadGenome.Size = new System.Drawing.Size(96, 18);
			this.btnLoadGenome.TabIndex = 7;
			this.btnLoadGenome.Text = "Load Genome";
			this.btnLoadGenome.Click += new System.EventHandler(this.btnLoadGenome_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(760, 406);
			this.Controls.Add(this.btnLoadGenome);
			this.Controls.Add(this.cmbSimulationType);
			this.Controls.Add(this.pnlPoleBalancing);
			this.Controls.Add(this.gbxStatus);
			this.Controls.Add(this.gbxManual);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.gbxAutomatic);
			this.Name = "Form1";
			this.Text = "SharpNEAT Pole Balancing Simulator";
			this.gbxManual.ResumeLayout(false);
			this.gbxAutomatic.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tbrSpeed)).EndInit();
			this.gbxStatus.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Main

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		#endregion

		#region Event Handlers

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			//----- Save the XmlDocument to the file syatem.
			OpenFileDialog oDialog = new OpenFileDialog();
			oDialog.AddExtension = true;
			oDialog.DefaultExt = "xml";
			oDialog.Title = "Load network XML";
			oDialog.RestoreDirectory = true;

			// Show Open Dialog and Respond to OK
			if(oDialog.ShowDialog() != DialogResult.OK)
				return;

			//----- Load the file into an XmlDocument.
			XmlDocument doc = new XmlDocument();
			

			//----- Read the network structure from the XmlDocument.
			//XmlNetworkReader nr = new XmlNetworkReader();
			try
			{
				doc.Load(oDialog.FileName);
				network = XmlNetworkReaderStatic.Read(doc);
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Error loading network xml: \r\n" + ex.Message);
				network=null;
			}

			//-----
			SetAccess();
		}

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
				network = GenomeDecoder.DecodeToFloatFastConcurrentNetwork(genome, new SteepenedSigmoidApproximation());
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Error loading network xml: \r\n" + ex.Message);
				network=null;
			}

			//-----
			SetAccess();
		

		}

		private void btnInitialiseRnd_Click(object sender, System.EventArgs e)
		{
			InitialiseTrial_Random();
		}

		private void btnStep_Click(object sender, System.EventArgs e)
		{
			PeformSingleCycle();
		}

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			if(!IsNetworkCompatibleWithSimulator())
			{
				MessageBox.Show(this, "The loaded network is not compatible with the selected simulation  :" + 
										"\rExpected simulation inputs=" + simulator.InputNeuronCount + ", outputs=" + simulator.OutputNeuronCount +
										"\rNetwork inputs=" + network.InputNeuronCount + ", outputs=" + network.OutputNeuronCount);
										
				return;
			}

			SetAccess();
			InitialiseTrial_Random();
			network.ClearSignals();
			if(timer==null)
			{
				timer = new Timer();
				timer.Tick += new EventHandler(OnTickEvent);
			}
			SetAccess();
			timer.Interval = tbrSpeed.Value;
			timer.Start();
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
			timer.Stop();
			timer = null;
			SetAccess();
		}

		private void OnTickEvent(Object myObject, EventArgs myEventArgs) 
		{
			timer.Interval = tbrSpeed.Value;
			if(PeformSingleCycle())
			{
				if(chkContinuous.Checked)
				{
					timer.Stop();
					if(chkDelay.Checked)
					{	
						Application.DoEvents();
						System.Threading.Thread.Sleep(1000);
					}
					InitialiseTrial_Random();
					network.ClearSignals();
					timer.Start();
				}
				else
				{
					timer.Stop();
					timer = null;
					SetAccess();
				}
			}
		}

		private void cmbSimulationType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Control poleBalancingControl;

			ListItem listItem = (ListItem)cmbSimulationType.SelectedItem;
			int type = (int)listItem.Data;
			switch(type)
			{
				case 0:
				{					
					if(pnlPoleBalancing.Controls.Count>0)
						pnlPoleBalancing.Controls.Remove(pnlPoleBalancing.Controls[0]);

					SinglePoleBalancingNetworkEvaluator tmp1 = new SinglePoleBalancingNetworkEvaluator();
					simulator = tmp1;

					poleBalancingControl = new SinglePoleBalancingControl(tmp1);
					poleBalancingControl.Dock = DockStyle.Fill;
					pnlPoleBalancing.Controls.Add(poleBalancingControl);
					break;
				}
				case 1:
				{
					if(pnlPoleBalancing.Controls.Count>0)
						pnlPoleBalancing.Controls.Remove(pnlPoleBalancing.Controls[0]);

					DoublePoleBalancingNetworkEvaluator tmp2 = new DoublePoleBalancingNetworkEvaluator();
					simulator = tmp2;

					poleBalancingControl = new DoublePoleBalancingControl(tmp2);
					poleBalancingControl.Dock = DockStyle.Fill;
					pnlPoleBalancing.Controls.Add(poleBalancingControl);
					break;
				}
				case 2:
				{
					if(pnlPoleBalancing.Controls.Count>0)
						pnlPoleBalancing.Controls.Remove(pnlPoleBalancing.Controls[0]);

					DoublePoleBalancingNetworkEvaluator tmp3 = new NvDoublePoleBalancingNetworkEvaluator();
					simulator = tmp3;

					poleBalancingControl = new DoublePoleBalancingControl(tmp3);
					poleBalancingControl.Dock = DockStyle.Fill;
					pnlPoleBalancing.Controls.Add(poleBalancingControl);
					break;
				}
			}
		}

		#endregion

		#region Private Methods

		private void PopulateSimulationTypeCombo()
		{
			cmbSimulationType.Items.Add(new ListItem("", "Single Pole", 0));
			cmbSimulationType.Items.Add(new ListItem("", "Double Pole", 1));
			cmbSimulationType.Items.Add(new ListItem("", "Double Pole[No Velocity]", 2));
		}

		private void InitialiseTrial_Random()
		{
			simulator.Initialise_Random();
			pnlPoleBalancing.Refresh();
			network.ClearSignals();
			RefreshStats();
		}

		private void RefreshStats()
		{
			if(simulator is SinglePoleBalancingNetworkEvaluator)
			{
				SinglePoleBalancingNetworkEvaluator spbe = (SinglePoleBalancingNetworkEvaluator)simulator;
				txtStatusRealtime.Text = ((double)(SinglePoleBalancingNetworkEvaluator.TIME_DELTA * spbe.CurrentTimestep)).ToString("0.00");
				txtStatusCartX.Text = spbe.CartPosX.ToString("0.00");
				txtStatusCartVelocity.Text = spbe.CartVelocityX.ToString("0.00");
			}
			else if(simulator is DoublePoleBalancingNetworkEvaluator)
			{
				DoublePoleBalancingNetworkEvaluator dpbe = (DoublePoleBalancingNetworkEvaluator)simulator;
				txtStatusRealtime.Text = ((double)(DoublePoleBalancingNetworkEvaluator.TIME_DELTA * dpbe.CurrentTimestep)).ToString("0.00");
				txtStatusCartX.Text = dpbe.CartPosX.ToString("0.00");
				txtStatusCartVelocity.Text = dpbe.CartVelocityX.ToString("0.00");
			}
//			txtStatusRealtime.Text = ((double)(SinglePoleBalancingEvaluator.TIME_DELTA * singlePoleBalancing.CurrentTimestep)).ToString("0.0");
//			txtStatusCartX.Text = singlePoleBalancing.CartPosX.ToString("0.00");
//			txtStatusCartVelocity.Text = singlePoleBalancing.CartVelocityX.ToString("0.00");
//			txtStatusPoleAngle.Text = ((double)((singlePoleBalancing.PoleAngle / (Math.PI*2.0)) * 360.0)).ToString("000");
//			txtStatusPoleVelocity.Text = ((double)((singlePoleBalancing.PoleAngularVelocity / (Math.PI*2.0)) * 360.0)).ToString("000");
		}

		private void SetAccess()
		{
			if(timer==null)
			{
				btnLoad.Enabled = true;
				btnStop.Enabled = false;

				if(network==null)
				{
					btnInitialiseRnd.Enabled = false;
					btnStep.Enabled = false;
					btnGo.Enabled = false;
				}
				else
				{
					btnInitialiseRnd.Enabled = true;
					btnStep.Enabled = true;
					btnGo.Enabled = true;
				}
			}
			else
			{
				btnLoad.Enabled = false;
				btnInitialiseRnd.Enabled = false;
				btnStep.Enabled = false;
				btnGo.Enabled = false;
				btnStop.Enabled = true;
			}
		}

		private bool PeformSingleCycle()
		{
			bool failed = simulator.PerformSingleStep(network);
			this.pnlPoleBalancing.Refresh();
			RefreshStats();

			return failed;
		}


		private bool IsNetworkCompatibleWithSimulator()
		{
			return (simulator.InputNeuronCount == network.InputNeuronCount) &&
					(simulator.OutputNeuronCount == network.OutputNeuronCount);
		}

		#endregion






	}
}
