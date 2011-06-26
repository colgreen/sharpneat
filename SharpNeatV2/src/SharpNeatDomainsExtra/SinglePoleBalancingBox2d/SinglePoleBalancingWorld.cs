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
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using SharpNeat.DomainsExtra.Box2D;
using SysMath = System.Math;

namespace SharpNeat.DomainsExtra.SinglePoleBalancingBox2d
{
    /// <summary>
    /// Box2D world for the single pole balancing domain.
    /// </summary>
    public class SinglePoleBalancingWorld : SimulationWorld
    {
        #region Instance Fields

        float _initialPoleAngle;
        float _trackLength;
        float _trackLengthHalf;
        Body _cartBody;
        RevoluteJoint _poleJoint;
        PrismaticJoint _cartTrackJoint;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Construct with default world settings.
        /// </summary>
        public SinglePoleBalancingWorld() : this(4.8f, (float)SysMath.PI)
        {}

        /// <summary>
        /// Construct with specified world settings.
        /// </summary>
        public SinglePoleBalancingWorld(float trackLength, float initialPoleAngle)
        {
            _trackLength = trackLength;
            _trackLengthHalf = trackLength * 0.5f;
            _initialPoleAngle = initialPoleAngle;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inititialise world.
        /// </summary>
        public override void InitSimulationWorld()
        {
            SimulationParameters simParams = new SimulationParameters();
            simParams._lowerBoundPhysics.Set(-_trackLengthHalf - 2f , -1.0f);
            simParams._upperBoundPhysics.Set(_trackLengthHalf + 2f, 2f);
            simParams._lowerBoundView.Set(-_trackLengthHalf - 1f, -1.0f);
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
        /// Gets the pole angle in radians. Zero is up, positive angles => counter-clockwise.
        /// </summary>
        public float PoleAngle
        {
            get 
            {
                // Joint angle is cumulative (increases for successive revolutions), so first we factor out the excess revolutions.
                double angle = (_initialPoleAngle + _poleJoint.JointAngle) % (2.0*SysMath.PI);

                // Now define an alternative scheme with 0 degrees at the top and +-180 degrees at the bottom (+ve is CCW, -ve is CW).
                if(angle > SysMath.PI) {
                    angle = angle - (2.0*SysMath.PI);
                }
                return (float)angle; 
            }
        }

        /// <summary>
        /// Gets the pole's angular velocity in radians/sec.
        /// </summary>
        public float PoleAngularVelocity
        {
            get { return _poleJoint.JointSpeed; }
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

        // ===== Create pole.
            const float poleRadius = 0.025f;	// Half the thickness of the pole.
            Vec2 polePosBase = new Vec2(0f, 0.275f);
            Body poleBody = CreatePole(polePosBase, _initialPoleAngle, poleRadius, 0f, 0f, 2f, 0x0);

            // Join pole to cart.
            RevoluteJointDef poleJointDef = new RevoluteJointDef();
            poleJointDef.CollideConnected = false;
            poleJointDef.EnableMotor = false;
            poleJointDef.MaxMotorTorque = 0f;

            poleJointDef.Initialize(_cartBody, poleBody, polePosBase);
            _poleJoint = (RevoluteJoint)_world.CreateJoint(poleJointDef);
        }

        private Body CreatePole(Vec2 basePose, float angle, float radius, float friction, float restitution, float density, ushort layers)
        {
            const float __poleLength = 1f;

            // ==== Create bar. ====
            // Determine position of top of pole with base position of (0, 0), length of 1m and an angle CCW is +ve.
            Vec2 polePosTop = new Vec2(__poleLength * (float)-SysMath.Sin(angle), __poleLength * (float)SysMath.Cos(angle));
            Vec2 polePosCenter = (polePosTop * 0.5f) + basePose;

            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(polePosCenter.X, polePosCenter.Y);
            bodyDef.Angle = angle;

            // Create body object; The body is also added to the world.
            Body body = _world.CreateBody(bodyDef);
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(radius, __poleLength * 0.5f);
            shapeDef.Friction = friction;
            shapeDef.Restitution = restitution;
            shapeDef.Density = density;
            shapeDef.Filter.MaskBits = layers;
            shapeDef.Filter.CategoryBits = 0x3;
            body.CreateShape(shapeDef);

            // ==== Place some end caps on the pole. ====
            CircleDef circleDef = new CircleDef();
            circleDef.Radius = radius;
            circleDef.Friction = friction;
            circleDef.Restitution = restitution;
            circleDef.Density = 0f;
            circleDef.Filter.MaskBits = layers;
            circleDef.Filter.CategoryBits = 0x3;
            // Top cap.
            circleDef.LocalPosition.Set(0f, __poleLength * 0.5f);
            body.CreateShape(circleDef);

            // Bottom cap.
            circleDef.LocalPosition.Set(0f, -__poleLength * 0.5f);
            body.CreateShape(circleDef);

            body.SetMassFromShapes();
            return body;
        }

        #endregion
    }
}
