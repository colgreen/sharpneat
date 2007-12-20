using System;
using SharpNeatLib.Maths;
using SharpNeatLib.NeuralNetwork;


namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// A class that simulates 2D single pole balancing. The pole is attached to a cart with
	/// a hinge, the cart sits on a track with a fixed length. This simulation defines the
	/// cart's position and velocity on track along with the pole's current angle and angular
	/// velocity.
	/// A single trial runs until the pole angle passes a threshold or the cart runs off the track
	/// or maxTimesteps is reached (success).
	/// 
	/// Ported from the C/C++ code in NEAT by K.Stanley (http://www.cs.utexas.edu/users/kstanley/neat.html).
	/// That code in turn was taken from the pole simulator written by Richard Sutton and Charles Anderson.
	/// 
	/// This simulator uses normalized, continuous inputs instead of discretizing the input space.
	/// </summary>
	public class SinglePoleBalancingNetworkEvaluator : INetworkEvaluator, ISimulator
	{
		#region Constants

		// Some useful physical model constants.
		const double GRAVITY=9.8;
		const double MASSCART=1.0;
		const double MASSPOLE=0.1;
		const double TOTAL_MASS=(MASSPOLE + MASSCART);
		const double LENGTH=0.5;	  /* actually half the pole's length */
		const double POLEMASS_LENGTH=(MASSPOLE * LENGTH);
		const double FORCE_MAG=10.0;
		public const double TIME_DELTA=0.02;	  /* seconds between state updates */
		const double FOURTHIRDS= 4.0/3.0;

		// Some useful angle constants.
		const double one_degree			= Math.PI / 180.0;	//= 0.0174532;	/* 2pi/360 */
		const double six_degrees		= Math.PI / 30.0;	//= 0.1047192;
		const double twelve_degrees		= Math.PI / 15.0;	//= 0.2094384;
		const double twentyfour_degrees = Math.PI / 7.5;	//= 0.2094384;
		const double thirty_six_degrees	= Math.PI / 5.0;	//= 0.628329;
		const double fifty_degrees		= Math.PI / 3.6;	//= 0.87266;

		#endregion

		#region Class Variables

		FastRandom random = new FastRandom();

		// Domain parameters
		double trackLength;
		double trackLengthHalfed;
		int	maxTimesteps;
		double poleAngleThreshold;
		
		// State variables
		int	currentTimestep;
		double cart_pos_x;				// Cart position, meters.
		double cart_velocity_x;			// m/s.
		double pole_angle;				// in radians.
		double pole_angular_velocity;	// radians/sec.

		// Other stuff.
		bool action;

		#endregion

		#region Constructors

		public SinglePoleBalancingNetworkEvaluator():this(4.8, 100000, twelve_degrees)
		{}

		public SinglePoleBalancingNetworkEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
		{
			this.trackLength = trackLength;
			this.trackLengthHalfed = trackLength / 2.0;
			this.maxTimesteps = maxTimesteps;
			this.poleAngleThreshold = poleAngleThreshold;
		}

		#endregion

		#region Properties

		public int CurrentTimestep
		{
			get
			{
				return currentTimestep;
			}
		}

		public double TrackLength
		{
			get
			{
				return trackLength;
			}
		}

		/// <summary>
		/// Cart position, meters.
		/// </summary>
		public double CartPosX 
		{
			get
			{
				return cart_pos_x;
			}
		}				

		/// <summary>
		/// m/s.
		/// </summary>
		public double CartVelocityX
		{
			get
			{
				return cart_velocity_x;
			}
		}

		/// <summary>
		/// in radians.
		/// </summary>
		public double PoleAngle
		{
			get
			{
				return pole_angle;
			}
		}

		public double PoleAngularVelocity
		{
			get
			{
				return pole_angular_velocity;
			}
		}

		public bool Action
		{
			get
			{
				return action;
			}
		}

		#endregion

		#region Public Methods

		public void Initialise_Random()
		{
			currentTimestep=0;
			cart_pos_x = 0;
			cart_velocity_x = 0;
			pole_angle = (random.NextDouble() * twentyfour_degrees) - twentyfour_degrees/2.0;
			pole_angular_velocity = 0.0;
		}
	
		public bool PerformSingleStep(INetwork network)
		{
			// Provide state info to the network (normalised to +-1.0).
			network.SetInputSignal(0, cart_pos_x / trackLengthHalfed);	// cart_pos_x is +-trackLengthHalfed
			network.SetInputSignal(1, cart_velocity_x / 0.75);			// cart_velocity_x is typically +-0.75
			network.SetInputSignal(2, pole_angle / twelve_degrees);		// pole_angle is +-twelve_degrees. Values outside of this range stop the simulation.
			network.SetInputSignal(3, pole_angular_velocity);			// pole_angular_velocity is typically +-1.0 radians. No scaling required.

			// Activate the network.
			network.MultipleSteps(1);
			action = network.GetOutputSignal(0)>0.5;
			SimulateTimestep(action);
			currentTimestep++;

			// Check for failure state. Has the cart run off the ends of the track or has the pole
			// angle gone beyond the threshold.
			if((cart_pos_x< -trackLengthHalfed) || (cart_pos_x> trackLengthHalfed))
				//	|| (pole_angle > poleAngleThreshold) || (pole_angle < -poleAngleThreshold))
				return true;
			
			return false;
		}

		#endregion

		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			// Initialise state. 
			cart_pos_x = 0;
			cart_velocity_x = 0;
			pole_angle = six_degrees;
			pole_angular_velocity = 0.0;

			// Run the pole-balancing simulation.
			for(currentTimestep=0; currentTimestep<maxTimesteps; currentTimestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				network.SetInputSignal(0, cart_pos_x / trackLengthHalfed);	// cart_pos_x is +-trackLengthHalfed
				network.SetInputSignal(1, cart_velocity_x / 0.75);			// cart_velocity_x is typically +-0.75
				network.SetInputSignal(2, pole_angle / twelve_degrees);		// pole_angle is +-twelve_degrees. Values outside of this range stop the simulation.
				network.SetInputSignal(3, pole_angular_velocity);			// pole_angular_velocity is typically +-1.0 radians. No scaling required.

				// Activate the network.
				network.MultipleSteps(1);
				SimulateTimestep(network.GetOutputSignal(0)>0.5);
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if((cart_pos_x< -trackLengthHalfed) || (cart_pos_x> trackLengthHalfed)
					|| (pole_angle > poleAngleThreshold) || (pole_angle < -poleAngleThreshold))
					break;
			}
			// return the number of timesteps that passed before failure. This is the network's fitness.
			return currentTimestep;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		/// <summary>
		/// Takes an action and the current values of the four state variables and updates their
		/// values by estimating the state TIME_DELTA seconds later.
		/// </summary>
		/// <param name="action">push direction, left(false) or right(true). Force magnitude is fixed.</param>
		private void SimulateTimestep(bool action)
		{
			//float xacc,thetaacc,force,costheta,sintheta,temp;
			double force = action? FORCE_MAG : -FORCE_MAG;
			double costheta = Math.Cos(pole_angle);
			double sintheta = Math.Sin(pole_angle);
			double temp = (force + POLEMASS_LENGTH * pole_angular_velocity * pole_angular_velocity * sintheta) / TOTAL_MASS;
	
			double thetaacc = (GRAVITY * sintheta - costheta * temp)
								/ (LENGTH * (FOURTHIRDS - MASSPOLE * costheta * costheta
								/ TOTAL_MASS));
			  
			double xacc  = temp - POLEMASS_LENGTH * thetaacc * costheta / TOTAL_MASS;
			  
			/*** Update the four state variables, using Euler's method. ***/
	
			cart_pos_x				+= TIME_DELTA * cart_velocity_x;
			cart_velocity_x			+= TIME_DELTA * xacc;
			pole_angle				+= TIME_DELTA * pole_angular_velocity;
			pole_angular_velocity	+= TIME_DELTA * thetaacc;
		}
	
		#endregion

		#region ISimulator

		public int InputNeuronCount
		{
			get
			{
				return 4;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 1;
			}
		}

		#endregion
	}
}
