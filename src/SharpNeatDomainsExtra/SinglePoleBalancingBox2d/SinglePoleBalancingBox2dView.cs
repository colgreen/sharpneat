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
using SharpNeat.DomainsExtra.Box2D;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeat.DomainsExtra.SinglePoleBalancingBox2d
{
    /// <summary>
    /// View class for the box2d single pole balancing domain.
    /// </summary>
    public class SinglePoleBalancingBox2dView : Box2dDomainView
    {
        #region Constants

        const float __TrackLength = 4.8f;
        const float __TrackLengthHalf = __TrackLength * 0.5f;
        const float __MaxForceNewtons   = 100f;
        const float __MaxForceNewtonsX2 = __MaxForceNewtons * 2f;
        const float __SixDegrees		= (float)(Math.PI / 30.0);	//= 0.1047192;
        const float __TwelveDegrees		= (float)(Math.PI / 15.0);	//= 0.2094384;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder, this is used to decode genome(s) into IBlackBox controllers.
        /// </summary>
        public SinglePoleBalancingBox2dView(IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
            : base(genomeDecoder)
        {}

        #endregion

        #region Override Methods [BOX2D Simulation Creation / Control]

        /// <summary>
        /// Create a Box2D simulation world.
        /// </summary>
        protected override SimulationWorld CreateSimulationWorld()
        {
            // Init sim world. We add extra length to the track to allow cart to overshoot, we then detect overshooting by monitoring the cart's X position 
            // (this is just simpler and more robust than detecting if the cart has hit the ends of the track exactly).
            SinglePoleBalancingWorld simWorld = new SinglePoleBalancingWorld(__TrackLength + 0.5f, __SixDegrees);
            simWorld.InitSimulationWorld();
            return simWorld;
        }

        /// <summary>
        /// Invoke any required control logic in the Box2D world.
        /// </summary>
        protected override void InvokeController()
        {
            // Provide state info to the black box inputs.
            SinglePoleBalancingWorld simWorld = (SinglePoleBalancingWorld)_simWorld;
            // _box is updated by other threads so copy the reference so that we know we are working with the same IBlackBox within this method.
            IBlackBox box = _box;
            box.InputSignalArray[0] = simWorld.CartPosX / __TrackLengthHalf;    // CartPosX range is +-trackLengthHalf. Here we normalize it to [-1,1].
            box.InputSignalArray[1] = simWorld.CartVelocityX;                   // Cart track velocity x is typically +-0.75.
            box.InputSignalArray[2] = simWorld.PoleAngle / __TwelveDegrees;     // Rescale angle to match range of values during balancing.
            box.InputSignalArray[3] = simWorld.PoleAngularVelocity;             // Pole angular velocity is typically +-1.0 radians. No scaling required.

            // Activate the network.
            box.Activate();

            // Read the network's force signal output.
            float force = (float)(box.OutputSignalArray[0] - 0.5f) * __MaxForceNewtonsX2;
            simWorld.SetCartForce(force);
        }

        #endregion
    }
}
