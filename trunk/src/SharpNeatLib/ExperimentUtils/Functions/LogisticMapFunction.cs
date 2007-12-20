using System;

using SharpNeatLib;

namespace SharpNeatLib.Experiments
{
	public class LogisticMapFunction : IFunction
	{
		#region Class Variables

		// Logistic map parameters.
		const double  x_init = 0.8;
		const double  r = 4.0;
		
//		// Reading_interval defines the rate at which we take a reading from the samples.
//		int reading_interval = 10;

		//--- Working variables.
		double x_current;
		
		#endregion

		#region Private Methods

		private void InitialiseFunction()
		{
			x_current = x_init;
		}

		private double GetNextValue()
		{
			x_current = r * x_current*(1-x_current);
			return x_current;
		}

		#endregion

		#region IFunction Members

		public double[] GetFunctionValueArray(int length)
		{
			InitialiseFunction();

			double[] valueArray = new double[length];

			for(int i=0; i<length; i++)
			{
//				// Skip some samples.
//				for(int j=1; j<reading_interval; j++)
//					GetNextValue();

				valueArray[i] = GetNextValue();
			}

			Utilities.NormalizeValueArray(0.1, 0.9, valueArray);
			return valueArray;
		}

		#endregion
	}
}
