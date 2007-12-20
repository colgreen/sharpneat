using System;

namespace SharpNeatLib
{
	/// <summary>
	/// Very similar to a System.Drawing.Point. Except it isn't a struct (which eliminates
	/// need for boxing) and you don't need a reference to System.Drawing.
	/// </summary>
	public class Coord
	{
		public int x;
		public int y;

		public Coord(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}
}
