/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using SharpNeat.Core;
using SharpNeat.DomainsExtra.Box2D;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace SharpNeat.DomainsExtra.InvertedDoublePendulum
{
    /// <summary>
    /// View class for the box2d single pole balancing domain.
    /// </summary>
    public class InvertedDoublePendulumView : Box2dDomainView
    {
        #region Constants

        const float __TrackLength = 4.8f;
        const float __TrackLengthHalf = __TrackLength * 0.5f;
        const float __MaxForceNewtons   = 100f;
        const float __MaxForceNewtonsX2 = __MaxForceNewtons * 2f;

        const float __TwoDegrees		= (float)(Math.PI / 90.0);
        const float __ThreeDegrees		= (float)(Math.PI / 60.0);
        const float __SixDegrees		= (float)(Math.PI / 30.0);	//= 0.1047192;
        const float __TwelveDegrees		= (float)(Math.PI / 15.0);	//= 0.2094384;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder, this is used to decode genome(s) into IBlackBox controllers.
        /// </summary>
        public InvertedDoublePendulumView(IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
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
            InvertedDoublePendulumWorld simWorld = new InvertedDoublePendulumWorld(__TrackLength + 0.5f, __ThreeDegrees, 0f);
            simWorld.InitSimulationWorld();
            return simWorld;
        }

        /// <summary>
        /// Invoke any required control logic in the Box2D world.
        /// </summary>
        protected override void InvokeController()
        {
            // Provide state info to the black box inputs.
            InvertedDoublePendulumWorld simWorld = (InvertedDoublePendulumWorld)_simWorld;
            // _box is updated by other threads so copy the reference so that we know we are workign with the same IBlackBox within this method.
            IBlackBox box = _box;
            box.InputSignalArray[0] = simWorld.CartPosX / __TrackLengthHalf;    // CartPosX range is +-trackLengthHalf. Here we normalize it to [-1,1].
            box.InputSignalArray[1] = simWorld.CartVelocityX;                   // Cart track velocity x is typically +-0.75.

            box.InputSignalArray[2] = simWorld.CartJointAngle / __TwelveDegrees;// Rescale angle to match range of values during balancing.
            box.InputSignalArray[3] = simWorld.CartJointAngularVelocity;        // Pole angular velocity is typically +-1.0 radians. No scaling required.

            box.InputSignalArray[4] = simWorld.ElbowJointAngle / __TwelveDegrees;
            box.InputSignalArray[5] = simWorld.ElbowJointAngularVelocity;

            // Activate the network.
            box.Activate();

            // Read the network's force signal output.
            // FIXME: Force mag should be configurable somewhere.
            float force = (float)(box.OutputSignalArray[0] - 0.5f) * __MaxForceNewtonsX2;
            simWorld.SetCartForce(force);
        }

        #endregion
    }
}
