/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.CartPole.SinglePole
{
    /// <summary>
    /// Evaluator for the cart and single pole balancing task.
    /// </summary>
    public class CartSinglePoleEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        #region Constants

        // Maximum force applied to the cart, in Newtons.
        const double __MaxForce = 10.0;

        // Some useful angles (in radians).
		const double __MaxPoleAngle = (16.0 * Math.PI) / 180.0;
        const double __MaxPoleAngle_Reciprocal = 1.0 / __MaxPoleAngle;

        // Track half length (in metres).
        const double __TrackLengthHalf = 1.2;
        const double __TrackLengthHalf_Reciprocal = 1.0 / __TrackLengthHalf;

        #endregion

        #region Instance Fields

		readonly int _maxTimesteps;
        readonly CartSinglePolePhysicsRK4 _physics;

        #endregion		

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
        /// <remarks>
        /// Default to 1440 timesteps, or 1440/16 = 90 seconds of clock time.</remarks>
		public CartSinglePoleEvaluator() 
            : this(1440)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public CartSinglePoleEvaluator(int maxTimesteps)
		{
			_maxTimesteps = maxTimesteps;
            _physics = new CartSinglePolePhysicsRK4();
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate the provided black box against the cart and single pole balancing task,
        /// and return its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            // Reset model state.
            _physics.ResetState(DegreesToRadians(6.0));

            // Get a local variable ref to the internal model state array.
            double[] state = _physics.State;

			// Run the cart-pole simulation.
            int timestep = 0;
			for(; timestep < _maxTimesteps; timestep++)
			{
				// Provide model state to the black box inputs (normalised to +-1.0).
                box.InputVector[0] = 1.0; // Bias input.
                box.InputVector[1] = state[0] * __TrackLengthHalf_Reciprocal; // Cart X position range is +-__TrackLengthHalf; here we normalize to [-1,1].
                box.InputVector[2] = state[2] * __MaxPoleAngle_Reciprocal;    // Pole angle range is +-__MaxPoleAngle radians; here we normalize to [-1,1].

				// Activate the network.
                box.Activate();

                // Read the output to determine the force to be applied to the cart by the controller.
                double force = box.OutputVector[0] - 0.5;
                ClipForce(ref force);
                force *= __MaxForce;

                // Update model state, i.e. move the model forward by one timestep.
                _physics.Update(force);

				// Check for failure state. I.e. has the cart run off the ends of the track, or has the pole
				// angle exceeded the defined threshold.
				if(Math.Abs(state[0]) > __TrackLengthHalf || Math.Abs(state[2]) > __MaxPoleAngle) {
					break;
                }
			}

            // Fitness is defined as the number of timesteps that elapsed before failure, plus a small bonus fitness for cart proximity to the middle of the track's range (x=0).
            //double fitness = timestep + (__TrackLengthHalf - Math.Abs(state[0])) * 5.0;

            // Fitness is given by the combination of four fitness components:
            // 1) The number of timesteps that elapsed before the pole angle exceeded the maximum angle threshold. Max score is 1440.
            // 2) Pole angle component. Max fitness of 1.0 for a pole angle of 0 degrees (vertical pole).
            // 3) Pole angular velocity component. Maximum fitness 1.0 for a velocity of zero.
            // 4) Cart position component. Max fitness of 6.0 when the cart is in the centre of the track range (x=0).
            //
            // Therefore the maximum possible fitness is 1448, when the pole is perfectly stationary, and the cart is in the middle of the track.
            double fitness = 
                + timestep
                + (1.0 - (Math.Abs(state[2]) * __MaxPoleAngle_Reciprocal)) 
                + (1.0 - Math.Min(Math.Abs(state[3]), 1.0))
                + (__TrackLengthHalf - Math.Abs(state[0])) * 5.0;

            return new FitnessInfo(fitness);
        }

        #endregion

        #region Private Static Methods

        private static void ClipForce(ref double x)
        {
            if(x < -1.0) x = -1.0;
            else if(x > 1.0) x = 1.0;
        }

        private static double DegreesToRadians(double degrees)
        {
            return (Math.PI * degrees) / 180.0;
        }


        #endregion
    }
}
