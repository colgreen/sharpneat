using System;
using System.Collections;
using System.Drawing;
using SharpNeatLib.Experiments;

namespace ptsp_entry
{

	public class MonitorRouteViewport : RouteViewport
	{
		/// <summary>
		/// Points that describe the path of the agent.
		/// </summary>
		private ArrayList points = new ArrayList();

		public void AddPoint(Vector2d p)
		{
			points.Add(p);
		}

		public void ResetPoints()
		{
			points.Clear();
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint (e);

			Graphics g = pictureBox1.CreateGraphics();
			foreach(Vector2d v in points)
			{
				g.DrawEllipse(penBlack, (float)v.x, (float)v.y, 1F, 1F);
			}
		}
	}
}
