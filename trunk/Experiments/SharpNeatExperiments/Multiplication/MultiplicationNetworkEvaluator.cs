using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// Multiplication of fractional operands between -1.0 and 1.0.
	/// </summary>
	public class MultiplicationNetworkEvaluator : INetworkEvaluator
	{
		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			double fitness=0.0;

			for(double op1=-1.0; op1<=1.0; op1+=0.1)
			{
				for(double op2=-1.0; op2<=1.0; op2+=0.1)
				{
					double correctAnswer = op1 * op2;
					
					network.ClearSignals();

					// Apply the operand to the network's input neurons.
					network.SetInputSignal(0, op1);
					network.SetInputSignal(1, op2);

					// Activate the network.
					//network.RelaxNetwork(7, 1e-8);// 0.0000000001);
					network.MultipleSteps(3);

					// Read the output. Shift and translate to expected range. 
					double networkAnswer = (network.GetOutputSignal(0)*2.0)-1.0;

					// Compare against the expected output
					double error = Math.Pow(Math.Abs(correctAnswer-networkAnswer),0.3);
					fitness += 1.0 - Math.Min(1.0, error);
				}
			}

			return fitness;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		#endregion
	}
}
