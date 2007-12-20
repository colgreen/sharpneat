using System;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// Describes a function for the function regression experiments.
	/// </summary>
	public interface IFunction
	{
		/// <summary>
		/// Gets an array of values sampled over a continuous range of some function.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		double[] GetFunctionValueArray(int length);
	}
}
