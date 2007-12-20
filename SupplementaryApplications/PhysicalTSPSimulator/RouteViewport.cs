using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using SharpNeatLib.Experiments;

namespace ptsp_entry
{
	public class RouteViewport : UserControl
	{
		#region Constants

		const int OFFSET_X = 20;
		const int OFFSET_Y = 20;

		#endregion

		#region Class Variables

		Image image;
		const PixelFormat pixelFormat = PixelFormat.Format16bppRgb565;
		Size viewportSize;
		
		//Point[] coords = null;
		//int[] route = null;
		Vector2d[] route;
		
		Brush brushBackground = new SolidBrush(Color.Cornsilk);
		Brush brushPoint = new SolidBrush(Color.Red);
		protected Pen penBlack = new Pen(Color.Black, 1.0F);
		Pen penConnection = new Pen(Color.Red);

		protected System.Windows.Forms.PictureBox pictureBox1;  

		#endregion

		#region Constructor

		public RouteViewport()
		{
			InitializeComponent();

			// Create a bitmap for the picturebox.
			image = new Bitmap(Width, Height, pixelFormat);			
			pictureBox1.Image = image;
		}

		#endregion

		#region Public Methods

		public void SetRoute(Vector2d[] route)
		{
			this.route = route;
			RefreshImage();
		}

		#endregion

		#region Private Methods

		private void RefreshImage()
		{
			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(brushBackground, 0, 0, image.Width, image.Height);

			PaintRoute(g);
			Refresh();
		}

		private void PaintRoute(Graphics g)
		{
			if(route==null)
				return;

			// Loop over each point in the route. Each integer is an index into the coords array.
			Vector2d prevCoord=new Vector2d();
			bool bNotFirstPoint=false;
			foreach(Vector2d coord in route)
			{
				PaintPoint(g, LogicalToViewport(coord));

				if(bNotFirstPoint)
				{
					g.DrawLine(penConnection, LogicalToViewport(prevCoord), LogicalToViewport(coord));
				}
				prevCoord = coord;
				bNotFirstPoint=true;
			}
		}

		int POINT_DIAMETER = 8;
		int POINT_DIAMETER_HALF = 4;

		private void PaintPoint(Graphics g, Point center)
		{
			Point p = new Point(center.X-POINT_DIAMETER_HALF, center.Y-POINT_DIAMETER_HALF);
			Size s = new Size(POINT_DIAMETER, POINT_DIAMETER);
			Rectangle r = new Rectangle(p,s);

			//g.FillEllipse(brushNeuron, r);
			g.FillRectangle(brushPoint, r);
			g.DrawRectangle(penBlack, r);
		}

		private Point LogicalToViewport(ShortCoord p)
		{
			return new Point(p.X*2 + OFFSET_X, (240-p.Y)*2 + OFFSET_Y);
		}

		private Point LogicalToViewport(Vector2d v)
		{
			return new Point((int)(v.x*2.0 + OFFSET_X), (int)((240.0-v.y)*2.0 + OFFSET_Y));
		}

		public Vector2d LogicalToViewport_Vector2d(Vector2d v)
		{
			return new Vector2d(v.x*2.0 + OFFSET_X, (240.0-v.y)*2.0 + OFFSET_Y);
		}

		#endregion

		#region Component Designer Methods

		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(150, 150);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
			// 
			// SequenceViewport
			// 
			this.Controls.Add(this.pictureBox1);
			this.Name = "SequenceViewport";
			this.ResumeLayout(false);

		}

		#endregion

		#region Event Handlers

		private void pictureBox1_SizeChanged(object sender, System.EventArgs e)
		{
			const float SIZE_CHANGE_DELTA = 200;

			//System.Diagnostics.Debug.WriteLine("W=" + Width + ", H=" + Height);
			viewportSize = new Size(Width, Height);

			// Required during control initialization.
			if(image==null)
				return;

			if(pictureBox1.Width > image.Width || pictureBox1.Height > image.Height) 
				//				pictureBox1.Width < image.Width-SIZE_CHANGE_DELTA || pictureBox1.Height < image.Height-SIZE_CHANGE_DELTA)
			{	// Reset the image's size.
				int width = (int)(Math.Ceiling((float)pictureBox1.Width / SIZE_CHANGE_DELTA) * SIZE_CHANGE_DELTA);
				int height = (int)(Math.Ceiling((float)pictureBox1.Height / SIZE_CHANGE_DELTA) * SIZE_CHANGE_DELTA);
				image = new Bitmap(width, height, pixelFormat);
				pictureBox1.Image = image;

				RefreshImage();
				//				System.Diagnostics.Debug.WriteLine("w=" + width + ", h=" + height);
			}
		}

		#endregion
	}
}
