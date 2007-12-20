using System;
using System.Drawing;

namespace RudyGraph
{
	/// <summary>
	/// Represents a fixed set of plot points that can be bound to an Axis.
	/// </summary>
	public class PlotSet : IPlotSet
	{
		#region Class Variables

		double[] plotValueArray;
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
		/// Constructs a PlotSet with the provided array of doubles and default styles.
		/// </summary>
		/// <param name="plotValueArray"></param>
		public PlotSet(double[] plotValueArray) : this(plotValueArray, LineType.Straight, PlotPointType.None, Color.Black, Color.Black)
		{}

		/// <summary>
		/// Constructs a PlotSet the provided array of doubles and associated line and point styles.
		/// </summary>
		/// <param name="plotValueArray"></param>
		/// <param name="lineType"></param>
		/// <param name="plotPointType"></param>
		/// <param name="lineColor"></param>
		/// <param name="plotPointColor"></param>
		public PlotSet(double[] plotValueArray, LineType lineType, PlotPointType plotPointType, Color lineColor, Color plotPointColor)
		{
			// Create a copy of the array, otherwise the caller could modify at any time.
			this.plotValueArray = (double[])plotValueArray.Clone();
			this.lineType = lineType;
			this.plotPointType = plotPointType;
			this.lineColor = lineColor;
			this.plotPointColor = plotPointColor;

			UpdateMinMax();
		}

		#endregion

		#region Properties / Indexers

		public int Width
		{
			get
			{
				return plotValueArray.Length;
			}
		}

		public int Count
		{
			get
			{
				return plotValueArray.Length;
			}
		}

		public double this[int index]
		{
			get
			{
				return plotValueArray[index];
			}
		}

		public double MinValue
		{
			get
			{
				return minValue;
			}
		}

		public double MaxValue
		{
			get
			{
				return maxValue;
			}
		}

		
		public LineType LineType
		{
			get
			{
				return lineType;
			}
		}
		
		public PlotPointType PlotPointType
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
		
		/// <summary>
		/// Loads new values into the plot set replacing existing values. The new values are loaded in starting at 
		/// element 0 of the plot set. If the provided array is shorter than the plot set then all of the value are 
		/// loaded and existing plot points remain unchanged at the end of the plot set. 
		/// If the provided array is longe rthan the plot set then the excess values are not loaded, the plot set
		/// does not grow in size.
		/// The min and max values for the plot set are maintained.
		/// </summary>
		/// <param name="valueArray"></param>
		public void LoadValues(double[] valueArray)
		{
			OnBeforeChange();
			int bound = Math.Min(plotValueArray.Length, valueArray.Length);
			for(int i=0; i<bound; i++)
			{
				plotValueArray[i] = valueArray[i];
			}

			if(bound>0) UpdateMinMax();
			OnAfterChange();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the minValue and maxValue variables by looping over the values in the plot set.
		/// </summary>
		private void UpdateMinMax()
		{
			if(plotValueArray.Length==0)
			{
				throw new Exception("Empty plot point array.");
			}

			minValue = maxValue = plotValueArray[0];
			for(int i=1; i<plotValueArray.Length; i++)
			{
				if(plotValueArray[i]<minValue)
					minValue = plotValueArray[i];
				else if(plotValueArray[i]>maxValue)
					maxValue = plotValueArray[i];
			}
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
