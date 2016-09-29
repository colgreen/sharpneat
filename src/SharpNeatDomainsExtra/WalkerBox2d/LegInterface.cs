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
using Box2DX.Dynamics;
using Box2DX.Common;
using SysMath = System.Math;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Interface onto a walker's leg.
    /// </summary>
    public class LegInterface
    {
        #region Consts
        const float __jointMaxSpeed = (float)(System.Math.PI * 4.0);
        const float __jointFriction = 0.5f;
        #endregion

        RevoluteJoint _hipJoint;
        RevoluteJoint _kneeJoint;
        Body _lowerLegBody;
        float _hipAppliedTorque;
        float _kneeAppliedTorque;

        #region Constructor

        /// <summary>
        /// Construct leg interface onto the provided Box2D leg joints.
        /// </summary>
        public LegInterface(RevoluteJoint hipJoint, RevoluteJoint kneeJoint, Body lowerLegBody, float lowerLegLength)
        {
            _hipJoint = hipJoint;
            _kneeJoint = kneeJoint;
            _lowerLegBody = lowerLegBody;
        }

        #endregion

        #region Public Properties [Leg State]

        /// <summary>
        /// Gets the hip joint angle.
        /// </summary>
        public float HipJointAngle
        {
            get { return _hipJoint.JointAngle; }
        }

        /// <summary>
        /// Gets the hip joint angular velocity.
        /// </summary>
        public float HipJointVelocity
        {
            get { return _hipJoint.JointSpeed; }
        }

        /// <summary>
        /// Gets the hip joint angular velocity.
        /// </summary>
        public Vec2 HipJointPosition
        {
            get { return _hipJoint.Anchor1; }
        }

        /// <summary>
        /// Gets the knee joint angle.
        /// </summary>
        public float KneeJointAngle
        {
            get { return _kneeJoint.JointAngle; }
        }

        /// <summary>
        /// Gets the knee joint angular velocity.
        /// </summary>
        public float KneeJointVelocity
        {
            get { return _kneeJoint.JointSpeed; }
        }

        /// <summary>
        /// Gets the total amount of torque being applied to the leg's joint's (abs(knee_torque) + abs(hip_torque).
        /// </summary>
        public float TotalAppliedTorque
        {
            get { return SysMath.Abs(_hipAppliedTorque) + SysMath.Abs(_kneeAppliedTorque); }
        }

        /// <summary>
        /// Gets the height of the leg's foot. Note that this is actual the mid point of the circular leg cap which.
        /// </summary>
        public float FootHeight
        {
            get
            {   // Note. zero degrees is directly up, hence Cos(theta) == -1 when the leg is straight, and 1 when the leg is inverted.
                Box2DX.Common.Vec2 legPos = _lowerLegBody.GetPosition();
                float theta = _lowerLegBody.GetAngle();
                return legPos.Y + (0.5f * (float)SysMath.Cos(theta));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the hip joint's torque.
        /// </summary>
        public void SetHipJointTorque(float torque)
        {
            _hipAppliedTorque = torque;
            SetJointTorque(_hipJoint, torque);
        }

        /// <summary>
        /// Sets the knee joint's torque.
        /// </summary>
        public void SetKneeJointTorque(float torque)
        {
            _kneeAppliedTorque = torque;
            SetJointTorque(_kneeJoint, torque);
        }

        #endregion

        #region Private Methods

        private void SetJointTorque(RevoluteJoint joint, float torque)
        {
            float atorque; // abs(torque)
            float storque; // sign(torque)
		    if(torque > 0f) {
			    atorque = torque;	
			    storque = 1f;		
            } else if (torque < 0f) {
                atorque = -torque;
                storque = -1f;
            } else {
                atorque = 0f;
                storque = 0f;
            }

            //#
            //# We have to enumerate various cases because we are emulating friction
            //#  with active torque and we have to take care not to add energy to
            //#  the system when a reversal happens inside the timestep.
            //#
            //# Also, the underlying sim does not let us apply torque directly, but
            //#  rather we have to factor it into maxMotorTorque and motorSpeed, the
            //#  latter of which we set to an extreme we don't normally expect to reach.
            //#
            float speed = joint.JointSpeed;
            float fric = __jointFriction;

		    if(speed*storque >= 0f) //# Pushing in same direction as current motion (and against friction)
            {
                if(atorque > fric) {    //#     With enough force to overcome friction:
                    joint.SetMaxMotorTorque(atorque	- fric);    //# Remaining torque (after friction)...
                    joint.MotorSpeed = storque * 100f;          //# serves to accelerate current motion
                } else {                //#     Without enough force to overcome friction:
                    joint.SetMaxMotorTorque(fric - atorque);    //# Remaining friction...
                    joint.MotorSpeed = 0f;                      //# ...serves to stop the motion (but no more)
                }
            }
            else {                  //# Pushing against current motion (and with friction)
                joint.SetMaxMotorTorque(fric + atorque);    //# Forces sum...
                joint.MotorSpeed = storque * 100f;          //# ...in direction of torque
                //# BUG: This case unfortunately adds energy during reversals because friction
                //#  continues to contribute energy in the same direction after the reversal.
                //# Is there a fix for this?
	        }
        }

        #endregion
    }
}
