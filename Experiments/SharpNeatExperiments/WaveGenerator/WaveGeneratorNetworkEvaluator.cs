using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// Function Regression / Wave Generator. 
	/// 
	/// Here we use the network as a wave generator, taking a reading from teh single output once per
	/// activation and evaluating against a know wave form. Hence this is a type of function regression,
	/// but unlike the OneInputFunctionRegressionExperiment here we can keep activating the function and 
	/// generating a wave form forever.
	/// </summary>
	public class WaveGeneratorNetworkEvaluator : INetworkEvaluator
	{
		/// <summary>
		/// The number of activations to make after ACTIVATIONS_PRE for evaluating the network's
		/// response.
		/// </summary>
		public readonly int ACTIVATIONS_EVAL = 100;
		public readonly double fitness_rebase_factor;

		double[] correctResponseArray;
		double[] networkResponseArray;

		#region Constructor

		public WaveGeneratorNetworkEvaluator()
		{
			//IFunction fn = new MackyGlassFunction();
			IFunction fn = new LogisticMapFunction();
			correctResponseArray = fn.GetFunctionValueArray(ACTIVATIONS_EVAL);
			networkResponseArray = new double[ACTIVATIONS_EVAL];

			fitness_rebase_factor =  64.0 / ACTIVATIONS_EVAL;
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
			double fitness=(double)ACTIVATIONS_EVAL;
			
			// Clear any existing network signals.
			network.ClearSignals();

			network.SetInputSignal(0, 1.0);

			// Activate a few times before we start taking readings.
			network.SingleStep();
			network.SetInputSignal(0, 0.0);





			// Loop over the sample points and subtract the modified error from 'fitness' for each one.
//			double prev_error = 0.0;
			
			for(int i=0; i<ACTIVATIONS_EVAL; i++)
			{
				// Activate the network.
				network.SingleStep();
				
				// Take a reading from the network, calculate the error and accumulate squared error within the fitness variable.
				networkResponseArray[i] = network.GetOutputSignal(0);
				double error = networkResponseArray[i]-correctResponseArray[i];

				// Range is limited to 0.8 so we should scale this back up to 1.0 to maximise our sensitivity to errors.
				error *=1.25;

				error = error*error;

//				double prev_error_factor = prev_error*3.0 + 1.0;
//				error = Math.Min(1.0, error*prev_error_factor);
//				prev_error = error;

				fitness -= error;
			}

			fitness *= fitness_rebase_factor;
			return (fitness*fitness*fitness*fitness)/4096.0;
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
