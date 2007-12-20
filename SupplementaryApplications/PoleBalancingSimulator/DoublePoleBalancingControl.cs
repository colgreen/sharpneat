using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using SharpNeatLib.Experiments;


namespace SharpNeat.PoleBalancingSimulator
{
	public class DoublePoleBalancingControl : System.Windows.Forms.UserControl
	{
		#region Class Variables

		DoublePoleBalancingNetworkEvaluator poleBalancing;

		Pen penTrack = new Pen(Color.Black, 2);
		Pen penPole = new Pen(Color.Green, 2);
		SolidBrush brushBackground = new SolidBrush(Color.White);
		SolidBrush brushCart = new SolidBrush(Color.Red);
		SolidBrush brushForce = new SolidBrush(Color.Black);

		#endregion
	
		#region Windows Form Designer Variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public DoublePoleBalancingControl(DoublePoleBalancingNetworkEvaluator poleBalancing)
		{
		//----- Store reference to the polebalancing simulator object.
			this.poleBalancing = poleBalancing;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

		//----- Prepare control for our own painting routines.
			// Request that only invalid regions are redrawn.
			SetStyle(ControlStyles.ResizeRedraw, false);

			// Enable double-buffering.
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
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
			// 
			// PoleBalancingControl
			// 
			this.Name = "PoleBalancingControl";
		}

		#endregion

		#region Private Methods [Control Painting]

		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics  g				= pe.Graphics;
			Rectangle invalidRect	= pe.ClipRectangle;

			g.CompositingQuality	= CompositingQuality.HighSpeed;
			g.SmoothingMode			= SmoothingMode.HighSpeed;
			g.InterpolationMode		= InterpolationMode.Low;
 
			// Paint background.
			g.FillRectangle(brushBackground, invalidRect);

			PaintTrack(g);
			PaintPoles(g);
			PaintCart(g);
		}

		private void PaintTrack(Graphics g)
		{
			int y = TrackGdiY;
			int xLeft = GetGdiX(-poleBalancing.TrackLength/2.0);
			int xRight = GetGdiX(poleBalancing.TrackLength/2.0);
			g.DrawLine(penTrack, xLeft, y, xRight, y);
		}

		private void PaintCart(Graphics g)
		{
			int cartWidth = GetGdiYLength(1.0)/5;

			// Draw the cart.
			int x = GetGdiX(poleBalancing.CartPosX);
			g.FillRectangle(brushCart, x-cartWidth/2, TrackGdiY, cartWidth, 5);

			// Draw a force arrow acting on the cart.
			Point[] triangle = new Point[3];

			// Vary the width to represent the action force magnitude.
			int arrowWidth = 5+(int)Math.Abs(((poleBalancing.Action-0.5) * 50.0));
			int arrowHeightHalfed = 5+(int)Math.Abs(((poleBalancing.Action-0.5) * 5.0));
			int xArrowHead;
			if(poleBalancing.Action>0.5)
			{
				xArrowHead = x-cartWidth/2;
				triangle[0] = new Point(xArrowHead-arrowWidth, TrackGdiY+arrowHeightHalfed);
				triangle[1] = new Point(xArrowHead-arrowWidth, TrackGdiY-arrowHeightHalfed);
				triangle[2] = new Point(xArrowHead, TrackGdiY);	
			}
			else if(poleBalancing.Action<0.5)
			{
				xArrowHead = x+(cartWidth/2);
				triangle[0] = new Point(xArrowHead+arrowWidth, TrackGdiY+arrowHeightHalfed);
				triangle[1] = new Point(xArrowHead+arrowWidth, TrackGdiY-arrowHeightHalfed);
				triangle[2] = new Point(xArrowHead, TrackGdiY);
			}
			g.FillPolygon(brushForce, triangle);
		}

		private void PaintPoles(Graphics g)
		{
			int pole1GdiLength = GetGdiYLength(1.0);
			int pole2GdiLength = GetGdiYLength(0.3);

			int xOffset = (int)(pole1GdiLength/10.0);

			// Pole 1
			int xBase = GetGdiX(poleBalancing.CartPosX);
			int yBase = TrackGdiY;

			int xTip = xBase+(int)(Math.Sin(poleBalancing.Pole1Angle) * pole1GdiLength);
			int yTip = yBase - (int)(Math.Cos(poleBalancing.Pole1Angle) * pole1GdiLength);
			
			g.DrawLine(penPole, xBase-xOffset, yBase, xTip-xOffset, yTip);

			// Pole 2
			xBase = GetGdiX(poleBalancing.CartPosX);
			yBase = TrackGdiY;

			xTip = xBase+(int)(Math.Sin(poleBalancing.Pole2Angle) * pole2GdiLength);
			yTip = yBase - (int)(Math.Cos(poleBalancing.Pole2Angle) * pole2GdiLength);
			
			g.DrawLine(penPole, xBase+xOffset, yBase, xTip+xOffset, yTip);
		}

		#endregion

		#region Private Methods [Coordinate Mapping]

		private int GetGdiX(double trackX)
		{
			const int gdiBufferWidth = 40;

			double cartDistanceFromLeft = (trackX + poleBalancing.TrackLength/2.0);
			double cartProportionFromLeft = (cartDistanceFromLeft / poleBalancing.TrackLength);

			return (int)(((Width - (gdiBufferWidth*2.0)) * cartProportionFromLeft)+gdiBufferWidth);
		}

		const double MAX_SIM_Y = 1.0;
		const int gdiBaseBuffer=15;
		const int gdiTopBuffer=10;
		const int gdiTotalBuffer= gdiBaseBuffer+gdiTopBuffer;
		private int GetGdiY(double Y)
		{
			// Proportion from bottom (simulation).
			double proportionFromTop = Math.Min(MAX_SIM_Y, MAX_SIM_Y-Y)/MAX_SIM_Y;

			return gdiTopBuffer + (int)(Math.Max(0, Height-gdiTotalBuffer)*(proportionFromTop*MAX_SIM_Y));
		}

		private int GetGdiYLength(double Y)
		{
			return (int)((Y/MAX_SIM_Y)*Math.Max(0, Height-gdiTotalBuffer));
		}

		int TrackGdiY
		{
			get
			{
				return GetGdiY(0);
			}
		}

		#endregion
	}
}
