using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// Function Regression. Here we evaluate a network's ability to find an approximation for
	/// a function with a single input parameter.
	/// </summary>
	public class OneInputFunctionRegressionNetworkEvaluator : INetworkEvaluator
	{
		/// <summary>
		/// The sampling resolution. This is the number of points on the target function that we
		/// evaluate for evaluation of a network's response and ability to fit the target function.
		/// </summary>
		public readonly int INPUT_VAR_RESOLUTION = 64;

		double[] correctResponseArray;
		double[] networkResponseArray;

		#region Constructor

		public OneInputFunctionRegressionNetworkEvaluator()
		{
			// Precalculate correct responses and store them in an array.
			double i1=0.00;
			double i1_inc = 1.0/(INPUT_VAR_RESOLUTION-1);
			correctResponseArray = new double[INPUT_VAR_RESOLUTION];
			networkResponseArray = new double[INPUT_VAR_RESOLUTION];

			for(int i=0; i<INPUT_VAR_RESOLUTION; i++)
			{
				correctResponseArray[i] = TargetFunction(i1);
				i1+=i1_inc;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Precalculated correct responses for each sample point.
		/// </summary>
		public double[] CorrectResponseArray
		{
			get
			{
				return correctResponseArray;
			}
		}

		/// <summary>
		/// The responses of the last network to have been evaluated. This maintained primarily for use
		/// by this evlauator's experiment view.
		/// </summary>
		public double[] NetworkResponseArray
		{
			get
			{
				return networkResponseArray;
			}
		}

		#endregion

		#region INetworkEvaluator

		/// <summary>
		/// The output at each sample point is between 0 and 1, therefore the error is also
		/// between 0 and 1. We therefore assign a max fitness of 1 for each sample point, which of
		/// course becomes a fitness of zero if the error is at its maximum of 1.
		/// 
		/// To punishment of incorrect responses is modified to give a steeper increase in fitness as
		/// the actual error reduces. This is done by taking the 5th root of the roor (error^0.2). In simple terms
		/// this gives more weight to movements in the estimated position if it is close to the target value, e.g.
		/// consider that two points move and increase their error by 0.1, the first point has moved away from the 
		/// correct response and so is therefore still close too it. The second point however has increased its
		/// error from 0.9 to 1.0. Given the choice between the approximation before and after the two points moved
		/// it seem intuitive (to me!) that the initial position is better since one of the points was correct and one 
		/// was very wrong, whereas in the second case we have two points that are wrong, one of which has shifted
		/// from being very wrong to even more wrong!
		/// </summary>
		/// <param name="network"></param>
		/// <returns></returns>
		public double EvaluateNetwork(INetwork network)
		{
			// The max fitness at each point is 1.0, therefore the max fitness overall is this time the number of sample points.
			// We assign a max fitness initially and subtract the modified error for each sample point.
			double fitness=(double)INPUT_VAR_RESOLUTION;
			

			// The inpu trange is actually 0.01 to 1.0. This avoids an input of 0 which can be an awkward value
			// for ANN's to use, although a bias signal can overcome this issue that just adds another barrier
			// to success for the problem at hand.
			// Set an initial value for our input value and calculate the amount it will be incremented to
			// obtain each of the sample points.
			double i1=0.01;
			double i1_inc = 0.99/(INPUT_VAR_RESOLUTION-1);

			// Loop over the sample points and subtract the modified error from 'fitness' for each one.
			for(int i=0; i<INPUT_VAR_RESOLUTION; i++)
			{
				network.SetInputSignal(0, i1); 
				i1+=i1_inc;

				// How many activations is appropriate? Could relaxing the network also work?
				network.MultipleSteps(4);
				
				networkResponseArray[i] = network.GetOutputSignal(0);
				double error = networkResponseArray[i]-correctResponseArray[i];
				fitness -= error*error;
			
			

				//fitness -= Math.Pow(Math.Abs(networkResponseArray[i]-correctResponseArray[i]), 0.2);
				//fitness -= Math.Min(Math.Abs(networkResponseArray[i]-correctResponseArray[i]), 0.5)*2.0;
				//fitness -= Math.Abs(networkResponseArray[i]-correctResponseArray[i]);

				

				// We assumed that the network was cleared for the first entry of the loop for efficiency reasons.
				// At time of coding this was a safe assumption.
				network.ClearSignals();
			}



			return (fitness*fitness*fitness*fitness)/4096.0;
			//return Math.Pow(fitness/64.0, 12)*4096.0;
			
			//return fitness*fitness;
			//return fitness;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// The target function that we are trying to approximate / match / fit.
		/// The function can describe any shape and be as simple or complex as you wish but there are some
		/// points that must be noted:
		/// 
		/// 1) The function will be evaluated for values of i1 over the range 0.0 to 1.0. The precise number
		/// of evaluations is determined by INPUT_VAR_RESOLUTION.
		/// 
		/// 2) The output range of a standard artificial neuron used by SharpNEAT (at time of writing) is 0.0 to
		/// 1.0. This network evaluation scheme does not perform any translation or scaling of values to allow for
		/// target functions with output ranges outside this range, therefore target functions must produce values 
		/// within the range of 0.0 to 1.0 for values of i1 (also in the range 0.0 to 1.0).
		/// 
		/// Effectively any translation and/or scaling must be done as part of the target function.
		/// </summary>
		/// <param name="i1"></param>
		/// <returns></returns>
		private double TargetFunction(double i1)
		{
			const double _2_PI = Math.PI * 2.0;
			const double _3_PI = Math.PI * 3.0;
			const double _4_PI = Math.PI * 4.0;
			const double _5_PI = Math.PI * 5.0;
			const double _8_PI = Math.PI * 8.0;
			const double PI_2 = Math.PI * 0.5;

			//return (Math.Sin(i1 * PI_2) * 0.8) + 0.1;		// Quarter wave.
			//return (Math.Sin(i1 * Math.PI) * 0.8)+0.1;	// Half wave.
			//return (Math.Sin(i1 * _2_PI) * 0.5) + 0.5;	// Full wave.
			//return (Math.Sin(i1 * _4_PI) * 0.5) + 0.5;	// Two waves.
			//return (Math.Sin(i1 * _8_PI) * 0.4) + 0.5;	// Four waves.
			//return Math.Max(0.5, (Math.Sin(i1 * _8_PI) * 0.5) + 0.5);	// Four waves with bottom clipped.
			//return i1<0.5 ? 0.0 : 1.0;					// Step up;
			//return i1<0.5 ? 1.0 : 0.0;					// Step down;
			//return Math.Exp(-Math.Pow(i1-0.5, 2.0)*32);	// Bell curve / Normal distribution.
			//return ((i1*3.2)%0.8)+0.1;					// Saw teeth (four).

			// Combinations of sin waves.
			//return Math.Max((Math.Sin(i1 * _2_PI) * 0.5) + 0.5, (Math.Cos(i1 * Math.PI) * 0.5) + 0.5);
			//return Math.Max((Math.Sin(i1 * _8_PI) * 0.5) + 0.5, (Math.Cos(i1 * _8_PI) * 0.5) + 0.5);
			//return Math.Max((Math.Sin(i1 * _8_PI) * 0.5) + 0.5, (Math.Cos(i1 * _4_PI) * 0.5) + 0.5);

			// Mattias Fagerlung's functions.
			//return (i1*i1*i1*3.0 + Math.Sqrt(i1)) / 4.0;
			//return (Math.Round(i1*2.0)) / 2.0;			// 2 steps upwards (left to right)
			//return Math.Abs(i1-0.5)*2.0;					// V shape.
			//return (Math.Tanh((i1*4.0)-2.0)+1.0)*0.5;		// Sigmoid.


			//---- Frankenstein Function #1.
			if(i1<0.05)
			{	// Straight line (flat).
				return 0.1;
			}
			else if(i1<0.1)
			{	// Straight line (flat).
				return 0.9;
			}
			else if(i1<0.35)
			{	// Straight line (rising).
				return (i1*1.8)+0.27;
			}
			else if(i1<0.5)
			{	// Sine wave troff (1/4 of a complete wave)
				return Math.Sin(((i1-0.35)*(Math.PI/0.3))-Math.PI)*0.8 + 0.9;
			}
			else if(i1<0.7)
			{	// x^4 curve.
				return Math.Pow((i1-0.5)*5, 4.0)*0.45 + 0.45;
			}
			//else if(i1<1.0)
			return (1.0/((i1-0.7)*50+1.11112))*0.8 +0.1;
		}

		#endregion
	}
}
