using System;

using SharpNeatLib;

namespace SharpNeatLib.Experiments
{
	public class MackyGlassFunction : IFunction
	{
		#region Class Variables

		//--- Mackey-Glass parameters.
		const double a = 0.2;
		const double b = 0.1;
		const double c = 10.0;
		const double tau = 17.0;

		//--- Sampling parameters.
		double x_init = 0.8;
		double t_delta = 0.1;
		int x_history_length; 

		// t_delta defines the sampling resolution. reading_interval defines the rate at which 
		// we take a reading from the samples.
		int reading_interval = 10;

		// Sampling always starts at time t, reading_start_t defines when we actually start
		// taking readings.
		double reading_start_t = 200.0;

		//--- Working variables.
		double x_current;
		double t_current;
		DoubleCircularBuffer x_history_buffer;

		#endregion

		#region Private Methods

		/// <summary>
		/// The Mackey_Glass equation.
		/// Returns the value of x at time t+1 given x at time t, and x at time t-tau.
		/// </summary>
		/// <param name="x">Current value of x.</param>
		/// <param name="x_tau"></param>
		/// <returns></returns>
		private double mg_equation(double x, double x_tau)
		{
			return ((a*x_tau)/(1.0+Math.Pow(x_tau,10.0))) - b*x;
		}

		/// <summary>
		/// Runge_Kutta approximation of the next value of the Mackey_Glass equation.
		/// x - current value of x, at time t.
		/// t - the current time.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="t"></param>
		/// <param name="x_tau"></param>
		/// <returns></returns>
		private double rk4(double x, double t)
		{
			double x1, x2, x3, x4;

			// We only need to get the vaue of x at t-tau once because the sample point at half 
			// t_delta still points to the same historical x value.
			double x_tau = x_history(tau);

			x1 = t_delta * mg_equation(x, x_history(tau));
			x2 = t_delta * mg_equation(x+0.5*x1, x_tau);
			x3 = t_delta * mg_equation(x+0.5*x2, x_tau);
			x4 = t_delta * mg_equation(x+0.5*x3, x_tau);

			return x + (x1+x4)/6.0 + (x2+x3)/3.0;
		}

		/// <summary>
		/// Get a the already calculated value of x at time t-t_ago [from the history buffer].
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private double x_history(double t_ago)
		{
			// How many sample points ago? Cast directly to an int. Don't bother rounding.
			int points = (int)(t_ago/t_delta);

			int buffer_length = x_history_buffer.Length;
			if(buffer_length>=points)
			{	// The history goes far back enough. Return the point's x value.
				return x_history_buffer[buffer_length-points];
			}

			// The history doesn't go that far back. The point must be before t=0, therefore
			// the value of x is predefiend as being x_init.
			return x_init;
		}

		private void InitialiseFunction()
		{
			x_history_length = (int)Math.Ceiling(tau/t_delta);
			x_history_buffer = new DoubleCircularBuffer(x_history_length);

			x_current = x_init;
			t_current = 0.0;

			while(t_current<reading_start_t)
			{
				GetNextValue();
			}
		}

		private double GetNextValue()
		{
			// Calculate new value for x.
			double x_dot = rk4(x_current, t_current);

			// Place value into the history buffer.
			x_history_buffer.Enqueue(x_dot);

			// Update state variables.
			x_current = x_dot;
			t_current+=t_delta;

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
				// Skip some samples.
				for(int j=1; j<reading_interval; j++)
					GetNextValue();

				valueArray[i] = GetNextValue();
			}

			Utilities.NormalizeValueArray(0.1, 0.9, valueArray);
			return valueArray;
		}

		#endregion
	}
}
