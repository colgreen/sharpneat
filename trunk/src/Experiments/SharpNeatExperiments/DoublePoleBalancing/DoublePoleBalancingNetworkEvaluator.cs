using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This is a port of the double pole balancing code in Kenneth Stanley's original NEAT implementation.
	/// It is not an exact port, but close enough to simulate networks evolved within NEAT and hence is good
	/// for comparing SharpNEAT with original NEAT.
	/// </summary>
	public class DoublePoleBalancingNetworkEvaluator : INetworkEvaluator, ISimulator
	{
		#region Constants

		// Some useful physical model constants.
		protected const double GRAVITY	= -9.8;
		protected const double MASSCART	= 1.0;
		protected const double LENGTH_1	= 0.5;	  /* actually half the pole's length */
		protected const double MASSPOLE_1	= 0.1;
		protected const double LENGTH_2	= 0.05;
		protected const double MASSPOLE_2 = 0.01;
		protected const double FORCE_MAG	= 10.0;
		public const double TIME_DELTA	= 0.01;	  /* seconds between state updates */
		protected const double FOURTHIRDS	= 4.0/3.0;

		protected const double MUP = 0.000002;

		// Some useful angle constants.
		protected const double one_degree			= Math.PI / 180.0;	//= 0.0174532;
		protected const double four_degrees			= Math.PI / 45.0;	//= 0.06981317;
		protected const double six_degrees			= Math.PI / 30.0;	//= 0.1047192;
		protected const double twelve_degrees		= Math.PI / 15.0;	//= 0.2094384;
		protected const double eighteen_degrees		= Math.PI / 10.0;	//= 0.3141592;
		protected const double twentyfour_degrees	= Math.PI / 7.5;	//= 0.4188790;
		protected const double thirtysix_degrees	= Math.PI / 5.0;	//= 0.628329;
		protected const double fifty_degrees		= Math.PI / 3.6;	//= 0.87266;
		protected const double seventytwo_degrees	= Math.PI / 2.5;	//= 1.256637;

		#endregion

		#region Class Variables

		// State variables
		protected int	currentTimestep;

		/// <summary>
		/// [0] - Cart Position (meters).
		/// [1] - Cart velocity (m/s).
		/// [2] - Pole 1 angle (radians)
		/// [3] - Pole 1 angular velocity (radians/sec).
		/// [4] - Pole 2 angle (radians)
		/// [5] - Pole 2 angular velocity (radians/sec).
		/// </summary>
		protected double[] state = new double[6];


		// Domain parameters
		protected double trackLength;
		protected double trackLengthHalfed;
		protected int	maxTimesteps;
		protected double poleAngleThreshold;

		// Other stuff.
		protected double action;

		#endregion

		#region Constructors

		public DoublePoleBalancingNetworkEvaluator():this(4.8, 100000, thirtysix_degrees)
		{}

		public DoublePoleBalancingNetworkEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
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
				return state[0];
			}
		}				

		/// <summary>
		/// m/s.
		/// </summary>
		public double CartVelocityX
		{
			get
			{
				return state[1];
			}
		}

		/// <summary>
		/// in radians.
		/// </summary>
		public double Pole1Angle
		{
			get
			{
				return state[2];
			}
		}

		public double Pole1AngularVelocity
		{
			get
			{
				return state[3];
			}
		}

		/// <summary>
		/// in radians.
		/// </summary>
		public double Pole2Angle
		{
			get
			{
				return state[4];
			}
		}

		public double Pole2AngularVelocity
		{
			get
			{
				return state[5];
			}
		}

		public double Action
		{
			get
			{
				return action;
			}
		}

		#endregion

		#region Public Methods

		public virtual void Initialise_Random()
		{
			// Initialise state. 
			state[0] = state[1] = state[3] = state[4] = state[5] = 0;
			state[2] = one_degree;//(Utilities.NextDouble()*six_degrees) - six_degrees/2.0;
		}

		public virtual bool PerformSingleStep(INetwork network)
		{
			// Provide state info to the network (normalised to +-1.0).
			// Markovian (With velocity info)
			network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
			network.SetInputSignal(1, state[1] / 0.75);					// Cart velocity is typically +-0.75
			network.SetInputSignal(2, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
			network.SetInputSignal(3, state[3]);						// Pole angular velocity is typically +-1.0 radians. No scaling required.
			network.SetInputSignal(4, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
			network.SetInputSignal(5, state[5]);						// Pole angular velocity is typically +-1.0 radians. No scaling required.

			// Only 1 activation required to solve this problem. No hidden nodes or 
			// recursive connections required.
			network.MultipleSteps(1);
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

		public virtual double EvaluateNetwork(INetwork network)
		{
			// Initialise state. 
			state[0] = state[1] = state[3] = state[4] = state[5] = 0;
			state[2] = four_degrees;

			// Run the pole-balancing simulation.
			for(currentTimestep=0; currentTimestep<maxTimesteps; currentTimestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				// Markovian (With velocity info)
				network.SetInputSignal(0, state[0] / trackLengthHalfed);	// Cart Position is +-trackLengthHalfed
				network.SetInputSignal(1, state[1] / 0.75);					// Cart velocity is typically +-0.75
				network.SetInputSignal(2, state[2] / thirtysix_degrees);	// Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
				network.SetInputSignal(3, state[3]);						// Pole angular velocity is typically +-1.0 radians. No scaling required.
				network.SetInputSignal(4, state[4] / thirtysix_degrees);	// pole_angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
				network.SetInputSignal(5, state[5]);						// Pole angular velocity is typically +-1.0 radians. No scaling required.

				// Activate the network.
				network.MultipleSteps(1);
				performAction(network.GetOutputSignal(0));
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if((state[0]< -trackLengthHalfed) || (state[0]> trackLengthHalfed)
					|| (state[2] > poleAngleThreshold) || (state[2] < -poleAngleThreshold)
					|| (state[4] > poleAngleThreshold) || (state[4] < -poleAngleThreshold))
					break;
			}
			// return the number of timesteps that passed before failure. This is the network's fitness.
			return (double)currentTimestep;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		#endregion

		#region Protected Methods

		protected void performAction(double output)
		{ 
			int i;
			double[] dydx = new double[6];

			

			/*--- Apply action to the simulated cart-pole ---*/
			// Runge-Kutta 4th order integration method
			for(i=0;i<2;++i)
			{
				dydx[0] = state[1];
				dydx[2] = state[3];
				dydx[4] = state[5];
				step(output, state, ref dydx);
				rk4(output, state, dydx, ref state);
			}
			
//			const double EULER_TAU= TIME_DELTA/4;
//			for(i=0;i<8;++i)
//			{
//				step(output, state, ref dydx);
//				state[0] += EULER_TAU * dydx[0];
//				state[1] += EULER_TAU * dydx[1];
//				state[2] += EULER_TAU * dydx[2];
//				state[3] += EULER_TAU * dydx[3];
//				state[4] += EULER_TAU * dydx[4];
//				state[5] += EULER_TAU * dydx[5];
//			}
		}

		#endregion

		#region Private Methods

		private void step(double action, double[] st, ref double[] derivs)
		{
			double	force,
				costheta_1,
				costheta_2,
				sintheta_1,
				sintheta_2,
				gsintheta_1,
				gsintheta_2,
				temp_1,
				temp_2,
				ml_1,
				ml_2,
				fi_1,
				fi_2,
				mi_1,
				mi_2;

			force		= (action - 0.5) * FORCE_MAG * 2;
			costheta_1	= Math.Cos(st[2]);
			sintheta_1	= Math.Sin(st[2]);
			gsintheta_1 = GRAVITY * sintheta_1;
			costheta_2	= Math.Cos(st[4]);
			sintheta_2	= Math.Sin(st[4]);
			gsintheta_2 = GRAVITY * sintheta_2;

			ml_1		= LENGTH_1 * MASSPOLE_1;
			ml_2		= LENGTH_2 * MASSPOLE_2;
			temp_1		= MUP * st[3] / ml_1;
			temp_2		= MUP * st[5] / ml_2;

			fi_1		= (ml_1 * st[3] * st[3] * sintheta_1) +
				(0.75 * MASSPOLE_1 * costheta_1 * (temp_1 + gsintheta_1));

			fi_2		= (ml_2 * st[5] * st[5] * sintheta_2) +
				(0.75 * MASSPOLE_2 * costheta_2 * (temp_2 + gsintheta_2));

			mi_1 = MASSPOLE_1 * (1 - (0.75 * costheta_1 * costheta_1));
			mi_2 = MASSPOLE_2 * (1 - (0.75 * costheta_2 * costheta_2));

			derivs[1] = (force + fi_1 + fi_2) / (mi_1 + mi_2 + MASSCART);
			derivs[3] = -0.75 * (derivs[1] * costheta_1 + gsintheta_1 + temp_1) / LENGTH_1;
			derivs[5] = -0.75 * (derivs[1] * costheta_2 + gsintheta_2 + temp_2) / LENGTH_2;
		}

		private void rk4(double f, double[] y, double[] dydx, ref double[] yout)
		{
			int i;

			double hh,h6;
			double[] dym = new double[6];
			double[] dyt = new double[6];
			double[] yt = new double[6];

			hh=TIME_DELTA*0.5;
			h6=TIME_DELTA/6.0;
			for (i=0;i<=5;i++) yt[i]=y[i]+hh*dydx[i];
			step(f, yt, ref dyt);
			dyt[0] = yt[1];
			dyt[2] = yt[3];
			dyt[4] = yt[5];
			for (i=0;i<=5;i++) yt[i]=y[i]+hh*dyt[i];
			step(f,yt, ref dym);
			dym[0] = yt[1];
			dym[2] = yt[3];
			dym[4] = yt[5];
			for (i=0;i<=5;i++) 
			{
				yt[i]=y[i]+TIME_DELTA*dym[i];
				dym[i] += dyt[i];
			}
			step(f,yt, ref dyt);
			dyt[0] = yt[1];
			dyt[2] = yt[3];
			dyt[4] = yt[5];
			for (i=0;i<=5;i++)
				yout[i]=y[i]+h6*(dydx[i]+dyt[i]+2.0*dym[i]);
		}

		#endregion

		#region ISimulator

		public virtual int InputNeuronCount
		{
			get
			{
				return 6;
			}
		}

		public virtual int OutputNeuronCount
		{
			get
			{
				return 1;
			}
		}

		#endregion
	}
}
