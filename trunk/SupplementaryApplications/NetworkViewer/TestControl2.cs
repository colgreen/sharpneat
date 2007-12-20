using System;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetworkViewer
{

	public class TestControl2 : PictureBox
	{
		SolidBrush brushBackground = new SolidBrush(Color.White);
		Point dot = new Point(0,0);

		public TestControl2()
		{
			this.Image = new Bitmap(300, 100);

			this.MouseDown += new MouseEventHandler(OnMouseDown);
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{ 
			dot = new Point(e.X, e.Y);

			Graphics g = Graphics.FromImage(this.Image);
			g.FillEllipse(brushBackground, dot.X, dot.Y, 10, 10);


			
			this.Refresh();
		}

//		protected override void OnPaint(PaintEventArgs pe)
//		{
//			base.OnPaint(pe);
//
//			Graphics  g				= pe.Graphics;
//			Rectangle invalidRect	= pe.ClipRectangle;
//
//			g.CompositingQuality	= CompositingQuality.HighSpeed;
//			g.SmoothingMode			= SmoothingMode.HighSpeed;
//			g.InterpolationMode		= InterpolationMode.Low;
//
//			
//
//			g.FillEllipse(brushBackground, dot.X, dot.Y, 10, 10);
//			
//			// Paint background.
//			//g.DrawRectangle(brushBackground, invalidRect);
//		}

	}
}
