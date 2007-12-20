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

namespace SharpNeat.PreyCaptureSimulator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		Timer	timer=null;
		int		initialMoves=4;
		double	preySpeed=0.3;
		double	sensorRange=4.5;
		INetwork		network = null;
		PreyCaptureBase	preyCapture;

		#region Windows Form Designer Variables

		private SharpNeat.PreyCaptureSimulator.PreyCaptureGrid preyCaptureGrid1;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.GroupBox gbxManual;
		private System.Windows.Forms.Button btnStep;
		private System.Windows.Forms.GroupBox gbxAutomatic;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.GroupBox gbxStatus;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPreyCoords;
		private System.Windows.Forms.TextBox txtAgentCoords;
		private System.Windows.Forms.TrackBar tbrSpeed;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkContinuous;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPreySpeed;
		private System.Windows.Forms.TextBox txtInitialMoves;
		private System.Windows.Forms.CheckBox chkDelay;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtSensorRange;
		private System.Windows.Forms.Button btnInitialiseDet;
		private System.Windows.Forms.Button btnInitialiseRnd;
		private System.Windows.Forms.Button btnLoadGenome;
		private System.Windows.Forms.Button btnInitialiseUser;
		private System.Windows.Forms.TextBox txtAgentX;
		private System.Windows.Forms.TextBox txtAgentY;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtPreyY;
		private System.Windows.Forms.TextBox txtPreyX;
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

			txtInitialMoves.Text = initialMoves.ToString();
			txtPreySpeed.Text = preySpeed.ToString();
			txtSensorRange.Text = sensorRange.ToString();
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
			this.btnInitialiseDet = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.txtSensorRange = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtPreySpeed = new System.Windows.Forms.TextBox();
			this.txtInitialMoves = new System.Windows.Forms.TextBox();
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
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPreyCoords = new System.Windows.Forms.TextBox();
			this.txtAgentCoords = new System.Windows.Forms.TextBox();
			this.preyCaptureGrid1 = new SharpNeat.PreyCaptureSimulator.PreyCaptureGrid();
			this.btnLoadGenome = new System.Windows.Forms.Button();
			this.btnInitialiseUser = new System.Windows.Forms.Button();
			this.txtAgentX = new System.Windows.Forms.TextBox();
			this.txtAgentY = new System.Windows.Forms.TextBox();
			this.txtPreyY = new System.Windows.Forms.TextBox();
			this.txtPreyX = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.gbxManual.SuspendLayout();
			this.gbxAutomatic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbrSpeed)).BeginInit();
			this.gbxStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(8, 352);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(168, 24);
			this.btnLoad.TabIndex = 0;
			this.btnLoad.Text = "Load Network";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// gbxManual
			// 
			this.gbxManual.Controls.Add(this.txtAgentX);
			this.gbxManual.Controls.Add(this.label8);
			this.gbxManual.Controls.Add(this.txtPreyX);
			this.gbxManual.Controls.Add(this.label7);
			this.gbxManual.Controls.Add(this.txtPreyY);
			this.gbxManual.Controls.Add(this.txtAgentY);
			this.gbxManual.Controls.Add(this.btnInitialiseUser);
			this.gbxManual.Controls.Add(this.btnInitialiseDet);
			this.gbxManual.Controls.Add(this.label6);
			this.gbxManual.Controls.Add(this.txtSensorRange);
			this.gbxManual.Controls.Add(this.label3);
			this.gbxManual.Controls.Add(this.label5);
			this.gbxManual.Controls.Add(this.txtPreySpeed);
			this.gbxManual.Controls.Add(this.txtInitialMoves);
			this.gbxManual.Controls.Add(this.btnStep);
			this.gbxManual.Controls.Add(this.btnInitialiseRnd);
			this.gbxManual.Location = new System.Drawing.Point(168, 384);
			this.gbxManual.Name = "gbxManual";
			this.gbxManual.Size = new System.Drawing.Size(176, 232);
			this.gbxManual.TabIndex = 2;
			this.gbxManual.TabStop = false;
			this.gbxManual.Text = "Manual Control";
			// 
			// btnInitialiseDet
			// 
			this.btnInitialiseDet.Enabled = false;
			this.btnInitialiseDet.Location = new System.Drawing.Point(8, 40);
			this.btnInitialiseDet.Name = "btnInitialiseDet";
			this.btnInitialiseDet.Size = new System.Drawing.Size(160, 24);
			this.btnInitialiseDet.TabIndex = 18;
			this.btnInitialiseDet.Text = "Initialise [Deterministic]";
			this.btnInitialiseDet.Click += new System.EventHandler(this.btnInitialiseDet_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(72, 144);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 16);
			this.label6.TabIndex = 17;
			this.label6.Text = "Sensor Range";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtSensorRange
			// 
			this.txtSensorRange.Location = new System.Drawing.Point(8, 144);
			this.txtSensorRange.Name = "txtSensorRange";
			this.txtSensorRange.Size = new System.Drawing.Size(64, 20);
			this.txtSensorRange.TabIndex = 16;
			this.txtSensorRange.Text = "4.4";
			this.txtSensorRange.Validating += new System.ComponentModel.CancelEventHandler(this.txtSensorRange_Validating);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(72, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 15;
			this.label3.Text = "Prey Speed";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(72, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 16);
			this.label5.TabIndex = 14;
			this.label5.Text = "Initial Moves";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtPreySpeed
			// 
			this.txtPreySpeed.Location = new System.Drawing.Point(8, 120);
			this.txtPreySpeed.Name = "txtPreySpeed";
			this.txtPreySpeed.Size = new System.Drawing.Size(64, 20);
			this.txtPreySpeed.TabIndex = 13;
			this.txtPreySpeed.Text = "0.3";
			this.txtPreySpeed.Validating += new System.ComponentModel.CancelEventHandler(this.txtPreySpeed_Validating);
			// 
			// txtInitialMoves
			// 
			this.txtInitialMoves.Location = new System.Drawing.Point(8, 96);
			this.txtInitialMoves.Name = "txtInitialMoves";
			this.txtInitialMoves.Size = new System.Drawing.Size(64, 20);
			this.txtInitialMoves.TabIndex = 12;
			this.txtInitialMoves.Text = "4";
			this.txtInitialMoves.Validating += new System.ComponentModel.CancelEventHandler(this.txtInitialMoves_Validating);
			// 
			// btnStep
			// 
			this.btnStep.Enabled = false;
			this.btnStep.Location = new System.Drawing.Point(8, 64);
			this.btnStep.Name = "btnStep";
			this.btnStep.Size = new System.Drawing.Size(160, 24);
			this.btnStep.TabIndex = 1;
			this.btnStep.Text = "Step";
			this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
			// 
			// btnInitialiseRnd
			// 
			this.btnInitialiseRnd.Enabled = false;
			this.btnInitialiseRnd.Location = new System.Drawing.Point(8, 16);
			this.btnInitialiseRnd.Name = "btnInitialiseRnd";
			this.btnInitialiseRnd.Size = new System.Drawing.Size(160, 24);
			this.btnInitialiseRnd.TabIndex = 0;
			this.btnInitialiseRnd.Text = "Initialise [Random]";
			this.btnInitialiseRnd.Click += new System.EventHandler(this.btnInitialiseRnd_Click);
			// 
			// gbxAutomatic
			// 
			this.gbxAutomatic.Controls.Add(this.chkDelay);
			this.gbxAutomatic.Controls.Add(this.chkContinuous);
			this.gbxAutomatic.Controls.Add(this.label2);
			this.gbxAutomatic.Controls.Add(this.tbrSpeed);
			this.gbxAutomatic.Controls.Add(this.btnStop);
			this.gbxAutomatic.Controls.Add(this.btnGo);
			this.gbxAutomatic.Location = new System.Drawing.Point(8, 384);
			this.gbxAutomatic.Name = "gbxAutomatic";
			this.gbxAutomatic.Size = new System.Drawing.Size(152, 232);
			this.gbxAutomatic.TabIndex = 3;
			this.gbxAutomatic.TabStop = false;
			this.gbxAutomatic.Text = "Automatic Control";
			// 
			// chkDelay
			// 
			this.chkDelay.Checked = true;
			this.chkDelay.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDelay.Location = new System.Drawing.Point(8, 72);
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
			this.label2.Location = new System.Drawing.Point(8, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Simulation Speed";
			// 
			// tbrSpeed
			// 
			this.tbrSpeed.Location = new System.Drawing.Point(8, 112);
			this.tbrSpeed.Maximum = 500;
			this.tbrSpeed.Minimum = 10;
			this.tbrSpeed.Name = "tbrSpeed";
			this.tbrSpeed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbrSpeed.Size = new System.Drawing.Size(136, 45);
			this.tbrSpeed.SmallChange = 50;
			this.tbrSpeed.TabIndex = 6;
			this.tbrSpeed.TickFrequency = 50;
			this.tbrSpeed.Value = 100;
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
			this.gbxStatus.Controls.Add(this.label4);
			this.gbxStatus.Controls.Add(this.label1);
			this.gbxStatus.Controls.Add(this.txtPreyCoords);
			this.gbxStatus.Controls.Add(this.txtAgentCoords);
			this.gbxStatus.Location = new System.Drawing.Point(8, 616);
			this.gbxStatus.Name = "gbxStatus";
			this.gbxStatus.Size = new System.Drawing.Size(336, 40);
			this.gbxStatus.TabIndex = 4;
			this.gbxStatus.TabStop = false;
			this.gbxStatus.Text = "Status";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(240, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Prey Coords";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(72, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 10;
			this.label1.Text = "Agent Coords";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtPreyCoords
			// 
			this.txtPreyCoords.Location = new System.Drawing.Point(176, 16);
			this.txtPreyCoords.Name = "txtPreyCoords";
			this.txtPreyCoords.ReadOnly = true;
			this.txtPreyCoords.Size = new System.Drawing.Size(64, 20);
			this.txtPreyCoords.TabIndex = 9;
			this.txtPreyCoords.Text = "";
			// 
			// txtAgentCoords
			// 
			this.txtAgentCoords.Location = new System.Drawing.Point(8, 16);
			this.txtAgentCoords.Name = "txtAgentCoords";
			this.txtAgentCoords.ReadOnly = true;
			this.txtAgentCoords.Size = new System.Drawing.Size(64, 20);
			this.txtAgentCoords.TabIndex = 8;
			this.txtAgentCoords.Text = "";
			// 
			// preyCaptureGrid1
			// 
			this.preyCaptureGrid1.BackColor = System.Drawing.Color.White;
			this.preyCaptureGrid1.Location = new System.Drawing.Point(8, 8);
			this.preyCaptureGrid1.Name = "preyCaptureGrid1";
			this.preyCaptureGrid1.Size = new System.Drawing.Size(336, 336);
			this.preyCaptureGrid1.TabIndex = 5;
			// 
			// btnLoadGenome
			// 
			this.btnLoadGenome.Location = new System.Drawing.Point(184, 352);
			this.btnLoadGenome.Name = "btnLoadGenome";
			this.btnLoadGenome.Size = new System.Drawing.Size(160, 24);
			this.btnLoadGenome.TabIndex = 6;
			this.btnLoadGenome.Text = "Load Genome";
			this.btnLoadGenome.Click += new System.EventHandler(this.btnLoadGenome_Click);
			// 
			// btnInitialiseUser
			// 
			this.btnInitialiseUser.Enabled = false;
			this.btnInitialiseUser.Location = new System.Drawing.Point(8, 176);
			this.btnInitialiseUser.Name = "btnInitialiseUser";
			this.btnInitialiseUser.Size = new System.Drawing.Size(160, 24);
			this.btnInitialiseUser.TabIndex = 19;
			this.btnInitialiseUser.Text = "Initialise [User]";
			this.btnInitialiseUser.Click += new System.EventHandler(this.btnInitialiseUser_Click);
			// 
			// txtAgentX
			// 
			this.txtAgentX.Location = new System.Drawing.Point(40, 204);
			this.txtAgentX.Name = "txtAgentX";
			this.txtAgentX.Size = new System.Drawing.Size(24, 20);
			this.txtAgentX.TabIndex = 20;
			this.txtAgentX.Text = "6";
			// 
			// txtAgentY
			// 
			this.txtAgentY.Location = new System.Drawing.Point(64, 204);
			this.txtAgentY.Name = "txtAgentY";
			this.txtAgentY.Size = new System.Drawing.Size(24, 20);
			this.txtAgentY.TabIndex = 21;
			this.txtAgentY.Text = "6";
			// 
			// txtPreyY
			// 
			this.txtPreyY.Location = new System.Drawing.Point(144, 204);
			this.txtPreyY.Name = "txtPreyY";
			this.txtPreyY.Size = new System.Drawing.Size(24, 20);
			this.txtPreyY.TabIndex = 23;
			this.txtPreyY.Text = "8";
			// 
			// txtPreyX
			// 
			this.txtPreyX.Location = new System.Drawing.Point(120, 204);
			this.txtPreyX.Name = "txtPreyX";
			this.txtPreyX.Size = new System.Drawing.Size(24, 20);
			this.txtPreyX.TabIndex = 22;
			this.txtPreyX.Text = "8";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(96, 204);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 16);
			this.label7.TabIndex = 24;
			this.label7.Text = "Prey";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 204);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 16);
			this.label8.TabIndex = 25;
			this.label8.Text = "Agent";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(350, 664);
			this.Controls.Add(this.btnLoadGenome);
			this.Controls.Add(this.preyCaptureGrid1);
			this.Controls.Add(this.gbxStatus);
			this.Controls.Add(this.gbxAutomatic);
			this.Controls.Add(this.gbxManual);
			this.Controls.Add(this.btnLoad);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Form1";
			this.Text = "SharpNeat Prey Capture Simulator";
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
			oDialog.Title = "Save best organism as network XML";
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
			oDialog.Title = "Save best organism as genome XML";
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


		private void btnInitialiseUser_Click(object sender, System.EventArgs e)
		{
			InitialiseTrial_User();
		}

		private void btnInitialiseDet_Click(object sender, System.EventArgs e)
		{
			//InitialiseTrial_Deterministic();
		}


		private void btnStep_Click(object sender, System.EventArgs e)
		{
			PeformSingleCycle();
		}

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			if(preyCapture==null)
				InitialiseTrial_Random();

			if(!IsNetworkCompatibleWithSimulator())
			{
				MessageBox.Show(this, "The loaded network is not compatible with the simulation  :" + 
					"\rExpected simulation inputs=" + ((ISimulator)preyCapture).InputNeuronCount + ", outputs=" + ((ISimulator)preyCapture).OutputNeuronCount +
					"\rNetwork inputs=" + network.InputNeuronCount + ", outputs=" + network.OutputNeuronCount);
										
				return;
			}

			SetAccess();
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
			PeformSingleCycle();
			timer.Interval = tbrSpeed.Value;
			if(preyCapture.IsCaptured())
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

		private void txtInitialMoves_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				initialMoves = int.Parse(txtInitialMoves.Text);
			}
			catch(FormatException)
			{
				txtInitialMoves.Text = initialMoves.ToString();
			}
			if(initialMoves<0)
			{
				initialMoves=0;
				txtInitialMoves.Text = initialMoves.ToString();
			}
		}

		private void txtPreySpeed_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				preySpeed = double.Parse(txtPreySpeed.Text);
			}
			catch(FormatException)
			{
				txtPreySpeed.Text = preySpeed.ToString("0.0");
			}
			if(preySpeed>1.0 || preySpeed < 0.0)
			{
				preySpeed=0.0;
				txtPreySpeed.Text = preySpeed.ToString("0.0");
			}
		}

		private void txtSensorRange_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				sensorRange = double.Parse(txtSensorRange.Text);
			}
			catch(FormatException)
			{
				txtSensorRange.Text = sensorRange.ToString("0.0");
			}
			if(sensorRange < 0.0)
			{
				sensorRange=0.0;
				txtSensorRange.Text = sensorRange.ToString("0.0");
			}
		}

		#endregion

		#region Private Methods

		private void InitialiseTrial_Random()
		{
			preyCapture = new PreyCaptureNetworkEvaluator(24, initialMoves, preySpeed, 0, sensorRange);
			preyCapture.InitialiseSimulation();

			System.Diagnostics.Debug.WriteLine("init: Agent(" + preyCapture.AgentX + "," + preyCapture.AgentY + ") " 
													+ "Prey(" + preyCapture.PreyX + "," + preyCapture.PreyY + ")");

			network.ClearSignals();
			preyCaptureGrid1.Initialise(preyCapture);
			RefreshStats();
		}

		private void InitialiseTrial_User()
		{
			preyCapture = new PreyCaptureNetworkEvaluator(24, initialMoves, preySpeed, 0, sensorRange);
			preyCapture.InitialiseSimulation(	int.Parse(txtAgentX.Text),
												int.Parse(txtAgentY.Text),
												int.Parse(txtPreyX.Text),
												int.Parse(txtPreyY.Text));
			network.ClearSignals();
			preyCaptureGrid1.Initialise(preyCapture);
			RefreshStats();
		}

		private void RefreshStats()
		{
			txtAgentCoords.Text = "(" + preyCapture.AgentX.ToString() + ", " + preyCapture.AgentY.ToString() + ")";
			txtPreyCoords.Text = "(" + preyCapture.PreyX.ToString() + ", " + preyCapture.PreyY.ToString() + ")";
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
					btnInitialiseDet.Enabled = false;
					btnInitialiseUser.Enabled = false;
					btnStep.Enabled = false;
					btnGo.Enabled = false;
				}
				else
				{
					btnInitialiseRnd.Enabled = true;
					btnInitialiseDet.Enabled = true;
					btnInitialiseUser.Enabled = true;
					btnStep.Enabled = true;
					btnGo.Enabled = true;
				}
			}
			else
			{
				btnLoad.Enabled = false;
				btnInitialiseRnd.Enabled = false;
				btnInitialiseDet.Enabled = false;
				btnInitialiseUser.Enabled = false;
				btnStep.Enabled = false;
				btnGo.Enabled = false;
				btnStop.Enabled = true;
			}
		}

		private void PeformSingleCycle()
		{
			preyCapture.PerformSingleStep(network);
			preyCaptureGrid1.RefreshGrid();
			RefreshStats();
		}

		private bool IsNetworkCompatibleWithSimulator()
		{
			return (((ISimulator)preyCapture).InputNeuronCount == network.InputNeuronCount) &&
				(((ISimulator)preyCapture).OutputNeuronCount == network.OutputNeuronCount);
		}

		#endregion
	}
}
