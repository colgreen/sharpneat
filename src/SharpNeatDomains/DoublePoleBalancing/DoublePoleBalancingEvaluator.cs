/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.DoublePoleBalancing
{
    /// <summary>
    /// Evaluator for the double pole balancing task.
    /// </summary>
    public class DoublePoleBalancingEvaluator : IPhenomeEvaluator<IBlackBox>
    {
		#region Constants

        // Disable comment warnings for constants with clear names.
        #pragma warning disable 1591

		// Some physical model constants.
		protected const double Gravity	= -9.8;
		protected const double MassCart	= 1.0;
		protected const double Length1	= 0.5;	  /* actually half the pole's length */
		protected const double MassPole1 = 0.1;
		protected const double Length2 = 0.05;
		protected const double MassPole2 = 0.01;
		protected const double ForceMag	= 10.0;
        /// <summary>Time increment interval in seconds.</summary>
		public const double TimeDelta = 0.01;
		protected const double FourThirds = 4.0/3.0;
        /// <summary>Uplifting moment?</summary>
		protected const double MUP = 0.000002;

		// Some useful angle constants.
		protected const double OneDegree			= Math.PI / 180.0;	//= 0.0174532;
		protected const double FourDegrees			= Math.PI / 45.0;	//= 0.06981317;
		protected const double SixDegrees			= Math.PI / 30.0;	//= 0.1047192;
		protected const double TwelveDegrees		= Math.PI / 15.0;	//= 0.2094384;
		protected const double EighteenDegrees		= Math.PI / 10.0;	//= 0.3141592;
		protected const double TwentyFourDegrees	= Math.PI / 7.5;	//= 0.4188790;
		protected const double ThirtySixDegrees	    = Math.PI / 5.0;	//= 0.628329;
		protected const double FiftyDegrees		    = Math.PI / 3.6;	//= 0.87266;
		protected const double SeventyTwoDegrees	= Math.PI / 2.5;	//= 1.256637;

        #pragma warning restore 1591
		#endregion

		#region Class Variables
        #pragma warning disable 1591

        // Domain parameters.
		double _trackLength;
		protected double _trackLengthHalf;
		protected int	_maxTimesteps;
		protected double _poleAngleThreshold;

        // Evaluator state.
        protected ulong _evalCount;
        protected bool _stopConditionSatisfied;

        #pragma warning restore 1591
		#endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluator() : this(4.8, 100000, ThirtySixDegrees)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public DoublePoleBalancingEvaluator(double trackLength, int maxTimesteps, double poleAngleThreshold)
		{
			_trackLength = trackLength;
			_trackLengthHalf = trackLength / 2.0;
			_maxTimesteps = maxTimesteps;
			_poleAngleThreshold = poleAngleThreshold;
		}

		#endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public virtual FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;

		    // [0] - Cart Position (meters).
		    // [1] - Cart velocity (m/s).
		    // [2] - Pole 1 angle (radians)
		    // [3] - Pole 1 angular velocity (radians/sec).
		    // [4] - Pole 2 angle (radians)
		    // [5] - Pole 2 angular velocity (radians/sec).
            double[] state = new double[6];
            state[2] = FourDegrees;
            
			// Run the pole-balancing simulation.
            int timestep = 0;
			for(; timestep < _maxTimesteps; timestep++)
			{
				// Provide state info to the network (normalised to +-1.0).
				// Markovian (With velocity info)
                box.InputSignalArray[0] = state[0] / _trackLengthHalf;    // Cart Position is +-trackLengthHalfed
                box.InputSignalArray[1] = state[1] / 0.75;                  // Cart velocity is typically +-0.75
                box.InputSignalArray[2] = state[2] / ThirtySixDegrees;      // Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
                box.InputSignalArray[3] = state[3];                         // Pole angular velocity is typically +-1.0 radians. No scaling required.
                box.InputSignalArray[4] = state[4] / ThirtySixDegrees;      // Pole Angle is +-thirtysix_degrees. Values outside of this range stop the simulation.
                box.InputSignalArray[5] = state[5];                         // Pole angular velocity is typically +-1.0 radians. No scaling required.

				// Activate the black box.
                box.Activate();

                // Get black box response and calc next timestep state.
				performAction(state, box.OutputSignalArray[0]);
		
				// Check for failure state. Has the cart run off the ends of the track or has the pole
				// angle gone beyond the threshold.
				if(     (state[0]< -_trackLengthHalf) || (state[0]> _trackLengthHalf)
					||  (state[2] > _poleAngleThreshold) || (state[2] < -_poleAngleThreshold)
					||  (state[4] > _poleAngleThreshold) || (state[4] < -_poleAngleThreshold))
                {
					break;
                }
			}

            if(timestep == _maxTimesteps) {
                _stopConditionSatisfied = true;
            }

            // The controller's fitness is defined as the number of timesteps that elapse before failure.
            double fitness = timestep;
            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
        }

        #endregion

		#region Private Methods

		/// <summary>
        /// Calculates a state update for the next timestep using current model state and a single action from the
        /// controller. The action is a continuous variable with range [0:1]. 0 -> push left, 1 -> push right.
		/// </summary>
        /// <param name="state">Model state.</param>
		/// <param name="output">Push force.</param>
		protected void performAction(double[] state, double output)
		{ 
			int i;
			double[] dydx = new double[6];

			/*--- Apply action to the simulated cart-pole ---*/
			// Runge-Kutta 4th order integration method
			for(i=0; i<2; ++i)
			{
				dydx[0] = state[1];
				dydx[2] = state[3];
				dydx[4] = state[5];
				step(output, state, ref dydx);
				rk4(output, state, dydx, ref state);
			}
		}

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

			force		= (action - 0.5) * ForceMag * 2;
			costheta_1	= Math.Cos(st[2]);
			sintheta_1	= Math.Sin(st[2]);
			gsintheta_1 = Gravity * sintheta_1;
			costheta_2	= Math.Cos(st[4]);
			sintheta_2	= Math.Sin(st[4]);
			gsintheta_2 = Gravity * sintheta_2;

			ml_1		= Length1 * MassPole1;
			ml_2		= Length2 * MassPole2;
			temp_1		= MUP * st[3] / ml_1;
			temp_2		= MUP * st[5] / ml_2;

			fi_1		= (ml_1 * st[3] * st[3] * sintheta_1) +
				(0.75 * MassPole1 * costheta_1 * (temp_1 + gsintheta_1));

			fi_2		= (ml_2 * st[5] * st[5] * sintheta_2) +
				(0.75 * MassPole2 * costheta_2 * (temp_2 + gsintheta_2));

			mi_1 = MassPole1 * (1 - (0.75 * costheta_1 * costheta_1));
			mi_2 = MassPole2 * (1 - (0.75 * costheta_2 * costheta_2));

			derivs[1] = (force + fi_1 + fi_2) / (mi_1 + mi_2 + MassCart);
			derivs[3] = -0.75 * (derivs[1] * costheta_1 + gsintheta_1 + temp_1) / Length1;
			derivs[5] = -0.75 * (derivs[1] * costheta_2 + gsintheta_2 + temp_2) / Length2;
		}

		private void rk4(double f, double[] y, double[] dydx, ref double[] yout)
		{
			int i;

			double hh,h6;
			double[] dym = new double[6];
			double[] dyt = new double[6];
			double[] yt = new double[6];

			hh = TimeDelta * 0.5;
			h6 = TimeDelta / 6.0;
			for(i=0; i<=5; i++) {
                yt[i] = y[i] + (hh * dydx[i]);
            }
			step(f, yt, ref dyt);
			dyt[0] = yt[1];
			dyt[2] = yt[3];
			dyt[4] = yt[5];
			for (i=0; i<=5; i++) {
                yt[i] = y[i] + (hh * dyt[i]);
            }
			step(f,yt, ref dym);
			dym[0] = yt[1];
			dym[2] = yt[3];
			dym[4] = yt[5];
			for(i=0; i<=5; i++) 
			{
				yt[i] = y[i] + (TimeDelta * dym[i]);
				dym[i] += dyt[i];
			}
			step(f,yt, ref dyt);
			dyt[0] = yt[1];
			dyt[2] = yt[3];
			dyt[4] = yt[5];
			for (i=0;i<=5;i++) {
				yout[i] = y[i] + h6 * (dydx[i]+dyt[i]+2.0*dym[i]);
            }
		}

		#endregion
    }
}
