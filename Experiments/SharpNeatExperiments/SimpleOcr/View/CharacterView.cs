using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using SharpNeatLib.Experiments;

namespace SharpNeatLib.Experiments.Views
{
	public class CharacterView : System.Windows.Forms.UserControl
	{
		BitImage bitmapImage;
		char c;

		#region Component Designer variables

		private System.Windows.Forms.Panel panel1;
		private SharpNeatLib.Experiments.Views.BitImageViewport bitImageViewport1;
		private System.Windows.Forms.Label lblChar;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public CharacterView(BitImage bitImage, char c)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Keep a referecne the passed in data in case we want to redraw the control.
			this.bitmapImage = bitImage;
			this.c = c;
			lblChar.Text = c.ToString();
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.bitImageViewport1 = new SharpNeatLib.Experiments.Views.BitImageViewport();
			this.lblChar = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.bitImageViewport1);
			this.panel1.Controls.Add(this.lblChar);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(40, 80);
			this.panel1.TabIndex = 0;
			// 
			// bitImageViewport1
			// 
			this.bitImageViewport1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bitImageViewport1.Location = new System.Drawing.Point(0, 0);
			this.bitImageViewport1.Name = "bitImageViewport1";
			this.bitImageViewport1.Size = new System.Drawing.Size(36, 60);
			this.bitImageViewport1.TabIndex = 3;
			// 
			// lblChar
			// 
			this.lblChar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblChar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblChar.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblChar.Location = new System.Drawing.Point(0, 60);
			this.lblChar.Name = "lblChar";
			this.lblChar.Size = new System.Drawing.Size(36, 16);
			this.lblChar.TabIndex = 2;
			this.lblChar.Text = "a";
			this.lblChar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CharacterView
			// 
			this.Controls.Add(this.panel1);
			this.Name = "CharacterView";
			this.Size = new System.Drawing.Size(40, 80);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Methods

		public void RepaintImage()
		{
			bitImageViewport1.RebuildImage(bitmapImage);
		}

		public void SetLabel(string text, bool highlight)
		{
			lblChar.BackColor = highlight ?  Color.Red : lblChar.BackColor = Color.Green;
			lblChar.Text = text;
		}

		#endregion
	}
}
