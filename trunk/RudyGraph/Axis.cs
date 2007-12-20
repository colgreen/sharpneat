using System;
using System.Collections;

namespace RudyGraph
{
	/// <summary>
	/// This class is currently used for representing Y-Axes. PlotSet objects can be bound to an 
	/// axis and this allows the axis to determine the range of values that it is representing.
	/// An axis can also be created with a fixed range so that plot points outside the range are 
	/// not shown.
	/// </summary>
	public class Axis
	{
		#region Constants

		const double RANGEMIN_DEFAULT = 0.0;
		const double RANGEMAX_DEFAULT = 100.0;

		#endregion

		#region Class Variables

		ArrayList plotSetList = new ArrayList();
		bool fixedRangeFlag;
		double fixedRangeMin;
		double fixedRangeMax;

		#endregion

		#region Event Declarations

		public event EventHandler BeforeChange;
		public event EventHandler AfterChange;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor. Creates an Axis with no fixed range.
		/// </summary>
		public Axis()
		{
			this.fixedRangeMin = RANGEMIN_DEFAULT;
			this.fixedRangeMax = RANGEMAX_DEFAULT;
			this.fixedRangeFlag = false;
		}

		/// <summary>
		/// Creates an Axis with a fixed range.
		/// </summary>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		public Axis(double minValue, double maxValue)
		{
			this.fixedRangeMin = minValue;
			this.fixedRangeMax = maxValue;
			this.fixedRangeFlag = true;
		}

		#endregion

		#region Properties / Indexers

		/// <summary>
		/// The number of PlotSets bound to this axis.
		/// </summary>
		public int PlotSetCount
		{
			get
			{
				return plotSetList.Count;
			}
		}

		/// <summary>
		/// Gets the PlotSet at the specified index.
		/// </summary>
		public IPlotSet this[int index]
		{
			get
			{
				return (IPlotSet)plotSetList[index];
			}
		}

		/// <summary>
		/// Gets a flag indicating if this Axis has a fixed range.
		/// </summary>
		public bool FixedRangeFlag
		{
			get
			{
				return fixedRangeFlag;
			}
			set
			{
				OnBeforeChange();
				fixedRangeFlag = value;
				OnAfterChange();
			}
		}

		/// <summary>
		/// If the Axis has a fixed range then gets or sets the range's minimum.
		/// </summary>
		public double FixedRangeMin
		{	
			get
			{
				return fixedRangeMin;
			}
			set
			{
				if(value>=fixedRangeMax)
				{	// Ignore the value.
					return;
				}

				OnBeforeChange();
				fixedRangeMin = value;
				OnAfterChange();
			}
		}	

		/// <summary>
		/// If the Axis has a fixed range then gets or sets the range's maximum.
		/// </summary>
		public double FixedRangeMax
		{	
			get
			{
				return fixedRangeMax;
			}
			set
			{
				if(value<=fixedRangeMin)
				{	// Ignore the value.
					return;
				}

				OnBeforeChange();
				fixedRangeMax = value;
				OnAfterChange();
			}
		}

		/// <summary>
		/// Gets the range minimum for the Axis. If the range is fixed then this is the same as
		/// FixedRangeMin, otherwise it is lowest value in all of the PlotSets bound to this axis.
		/// If all of the PlotSets are empty then this is 0.
		/// </summary>
		public double RangeMin
		{
			get
			{
				if(fixedRangeFlag)
				{
					return fixedRangeMin;
				}

				if(plotSetList.Count==0)
					return RANGEMIN_DEFAULT;

				double min = ((IPlotSet)plotSetList[0]).MinValue;
 
				for(int i=1; i<plotSetList.Count; i++)
				{
					min = Math.Min(min, ((IPlotSet)plotSetList[i]).MinValue);
				}

				return min;
			}
		}

		/// <summary>
		/// Gets the range maximum for the Axis. If the range is fixed then this is the same as
		/// FixedRangeMax, otherwise it is highest value in all of the PlotSets bound to this axis.
		/// If all of the PlotSets are empty then this is 0.
		/// </summary>
		public double RangeMax
		{
			get
			{
				if(fixedRangeFlag)
				{
					return fixedRangeMax;
				}

				if(plotSetList.Count==0)
					return RANGEMAX_DEFAULT;

				double max = ((IPlotSet)plotSetList[0]).MaxValue;
 
				for(int i=1; i<plotSetList.Count; i++)
				{
					max = Math.Max(max, ((IPlotSet)plotSetList[i]).MaxValue);
				}

				return max;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Binds a PlotSet to this Axis.
		/// </summary>
		/// <param name="plotSet"></param>
		public void Bind(IPlotSet plotSet)
		{
			OnBeforeChange();
			plotSetList.Add(plotSet);
			plotSet.BeforeChange += new EventHandler(plotSet_BeforeChange);
			plotSet.AfterChange += new EventHandler(plotSet_AfterChange);
			OnAfterChange();
		}

		/// <summary>
		/// Unbinds a PlotSet from this Axis.
		/// </summary>
		/// <param name="plotSet"></param>
		public void UnBind(IPlotSet plotSet)
		{
			OnBeforeChange();
			plotSet.BeforeChange -= new EventHandler(plotSet_BeforeChange);
			plotSet.AfterChange -= new EventHandler(plotSet_AfterChange);
			plotSetList.Remove(plotSet);
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

		#region Private Methods [Event Handler]

		private void plotSet_BeforeChange(object sender, EventArgs e)
		{	
			// Pass on the event to listeners to his axis.
			OnBeforeChange();
		}

		private void plotSet_AfterChange(object sender, EventArgs e)
		{	
			// Pass on the event to listeners to his axis.
			OnAfterChange();
		}
		
		#endregion
	}
}
