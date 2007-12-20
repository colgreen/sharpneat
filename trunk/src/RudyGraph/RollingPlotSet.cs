using System;
using System.Drawing;

namespace RudyGraph
{

	/// <summary>
	/// Represents a rolling set of plot points that can be bound to an Axis.
	/// A rolling set has a fixed capacity but the number of valid values within the set varies
	/// and is initially zero. New values can be added to the set using the Enqueue method to 
	/// fill up the set up to its capacity. Enqueuing values once the set is full to capacity causes
	/// the oldest value in the set to be overwritten.
	/// </summary>
	public class RollingPlotSet : IPlotSet
	{
		#region Class Variables

		DoubleCircularBuffer plotValueBuffer;
		int validCount;
		double minValue;
		double maxValue;

		LineType lineType;
		PlotPointType plotPointType;
		Color lineColor;
		Color plotPointColor;

		#endregion

		#region Event Declarations

		public event EventHandler BeforeChange;
		public event EventHandler AfterChange;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a RollingPlotSet with a specified capacity and default styles.
		/// </summary>
		/// <param name="size"></param>
		public RollingPlotSet(int size) : this(size, LineType.Straight, PlotPointType.Circle, Color.Black, Color.Black)
		{}

		/// <summary>
		/// Constructs a RollingPlotSet with a specified capacity and styles.
		/// </summary>
		/// <param name="size"></param>
		/// <param name="lineType"></param>
		/// <param name="plotPointType"></param>
		/// <param name="lineColor"></param>
		/// <param name="plotPointColor"></param>
		public RollingPlotSet(int size, LineType lineType, PlotPointType plotPointType, Color lineColor, Color plotPointColor)
		{
			this.plotValueBuffer = new DoubleCircularBuffer(size);
			this.validCount = 0;
			this.lineType = lineType;
			this.plotPointType = plotPointType;
			this.lineColor = lineColor;
			this.plotPointColor = plotPointColor;

			minValue = 0.0;
			maxValue = 0.0;
		}

		#endregion

		#region Properties / Indexers

		public int Width
		{
			get
			{
				return plotValueBuffer.Length;
			}
		}

		public int Count
		{
			get
			{
				return plotValueBuffer.Length;
			}
		}

//		public int ValidCount
//		{
//			get
//			{
//				return plotValueBuffer.Length;
//			}
//		}

		public double this[int index]
		{
			get
			{
				return plotValueBuffer[index];
			}
		}

		public double MinValue
		{
			get
			{
				return plotValueBuffer.Min;
			}
		}

		public double MaxValue
		{
			get
			{
				return plotValueBuffer.Max;
			}
		}

		public RudyGraph.LineType LineType
		{
			get
			{
				return lineType;
			}
		}

		public RudyGraph.PlotPointType PlotPointType
		{
			get
			{
				return plotPointType;
			}
		}

		public Color LineColor
		{
			get
			{
				return lineColor;
			}
		}

		public Color PlotPointColor
		{
			get
			{
				return plotPointColor;
			}
		}

		#endregion

		#region Public Methods

		public void Clear()
		{
			OnBeforeChange();
			plotValueBuffer.Clear();
			OnAfterChange();
		}

		public void Enqueue(double item)
		{
			OnBeforeChange();
			plotValueBuffer.Enqueue(item);
			OnAfterChange();
		}

		#endregion

		#region Private Methods [Event Triggers]

		private void OnBeforeChange()
		{
			if(BeforeChange!=null)
				BeforeChange(this, EventArgs.Empty);
		}

		private void OnAfterChange()
		{
			if(AfterChange!=null)
				AfterChange(this, EventArgs.Empty);
		}

		#endregion
	}
}
