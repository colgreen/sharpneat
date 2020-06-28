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
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using SharpNeat.DomainsExtra.Box2D;
using SysMath = System.Math;

namespace SharpNeat.DomainsExtra.InvertedDoublePendulum
{
    /// <summary>
    /// Evaluator for the inverted double pendulum task.
    /// </summary>
    public class InvertedDoublePendulumWorld : SimulationWorld
    {
        const float __ArmLength = 0.5f;
        const float __TwoDegrees		= (float)(SysMath.PI / 90.0);
        const float __ThreeDegrees		= (float)(SysMath.PI / 60.0);
        const float __SixDegrees		= (float)(SysMath.PI / 30.0);   //= 0.1047192;

        #region Instance Fields

        readonly float _cartJointInitialAngle;
        readonly float _elbowJointInitialAngle;
        readonly float _trackLengthHalf;
        Body _cartBody;
        Body _arm2Body;

        RevoluteJoint _cartJoint;
        RevoluteJoint _elbowJoint;
        PrismaticJoint _cartTrackJoint;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with default world settings.
        /// </summary>
        public InvertedDoublePendulumWorld() : this(4.8f, __ThreeDegrees, 0f)
        {}

        /// <summary>
        /// Construct with specified world settings.
        /// </summary>
        public InvertedDoublePendulumWorld(float trackLength, float cartJointInitialAngle, float elbowJointInitialAngle)
        {
            _trackLengthHalf = trackLength * 0.5f;
            _cartJointInitialAngle = cartJointInitialAngle;
            _elbowJointInitialAngle = elbowJointInitialAngle;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inititialise world.
        /// </summary>
        public override void InitSimulationWorld()
        {
            SimulationParameters simParams = new SimulationParameters();
            simParams._lowerBoundPhysics.Set(-_trackLengthHalf - 2f , -1f);
            simParams._upperBoundPhysics.Set(_trackLengthHalf + 2f, 2f);
            simParams._lowerBoundView.Set(-_trackLengthHalf - 1f, -1f);
            simParams._upperBoundView.Set(_trackLengthHalf + 1, 2f);
            base.InitSimulationWorld(simParams);
        }

        #endregion

        #region Properties / Methods [World State & Control Interface]

        /// <summary>
        /// Gets the cart's X position. Zero => center, negative => left of center, positive => right of center.
        /// </summary>
        public float CartPosX
        {
            get { return _cartBody.GetPosition().X; }
        }

        /// <summary>
        /// Gets the cart's track (horizontal) velocity.
        /// </summary>
        public float CartVelocityX
        {
            get { return _cartBody.GetLinearVelocity().X; }
        }

        /// <summary>
        /// Get the angle of the first joint (cart-pole joint).
        /// </summary>
        public float CartJointAngle
        {
            get 
            {
                // Joint angle is cumulative (increases for successive revolutions), so first we factor out the excess revolutions.
                double angle = (_cartJointInitialAngle + _cartJoint.JointAngle) % (2.0*SysMath.PI);

                // Now define an alternative scheme with 0 degrees at the top and +-180 degrees at the bottom (+ve is CCW, -ve is CW).
                if(angle > SysMath.PI) {
                    angle = angle - (2.0*SysMath.PI);
                }
                return (float)angle;
            }
        }

        /// <summary>
        /// Get the angle of the second joint (pole elbow joint).
        /// </summary>
        public float ElbowJointAngle
        {
            get 
            {
                // Joint angle is cumulative (increases for successive revolutions), so first we factor out the excess revolutions.
                double angle = (_elbowJointInitialAngle + _elbowJoint.JointAngle) % (2.0*SysMath.PI);

                // Now define an alternative scheme with 0 degrees at the top and +-180 degrees at the bottom (+ve is CCW, -ve is CW).
                if(angle > SysMath.PI) {
                    angle = angle - (2.0*SysMath.PI);
                }
                return (float)angle;
            }
        }

        /// <summary>
        /// Get the angular velocity of the first joint (cart-pole joint).
        /// </summary>
        public float CartJointAngularVelocity
        {
            get { return _cartJoint.JointSpeed; }
        }

        /// <summary>
        /// Get the angular velocity of the second joint (pole elbow joint).
        /// </summary>
        public float ElbowJointAngularVelocity
        {
            get { return _elbowJoint.JointSpeed; }
        }

        /// <summary>
        /// Get the position of the pole's end.
        /// </summary>
        public Vec2 PoleTopPos
        {
            get { return CalcPoleEndPos(_arm2Body); }
        }

        /// <summary>
        /// Set horizontal force in newtons to apply to cart.
        /// </summary>
        /// <param name="n"></param>
        public void SetCartForce(float n)
        {
            if(n > 0)
            {
                _cartTrackJoint._motorSpeed = 1000f;
                _cartTrackJoint._maxMotorForce = n;
            }
            else
            {
                _cartTrackJoint._motorSpeed = -1000f;
                _cartTrackJoint._maxMotorForce = -n;
            }
        }

        #endregion

        #region World Population Methods

        /// <summary>
        /// Add objects to the Box2d world.
        /// </summary>
        protected override void PopulateWorld()
        {
		// ==== Define the ground body ====
			BodyDef groundBodyDef = new BodyDef();
			groundBodyDef.Position.Set(0f, -0.25f);
            
   			// Call the body factory which creates the ground box shape.
			// The body is also added to the world.
			Body groundBody = _world.CreateBody(groundBodyDef);

			// Define the ground box shape.
			PolygonDef groundShapeDef = new PolygonDef();

			// The extents are the half-widths of the box.
			groundShapeDef.SetAsBox(_trackLengthHalf + 1f, 0.25f);
            groundShapeDef.Friction = _simParams._defaultFriction;
            groundShapeDef.Restitution = _simParams._defaultRestitution;
            groundShapeDef.Filter.CategoryBits = 0x3;

			// Add the ground shape to the ground body.
            groundBody.CreateShape(groundShapeDef);

        // ==== Define the cart body.
            BodyDef cartBodyDef = new BodyDef();
            cartBodyDef.Position.Set(0f, 0.15f);

            // Create cart body.
            _cartBody = _world.CreateBody(cartBodyDef);
            PolygonDef cartShapeDef = new PolygonDef();
            cartShapeDef.SetAsBox(0.5f, 0.125f);
            cartShapeDef.Friction = 0f;
            cartShapeDef.Restitution = 0f;
            cartShapeDef.Density = 16f;
            _cartBody.CreateShape(cartShapeDef);
            _cartBody.SetMassFromShapes();

            // Fix cart to 'track' (prismatic joint).
            PrismaticJointDef cartTrackJointDef = new PrismaticJointDef();
            cartTrackJointDef.EnableMotor = true;
            cartTrackJointDef.LowerTranslation = -_trackLengthHalf;
            cartTrackJointDef.UpperTranslation = _trackLengthHalf;
            cartTrackJointDef.EnableLimit = true;
            cartTrackJointDef.Initialize(groundBody, _cartBody, new Vec2(0f, 0f), new Vec2(1f, 0f));
            _cartTrackJoint = (PrismaticJoint)_world.CreateJoint(cartTrackJointDef);

        // ===== Create arm1.
            const float poleRadius = 0.025f;	// Half the thickness of the pole.
            Vec2 arm1PosBase = new Vec2(0f, 0.275f);
            Body arm1Body = CreatePole(arm1PosBase, _cartJointInitialAngle, poleRadius, 0f, 0f, 2f, 0x0);

            // Join arm1 to cart.
            RevoluteJointDef poleJointDef = new RevoluteJointDef();
            poleJointDef.CollideConnected = false;
            poleJointDef.EnableMotor = false;
            poleJointDef.MaxMotorTorque = 0f;
            poleJointDef.Initialize(_cartBody, arm1Body, arm1PosBase);
            _cartJoint = (RevoluteJoint)_world.CreateJoint(poleJointDef);

        // ===== Create arm2.
            Vec2 arm2PosBase = CalcPoleEndPos(arm1Body);
            Body arm2Body = CreatePole(arm2PosBase, _elbowJointInitialAngle, poleRadius, 0f, 0f, 2f, 0x0);
            _arm2Body = arm2Body;

            // Join arm2 to arm1.            
            poleJointDef.CollideConnected = false;
            poleJointDef.EnableMotor = false;
            poleJointDef.MaxMotorTorque = 0f;
            poleJointDef.Initialize(arm1Body, arm2Body, arm2PosBase);
            _elbowJoint = (RevoluteJoint)_world.CreateJoint(poleJointDef);
        }

        private Body CreatePole(Vec2 basePos, float angle, float radius, float friction, float restitution, float density, ushort layers)
        {
            // ==== Create bar. ====
            // Determine position of top of pole relative to its center of mass.
            Vec2 polePosTopRelative = new Vec2(__ArmLength * (float)-SysMath.Sin(angle), __ArmLength * (float)SysMath.Cos(angle));
            Vec2 polePosTop = polePosTopRelative + basePos;
            Vec2 polePosCenter = (polePosTopRelative * 0.5f) + basePos;

            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(polePosCenter.X, polePosCenter.Y);
            bodyDef.Angle = angle;

            // Create body object; The body is also added to the world.
            Body body = _world.CreateBody(bodyDef);
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(radius, __ArmLength * 0.5f);
            shapeDef.Friction = friction;
            shapeDef.Restitution = restitution;
            shapeDef.Density = density;
            shapeDef.Filter.MaskBits = layers;
            shapeDef.Filter.CategoryBits = 0x3;
            body.CreateShape(shapeDef);

            // ==== Place some end caps on the bar. ====
            CircleDef circleDef = new CircleDef();
            circleDef.Radius = radius;
            circleDef.Friction = friction;
            circleDef.Restitution = restitution;
            circleDef.Density = 0f;
            circleDef.Filter.MaskBits = layers;
            circleDef.Filter.CategoryBits = 0x3;
            // Top cap.
            circleDef.LocalPosition.Set(0f, __ArmLength * 0.5f);
            body.CreateShape(circleDef);

            // Bottom cap.
            circleDef.LocalPosition.Set(0f, -__ArmLength * 0.5f);
            body.CreateShape(circleDef);

            body.SetMassFromShapes();
            return body;
        }

        private Vec2 CalcPoleEndPos(Body poleBody)
        {
            // Determine position of top of pole relative to its center of mass.
            float angle = poleBody.GetAngle();
            Vec2 polePosTopRelative = new Vec2(__ArmLength * (float)-SysMath.Sin(angle), __ArmLength * (float)SysMath.Cos(angle));
            return poleBody.GetPosition() + (polePosTopRelative * 0.5f);
        }

        #endregion
    }
}
