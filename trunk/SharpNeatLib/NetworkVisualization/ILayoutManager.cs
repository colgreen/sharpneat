using System;
using System.Drawing;

namespace SharpNeatLib.NetworkVisualization
{
	public interface ILayoutManager
	{
		void Layout(NetworkModel nm, Size areaSize);
	}
}
