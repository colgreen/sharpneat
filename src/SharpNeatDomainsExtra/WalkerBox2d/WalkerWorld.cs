﻿/* ***************************************************************************
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
using Redzen.Random;
using SharpNeat.DomainsExtra.Box2D;
using SysMath = System.Math;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Box2D world for the Walker problem domain.
    /// </summary>
    public class WalkerWorld : SimulationWorld
    {
        const float __lowerLegLength = 0.5f;

        readonly IRandomSource _rng;

        readonly float _trackLength;
        readonly float _trackLengthHalf;

        Body _torsoBody;
        RevoluteJoint _leftHipJoint;
        RevoluteJoint _leftKneeJoint;
        Body _leftLowerLegBody;

        RevoluteJoint _rightHipJoint;
        RevoluteJoint _rightKneeJoint;       
        Body _rightLowerLegBody;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WalkerWorld() : this(RandomDefaults.CreateRandomSource(), 300)
        {}

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WalkerWorld(IRandomSource rng) : this(rng, 300)
        {}

        /// <summary>
        /// Constructor accepting a trackLength parameter (length of the track that the walker is walking along).
        /// </summary>
        /// <param name="trackLength"></param>
        /// <param name="rng">Random number generator.</param>
        public WalkerWorld(IRandomSource rng, float trackLength)
        {
            _rng = rng;
            _trackLength = trackLength;
            _trackLengthHalf = trackLength * 0.5f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise world.
        /// </summary>
        public override void InitSimulationWorld()
        {
            SimulationParameters simParams = new SimulationParameters();
            simParams._lowerBoundPhysics.Set(- 2f, -1f);
            simParams._upperBoundPhysics.Set(_trackLength + 1f, 4f);
            simParams._lowerBoundView.Set(- 2f, -0.2f);
            simParams._upperBoundView.Set(6, 3f);

            base.InitSimulationWorld(simParams);
        }

        /// <summary>
        /// Create an interface onto the walker player.
        /// </summary>
        /// <returns></returns>
        public WalkerInterface CreateWalkerInterface()
        {
            LegInterface legIfaceLeft = new LegInterface(_leftHipJoint, _leftKneeJoint, _leftLowerLegBody, __lowerLegLength);
            LegInterface legIfaceRight = new LegInterface(_rightHipJoint, _rightKneeJoint, _rightLowerLegBody, __lowerLegLength);
            return new WalkerInterface(_torsoBody, legIfaceLeft, legIfaceRight);
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
			groundBodyDef.Position.Set(_trackLengthHalf, -1f);

   			// Call the body factory which creates the ground box shape.
			// The body is also added to the world.
			Body groundBody = _world.CreateBody(groundBodyDef);

			// Define the ground box shape.
			PolygonDef groundShapeDef = new PolygonDef();

			// The extents are the half-widths of the box.
			groundShapeDef.SetAsBox(_trackLengthHalf + 1f, 1f);
            groundShapeDef.Friction = _simParams._defaultFriction;
            groundShapeDef.Restitution = _simParams._defaultRestitution;
            groundShapeDef.Filter.CategoryBits = 0x3;

			// Add the ground shape to the ground body.
            groundBody.CreateShape(groundShapeDef);

            // Add some small mounds/bumps to the ground.
            for (float x = -1f; x < 40f; x += 0.4f + ((_rng.NextFloat()-0.5f) * 0.15f)) {
                WalkerWorldUtils.CreateMound(_world, x, 0f, _simParams._defaultFriction, _simParams._defaultRestitution);
            }

            // ==== Define walker torso.
            float walkerX = 0f;
            float walkerY = 1.30f;// + ((float)_rng.NextDouble() * 0.1f);

            BodyDef torsoBodyDef = new BodyDef();
            torsoBodyDef.Position.Set(walkerX, walkerY);
            torsoBodyDef.IsBullet = true;

            // Create walker torso.
            _torsoBody = _world.CreateBody(torsoBodyDef);
            PolygonDef torsoShapeDef = new PolygonDef();
            torsoShapeDef.SetAsBox(0.10f, 0.30f);
            torsoShapeDef.Friction = _simParams._defaultFriction;
            torsoShapeDef.Restitution = 0f;
            torsoShapeDef.Density = 2f;
            torsoShapeDef.Filter.CategoryBits = 0x2;
            _torsoBody.CreateShape(torsoShapeDef);
            _torsoBody.SetMassFromShapes();

        // ===== Create legs.
            // Leg joint definition.
            RevoluteJointDef leftKneeJointDef = new RevoluteJointDef();
            leftKneeJointDef.CollideConnected = false;
            leftKneeJointDef.EnableMotor = true;
            leftKneeJointDef.MaxMotorTorque = 100f;

            RevoluteJointDef leftHipJointDef = new RevoluteJointDef();
            leftHipJointDef.CollideConnected = false;
            leftHipJointDef.EnableMotor = true;
            leftHipJointDef.MaxMotorTorque = 100f;

            RevoluteJointDef rightKneeJointDef = new RevoluteJointDef();
            rightKneeJointDef.CollideConnected = false;
            rightKneeJointDef.EnableMotor = true;
            rightKneeJointDef.MaxMotorTorque = 100f;

            RevoluteJointDef rightHipJointDef = new RevoluteJointDef();
            rightHipJointDef.CollideConnected = false;
            rightHipJointDef.EnableMotor = true;
            rightHipJointDef.MaxMotorTorque = 100f;

            // Other re-usable stuff .
            const float legRadius = 0.05f;	// Half the thickness of the leg
            Vec2 upperLegPosBase = new Vec2(walkerX, walkerY - 0.25f);
            Vec2 lowerLegPosBase = new Vec2(walkerX, walkerY - 0.75f);

        // ===== Create left leg.
            // Upper leg.
            Body upperLeftLegBody = CreatePole(upperLegPosBase, 0.5f, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to torso (hip joint)
            leftHipJointDef.Initialize(_torsoBody, upperLeftLegBody, upperLegPosBase);
            _leftHipJoint = (RevoluteJoint)_world.CreateJoint(leftHipJointDef);

            // Lower leg.
            _leftLowerLegBody = CreatePole(lowerLegPosBase, __lowerLegLength, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to upper leg (knee joint)
            leftKneeJointDef.Initialize(upperLeftLegBody, _leftLowerLegBody, lowerLegPosBase);
            _leftKneeJoint = (RevoluteJoint)_world.CreateJoint(leftKneeJointDef);

        // ===== Create right leg.
            // Upper leg.
            Body upperRightLegBody = CreatePole(upperLegPosBase, 0.5f, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to torso (hip joint)
            rightHipJointDef.Initialize(_torsoBody, upperRightLegBody, upperLegPosBase);
            _rightHipJoint = (RevoluteJoint)_world.CreateJoint(rightHipJointDef);

            // Lower leg.
            _rightLowerLegBody = CreatePole(lowerLegPosBase, __lowerLegLength, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to upper leg (knee joint)
            rightKneeJointDef.Initialize(upperRightLegBody, _rightLowerLegBody, lowerLegPosBase);
            _rightKneeJoint = (RevoluteJoint)_world.CreateJoint(rightKneeJointDef);
        }

        private Body CreatePole(Vec2 basePose, float length, float angle, float radius, float density, ushort layers)
        {
            // ==== Create bar. ====
            // Determine position of top of pole with base position of (0, 0), length of 1m and an angle CCW is +ve.
            Vec2 polePosTop = new Vec2(length * (float)-SysMath.Sin(angle), length * (float)SysMath.Cos(angle));
            Vec2 polePosCenter = (polePosTop * 0.5f) + basePose;

            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(polePosCenter.X, polePosCenter.Y);
            bodyDef.Angle = angle;
            bodyDef.IsBullet = true;

            // Create body object; The body is also added to the world.
            Body body = _world.CreateBody(bodyDef);
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(radius, length * 0.5f);
            shapeDef.Friction = _simParams._defaultFriction;
            shapeDef.Restitution = _simParams._defaultRestitution;
            shapeDef.Density = density;
            shapeDef.Filter.MaskBits = 0xFFFE;
            shapeDef.Filter.CategoryBits = layers;
            body.CreateShape(shapeDef);

            // ==== Place some end caps on the pole. ====
            CircleDef circleDef = new CircleDef();
            circleDef.Radius = radius;
            circleDef.Friction = _simParams._defaultFriction;
            circleDef.Restitution = _simParams._defaultRestitution;
            circleDef.Density = density;
            circleDef.Filter.MaskBits = 0xFFFE;
            circleDef.Filter.CategoryBits = layers;

            // Top cap.
            circleDef.LocalPosition.Set(0f, length * 0.5f);
            body.CreateShape(circleDef);

            // Bottom cap.
            circleDef.LocalPosition.Set(0f, -length * 0.5f);
            body.CreateShape(circleDef);
            body.SetMassFromShapes();
            return body;
        }

        #endregion
    }
}
