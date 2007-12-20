using System;

namespace SharpNeatLib.NeuralNetwork
{
	public class SteepenedSigmoidApproximation : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
			const double one = 1.0;
			const double one_half = 0.5; 

			if(inputSignal<-1.0)
			{
				return 0.0;
			}
			else if(inputSignal<0.0)
			{
				return (inputSignal+one)*(inputSignal+one)*one_half;
			}
			else if(inputSignal<1.0)
			{
				return 1.0-(inputSignal-one)*(inputSignal-one)*one_half;
			}
			else
			{
				return 1.0;
			}
		}

		public float Calculate(float inputSignal)
		{
			const float one = 1.0F;
			const float one_half = 0.5F; 

			if(inputSignal<-1.0F)
			{
				return 0.0F;
			}
			else if(inputSignal<0.0F)
			{
//				float d=inputSignal+four;
//				return d*d*one_32nd;
				return (inputSignal+one)*(inputSignal+one)*one_half;
			}
			else if(inputSignal<1.0F)
			{
//				float d=inputSignal-four;
//				return 1.0F-d*d*one_32nd;
				return 1.0F-(inputSignal-one)*(inputSignal-one)*one_half;
			}
			else
			{
				return 1.0F;
			}
		}

		/// <summary>
		/// Unique ID. Stored in network XML to identify which function network the network is supposed to use.
		/// </summary>
		public string FunctionId
		{
			get
			{
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// The function as a string in a platform agnostic form. For documentation purposes only, this isn;t actually compiled!
		/// </summary>
		public string FunctionString
		{
			get
			{
				return "";
			}
		}


		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		public string FunctionDescription
		{
			get
			{
				return "";
			}
		}
	}
}
