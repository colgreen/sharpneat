using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

using SharpNeatLib.Experiments;

namespace SharpNeatLib.Experiments.Views
{
	public class BitImageViewport : System.Windows.Forms.UserControl
	{
		PictureBox moPictureBox=null;
		Image image;
		BitImage bitImage;

		Pen penBlack = new Pen(Color.Black, 1.0F);
		Brush brushBlack = new SolidBrush(Color.Black);
		Brush brushWhite = new SolidBrush(Color.White);

		#region Component Designer variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public BitImageViewport()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Create a new picture box
			moPictureBox = new PictureBox();
			moPictureBox.Dock = DockStyle.Fill;
			this.Controls.Add(moPictureBox);
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Rebuild the underlying picture box using the controls current size as a basis.
		/// </summary>
		/// <param name="bitmapImage"></param>
		public void RebuildImage(BitImage bitImage)
		{
			this.bitImage = bitImage;
			image = new Bitmap(moPictureBox.Width, moPictureBox.Height, PixelFormat.Format16bppRgb565);
			moPictureBox.Image = image;
			PaintBitImage();
		}

		#endregion

		#region Private Methods

		private void PaintBitImage()
		{
			Graphics g = Graphics.FromImage(image);
			
			// Paint the background white.
			g.FillRectangle(brushWhite, 0, 0, image.Width, image.Height);


			float pixelsPerBitX = image.Width / bitImage.Width ;
			float pixelsPerBitY = image.Height / bitImage.Height;

			int blockWidth = (int)Math.Ceiling(pixelsPerBitX);
			int blockHeight = (int)Math.Ceiling(pixelsPerBitY);

			for(int bitIdx_X=0; bitIdx_X<bitImage.Width; bitIdx_X++)
			{
				for(int bitIdx_Y=0; bitIdx_Y<bitImage.Height; bitIdx_Y++)
				{
					if(bitImage.GetPixel(bitIdx_X, bitIdx_Y))
					{
						int pixelX = (int)(bitIdx_X * pixelsPerBitX);
						int pixelY = (int)(bitIdx_Y * pixelsPerBitY);

						g.FillRectangle(brushBlack, pixelX, pixelY, blockWidth, blockHeight);
						//g.DrawRectangle(penBlack, pixelX, pixelY, blockWidth, blockHeight);
					}
				}
			}
		}

		#endregion
	}
}
