using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.IO;
using System.Diagnostics;

using SharpNeatLib;
using SharpNeatLib.Experiments;
using SharpNeatLib.Experiments.TicTacToe;
using SharpNeatLib.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;

namespace SharpNeat.TicTacToeGame
{
	public class Form1 : System.Windows.Forms.Form
	{
		Timer	timer=null;
		Board board;
		TicTacToeControl ticTacToeControl;
		IPlayer player1=null;
		IPlayer player2=null;
		bool player1ToGo=true;

		int statsP1Wins=0;
		int statsP2Wins=0;
		int statsTies=0;

		ArrayList loadedNetworkList = new ArrayList();

		#region Windows Form Designer Variables

		private System.Windows.Forms.GroupBox gbxAutomatic;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.TrackBar tbrSpeed;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkContinuous;
		private System.Windows.Forms.CheckBox chkDelay;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.ComboBox cmbPlayer1;
		private System.Windows.Forms.ComboBox cmbPlayer2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnLoadGenome;
		private System.Windows.Forms.Button btnLoadNetwork;
		private System.Windows.Forms.Label lblPlayer1Wins;
		private System.Windows.Forms.Button btnResetPlayerLists;
		private System.Windows.Forms.Button btnResetStats;
		private System.Windows.Forms.Label lblTies;
		private System.Windows.Forms.Label lblPlayer2Wins;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtTrials;
		private System.Windows.Forms.Button btnRunTrials;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public Form1()
		{
			InitializeComponent();

			board = new Board();
			ticTacToeControl = new TicTacToeControl(board);
			ticTacToeControl.Dock = DockStyle.Fill;
			pnlMain.Controls.Add(ticTacToeControl);
			ticTacToeControl.GridClick += new TicTacToeClickHandler(OnTicTacToeGridClick);

			PopulatePlayerCombos();
			UpdateGuiState();
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
			this.btnLoadNetwork = new System.Windows.Forms.Button();
			this.gbxAutomatic = new System.Windows.Forms.GroupBox();
			this.chkDelay = new System.Windows.Forms.CheckBox();
			this.chkContinuous = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbrSpeed = new System.Windows.Forms.TrackBar();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnGo = new System.Windows.Forms.Button();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.cmbPlayer1 = new System.Windows.Forms.ComboBox();
			this.cmbPlayer2 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnLoadGenome = new System.Windows.Forms.Button();
			this.lblPlayer1Wins = new System.Windows.Forms.Label();
			this.lblTies = new System.Windows.Forms.Label();
			this.lblPlayer2Wins = new System.Windows.Forms.Label();
			this.btnResetPlayerLists = new System.Windows.Forms.Button();
			this.btnResetStats = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnRunTrials = new System.Windows.Forms.Button();
			this.txtTrials = new System.Windows.Forms.TextBox();
			this.gbxAutomatic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbrSpeed)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnLoadNetwork
			// 
			this.btnLoadNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoadNetwork.Location = new System.Drawing.Point(326, 8);
			this.btnLoadNetwork.Name = "btnLoadNetwork";
			this.btnLoadNetwork.Size = new System.Drawing.Size(152, 24);
			this.btnLoadNetwork.TabIndex = 0;
			this.btnLoadNetwork.Text = "Load Network";
			this.btnLoadNetwork.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// gbxAutomatic
			// 
			this.gbxAutomatic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbxAutomatic.Controls.Add(this.chkDelay);
			this.gbxAutomatic.Controls.Add(this.chkContinuous);
			this.gbxAutomatic.Controls.Add(this.label2);
			this.gbxAutomatic.Controls.Add(this.tbrSpeed);
			this.gbxAutomatic.Controls.Add(this.btnStop);
			this.gbxAutomatic.Controls.Add(this.btnGo);
			this.gbxAutomatic.Location = new System.Drawing.Point(326, 112);
			this.gbxAutomatic.Name = "gbxAutomatic";
			this.gbxAutomatic.Size = new System.Drawing.Size(152, 152);
			this.gbxAutomatic.TabIndex = 3;
			this.gbxAutomatic.TabStop = false;
			this.gbxAutomatic.Text = "Game Control";
			// 
			// chkDelay
			// 
			this.chkDelay.Checked = true;
			this.chkDelay.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDelay.Location = new System.Drawing.Point(8, 64);
			this.chkDelay.Name = "chkDelay";
			this.chkDelay.Size = new System.Drawing.Size(136, 16);
			this.chkDelay.TabIndex = 9;
			this.chkDelay.Text = "Delay Between Games";
			// 
			// chkContinuous
			// 
			this.chkContinuous.Checked = true;
			this.chkContinuous.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkContinuous.Location = new System.Drawing.Point(8, 48);
			this.chkContinuous.Name = "chkContinuous";
			this.chkContinuous.Size = new System.Drawing.Size(136, 16);
			this.chkContinuous.TabIndex = 8;
			this.chkContinuous.Text = "Continuous Games";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Game Speed";
			// 
			// tbrSpeed
			// 
			this.tbrSpeed.Location = new System.Drawing.Point(8, 104);
			this.tbrSpeed.Maximum = 1000;
			this.tbrSpeed.Minimum = 1;
			this.tbrSpeed.Name = "tbrSpeed";
			this.tbrSpeed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbrSpeed.Size = new System.Drawing.Size(136, 45);
			this.tbrSpeed.SmallChange = 50;
			this.tbrSpeed.TabIndex = 6;
			this.tbrSpeed.TickFrequency = 50;
			this.tbrSpeed.Value = 1000;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(88, 16);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(56, 24);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(8, 16);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(72, 24);
			this.btnGo.TabIndex = 0;
			this.btnGo.Text = "New Game";
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// pnlMain
			// 
			this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlMain.Location = new System.Drawing.Point(8, 64);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(312, 288);
			this.pnlMain.TabIndex = 5;
			// 
			// cmbPlayer1
			// 
			this.cmbPlayer1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPlayer1.Location = new System.Drawing.Point(8, 24);
			this.cmbPlayer1.Name = "cmbPlayer1";
			this.cmbPlayer1.Size = new System.Drawing.Size(152, 21);
			this.cmbPlayer1.TabIndex = 6;
			this.cmbPlayer1.SelectedIndexChanged += new System.EventHandler(this.cmbPlayer1_SelectedIndexChanged);
			// 
			// cmbPlayer2
			// 
			this.cmbPlayer2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPlayer2.Location = new System.Drawing.Point(160, 24);
			this.cmbPlayer2.Name = "cmbPlayer2";
			this.cmbPlayer2.Size = new System.Drawing.Size(160, 21);
			this.cmbPlayer2.TabIndex = 7;
			this.cmbPlayer2.SelectedIndexChanged += new System.EventHandler(this.cmbPlayer2_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Player 1 [Crosses]";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(160, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Player 2 [Naughts]";
			// 
			// btnLoadGenome
			// 
			this.btnLoadGenome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoadGenome.Location = new System.Drawing.Point(326, 32);
			this.btnLoadGenome.Name = "btnLoadGenome";
			this.btnLoadGenome.Size = new System.Drawing.Size(152, 24);
			this.btnLoadGenome.TabIndex = 11;
			this.btnLoadGenome.Text = "Load Genome";
			this.btnLoadGenome.Click += new System.EventHandler(this.btnLoadGenome_Click);
			// 
			// lblPlayer1Wins
			// 
			this.lblPlayer1Wins.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPlayer1Wins.Location = new System.Drawing.Point(8, 48);
			this.lblPlayer1Wins.Name = "lblPlayer1Wins";
			this.lblPlayer1Wins.Size = new System.Drawing.Size(104, 16);
			this.lblPlayer1Wins.TabIndex = 12;
			this.lblPlayer1Wins.Text = "1 Wins:";
			// 
			// lblTies
			// 
			this.lblTies.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTies.Location = new System.Drawing.Point(112, 48);
			this.lblTies.Name = "lblTies";
			this.lblTies.Size = new System.Drawing.Size(104, 16);
			this.lblTies.TabIndex = 13;
			this.lblTies.Text = "Ties:";
			// 
			// lblPlayer2Wins
			// 
			this.lblPlayer2Wins.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPlayer2Wins.Location = new System.Drawing.Point(216, 48);
			this.lblPlayer2Wins.Name = "lblPlayer2Wins";
			this.lblPlayer2Wins.Size = new System.Drawing.Size(104, 16);
			this.lblPlayer2Wins.TabIndex = 14;
			this.lblPlayer2Wins.Text = "2 Wins:";
			// 
			// btnResetPlayerLists
			// 
			this.btnResetPlayerLists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResetPlayerLists.Location = new System.Drawing.Point(326, 56);
			this.btnResetPlayerLists.Name = "btnResetPlayerLists";
			this.btnResetPlayerLists.Size = new System.Drawing.Size(152, 24);
			this.btnResetPlayerLists.TabIndex = 15;
			this.btnResetPlayerLists.Text = "Reset Player Lists";
			this.btnResetPlayerLists.Click += new System.EventHandler(this.btnResetPlayerLists_Click);
			// 
			// btnResetStats
			// 
			this.btnResetStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResetStats.Location = new System.Drawing.Point(326, 80);
			this.btnResetStats.Name = "btnResetStats";
			this.btnResetStats.Size = new System.Drawing.Size(152, 24);
			this.btnResetStats.TabIndex = 17;
			this.btnResetStats.Text = "Reset Stats";
			this.btnResetStats.Click += new System.EventHandler(this.btnResetStats_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnRunTrials);
			this.groupBox1.Controls.Add(this.txtTrials);
			this.groupBox1.Location = new System.Drawing.Point(328, 272);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(152, 80);
			this.groupBox1.TabIndex = 18;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Trials";
			// 
			// btnRunTrials
			// 
			this.btnRunTrials.Location = new System.Drawing.Point(8, 48);
			this.btnRunTrials.Name = "btnRunTrials";
			this.btnRunTrials.Size = new System.Drawing.Size(136, 24);
			this.btnRunTrials.TabIndex = 1;
			this.btnRunTrials.Text = "Run Trials";
			this.btnRunTrials.Click += new System.EventHandler(this.btnRunTrials_Click);
			// 
			// txtTrials
			// 
			this.txtTrials.Location = new System.Drawing.Point(8, 16);
			this.txtTrials.Name = "txtTrials";
			this.txtTrials.Size = new System.Drawing.Size(136, 20);
			this.txtTrials.TabIndex = 0;
			this.txtTrials.Text = "10000";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(486, 356);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnResetStats);
			this.Controls.Add(this.btnResetPlayerLists);
			this.Controls.Add(this.lblPlayer2Wins);
			this.Controls.Add(this.lblTies);
			this.Controls.Add(this.lblPlayer1Wins);
			this.Controls.Add(this.btnLoadGenome);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbPlayer2);
			this.Controls.Add(this.cmbPlayer1);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.btnLoadNetwork);
			this.Controls.Add(this.gbxAutomatic);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "SharpNEAT Tic-Tac-Toe";
			this.gbxAutomatic.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tbrSpeed)).EndInit();
			this.groupBox1.ResumeLayout(false);
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
			INetwork network = null;
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
				return;
			}

			if(IsNetworkCompatibleWithGame(network))
			{
				loadedNetworkList.Add(network);
				PopulatePlayerCombos();
			}
			else
			{
				MessageBox.Show(this, "The loaded network is not compatible with this TicTacToe game:" + 
					"\rExpected inputs=9, outputs=9" +
					"\rNetwork inputs=" + network.InputNeuronCount + ", outputs=" + network.OutputNeuronCount);
			}	
		}

		private void btnLoadGenome_Click(object sender, System.EventArgs e)
		{
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
			INetwork network = null;	
			try
			{
				doc.Load(oDialog.FileName);
				NeatGenome g = XmlNeatGenomeReaderStatic.Read(doc);
				network = GenomeDecoder.DecodeToConcurrentNetwork(g, new SteepenedSigmoidApproximation());
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Error loading genome xml: \r\n" + ex.Message);
				return;
			}

			if(IsNetworkCompatibleWithGame(network))
			{
				loadedNetworkList.Add(network);
				PopulatePlayerCombos();
			}
			else
			{
				MessageBox.Show(this, "The loaded network is not compatible with this TicTacToe game:" + 
					"\rExpected inputs=10, outputs=9" +
					"\rNetwork inputs=" + network.InputNeuronCount + ", outputs=" + network.OutputNeuronCount);
			}	
		}

		private void btnResetPlayerLists_Click(object sender, System.EventArgs e)
		{
			loadedNetworkList.Clear();
			PopulatePlayerCombos();
		}

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			// Reset the board.
			player1ToGo=true;
			playerPassFlag=false;
			board.Reset();
			ticTacToeControl.Refresh();

			if(timer==null)
			{
				timer = new Timer();
				timer.Tick += new EventHandler(OnTickEvent);
			}

			UpdateGuiState();

			// GO!
			timer.Interval = tbrSpeed.Value;
			timer.Start();
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
			timer.Stop();
			timer = null;
			UpdateGuiState();
		}

		bool playerPassFlag=false;
		private void OnTickEvent(Object myObject, EventArgs myEventArgs) 
		{
			timer.Interval = tbrSpeed.Value;

			IPlayer currentPlayer;
			if(player1ToGo)
				currentPlayer = player1;
			else
				currentPlayer = player2;
			
			HumanPlayer oHuman = currentPlayer as HumanPlayer;
			if(oHuman!=null)
			{	// Human player. Wait for human to make a move (they are so slow aren't they).
				return;
			}
				
			// CPU player to make a move.
			bool pass = !board.MakeMove(currentPlayer.GetNextMove(board), currentPlayer.PlayerType);
			if(pass)
			{
				if(playerPassFlag)
				{	// Two passes in a row.
					IncrementTies();
					ProcessEndOfGame();
					return;
				}
				else
				{	// Set flag so we know if two passes occured in a row.
					playerPassFlag = true;
				}
			}
			else
			{	// Reset pass flag when a valid move is made.
				playerPassFlag=false;
			}

			ticTacToeControl.Refresh();
			if(board.CheckForWin()) 
			{
				if(player1ToGo)
					IncrementP1Wins();
				else
					IncrementP2Wins();

				ProcessEndOfGame();
			}
			else if (board.IsBoardFull)
			{
				IncrementTies();
				ProcessEndOfGame();
			}
			else
			{
				player1ToGo = !player1ToGo;
			}
		}

		private void btnResetStats_Click(object sender, System.EventArgs e)
		{
			statsP1Wins = statsP2Wins = statsTies = 0;
			lblPlayer1Wins.Text = "1 Wins: " + statsP1Wins.ToString();
			lblPlayer2Wins.Text = "2 Wins: " + statsP1Wins.ToString();
			lblTies.Text = "Ties: " + statsTies.ToString();

			lblPlayer1Wins.BackColor = System.Drawing.SystemColors.Control;
			lblPlayer2Wins.BackColor = System.Drawing.SystemColors.Control;
			lblTies.BackColor = System.Drawing.SystemColors.Control;
		}

		private void cmbPlayer1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			player1 = (IPlayer)((ListItem)cmbPlayer1.SelectedItem).Data;
		}

		private void cmbPlayer2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			player2 = (IPlayer)((ListItem)cmbPlayer2.SelectedItem).Data;
		}

		private void OnTicTacToeGridClick(object sender, TicTacToeClickArgs e)
		{
			if(timer==null)
			{	// No game in progress. Silly Humans :)
				return;
			}

			IPlayer currentPlayer;
			if(player1ToGo)
				currentPlayer = player1;
			else
				currentPlayer = player2;
			
			HumanPlayer oHuman = currentPlayer as HumanPlayer;
			if(oHuman==null)
			{	// It's not a human's turn.
				return;
			}

			bool pass = !board.MakeMove(new ByteCoord(e.GridX, e.GridY), oHuman.PlayerType);
			if(pass)
			{
				if(playerPassFlag)
				{	// Two passes in a row.
					IncrementTies();
					ProcessEndOfGame();
				}
				else
				{	// Set flag so we know if two passes occured in a row.
					playerPassFlag = true;
				}
			}
			else
			{	// Reset pass flag when a valid move is made.
				playerPassFlag=false;
			}

			ticTacToeControl.Refresh();
			if(board.CheckForWin()) 
			{
				if(player1ToGo)
					IncrementP1Wins();
				else
					IncrementP2Wins();

				ProcessEndOfGame();
			}
			else if (board.IsBoardFull)
			{
				IncrementTies();
				ProcessEndOfGame();
			}
			else
			{
				player1ToGo = !player1ToGo;
			}
		}

		private void btnRunTrials_Click(object sender, System.EventArgs e)
		{
//			FastParetoDominanceCoEvTicTacToePopulationEvaluator evaluator = new FastParetoDominanceCoEvTicTacToePopulationEvaluator(new SteepenedSigmoidApproximation());
//
//			FitnessPair fitnesssPair = evaluator.EvaluatePlayerPair(player1, player2);
//			Debug.WriteLine(fitnesssPair.fitness1 + ", " + fitnesssPair.fitness2); 


			int trials;
			Game game = new Game();
			btnResetStats_Click(null, null);

			try
			{
				trials = int.Parse(txtTrials.Text);
			}
			catch(Exception)
			{
				return;
			}

			// Time how long the trials take.
			long ticksStart = System.DateTime.Now.Ticks;
			for(int i=0; i<trials; i++)
			{
				IPlayer winner = game.PlayGame(player1, player2);


				if(winner==player1)
				{
					statsP1Wins++;
				}
				else if(winner==player2)
				{
					statsP2Wins++;
				}
				else
				{
					statsTies++;
				}
			}
			long ticksDuration = System.DateTime.Now.Ticks - ticksStart;

			// Show the trial results.
			lblPlayer1Wins.Text = "1Wins: " + statsP1Wins.ToString() + GetPercentString(statsP1Wins);
			lblPlayer2Wins.Text = "2Wins: " + statsP2Wins.ToString() + GetPercentString(statsP2Wins);
			lblTies.Text = "Ties: " + statsTies.ToString() + GetPercentString(statsTies);			

			MessageBox.Show("Trials took " + (ticksDuration*0.0001).ToString("0.00") + "ms\r" + (trials/(ticksDuration*0.0000001)).ToString("0.00") + " Games/sec");
		}

		#endregion

		#region Private Methods

