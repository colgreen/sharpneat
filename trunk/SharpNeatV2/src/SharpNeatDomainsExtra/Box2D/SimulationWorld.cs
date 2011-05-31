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
using Tao.OpenGl;

namespace SharpNeat.DomainsExtra.Box2D
{
    /// <summary>
    /// Abstract class for Box2D simulations/worlds within SharpNEAT.
    /// </summary>
    public abstract class SimulationWorld
    {
        #region Instance Fields

        /// <summary>
        /// High level simulation parameters.
        /// </summary>
        protected SimulationParameters _simParams;
        /// <summary>
        /// The Box2D world.
        /// </summary>
        protected World _world;
        /// <summary>
        /// The world's current clock time (in seconds).
        /// </summary>
        float _currentTime = 0f;
        /// <summary>
        /// The mousejoint is created on a mouseclick and allows world objects to be grabbed and moved around.
        /// The joint is destroyed when the mouseclick is released (mouseup event).
        /// </summary>
        MouseJoint _mouseJoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct new SimulationWorld.
        /// </summary>
        public SimulationWorld()
        {}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the world's high level simulation parameters.
        /// </summary>
        public SimulationParameters SimulationParameters
        {
            get { return _simParams; }
        }

        /// <summary>
        /// Gets the world's current clock time (in seconds).
        /// </summary>
        public float CurrentTime
        {
            get { return _currentTime; }
        }

        #endregion

        #region Initialisation Methods

        /// <summary>
        /// Primary initialisation method. Override this method to do sub-class specific initialisation.
        /// </summary>
        public virtual void InitSimulationWorld()
        {
            InitSimulationWorld(new SimulationParameters());
        }

        /// <summary>
        /// Initialise world with the specified SimulationParameters.
        /// </summary>
        /// <param name="simParams"></param>
        protected void InitSimulationWorld(SimulationParameters simParams)
        {
            _simParams = simParams;
            _world = CreateBox2DWorld();

            // Allow physics calcs to use values from previous timestep.
            _world.SetWarmStarting(_simParams._warmStarting);

            // Enable additional collision detection for high speed objects (that might not ever contact each other at a given timestep due to speed).
            _world.SetContinuousPhysics(_simParams._continuousPhysics);

            // Put stuff in the world.
            PopulateWorld();

            // Create contact listener.
            ContactListener contactListener = CreateContactListener();
            if(null != contactListener) {
                _world.SetContactListener(contactListener);
            }
        }

        /// <summary>
        /// Create an empty Box2D world.
        /// </summary>
        private World CreateBox2DWorld()
        {
			AABB worldAABB = new AABB();
			worldAABB.LowerBound.Set(_simParams._lowerBoundPhysics.X, _simParams._lowerBoundPhysics.Y);
			worldAABB.UpperBound.Set(_simParams._upperBoundPhysics.X, _simParams._upperBoundPhysics.Y);
			Vec2 gravity = new Vec2();
			gravity.Set(0.0f, _simParams._gravity);
			bool doSleep = false;
			World world = new World(worldAABB, gravity, doSleep);
            return world;
        }

        /// <summary>
        /// Add objects to the Box2d world.
        /// </summary>
        protected abstract void PopulateWorld();

        /// <summary>
        /// Create contact listener.
        /// </summary>
        protected virtual ContactListener CreateContactListener()
        {   // No listener by default.
            return null;
        }

        /// <summary>
        /// Sets a drawing routine. Methods on the provided object are called-back to perform drawing on each update.
        /// </summary>
        /// <param name="debugDraw"></param>
        public void SetDebugDraw(DebugDraw debugDraw)
        {
            _world.SetDebugDraw(debugDraw);
        }

        #endregion

        #region Public Methods [Simulation Execution]

        /// <summary>
        /// Perform one simulation timestep; move the simulation forward by the timestep increment duration.
        /// </summary>
        public void Step()
        {
            //_contactListener.Reset();
            _world.Step(_simParams._timeStep, _simParams._velocityIters, _simParams._positionIters);
            _world.Validate();
            _currentTime += _simParams._timeStep;

            // Draw mouse joint if present.
            if (_mouseJoint != null)
            {
                DrawMouseJoint();
            }
        }

        #endregion

        #region Public Methods [External Mouse Event Handling]

        /// <summary>
        /// Mouse interaction event.
        /// </summary>
		public void MouseDown(Vec2 p)
		{
			if (_mouseJoint != null)
			{
				return;
			}

			// Make a small box.
			AABB aabb = new AABB();
			Vec2 d = new Vec2();
			d.Set(0.001f, 0.001f);
			aabb.LowerBound = p - d;
			aabb.UpperBound = p + d;

			// Query the world for overlapping shapes.
			int k_maxCount = 10;
			Shape[] shapes = new Shape[k_maxCount];
			int count = _world.Query(aabb, shapes, k_maxCount);
			Body body = null;
			for (int i = 0; i < count; ++i)
			{
				Body shapeBody = shapes[i].GetBody();
				if (shapeBody.IsStatic() == false && shapeBody.GetMass() > 0.0f)
				{
					bool inside = shapes[i].TestPoint(shapeBody.GetXForm(), p);
					if (inside)
					{
						body = shapes[i].GetBody();
						break;
					}
				}
			}

			if (body != null)
			{
				MouseJointDef md = new MouseJointDef();
				md.Body1 = _world.GetGroundBody();
				md.Body2 = body;
				md.Target = p;
                md.MaxForce = 1000.0f * body.GetMass();

				_mouseJoint = (MouseJoint)_world.CreateJoint(md);
				body.WakeUp();
			}
		}

        /// <summary>
        /// Mouse interaction event.
        /// </summary>
		public void MouseUp()
		{
			if (_mouseJoint != null)
			{
				_world.DestroyJoint(_mouseJoint);
				_mouseJoint = null;
			}
		}

        /// <summary>
        /// Mouse interaction event.
        /// </summary>
		public void MouseMove(Vec2 p)
		{
			if (_mouseJoint != null)
			{
				_mouseJoint.SetTarget(p);
			}
		}

        #endregion

        #region Private Methods [Mouse Joint Drawing]

        /// <summary>
        /// Draw the mouse joint (user interation via mouse events). This method can be removed to remove the dependency on OpenGL.
        /// </summary>
        private void DrawMouseJoint()
        {
            Body body = _mouseJoint.GetBody2();
            Vec2 p1 = body.GetWorldPoint(_mouseJoint._localAnchor);
            Vec2 p2 = _mouseJoint._target;

            Gl.glPointSize(4.0f);
            Gl.glColor3f(0.0f, 1.0f, 0.0f);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glVertex2f(p1.X, p1.Y);
            Gl.glVertex2f(p2.X, p2.Y);
            Gl.glEnd();
            Gl.glPointSize(1.0f);

            Gl.glColor3f(0.8f, 0.8f, 0.8f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2f(p1.X, p1.Y);
            Gl.glVertex2f(p2.X, p2.Y);
            Gl.glEnd();
        }

        #endregion
    }
}
