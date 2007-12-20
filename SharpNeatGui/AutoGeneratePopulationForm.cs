using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome;

namespace SharpNeat
{
	public class AutoGeneratePopulationForm : System.Windows.Forms.Form
	{
		Population population;
		NeatParameters neatParameters;
		int inputNeuronCount;
		int outputNeuronCount;

		#region Windows Designer Variables

		private System.Windows.Forms.TextBox txtSize;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.TextBox txtConnectionProportion;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public AutoGeneratePopulationForm(NeatParameters neatParameters, int inputNeuronCount, int outputNeuronCount)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.neatParameters = neatParameters;
			this.inputNeuronCount = inputNeuronCount;
			this.outputNeuronCount = outputNeuronCount;

			txtSize.Text = neatParameters.populationSize.ToString();
			txtConnectionProportion.Text = neatParameters.pInitialPopulationInterconnections.ToString();
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
			this.txtSize = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtConnectionProportion = new System.Windows.Forms.TextBox();
			this.btnGenerate = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtSize
			// 
			this.txtSize.Location = new System.Drawing.Point(104, 16);
			this.txtSize.Name = "txtSize";
			this.txtSize.Size = new System.Drawing.Size(48, 20);
			this.txtSize.TabIndex = 0;
			this.txtSize.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Population Size";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.txtConnectionProportion);
			this.groupBox1.Controls.Add(this.btnGenerate);
			this.groupBox1.Location = new System.Drawing.Point(8, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 56);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(136, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 24);
			this.label2.TabIndex = 6;
			this.label2.Text = "Connection Proportion";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtConnectionProportion
			// 
			this.txtConnectionProportion.Location = new System.Drawing.Point(96, 24);
			this.txtConnectionProportion.Name = "txtConnectionProportion";
			this.txtConnectionProportion.Size = new System.Drawing.Size(40, 20);
			this.txtConnectionProportion.TabIndex = 5;
			this.txtConnectionProportion.Text = "1.0";
			// 
			// btnGenerate
			// 
			this.btnGenerate.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnGenerate.Location = new System.Drawing.Point(8, 16);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(80, 32);
			this.btnGenerate.TabIndex = 4;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// AutoGeneratePopulationForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(280, 102);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSize);
			this.Name = "AutoGeneratePopulationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generate Random Population";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties

		public Population Population
		{
			get
			{
				return population;
			}
		}

		#endregion

		#region Public Methods

		public void SetPopulationSize(int size)
		{
			txtSize.Text = size.ToString();
		}

		#endregion

		private void btnGenerate_Click(object sender, System.EventArgs e)
		{
			int size = int.Parse(txtSize.Text);
			float connectionProportion = float.Parse(txtConnectionProportion.Text);

			IdGenerator idGenerator = new IdGenerator();
			
			GenomeList genomeList =  GenomeFactory.CreateGenomeList(
				neatParameters,
				idGenerator,
				inputNeuronCount,
				outputNeuronCount,
				connectionProportion,
				size);

			population = new Population(idGenerator, genomeList);
		}
	}
}