//		private void PopulatePlayerCombos()
//		{
//		//----- Player 1.
//			cmbPlayer1.Items.Clear();
//			cmbPlayer1.Items.Add(new ListItem("", "Human", new HumanPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Bad]", new BadPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Random]", new RandomPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Center]", new CenterPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Forking]", new ForkablePlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Best]", new BestPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.Items.Add(new ListItem("", "CPU [Competent Loser]", new CompetentLoserPlayer(BoardUnitState.Cross)));
//			cmbPlayer1.SelectedIndex=0;
//
//		//----- Player 2.
//			cmbPlayer2.Items.Clear();
//			cmbPlayer2.Items.Add(new ListItem("", "Human", new HumanPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Bad]", new BadPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Random]", new RandomPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Center]", new CenterPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Forking]", new ForkablePlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Best]", new BestPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.Items.Add(new ListItem("", "CPU [Competent Loser]", new CompetentLoserPlayer(BoardUnitState.Naught)));
//			cmbPlayer2.SelectedIndex=0;
//
//		}

		private void PopulatePlayerCombos()
		{
			

			//----- Player 1.
			cmbPlayer1.Items.Clear();
			cmbPlayer1.Items.Add(new ListItem("", "Human", new HumanPlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Bad]", new BadPlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Random]", new RandomPlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Center]", new CenterPlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Forking]", new ForkablePlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Best]", new BestPlayer(BoardUnitState.Cross)));
			cmbPlayer1.Items.Add(new ListItem("", "CPU [Competent Loser]", new CompetentLoserPlayer(BoardUnitState.Cross)));

			//----- Player 2.
			cmbPlayer2.Items.Clear();
			cmbPlayer2.Items.Add(new ListItem("", "Human", new HumanPlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Bad]", new BadPlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Random]", new RandomPlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Center]", new CenterPlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Forking]", new ForkablePlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Best]", new BestPlayer(BoardUnitState.Naught)));
			cmbPlayer2.Items.Add(new ListItem("", "CPU [Competent Loser]", new CompetentLoserPlayer(BoardUnitState.Naught)));

			
			int i=1;
			foreach(INetwork network in loadedNetworkList)
			{
//				LookupPlayer nnPlayerCross = new LookupPlayer(BoardUnitState.Cross, network);
//				LookupPlayer nnPlayerNaught = new LookupPlayer(BoardUnitState.Naught, network);

				NormalizedViewpointNeuralNetPlayer nnPlayerCross = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Cross);
				NormalizedViewpointNeuralNetPlayer nnPlayerNaught = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Naught);
				nnPlayerCross.SetNetwork(network);
				nnPlayerNaught.SetNetwork(network);

				cmbPlayer1.Items.Add(new ListItem("", "CPU [Neural Net #" + i.ToString() + "]", nnPlayerCross));
				cmbPlayer2.Items.Add(new ListItem("", "CPU [Neural Net #" + i.ToString() + "]", nnPlayerNaught));
				i++;
			}

			cmbPlayer1.SelectedIndex=0;
			cmbPlayer2.SelectedIndex=0;
		}

		private void UpdateGuiState()
		{
			if(timer==null)
			{
				cmbPlayer1.Enabled = true;
				cmbPlayer2.Enabled = true;

				btnGo.Enabled = true;
				btnStop.Enabled = false;

				btnLoadNetwork.Enabled = true;
				btnLoadGenome.Enabled = true;
				btnRunTrials.Enabled = true;
			}
			else
			{
				cmbPlayer1.Enabled = false;
				cmbPlayer2.Enabled = false;

				btnGo.Enabled = false;
				btnStop.Enabled = true;

				btnLoadNetwork.Enabled = false;
				btnLoadGenome.Enabled = false;
				btnRunTrials.Enabled = false;
			}
		}

		private bool IsNetworkCompatibleWithGame(INetwork network)
		{
			return (network.InputNeuronCount==9) &&
				(network.OutputNeuronCount==9);
		}

		private void ProcessEndOfGame()
		{
			System.Diagnostics.Debug.WriteLine("..END..");
			if(chkContinuous.Checked)
			{
				timer.Stop();
				if(chkDelay.Checked)
				{	
					Application.DoEvents();
					System.Threading.Thread.Sleep(1000);
				}
				player1ToGo=true;
				playerPassFlag=false;
				board.Reset();
				ticTacToeControl.Refresh();
				timer.Start();
			}
			else
			{
				timer.Stop();
				timer = null;
				UpdateGuiState();
			}
		}

		private void IncrementP1Wins()
		{
			statsP1Wins++;
		
			lblPlayer1Wins.Text = "1Wins: " + statsP1Wins.ToString() + GetPercentString(statsP1Wins);
			lblPlayer2Wins.Text = "2Wins: " + statsP2Wins.ToString() + GetPercentString(statsP2Wins);
			lblTies.Text = "Ties: " + statsTies.ToString() + GetPercentString(statsTies);

			lblPlayer1Wins.BackColor = Color.Red;
			lblPlayer2Wins.BackColor = System.Drawing.SystemColors.Control;
			lblTies.BackColor = System.Drawing.SystemColors.Control;
		}

		private void IncrementP2Wins()
		{
			statsP2Wins++;

			lblPlayer1Wins.Text = "1Wins: " + statsP1Wins.ToString() + GetPercentString(statsP1Wins);
			lblPlayer2Wins.Text = "2Wins: " + statsP2Wins.ToString() + GetPercentString(statsP2Wins);
			lblTies.Text = "Ties: " + statsTies.ToString() + GetPercentString(statsTies);

			lblPlayer1Wins.BackColor = System.Drawing.SystemColors.Control;
			lblPlayer2Wins.BackColor = Color.Red;
			lblTies.BackColor = System.Drawing.SystemColors.Control;
		}

		private void IncrementTies()
		{
			statsTies++;

			lblPlayer1Wins.Text = "1Wins: " + statsP1Wins.ToString() + GetPercentString(statsP1Wins);
			lblPlayer2Wins.Text = "2Wins: " + statsP2Wins.ToString() + GetPercentString(statsP2Wins);
			lblTies.Text = "Ties: " + statsTies.ToString() + GetPercentString(statsTies);

			lblPlayer1Wins.BackColor = System.Drawing.SystemColors.Control;
			lblPlayer2Wins.BackColor = System.Drawing.SystemColors.Control;
			lblTies.BackColor = Color.Red;
		}



		private string GetPercentString(int numerator)
		{
			int denominator = statsP1Wins + statsP2Wins + statsTies;
			return " (" + (((float)numerator / (float)denominator)*100F).ToString("0.00") + "%)";
		}



		#endregion
	}
}
