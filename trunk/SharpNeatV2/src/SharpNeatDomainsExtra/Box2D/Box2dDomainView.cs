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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Box2DX.Common;
using Box2DX.Dynamics;
using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.DomainsExtra.Box2D;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using Tao.OpenGl;

namespace SharpNeat.DomainsExtra.Box2D
{
    /// <summary>
    /// Abstract class for Box2D based problem domain visualization.
    /// </summary>
    public abstract partial class Box2dDomainView : AbstractDomainView // UserControl
    {
        #region Instance Fields

        IGenomeDecoder<NeatGenome,IBlackBox> _genomeDecoder;
        /// <summary>
        /// The black box controller used by the simulation thread.
        /// </summary>
        protected IBlackBox _box;
        /// <summary>
        /// Thread for running simulation.
        /// </summary>
        Thread _simThread;
        /// <summary>
        /// Indicates is a simulation is running. Access is thread synchronised using Interlocked.
        /// </summary>
        int _simRunningFlag = 0;
        /// <summary>
        /// Event that signals simulation thread to start a simulation.
        /// </summary>
        AutoResetEvent _simStartEvent = new AutoResetEvent(false);
        /// <summary>
        /// Signal sim thread to stop current simulation.
        /// </summary>
        bool _simStopFlag = false;
        /// <summary>
        /// The Box2D world.
        /// </summary>
        protected SimulationWorld _simWorld;
        int _timestepDelayMs;
        Vec2 _lowerBoundVisibleWorld;
        Vec2 _upperBoundVisibleWorld;
        DebugDraw _debugDraw;
        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder, this is used to decode genome(s) into IBlackBox controllers.
        /// </summary>
        public Box2dDomainView(IGenomeDecoder<NeatGenome,IBlackBox> genomeDecoder)
        {
            InitializeComponent();
            _genomeDecoder = genomeDecoder;

            // Init openGL viewport / debug drawing object.
            openGlControl.InitializeContexts();
            InitDebugDraw();

            // Create background thread for running simulation alongside NEAT algorithm.
            _simThread = new Thread(new ThreadStart(SimulationThread));
            _simThread.IsBackground = true;
            _simThread.Start();
        }

        #endregion

        #region Public Methods [Domain View]

        /// <summary>
        /// Refresh/update the view with the provided genome.
        /// </summary>
        public override void RefreshView(object genome)
        {
            // Zero indicates that the simulation is not currently running.
            if(0 == Interlocked.Exchange(ref _simRunningFlag, 1))
            {
                // We got the lock. Decode the genome and store result in an instance field.
                NeatGenome neatGenome = genome as NeatGenome;
                _box = _genomeDecoder.Decode(neatGenome);

                // Signal simulation thread to start running a simulation.
                _simStartEvent.Set();
            }
        }

        /// <summary>
        /// Define a default size for the window.
        /// </summary>
        public override System.Drawing.Size WindowSize
        {
            get { return new Size(655, 396); }
        }

        #endregion

        #region Abstract Methods [BOX2D Simulation Creation / Control]

        /// <summary>
        /// Create a Box2D simulation world.
        /// </summary>
        protected abstract SimulationWorld CreateSimulationWorld();
        /// <summary>
        /// Invoke any required control logic in the Box2D world.
        /// </summary>
        protected abstract void InvokeController();
        /// <summary>
        /// Test if the Box2D world has arrived at a stop condition.
        /// </summary>
        protected virtual bool TestStopCondition()
        {
            return _simStopFlag;
        }

        #endregion

        #region Private Methods [Initialisation / Viewport + Window Coordinate System Handling]

        /// <summary>
        /// Initialise debug drawing object (interface between Box2D world and openGL rendering).
        /// </summary>
        private void InitDebugDraw()
        {
            // Attach display window drawing routine to the box2d world.
            _debugDraw = new OpenGLDebugDraw();
            uint flags = 0;
            flags += (uint)DebugDraw.DrawFlags.Shape;
            //flags += (uint)DebugDraw.DrawFlags.Joint;
            flags += (uint)DebugDraw.DrawFlags.Controller;
            //flags += (uint)DebugDraw.DrawFlags.CoreShape;
            //flags += (uint)DebugDraw.DrawFlags.Aabb;
            //flags += (uint)DebugDraw.DrawFlags.Obb;
            //flags += (uint)DebugDraw.DrawFlags.Pair;
            //flags += (uint)DebugDraw.DrawFlags.CenterOfMass;
            _debugDraw.Flags = (DebugDraw.DrawFlags)flags;
        }

