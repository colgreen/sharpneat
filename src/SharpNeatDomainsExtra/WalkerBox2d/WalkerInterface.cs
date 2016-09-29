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
using Box2DX.Common;
using Box2DX.Dynamics;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Interface onto a walker.
    /// </summary>
    public class WalkerInterface
    {
        const float __jointMaxTorque = 1.25f;
        Body _torsoBody;
        LegInterface _leftLegIface;
        LegInterface _rightLegIface;

        #region Constructor

        /// <summary>
        /// Construct walker interface onto the provided walker body parts.
        /// </summary>
        public WalkerInterface(Body torsoBody, LegInterface leftLegIface, LegInterface rightLegIface)
        {
            _torsoBody = torsoBody;
            _leftLegIface = leftLegIface;
            _rightLegIface = rightLegIface;
        }

        #endregion

        #region Public Properties [Walker State]

        /// <summary>
        /// Gets the torso's position (center point of torso rectangle).
        /// </summary>
        public Vec2 TorsoPosition
        {
            get { return _torsoBody.GetPosition(); }
        }

        /// <summary>
        /// Gets the torso's velocity.
        /// </summary>
        public Vec2 TorsoVelocity
        {
            get { return _torsoBody.GetLinearVelocity(); }
        }

        /// <summary>
        /// Gets the torso's angle.
        /// </summary>
        public float TorsoAngle
        {
            get { return _torsoBody.GetAngle(); }
        }

        /// <summary>
        /// Gets the torso's angular velocity.
        /// </summary>
        public float TorsoAnglularVelocity
        {
            get { return _torsoBody.GetAngularVelocity(); }
        }

        /// <summary>
        /// Gets an interface onto the walker's left leg.
        /// </summary>
        public LegInterface LeftLegIFace
        {
            get { return _leftLegIface; }
        }

        /// <summary>
        /// Gets an interface onto the walker's right leg.
        /// </summary>
        public LegInterface RightLegIFace
        {
            get { return _rightLegIface; }
        }

        /// <summary>
        /// Gets the maximum torque allowable on a joint.
        /// </summary>
        public float JointMaxTorque
        {
            get { return __jointMaxTorque; }
        }

        /// <summary>
        /// Gets the sum total of absolute torque being applied to all joints
        /// </summary>
        public float TotalAppliedTorque
        {
            get { return _leftLegIface.TotalAppliedTorque + _rightLegIface.TotalAppliedTorque; }
        }

        #endregion
    }
}
