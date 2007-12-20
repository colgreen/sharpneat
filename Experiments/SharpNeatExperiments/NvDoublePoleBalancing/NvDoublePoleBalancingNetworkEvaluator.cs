using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This is a port of the double pole balancing code in NEAT. It is not an exact port, but close enough to 
	/// simulate networks evolved within NEAT and hence is good for comparing SharpNEAT with original NEAT.
	/// </summary>
	public class NvDoublePoleBalancingNetworkEvaluator : DoublePoleBalancingNetworkEvaluator
	{	
		#region Constructors

		public NvDoublePoleBalancingNetworkEvaluator():this(4.8, 100000, thirtysix_degrees)
		{}

		public NvDoublePoleBalancingNetworkEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
		{
			this.trackLength = trackLength;
			this.trackLengthHalfed = trackLength / 2.0;
			this.maxTimesteps = maxTimesteps;
			this.poleAngleThreshold = poleAngleThreshold;
		}

		#endregion
		
		#region Public Methods

		public override void Initialise_Random()
		{
			// Initialise state. 
			state[0] = state[1] = state[3] = state[4] = state[5] = 0;
			state[2] = one_degree;//(Utilities.NextDouble()*one_degree) - one_degree/2.0;
		}

		public override bool PerformSingleStep(INetwork network)
		{
			// Provide state info to the network (normalised to +-1.0).
			// Markovian (With velocity info)
			network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
			network.SetInputSignal(1, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
			network.SetInputSignal(2, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.

			// Activate network.
			network.MultipleSteps(3);
			action = network.GetOutputSignal(0);
			performAction(action);
			currentTimestep++;

			if((state[0]< -trackLengthHalfed) || (state[0]> trackLengthHalfed)
				|| (state[2] > poleAngleThreshold) || (state[2] < -poleAngleThreshold)
				|| (state[4] > poleAngleThreshold) || (state[4] < -poleAngleThreshold))
				return true;

			return false;
		}

		#endregion

		#region INetworkEvaluator

		public override double EvaluateNetwork(INetwork network)
		{
			// Initialise state. 
			state[0] = state[1] = state[3] = state[4] = state[5] = 0;
			state[2] = one_degree;//four_degrees;

			// Run the pole-balancing simulation.
			for(currentTimestep=0; currentTimestep<maxTimesteps; currentTimestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				// Markovian (With velocity info)
				network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
				network.SetInputSignal(1, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
				network.SetInputSignal(2, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.

				// Activate the network.
				network.MultipleSteps(3);
				action = network.GetOutputSignal(0);
				performAction(action);
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if((state[0]< -trackLengthHalfed) || (state[0]> trackLengthHalfed)
					|| (state[2] > poleAngleThreshold) || (state[2] < -poleAngleThreshold)
					|| (state[4] > poleAngleThreshold) || (state[4] < -poleAngleThreshold))
					break;
			}
			
			return (double)currentTimestep;
		}

		#endregion

		#region ISimulator

		public override int InputNeuronCount
		{
			get
			{
				return 3;
			}
		}

		public override int OutputNeuronCount
		{
			get
			{
				return 1;
			}
		}

		#endregion
	}
}