        /// <summary>
        /// Set the extents of the viewport and the box2D world. The extents of the two coordinate systems are set to
        /// preserve the correct aspect ratio when renderign the Box2d world. This is done simply be always rendering 
        /// the full height of the box2D world and varying the horizontal extents to maintain the aspect ratio as the 
        /// viewport is resized.
        /// </summary>
        private void SetView()
        {
            // Set viewport size to match teh size of its containing form control.
            int viewportWidth = openGlControl.Width;
            int viewportHeight = openGlControl.Height;
            Gl.glViewport(0, 0, viewportWidth, viewportHeight);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            
            // Set box2D world extents to maintain proper aspect ratio.
            double viewportAspectRatio = (double)viewportWidth / (double)viewportHeight;
            double worldTop = _simWorld.SimulationParameters._upperBoundView.Y;
            double worldBottom = _simWorld.SimulationParameters._lowerBoundView.Y;
            double worldHeight = worldTop - worldBottom;
            double worldCenterX = (_simWorld.SimulationParameters._upperBoundView.X + _simWorld.SimulationParameters._lowerBoundView.X) * 0.5f;
            double worldWidth = worldHeight * viewportAspectRatio;
            double worldLeft = worldCenterX - (worldWidth * 0.5f);
            double worldRight = worldCenterX + (worldWidth * 0.5f);

            Glu.gluOrtho2D(worldLeft, worldRight, worldBottom, worldTop);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // Store bounds of visible world.
            _lowerBoundVisibleWorld.Set((float)worldLeft, (float)worldBottom);
            _upperBoundVisibleWorld.Set((float)worldRight, (float)worldTop);
        }

        /// <summary>
        /// Convert screen coordinates to box2d world coords.
        /// </summary>
        private Vec2 ConvertScreenToWorld(float x, float y)
        {
            float ctrlWidth = openGlControl.Width;
            float ctrlHeight = openGlControl.Height;

            float xProp = x / ctrlWidth;
            float yProp = (ctrlHeight - y) / ctrlHeight;

            Vec2 p = new Vec2();
            p.X = _lowerBoundVisibleWorld.X + (xProp * (_upperBoundVisibleWorld.X - _lowerBoundVisibleWorld.X));
            p.Y = _lowerBoundVisibleWorld.Y + (yProp * (_upperBoundVisibleWorld.Y - _lowerBoundVisibleWorld.Y));
            return p;
        }

        #endregion

        #region Private Methods [Simulation Thread]

        /// <summary>
        /// Run simulations until thread is terminated.
        /// </summary>
        private void SimulationThread()
        {
            try
            {
                // Wait for first black box controller to be passed in.
                _simStartEvent.WaitOne();
                for(;;)
                {
                    try
                    {
                        RunTrial();
                    }
                    finally
                    {   // Simulation completed. Reset _simRunningFlag to allow another simulation to be started.
                        Interlocked.Exchange(ref _simRunningFlag, 0);
                    }
                }
            }
            catch(ThreadAbortException)
            {   // Thread abort exceptions are expected.
            }
        }

        /// <summary>
        /// Run one simulation.
        /// </summary>
        private void RunTrial()
        {
            // Ensure flag is reset before we enter the main trial loop.
            _simStopFlag = false;

            // Get local copy of black box pointer so that the same one is used throughout each individual simulation trial/run 
            // (_box is being continually updated by the evolution algorithm update events). This is probably an atomic
            // operation and thus thread safe.
            IBlackBox box = _box;
            box.ResetState();

            // Create/init new simulation world.
            _simWorld = CreateSimulationWorld();
            _simWorld.SetDebugDraw(_debugDraw);
            _timestepDelayMs = (int)(1000f / (float)_simWorld.SimulationParameters._frameRate);

            // Initialise the viewport on the GUI thread.
            Invoke(new MethodInvoker(delegate() 
            {   
                SetView(); 
            }));
            
            // Run main simulation loop until a stop condition occurs.
            for(;;)
            {
                // Perform GUI operations on the GUI thread.
                Invoke(new MethodInvoker(delegate() 
                {
                    // Clear viewport buffer.
			        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                    // Advance the simulation world forward by one timestep.
                    // The simulation world has callbacks to the debug drawer object, 
                    // hence this call is made on the GUI thread.
                    _simWorld.Step();

                    // Draw the world in its new/current state.
			        openGlControl.Draw();
                }));

                // Invoke controller logic (if any).
                InvokeController();

                if(TestStopCondition()) {
                    break;
                }

                // Slow simulation down to run it in realtime.
                Thread.Sleep(_timestepDelayMs);
            }
        }

        #endregion

        #region GUI Wiring [Event Handlers]

        private void btnReset_Click(object sender,EventArgs e)
        {
            _simStopFlag = true;
        }

        private void openGlControl_MouseDown(object sender,MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Left && null != _simWorld) {
				_simWorld.MouseDown(ConvertScreenToWorld(e.X, e.Y));
            }
        }

        private void openGlControl_MouseMove(object sender,MouseEventArgs e)
        {
            if(null == _simWorld) {
                return;
            }
			Vec2 p = ConvertScreenToWorld(e.X, e.Y);
			_simWorld.MouseMove(p);

            // Display mouse position in sim world coords.
            lblMouseWorldCoords.Text = string.Format("[{0:0.00}] [{1:0.00}]", p.X, p.Y);
        }

        private void openGlControl_MouseUp(object sender,MouseEventArgs e)
        {
            if(null == _simWorld) {
                return;
            }
            _simWorld.MouseUp();
        }

        private void openGlControl_Resize(object sender,EventArgs e)
        {
            if(null == _simWorld) {
                return;
            }
            SetView();
        }

        /// <summary>
        /// Event handler to clean-up on window closure.
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Stop the simulation thread. Otherwise painting requests to the dead control will throw an exception.
            if(null != _simThread) {
                _simThread.Abort();
            }
            base.OnHandleDestroyed(e);
        }

        #endregion
    }
}
