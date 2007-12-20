using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RudyGraph
{

	/// <summary>
	/// RudyGraphControl is so called because it is a rudimentary graph control.
	/// The control can show multiple sets of points represented by IPlotSet, each as a line with
	/// a configurable color and plot point symbol.
	/// IPlotSet objects are bound to one of this graph's two Axes, the PrimaryYAxis or the SecondaryYAxis.
	/// The range of the axes can be fixed by setting the appropriate properties on the Axes, or by 
	/// default the range will be determined automatically based on the range of values withinthe IPlotSets
	/// bound to each axis.
	/// 
	/// Static graphs can be produced using the PlotSet class, or live rolling graphs can be produced using the
	/// RollingPlotSetClass. Simply call the Enqueue() method to add more data points to the plot set, older
	/// values will be overwritten when the set fills up and the graph will redraw automatically.
	/// 
	/// Visually the graph is rudamentary showing only the two Y axes, their min and max values and the plotted 
	/// lines. Additonla decoration of the graph such as titles can be added on top of this control. Grids and
	/// axis ticks will require a little extra code within this control itself, but shouldn't be too difficult.
	/// </summary>
	public class RudyGraphControl : System.Windows.Forms.UserControl
	{
		#region Class Variables [Control Painting]

		int updateLevel=0;
		Brush brushBackground = new SolidBrush(Color.Cornsilk);
		PictureBox pictureBox1;
		Image image;
		const PixelFormat pixelFormat = PixelFormat.Format16bppRgb565;  

		static Brush brushBlack = new SolidBrush(Color.Black);
		static Pen penBlack = new Pen(Color.Black, 1.0F);
		static Font fontDefault = new Font("Microsoft Sans Serif", 7.0F);

		#endregion

		#region Class Variables [Graph Data]

		Axis primaryYAxis;
		Axis secondaryYAxis;

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		public RudyGraphControl()
		{
			InitialiseControl();

			primaryYAxis = new Axis();
			secondaryYAxis = new Axis();

			primaryYAxis.AfterChange +=new EventHandler(Axis_AfterChange);
			secondaryYAxis.AfterChange +=new EventHandler(Axis_AfterChange);

			RefreshImage();
		}

		#endregion
		
		#region Properties

		/// <summary>
		/// Primary Y Axis (left hand side). Bind IPlotSet objects to this to add lines to the graph control.
		/// </summary>
		public Axis PrimaryYAxis
		{
			get
			{
				return primaryYAxis;
			}
		}

		/// <summary>
		/// Secondary Y Axis (right hand side). Bind IPlotSet objects to this to add lines to the graph control.
		/// </summary>
		public Axis SecondaryYAxis
		{
			get
			{
				return secondaryYAxis;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Prevents the control from repainting until EndUpdate() is called. This allows for multiple
		/// changes to be made without causing lots of expensive repainting to occur.
		/// </summary>
		public void BeginUpdate()
		{
			updateLevel++;	
		}

		/// <summary>
		/// Ends an update. See BeginUpdate().
		/// </summary>
		public void EndUpdate()
		{
			updateLevel = Math.Max(0, --updateLevel);

			if(updateLevel==0)
			{
				RefreshImage();
			}
		}


		#endregion

		#region Constants [Painting]

		/// <summary>
		/// The size of the margin between the edge of the control and the axes.
		/// </summary>
		const int MARGIN_SIZE = 30;

		#endregion

		#region Methods [Control Painting]

		private void InitialiseControl()
		{
			this.SuspendLayout();
			this.Name = "RudyGraphControl";

			// 
			// pictureBox1
			// 
			this.pictureBox1 = new System.Windows.Forms.PictureBox();		
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
			this.Controls.Add(pictureBox1);

			// Create a bitmap for the picturebox.
			image = new Bitmap(Width, Height, pixelFormat);		
			pictureBox1.Image = image;
	
			this.ResumeLayout(false);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			const float SIZE_CHANGE_DELTA = 150;

			base.OnSizeChanged(e);

			// Create a new Image for the picturebox if the size has increased. 
			if(		(pictureBox1.Width > image.Width || pictureBox1.Height > image.Height)
				|| 	(pictureBox1.Width<image.Width-SIZE_CHANGE_DELTA || pictureBox1.Height<image.Height-SIZE_CHANGE_DELTA))
				//				
			{	// Reset the image's size in increments of SIZE_CHANGE_DELTA.
				int width = (int)(Math.Ceiling((float)pictureBox1.Width / SIZE_CHANGE_DELTA) * SIZE_CHANGE_DELTA);
				int height = (int)(Math.Ceiling((float)pictureBox1.Height / SIZE_CHANGE_DELTA) * SIZE_CHANGE_DELTA);

				if(width>0 && height >0)
				{
					image = new Bitmap(width, height, pixelFormat);
					pictureBox1.Image = image;
				}
			}

			RefreshImage();
		}

		private void RefreshImage()
		{
			if(updateLevel>0)
				return;

			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(brushBackground, 0, 0, image.Width, image.Height);
			PaintControl(g);

			Refresh();
		}
		
		private void PaintControl(Graphics g)
		{
			if(this.Width <= MARGIN_SIZE*2 || this.Height <= MARGIN_SIZE*2)
			{	// Control is too small to paint onto! Leave it blank.
				return;
			}

			// Calc some baseline variables.
			int x_left = MARGIN_SIZE;
			int x_right = this.Width - MARGIN_SIZE;
			int y_top = MARGIN_SIZE;
			int y_bottom = this.Height - MARGIN_SIZE;

			// Paint axes.
			g.DrawLine(penBlack, x_left, y_top, x_left, y_bottom); 
			g.DrawLine(penBlack, x_right, y_top, x_right, y_bottom);
			g.DrawLine(penBlack, x_left, y_bottom, x_right, y_bottom);

			// Display Y-axis ranges and plot sets.
			if(primaryYAxis.PlotSetCount>0)
			{
				double rangeMin = primaryYAxis.RangeMin;
				double rangeMax = primaryYAxis.RangeMax;

				g.DrawString(rangeMin.ToString("f2"), fontDefault, brushBlack, MARGIN_SIZE/6, y_bottom-8);
				g.DrawString(rangeMax.ToString("f2"), fontDefault, brushBlack, MARGIN_SIZE/6, y_top-5);

				for(int i=0; i<primaryYAxis.PlotSetCount; i++)
					PaintPlotSet(g, rangeMin, rangeMax, primaryYAxis[i]);
			}

			if(secondaryYAxis.PlotSetCount>0)
			{
				double rangeMin = secondaryYAxis.RangeMin;
				double rangeMax = secondaryYAxis.RangeMax;

				g.DrawString(rangeMin.ToString("f2"), fontDefault, brushBlack, x_right + MARGIN_SIZE/6, y_bottom-8);
				g.DrawString(rangeMax.ToString("f2"), fontDefault, brushBlack, x_right + MARGIN_SIZE/6, y_top-5);

				for(int i=0; i<secondaryYAxis.PlotSetCount; i++)
					PaintPlotSet(g, rangeMin, rangeMax, secondaryYAxis[i]);
			}
		}

		private void PaintPlotSet(Graphics g, double rangeMin, double rangeMax, IPlotSet plotSet)
		{
			if(plotSet.Count<2 || plotSet.Width<2)
			{	// Can't plot less than two points. This is a line graph! 
				return;
			}

			// If there is no range than create one artificially, presumably teh data is a single
			// point or a straight horizontal line, so the range is irrelevant.
			if(Math.Abs(rangeMin-rangeMax)<=0.1)
			{
				rangeMin -= 0.5;
				rangeMax += 0.5;
			}

			Pen penLine = new Pen(plotSet.LineColor, 1F);

			float x_incr = (float)(Width-(MARGIN_SIZE+MARGIN_SIZE+2)) / (float)(plotSet.Width-1);
			float x_curr = MARGIN_SIZE+1;

			double y_range_data = rangeMax - rangeMin;
			double y_range_pixels = Height-(MARGIN_SIZE+MARGIN_SIZE+2);
			int y_min_pixels = Height-(MARGIN_SIZE+1);

			Point p1 = new Point((int)x_curr, y_min_pixels-(int)(((plotSet[0]-rangeMin)/y_range_data)*y_range_pixels));
			Point p2 = new Point((int)x_curr, 0);

			for(int i=1; i<plotSet.Count; i++)
			{
				x_curr += x_incr;
				p2.X = (int)x_curr;
				p2.Y = y_min_pixels-(int)(((plotSet[i]-rangeMin)/y_range_data)*y_range_pixels);

				g.DrawLine(penLine, p1, p2);

				p1=p2;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if(updateLevel>0)
				return;

			base.OnPaint (e);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if(updateLevel>0)
				return;

			base.OnPaintBackground (pevent);
		}

		#endregion

		#region Event Handlers

		private void Axis_AfterChange(object sender, EventArgs e)
		{
			RefreshImage();
		}

		#endregion
	}
}
