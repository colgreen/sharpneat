using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace SharpNeat
{
	/// <summary>
	/// Summary description for AboutBox.
	/// </summary>
	public class AboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox txtDetails;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RichTextBox txtVersionInfo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			InitializeAboutBox();
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtDetails = new System.Windows.Forms.RichTextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtVersionInfo = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Georgia", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "SharpNEAT";
			// 
			// txtDetails
			// 
			this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDetails.Location = new System.Drawing.Point(8, 40);
			this.txtDetails.Name = "txtDetails";
			this.txtDetails.ReadOnly = true;
			this.txtDetails.Size = new System.Drawing.Size(360, 216);
			this.txtDetails.TabIndex = 1;
			this.txtDetails.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.txtVersionInfo);
			this.groupBox1.Location = new System.Drawing.Point(8, 264);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(360, 72);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Version Info";
			// 
			// txtVersionInfo
			// 
			this.txtVersionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtVersionInfo.Location = new System.Drawing.Point(3, 16);
			this.txtVersionInfo.Name = "txtVersionInfo";
			this.txtVersionInfo.ReadOnly = true;
			this.txtVersionInfo.Size = new System.Drawing.Size(354, 53);
			this.txtVersionInfo.TabIndex = 3;
			this.txtVersionInfo.Text = "";
			// 
			// AboutBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 344);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.txtDetails);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About SharpNEAT";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void InitializeAboutBox()
		{
			txtDetails.Text = @"NEAT is NeuroEvolution of Augmenting Topologies, a neuro evolution technique devised by Kenneth Stanley of the Neural Networks Research Group, University of Texas at Austin.

See http://www.cs.ucf.edu/~kstanley/ for more info on NEAT, including research papers, software, source code, a FAQ and an active discussion group.

SharpNeat is an implementation of NEAT written in C# by Colin Green. For more info on SharpNEAT see http://sharpneat.sourceforge.net/

SharpNeat is released under the Gnu General Public License (GPL).
SharpNeatLib is released under the Lesser General Public License (LGPL).

Colin Green, March 2006.";

			Version oVersion = Assembly.GetExecutingAssembly().GetName().Version;
			txtVersionInfo.Text =	"SharpNEAT\t" + oVersion.Major.ToString() + "." + oVersion.Minor.ToString()+ "." + oVersion.Revision.ToString() + "\tBuild " + oVersion.Build.ToString() + "\r\n" + 
									".NET Framework\t" + Environment.Version.ToString();
		}
	}
}
