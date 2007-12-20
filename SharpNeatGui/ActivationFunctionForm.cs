using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using SharpNeatLib.NeuralNetwork;
using RudyGraph;

namespace SharpNeat
{
	/// <summary>
	/// Summary description for ActivationFunctionGraph.
	/// </summary>
	public class ActivationFunctionForm : System.Windows.Forms.Form
	{
		const int PLOT_POINTS = 200;
		PlotSet moPlotSet;

		private RudyGraph.RudyGraphControl graph1;
		private System.Windows.Forms.Label lblActivationFunctionId;
		private System.Windows.Forms.TextBox txtXMin;
		private System.Windows.Forms.TextBox txtXMax;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ActivationFunctionForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			moPlotSet = new PlotSet(new double[PLOT_POINTS]);
			graph1.PrimaryYAxis.Bind(moPlotSet);
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.graph1 = new RudyGraph.RudyGraphControl();
			this.lblActivationFunctionId = new System.Windows.Forms.Label();
			this.txtXMin = new System.Windows.Forms.TextBox();
			this.txtXMax = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// graph1
			// 
			this.graph1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.graph1.Location = new System.Drawing.Point(8, 24);
			this.graph1.Name = "graph1";
			this.graph1.Size = new System.Drawing.Size(352, 320);
			this.graph1.TabIndex = 1;
			// 
			// lblActivationFunctionId
			// 
			this.lblActivationFunctionId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblActivationFunctionId.Location = new System.Drawing.Point(8, 5);
			this.lblActivationFunctionId.Name = "lblActivationFunctionId";
			this.lblActivationFunctionId.Size = new System.Drawing.Size(352, 16);
			this.lblActivationFunctionId.TabIndex = 2;
			this.lblActivationFunctionId.Text = "lblActivationFunctionId";
			// 
			// txtXMin
			// 
			this.txtXMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtXMin.Location = new System.Drawing.Point(72, 349);
			this.txtXMin.Name = "txtXMin";
			this.txtXMin.Size = new System.Drawing.Size(40, 20);
			this.txtXMin.TabIndex = 3;
			this.txtXMin.Text = "-5";
			this.txtXMin.Validating += new System.ComponentModel.CancelEventHandler(this.txtXMin_Validating);
			this.txtXMin.Validated += new System.EventHandler(this.txtXMin_Validated);
			// 
			// txtXMax
			// 
			this.txtXMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtXMax.Location = new System.Drawing.Point(136, 349);
			this.txtXMax.Name = "txtXMax";
			this.txtXMax.Size = new System.Drawing.Size(40, 20);
			this.txtXMax.TabIndex = 4;
			this.txtXMax.Text = "5";
			this.txtXMax.Validating += new System.ComponentModel.CancelEventHandler(this.txtXMax_Validating);
			this.txtXMax.Validated += new System.EventHandler(this.txtXMax_Validated);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 352);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "X Range:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(112, 352);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "to";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ActivationFunctionForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(368, 374);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtXMax);
			this.Controls.Add(this.txtXMin);
			this.Controls.Add(this.lblActivationFunctionId);
			this.Controls.Add(this.graph1);
			this.Name = "ActivationFunctionForm";
			this.Text = "SharpNEAT [Activation Function Plot]";
			this.ResumeLayout(false);

		}
		#endregion

		IActivationFunction moActivationFunction;
		public IActivationFunction ActivationFunction
		{
			get
			{
				return moActivationFunction;
			}
			set
			{
				moActivationFunction = value;
				lblActivationFunctionId.Text = moActivationFunction.FunctionId;
				PlotGraph();
			}
		}

		private void PlotGraph()
		{
			double[] data = new double[PLOT_POINTS];

            double x_cur = -5.0; 
            if(!double.TryParse(txtXMin.Text, out x_cur))
                txtXMin.Text = x_cur.ToString();

            double x_max = 5.0;
            if(!double.TryParse(txtXMax.Text, out x_max))
                txtXMax.Text = x_max.ToString();

            double x_inc = (x_max-x_cur)/PLOT_POINTS;

			for(int i=0; i<PLOT_POINTS; i++, x_cur+=x_inc)
				data[i] = moActivationFunction.Calculate(x_cur);
			
			graph1.BeginUpdate();
			try
			{
				moPlotSet.LoadValues(data);
			}
			finally
			{
				graph1.EndUpdate();
			}
		}

		private void txtXMin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				double.Parse(txtXMin.Text);
			}
			catch(Exception)
			{
				e.Cancel = true;
			}		
		}

		private void txtXMax_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				double.Parse(txtXMax.Text);
			}
			catch(Exception)
			{
				e.Cancel = true;
			}
		}

		private void txtXMin_Validated(object sender, System.EventArgs e)
		{
			PlotGraph();
		}

		private void txtXMax_Validated(object sender, System.EventArgs e)
		{
			PlotGraph();
		}
	}
}
