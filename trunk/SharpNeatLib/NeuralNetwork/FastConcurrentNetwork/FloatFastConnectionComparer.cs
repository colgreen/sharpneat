using System;
using System.Collections;

namespace SharpNeatLib.NeuralNetwork
{
	public class FloatFastConnectionComparer : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			FloatFastConnection a = (FloatFastConnection)x;
			FloatFastConnection b = (FloatFastConnection)y;


			int diff = a.sourceNeuronIdx - b.sourceNeuronIdx;
			if(diff==0)
			{
				// Secondary sort on targetNeuronIdx.
				return a.targetNeuronIdx - b.targetNeuronIdx;
			}
			else
			{
				return diff;
			}
		}

		#endregion
	}
}
