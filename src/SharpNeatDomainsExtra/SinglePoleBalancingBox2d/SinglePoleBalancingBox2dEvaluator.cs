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

namespace SharpNeat.DomainsExtra.SinglePoleBalancingBox2d
{
    /// <summary>
    /// Evaluator for the single pole balancing task.
    /// </summary>
    public class SinglePoleBalancingBox2dEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        #region Constants

        const float __TrackLength = 4.8f;
        const float __TrackLengthHalf = __TrackLength * 0.5f;
        const float __MaxForceNewtons   = 100f;
        const float __MaxForceNewtonsX2 = __MaxForceNewtons * 2f;
        // Some precalced angle constants.
        const float __SixDegrees		= (float)(Math.PI / 30.0);	//= 0.1047192;
        const float __TwelveDegrees		= (float)(Math.PI / 15.0);  //= 0.2094384;

        #endregion

        #region Instance Fields

        readonly int	_maxTimesteps;
        readonly float _poleAngleInitial;
        readonly float _poleAngleThreshold;

        // Evaluator state.
        ulong _evalCount;
        bool _stopConditionSatisfied;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with default task arguments/variables.
        /// </summary>
		public SinglePoleBalancingBox2dEvaluator() : this(21600, __SixDegrees, __TwelveDegrees)
		{}

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
		public SinglePoleBalancingBox2dEvaluator(int maxTimesteps, float poleAngleInitial, float poleAngleThreshold)
		{
			_maxTimesteps = maxTimesteps;
            _poleAngleInitial = poleAngleInitial;
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
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            // Init sim world. We add extra length to the track to allow cart to overshoot, we then detect overshooting by monitoring the cart's X position 
            // (this is just simpler and more robust than detecting if the cart has hit the ends of the track exactly).
            SinglePoleBalancingWorld simWorld = new SinglePoleBalancingWorld(__TrackLength + 0.5f, _poleAngleInitial);
            simWorld.InitSimulationWorld();

            // Run the pole-balancing simulation.
            int timestep = 0;
            for(; timestep < _maxTimesteps; timestep++)
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

                // Check for failure state. Has the cart run off the ends of the track or has the pole
                // angle gone beyond the threshold.
                if(     (simWorld.CartPosX < -__TrackLengthHalf) || (simWorld.CartPosX > __TrackLengthHalf)
                    ||  (simWorld.PoleAngle > _poleAngleThreshold) || (simWorld.PoleAngle < -_poleAngleThreshold)) 
                {
                    break;
                }
            }

            _evalCount++;
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
    }
}
