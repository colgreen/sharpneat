using System;
using System.Drawing;

namespace RudyGraph
{
	/// <summary>
	/// Describes the interface for a class that can be used to convey sets of plot points that
	/// can be plotted on a RudyGraphControl.
	/// </summary>
	public interface IPlotSet
	{
		#region Event Declarations

		/// <summary>
		/// Occurs before a change to the plot set data.
		/// </summary>
		event EventHandler BeforeChange;
		/// <summary>
		/// Occurs after a change to the plot set data.
		/// </summary>
		event EventHandler AfterChange;

		#endregion

		#region Properties / Indexers

		/// <summary>
		/// Width indicates to the graph drawing routine how compact the points in the set should be drawn.
		/// E.g. If we  width==100 and there are only 50 points (Count==50) then the line will be drawn in the 
		/// left hand 50% of the graph. 
		/// </summary>
		int Width
		{
			get;
		}

		/// <summary>
		/// The total number of plot points in the set. Note that some of these may not be valid, 
		/// see ValidCount.
		/// </summary>
		int Count
		{
			get;
		}

//		/// <summary>
//		/// The number of valid plot points in the set. This number is less than or equal to the Count
//		/// property. A valid point is one that can be plotted, and therefore an 'invalid' point is
//		/// one that should not plotted because its value has not been set yet, or isn't valid.
//		/// The RollingPlotSet is is a good example of how this property is used. A RollingPlotSet
//		/// has a fixed size but may not contain any valid plot points when it is initialised. Thus
//		/// the ValidCount will be zero. As points are added the plot points are drawn on the graph
//		/// from left to right, but spaced as though there are Count points so that the line grows across
//		/// the graph control
//		/// </summary>
//		int ValidCount
//		{
//			get;
//		}

		/// <summary>
		/// The plot value at the specified index in the plot set.
		/// </summary>
		double this[int index]
		{
			get;
		}

		/// <summary>
		/// The lowest value in the plot set.
		/// </summary>
		double MinValue
		{
			get;
		}

		/// <summary>
		/// The highest value in the plot set.
		/// </summary>
		double MaxValue
		{
			get;
		}

		/// <summary>
		/// The type of line we wish the plot set to be drawn with.
		/// </summary>
		LineType LineType
		{
			get;
		}

		/// <summary>
		/// Thetype of point we wish plot set points to be drawn with.
		/// </summary>
		PlotPointType PlotPointType
		{
			get;
		}

		/// <summary>
		/// The color we wish the plot set point to be drawn with.
		/// </summary>
		Color LineColor
		{
			get;
		}

		/// <summary>
		/// The color we wish the plot set points to be drawn with.
		/// </summary>
		Color PlotPointColor
		{
			get;
		}

		#endregion
	}
}
