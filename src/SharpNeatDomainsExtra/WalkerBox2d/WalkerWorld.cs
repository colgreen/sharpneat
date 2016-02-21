/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2012 Colin Green (sharpneat@gmail.com)
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
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
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

        float _trackLength;
        float _trackLengthHalf;

        Body _torsoBody;
        RevoluteJoint _leftHipJoint;
        RevoluteJoint _leftKneeJoint;
        Body _leftLowerLegBody;

        RevoluteJoint _rightHipJoint;
        RevoluteJoint _rightKneeJoint;       
        Body _rightLowerLegBody;

        #region Constructors

        /// <summary>
        /// Defautl constrcutor.
        /// </summary>
        public WalkerWorld() : this(300)
        {}

        /// <summary>
        /// Constructor accepting a trackLength parameter (length of the track that the walker is walking along).
        /// </summary>
        /// <param name="trackLength"></param>
        public WalkerWorld(float trackLength)
        {
            _trackLength = trackLength;
            _trackLengthHalf = trackLength * 0.5f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inititialise world.
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

        // ==== Define walker torso.
            BodyDef torsoBodyDef = new BodyDef();
            torsoBodyDef.Position.Set(0f, 1.45f);
            torsoBodyDef.IsBullet = true;

            // Create walker torso.
            _torsoBody = _world.CreateBody(torsoBodyDef);
            PolygonDef torsoShapeDef = new PolygonDef();
            torsoShapeDef.SetAsBox(0.10f, 0.45f);
            torsoShapeDef.Friction = _simParams._defaultFriction;
            torsoShapeDef.Restitution = 0f;
            torsoShapeDef.Density = 2f;
            torsoShapeDef.Filter.CategoryBits = 0x2;
            _torsoBody.CreateShape(torsoShapeDef);
            _torsoBody.SetMassFromShapes();

        // ===== Create legs.
            // Leg joint definition.
            RevoluteJointDef jointDef = new RevoluteJointDef();
            jointDef.CollideConnected = false;
            jointDef.EnableMotor = true;
            jointDef.MaxMotorTorque = 0f;

            // Other re-usable stuff .
            const float legRadius = 0.05f;	// Half the thickness of the leg
            Vec2 upperLegPosBase = new Vec2(0f, 1.05f);
            Vec2 lowerLegPosBase = new Vec2(0f, 0.55f);

        // ===== Create left leg.
            // Upper leg.
            Body upperLeftLegBody = CreatePole(upperLegPosBase, 0.5f, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to torso (hip joint)
            jointDef.Initialize(_torsoBody, upperLeftLegBody, upperLegPosBase);
            _leftHipJoint = (RevoluteJoint)_world.CreateJoint(jointDef);

            // Lower leg.
            _leftLowerLegBody = CreatePole(lowerLegPosBase, __lowerLegLength, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to upper leg (knee joint)
            jointDef.Initialize(upperLeftLegBody, _leftLowerLegBody, lowerLegPosBase);
            _leftKneeJoint = (RevoluteJoint)_world.CreateJoint(jointDef);

        // ===== Create right leg.
            // Upper leg.
            Body upperRightLegBody = CreatePole(upperLegPosBase, 0.5f, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to torso (hip joint)
            jointDef.Initialize(_torsoBody, upperRightLegBody, upperLegPosBase);
            _rightHipJoint = (RevoluteJoint)_world.CreateJoint(jointDef);

            // Lower leg.
            _rightLowerLegBody = CreatePole(lowerLegPosBase, __lowerLegLength, (float)SysMath.PI, legRadius, 2f, 0x1);
            // Join to upper leg (knee joint)
            jointDef.Initialize(upperRightLegBody, _rightLowerLegBody, lowerLegPosBase);
            _rightKneeJoint = (RevoluteJoint)_world.CreateJoint(jointDef);
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
