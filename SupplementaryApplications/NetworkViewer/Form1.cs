using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;

using SharpNeatLib;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using SharpNeatLib.NetworkVisualization;

using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
      

namespace NetworkViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		NetworkControl nc;

		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuFileOpenNetworkXml;
		private System.Windows.Forms.Panel panMain;
		private System.Windows.Forms.Button btnZoomIn;
		private System.Windows.Forms.Button btnZoomOut;
		private System.Windows.Forms.GroupBox gbxToolbox;
		private System.Windows.Forms.MenuItem mnuAutoLayout;
		private System.Windows.Forms.MenuItem mnuAutoLayoutGrid;
		private System.Windows.Forms.GroupBox gbxZoom;
        private System.Windows.Forms.TextBox txtZoom;
        private MenuItem mnuFileOpenGenomeXml;
        private IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			nc = new NetworkControl();
			nc.Dock = DockStyle.Fill;
			panMain.Controls.Add(nc);
			txtZoom.Text = nc.ZoomFactor.ToString();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mnuMain = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileOpenNetworkXml = new System.Windows.Forms.MenuItem();
            this.mnuAutoLayout = new System.Windows.Forms.MenuItem();
            this.mnuAutoLayoutGrid = new System.Windows.Forms.MenuItem();
            this.panMain = new System.Windows.Forms.Panel();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.gbxToolbox = new System.Windows.Forms.GroupBox();
            this.gbxZoom = new System.Windows.Forms.GroupBox();
            this.txtZoom = new System.Windows.Forms.TextBox();
            this.mnuFileOpenGenomeXml = new System.Windows.Forms.MenuItem();
            this.gbxToolbox.SuspendLayout();
            this.gbxZoom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuAutoLayout});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileOpenNetworkXml,
            this.mnuFileOpenGenomeXml});
            this.mnuFile.Text = "File";
            // 
            // mnuFileOpenNetworkXml
            // 
            this.mnuFileOpenNetworkXml.Index = 0;
            this.mnuFileOpenNetworkXml.Text = "Open Network Xml";
            this.mnuFileOpenNetworkXml.Click += new System.EventHandler(this.mnuFileOpenNetworkXml_Click);
            // 
            // mnuAutoLayout
            // 
            this.mnuAutoLayout.Index = 1;
            this.mnuAutoLayout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAutoLayoutGrid});
            this.mnuAutoLayout.Text = "Auto Layout";
            // 
            // mnuAutoLayoutGrid
            // 
            this.mnuAutoLayoutGrid.Index = 0;
            this.mnuAutoLayoutGrid.Text = "Grid Layout";
            this.mnuAutoLayoutGrid.Click += new System.EventHandler(this.mnuAutoLayoutGrid_Click);
            // 
            // panMain
            // 
            this.panMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panMain.Location = new System.Drawing.Point(72, 0);
            this.panMain.Name = "panMain";
            this.panMain.Size = new System.Drawing.Size(576, 424);
            this.panMain.TabIndex = 0;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomIn.Location = new System.Drawing.Point(3, 16);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(26, 24);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomOut.Location = new System.Drawing.Point(27, 16);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(26, 24);
            this.btnZoomOut.TabIndex = 2;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // gbxToolbox
            // 
            this.gbxToolbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxToolbox.Controls.Add(this.gbxZoom);
            this.gbxToolbox.Location = new System.Drawing.Point(3, 0);
            this.gbxToolbox.Name = "gbxToolbox";
            this.gbxToolbox.Size = new System.Drawing.Size(69, 422);
            this.gbxToolbox.TabIndex = 3;
            this.gbxToolbox.TabStop = false;
            // 
            // gbxZoom
            // 
            this.gbxZoom.Controls.Add(this.txtZoom);
            this.gbxZoom.Controls.Add(this.btnZoomIn);
            this.gbxZoom.Controls.Add(this.btnZoomOut);
            this.gbxZoom.Location = new System.Drawing.Point(8, 8);
            this.gbxZoom.Name = "gbxZoom";
            this.gbxZoom.Size = new System.Drawing.Size(56, 72);
            this.gbxZoom.TabIndex = 4;
            this.gbxZoom.TabStop = false;
            this.gbxZoom.Text = "Zoom";
            // 
            // txtZoom
            // 
            this.txtZoom.Location = new System.Drawing.Point(4, 44);
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.ReadOnly = true;
            this.txtZoom.Size = new System.Drawing.Size(46, 20);
            this.txtZoom.TabIndex = 3;
            this.txtZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mnuFileOpenGenomeXml
            // 
            this.mnuFileOpenGenomeXml.Index = 1;
            this.mnuFileOpenGenomeXml.Text = "Open Genome Xml";
            this.mnuFileOpenGenomeXml.Click += new System.EventHandler(this.mnuFileOpenGenomeXml_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(648, 425);
            this.Controls.Add(this.gbxToolbox);
            this.Controls.Add(this.panMain);
            this.Menu = this.mnuMain;
            this.Name = "Form1";
            this.Text = "SharpNeat Network Viewer";
            this.gbxToolbox.ResumeLayout(false);
            this.gbxZoom.ResumeLayout(false);
            this.gbxZoom.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Application Main

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

		private void mnuFileOpenNetworkXml_Click(object sender, System.EventArgs e)
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
			doc.Load(oDialog.FileName);

			//----- Read the network structure from the XmlDocument.
			ConcurrentNetwork network=null;
			try
			{
				network = XmlNetworkReaderStatic.Read(doc);
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Error loading network xml: \r\n" + ex.Message);
			}

			if(network==null)
			{
				// SetAccess();
				return;
			}

			NetworkModel nm = GenomeDecoder.DecodeToNetworkModel(network);
//			NetworkModel nm = new NetworkModel(network.MasterNeuronList);
			GridLayoutManager layoutManager = new GridLayoutManager();
			layoutManager.Layout(nm, nc.Size);
			nc.NetworkModel = nm;

			//-----
			//SetAccess();
		}

        private void mnuFileOpenGenomeXml_Click(object sender, EventArgs e)
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
            doc.Load(oDialog.FileName);

            //----- Read the network structure from the XmlDocument.
            NeatGenome genome=null;
            try
            {
                genome = XmlNeatGenomeReaderStatic.Read(doc);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Error loading genome xml: \r\n" + ex.Message);
            }

            if(genome==null)
            {
                // SetAccess();
                return;
            }

            NetworkModel nm = GenomeDecoder.DecodeToNetworkModel(genome);
            GridLayoutManager layoutManager = new GridLayoutManager();
            layoutManager.Layout(nm, nc.Size);
            nc.NetworkModel = nm;

            //-----
            //SetAccess();
        }

		private void btnZoomIn_Click(object sender, System.EventArgs e)
		{
			// Limit zoom to 5x. Memory requirements increase with viewport size.
			if(nc.ZoomFactor < 5.0F)
			{
				nc.ZoomFactor+=0.5F;
				txtZoom.Text = nc.ZoomFactor.ToString();
			}
		}

		private void btnZoomOut_Click(object sender, System.EventArgs e)
		{
			nc.ZoomFactor-=0.5F;
			txtZoom.Text = nc.ZoomFactor.ToString();
		}

		private void mnuAutoLayoutGrid_Click(object sender, System.EventArgs e)
		{
			if(nc.NetworkModel!=null)
			{
				GridLayoutManager layoutManager = new GridLayoutManager();
				layoutManager.Layout(nc.NetworkModel, nc.Size);
				nc.RefreshControl();
			}
		}

		#endregion


	}
}
