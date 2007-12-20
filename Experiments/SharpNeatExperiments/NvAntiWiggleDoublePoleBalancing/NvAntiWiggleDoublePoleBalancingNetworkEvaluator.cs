using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class NvAntiWiggleDoublePoleBalancingNetworkEvaluator : DoublePoleBalancingNetworkEvaluator
	{	
		JiggleBuffer jiggleBuffer1 = new JiggleBuffer(100);
		JiggleBuffer jiggleBuffer2 = new JiggleBuffer(100);

		#region Constructors

		public NvAntiWiggleDoublePoleBalancingNetworkEvaluator():this(4.8, 50000, thirtysix_degrees)
		{}

		public NvAntiWiggleDoublePoleBalancingNetworkEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
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
			state[2] = one_degree;
		}

		public override bool PerformSingleStep(INetwork network)
		{
			// Provide state info to the network (normalised to +-1.0).
			// Non-markovian (no velocity info)
			network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
			network.SetInputSignal(1, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
			network.SetInputSignal(2, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.

			// Activate the network.
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
			state[2] = one_degree;
	
			jiggleBuffer1.Clear();
			jiggleBuffer2.Clear();

			// Run the pole-balancing simulation.
			for(currentTimestep=0; currentTimestep<maxTimesteps; currentTimestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				// Non-markovian (no velocity info)
				network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
				network.SetInputSignal(1, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
				network.SetInputSignal(2, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.


				// Activate the network.
				network.MultipleSteps(3);
				performAction(network.GetOutputSignal(0));
		
				if(jiggleBuffer1.Length==100)
				{	// Feed an old value off of buffer 1 into buffer2.
					jiggleBuffer2.Enqueue(jiggleBuffer1.Dequeue());
				}

				// Place the latest jiggle value into buffer1.
				jiggleBuffer1.Enqueue(	Math.Abs(state[0]) + Math.Abs(state[1]) + 
										Math.Abs(state[2]) + Math.Abs(state[3]));

				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if((state[0]< -trackLengthHalfed) || (state[0]> trackLengthHalfed)
					|| (state[2] > poleAngleThreshold) || (state[2] < -poleAngleThreshold)
					|| (state[4] > poleAngleThreshold) || (state[4] < -poleAngleThreshold))
					break;

				// Give the simulation at least 500 timesteps(5secs) to stabilise before penalising
				// instability.
				if(currentTimestep>499 && jiggleBuffer2.Total>30.0)
				{	// Too much wiggling occuring. Stop simulation early. 30 was an experimentally determined value.
					break;					
				}
			}
		
			
			if(currentTimestep>499 && currentTimestep<600)
			{	// For the 100(1 sec) steps after the 500(5 secs) mark we punish wiggling based
				// on the values from the 1 sec just gone. This is on the basis that the values
				// in jiggleBuffer2 (from 2 to 1 sec ago) will refelct the large amount of
				// wiggling that occurs at the start of the simulation when the system is still stabilising.
				return currentTimestep + 10.0/Math.Max(1.0, jiggleBuffer1.Total);
			}
			else if(currentTimestep>599)
			{	// After 600 steps we use jiggleBuffer2 to punsih wiggling this contains data from between
				// 2 and 1 secs ago. This is on the basis that when the system becomes unstable and causes
				// the simulation to terminate prematurely, the immediately prior 1 secs data will reflect that
				// instability, which may not be indicative of the overall stability of the system up to that time.
				return currentTimestep + 10.0/Math.Max(1.0, jiggleBuffer2.Total);
			}
			else
			{	// Return just the currentTimestep without any extra fitness component.
				return currentTimestep;
			}
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
