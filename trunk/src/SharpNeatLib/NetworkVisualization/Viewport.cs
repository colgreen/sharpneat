using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

namespace SharpNeatLib.NetworkVisualization
{
	public class Viewport : System.Windows.Forms.UserControl
	{
		Brush brushBackground = new SolidBrush(Color.Cornsilk);

		NetworkModel networkModel;
		NetworkModelPainter modelPainter = new NetworkModelPainter();
		Image image;
		const PixelFormat pixelFormat = PixelFormat.Format16bppRgb565;   //.Format24bppRgb;
		Point viewportOrigin;
		Size viewportSize;
		float zoomFactor=1.0F;

		#region Component designer variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PictureBox pictureBox1;

		#endregion

		#region Constructor / Disposal

		public Viewport()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			viewportOrigin = new Point(0,0);

			// Create a bitmap for the picturebox.
			image = new Bitmap(Width, Height, pixelFormat);			
			pictureBox1.Image = image;
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(216, 232);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// Viewport
			// 
			this.Controls.Add(this.pictureBox1);
			this.Name = "Viewport";
			this.Size = new System.Drawing.Size(216, 232);
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties

		public NetworkModel NetworkModel
		{
			get
			{
				return networkModel;
			}
			set
			{
				networkModel = value;

				// Check for position info. This is needed for painting the network.
				if(!networkModel.HasPositionInfo)
					throw new ApplicationException("The provided NetworkModel is missing some or all of the required model element position information");

				RefreshImage();
			}
		}

//		public Point ViewportOrigin
//		{
//			get
//			{
//				return viewportOrigin;
//			}
//			set
//			{
//				viewportOrigin = value;
//				RefreshImage();
//			}
//		}

		public float ZoomFactor
		{
			get
			{
				return zoomFactor;
			}
			set
			{	
				zoomFactor = value;
				RefreshImage();
			}
		}

		#endregion

		#region Public Methods

		public void RefreshViewport()
		{
			RefreshImage();
		}

		#endregion

		#region Private Methods

		private void RefreshImage()
		{
			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(brushBackground, 0, 0, image.Width, image.Height);

			if(networkModel!=null)
				modelPainter.PaintNetwork(g, networkModel, zoomFactor, viewportOrigin, viewportSize, -5.0, 10.0);
			
			Refresh();
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

		private void pictureBox1_DoubleClick(object sender, System.EventArgs e)
		{
			OnDoubleClick(e);
		}

		#endregion
	}
}
