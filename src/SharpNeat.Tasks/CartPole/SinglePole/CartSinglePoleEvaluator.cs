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
    public sealed class CartSinglePoleEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        #region Constants

        // Maximum force applied to the cart, in Newtons.
        const float __MaxForce = 10f;

        // Some useful angles (in radians).
        const float __MaxPoleAngle = (16f * MathF.PI) / 180f;
        const float __MaxPoleAngle_Reciprocal = 1f / __MaxPoleAngle;

        // Track half length (in metres).
        const float __TrackLengthHalf = 2.0f;
        const float __TrackLengthHalf_Reciprocal = 1f / __TrackLengthHalf;

        #endregion

        #region Instance Fields

        readonly int _maxTimesteps;
        readonly float _maxTimesteps_Reciprocal = 1f;
        readonly CartSinglePolePhysicsRK4 _physics;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
        /// <remarks>
        /// Default to 960 timesteps, or 960/16 = 60 seconds of clock time.</remarks>
        public CartSinglePoleEvaluator()
            : this(960)
        {}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
        /// <param name="maxTimesteps">The maximum number of timesteps to run the physics simulation for.</param>
        public CartSinglePoleEvaluator(int maxTimesteps)
        {
            _maxTimesteps = maxTimesteps;
            _maxTimesteps_Reciprocal = 1f / maxTimesteps;
            _physics = new CartSinglePolePhysicsRK4();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate the provided black box against the cart and single pole balancing task,
        /// and return its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        /// <returns>A new instance of <see cref="FitnessInfo"/>.</returns>
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            // The evaluation consists of four separate trials, each with their own fitness score.
            // The final overall fitness is given by the root mean squared (RMS) fitness. Using an RMS
            // score ensures that improvements in the worst scoring trial are prioritised (by evolution) over
            // a similar level of improvement in a better scoring trial. RMS also has the nice quality of giving
            // a maximum overall fitness that is equal to the max fitness for a single trial.

            // Keep a running sum of the squared fitness scores.
            float fitnessSqrSum = 0f;

            // Trial 1.
            float fitness = RunTrial(box, 0.2f, DegreesToRadians(10f));
            fitnessSqrSum += fitness * fitness;

            // Trial 2.
            fitness = RunTrial(box, -0.4f, DegreesToRadians(12f));
            fitnessSqrSum += fitness * fitness;

            // Trial 3.
            fitness = RunTrial(box, -0.2f, DegreesToRadians(-6f));
            fitnessSqrSum += fitness * fitness;

            // Trial 4.
            fitness = RunTrial(box, -0.6f, DegreesToRadians(-8f));
            fitnessSqrSum += fitness * fitness;

            // Calculate the final overall fitness score.
            // Take the mean of the sum of squared fitnesses, and then take the square root.
            fitness = MathF.Sqrt(fitnessSqrSum / 4f);
            return new FitnessInfo(fitness);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Run a single cart-pole simulation/trial on the given black box, and with the given initial model state.
        /// </summary>
        /// <param name="box">The black box (neural net) to evaluate.</param>
        /// <param name="cartPos">Cart position on the track.</param>
        /// <param name="poleAngle">Pole angle in radians.</param>
        /// <returns>Fitness score.</returns>
        public float RunTrial(
            IBlackBox<double> box,
            float cartPos,
            float poleAngle)
        {
            // Reset black box state.
            box.ResetState();

            // Reset model state.
            _physics.ResetState(cartPos, poleAngle);

            // Get a local variable ref to the internal model state array.
            float[] state = _physics.State;

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
                float force = (float)(box.OutputVector[0] - 0.5) * 2f;
                ClipForce(ref force);
                force *= __MaxForce;

                // Update model state, i.e. move the model forward by one timestep.
                _physics.Update(force);

                // Check for failure state. I.e. has the cart run off the ends of the track, or has the pole
                // angle exceeded the defined threshold.
                if(MathF.Abs(state[0]) > __TrackLengthHalf || MathF.Abs(state[2]) > __MaxPoleAngle) {
                    break;
                }
            }

            // Fitness is given by the combination of four fitness components:
            // 1) Amount of simulation time that elapsed before the pole angle and/or cart position threshold was exceeded. Max score is 80 if the
            //    end of the trial is reached without exceeding any thresholds.
            // 2) Cart position component. Max fitness of 1.0 for a cart position of zero (i.e. the cart is in the middle of the track range);
            // 3) Pole angle component. Max fitness of 9.5 for a pole angle of 0 degrees (vertical pole).
            // 4) Pole angular velocity component. Maximum fitness 9.5 for a zero velocity.
            //
            // Therefore the maximum possible fitness is 100.0, when the pole is perfectly stationary, and the cart is in the middle of the track.
            float fitness =
                  (timestep * _maxTimesteps_Reciprocal * 80f)
                + (1f - (MathF.Min(MathF.Abs(state[0]), __TrackLengthHalf) * __TrackLengthHalf_Reciprocal))
                + ((1f - (MathF.Min(MathF.Abs(state[2]), __MaxPoleAngle) * __MaxPoleAngle_Reciprocal)) * 9.5f)
                + ((1f - MathF.Min(MathF.Abs(state[3]), 1f)) * 9.5f);

            return fitness;
        }

        #endregion

        #region Private Static Methods

        private static void ClipForce(ref float x)
        {
            if(x < -1f) x = -1f;
            else if(x > 1f) x = 1f;
        }

        private static float DegreesToRadians(float degrees)
        {
            return (MathF.PI * degrees) / 180f;
        }

        #endregion
    }
}
