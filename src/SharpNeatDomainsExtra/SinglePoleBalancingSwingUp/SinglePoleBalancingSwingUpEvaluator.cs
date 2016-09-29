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
using SharpNeat.DomainsExtra.SinglePoleBalancingBox2d;

namespace SharpNeat.DomainsExtra.SinglePoleBalancingSwingUp
{
    /// <summary>
    /// Evaluator for the single pole balancing task.
    /// </summary>
    public class SinglePoleBalancingSwingUpEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        #region Constants

        const float __TrackLength = 4.8f;
        const float __TrackLengthHalf = __TrackLength * 0.5f;
        const float __MaxForceNewtons   = 100f;
        const float __MaxForceNewtonsX2 = __MaxForceNewtons * 2f;
        // Some precalced angle constants.
        const float __SixDegrees		= (float)(Math.PI / 30.0);	//= 0.1047192;
        const float __TwelveDegrees		= (float)(Math.PI / 15.0);	//= 0.2094384;
        const float __180Degrees		= (float)(Math.PI / 2.0);

        #endregion

        #region Instance Fields

		int	_maxTimestepsPhase1;
        int	_maxTimestepsPhase2;
        float _poleAngleInitial;

        // Evaluator state.
        ulong _evalCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public SinglePoleBalancingSwingUpEvaluator() : this(450, 10800, (float)Math.PI)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public SinglePoleBalancingSwingUpEvaluator(int maxTimestepsPhase1, int maxTimestepsPhase2, float poleAngleInitial)
		{
			_maxTimestepsPhase1 = maxTimestepsPhase1;
            _maxTimestepsPhase2 = maxTimestepsPhase2;
            _poleAngleInitial = poleAngleInitial;
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
            get { return false; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;
            double fitness1 = Evaluate_Phase1_SwingUp(box);
            //double fitness2 = Evaluate_Phase2_Balancing(box);
            //double fitness = fitness1 * fitness2;
            return new FitnessInfo(fitness1, fitness1);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion

        #region Private Methods

        private double Evaluate_Phase1_SwingUp(IBlackBox box)
        {
            // Init sim world. We add extra length to the track to allow cart to overshoot, we then detect overshooting by monitoring the cart's X position 
            // (this is just simpler and more robust than detecting if the cart has hit the ends of the track exactly).
            SinglePoleBalancingWorld simWorld = new SinglePoleBalancingWorld(__TrackLength + 0.5f, __180Degrees);
            simWorld.InitSimulationWorld();

            // Record closest approach to target state and the timestep that it occured on.
            double lowestError = double.MaxValue;
            int timestepLowest = 0;
            
            // Run the pole-balancing simulation.
            int timestep = 0;
            for(; timestep < _maxTimestepsPhase1; timestep++)
            {
                SimulateOneStep(simWorld, box);

                // Calc state distance from target state.
                double cartPosError = Math.Abs(simWorld.CartPosX);
                double cartVelocityError = Math.Abs(simWorld.CartVelocityX);
                double poleAngleError = Math.Abs(simWorld.PoleAngle);
                double poleAnglularVelocityError = Math.Abs(simWorld.PoleAngularVelocity);
                double error = (poleAngleError) + (poleAnglularVelocityError) + (cartPosError);
                
                // Check for failure state. Has the cart run off the ends of the track.
                if((simWorld.CartPosX < -__TrackLengthHalf) || (simWorld.CartPosX > __TrackLengthHalf)) {
                    return 0.0;
                }

                // Track best state achieved.
                if(error < lowestError)
                {
                    lowestError = error;
                    timestepLowest = timestep;
                }
            }
            
            if(0.0 == lowestError) {
                return 10e9;
            }

            // Alternative form of 1/x that avoids rapid rise to infinity as lowestError tends towards zero.
            return Math.Log10(1.0 + (1/(lowestError + 0.1)));
        }

        private double Evaluate_Phase2_Balancing(IBlackBox box)
        {     
            // Init sim world. We add extra length to the track to allow cart to overshoot, we then detect overshooting by monitoring the cart's X position 
            // (this is just simpler and more robust than detecting if the cart has hit the ends of the track exactly).
            SinglePoleBalancingWorld simWorld = new SinglePoleBalancingWorld(__TrackLength + 0.5f, __SixDegrees);
            simWorld.InitSimulationWorld();


            // Run the pole-balancing simulation.
            int timestep = 0;
            for(; timestep < _maxTimestepsPhase2; timestep++)
            {
                SimulateOneStep(simWorld, box);

                // Check for failure state. Has the cart run off the ends of the track or has the pole
                // angle gone beyond the threshold.
                if(     (simWorld.CartPosX < -__TrackLengthHalf) || (simWorld.CartPosX > __TrackLengthHalf)
                    ||  (simWorld.PoleAngle > __TwelveDegrees) || (simWorld.PoleAngle < -__TwelveDegrees)) 
                {
                    break;
                }
            }

            return timestep;
        }

        private void SimulateOneStep(SinglePoleBalancingWorld simWorld, IBlackBox box)
        {
            // Provide state info to the black box inputs.
            box.InputSignalArray[0] = simWorld.CartPosX / __TrackLengthHalf;    // CartPosX range is +-trackLengthHalf. Here we normalize it to [-1,1].
            box.InputSignalArray[1] = simWorld.CartVelocityX;                   // Cart track velocity x is typically +-0.75.
            box.InputSignalArray[2] = simWorld.PoleAngle / __TwelveDegrees;     // Rescale angle to match range of values during balancing.
            box.InputSignalArray[3] = simWorld.PoleAngularVelocity;             // Pole angular velocity is typically +-1.0 radians. No scaling required.

            // Activate the network.
            box.Activate();

            // Read the network's force signal output.
            float force = (float)(box.OutputSignalArray[0] - 0.5f) * __MaxForceNewtonsX2;

            // Simulate one timestep.
            simWorld.SetCartForce(force);
            simWorld.Step();
        }

        #endregion
    }
}
