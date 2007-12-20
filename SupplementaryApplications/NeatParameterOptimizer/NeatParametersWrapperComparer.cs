using System;
using System.Collections;

namespace NeatParameterOptimizer
{
	public class NeatParametersWrapperComparer : IComparer
	{
		#region IComparer Members

		/// <summary>
		/// Implement contrary to the standard - ascending fitness instead of descending.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			// Test the most likely cases first.
			if(((NeatParametersWrapper)x).Fitness < ((NeatParametersWrapper)y).Fitness)
			{
				return 1;
			}
			else if (((NeatParametersWrapper)x).Fitness > ((NeatParametersWrapper)y).Fitness)
			{
				return -1;
			}
			else
			{
				// Secondary sort on the orderRandomizer.
				return ((NeatParametersWrapper)y).OrderRandomizer - ((NeatParametersWrapper)x).OrderRandomizer;
			}
		}

		#endregion
	}
}
