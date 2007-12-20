using System;

namespace SharpNeatLib.NeuralNetwork
{
	public class Linear : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
            if(inputSignal<-1.0)
                return 0.0;
            else if(inputSignal>1.0)
                return 1.0;
            else
                return (inputSignal+1.0)*0.5;  
		}

		public float Calculate(float inputSignal)
		{
            if(inputSignal<0.0F)
                return 0.0F;
            else if(inputSignal>1.0F)
                return 1.0F;
            else
                return (inputSignal+1.0F)*0.5F; 
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
                return "(x+1)/2 [min=0, max=1]";
			}
		}

		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		public string FunctionDescription
		{
			get
			{
                return "Linear";
			}
		}
	}
}
