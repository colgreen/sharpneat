using System;
using System.Windows.Forms;
using Box2DX.Common;
using Box2DX.Dynamics;
using SharpNeat.DomainsExtra.Box2D;
using Tao.OpenGl;
using SharpNeat.DomainsExtra.SinglePoleBalancingBox2d;
using SharpNeat.DomainsExtra.InvertedDoublePendulum;
using SharpNeat.DomainsExtra.WalkerBox2d;

namespace SharpNeat.Box2dTestHarness
{
    public partial class MainForm : Form
    {
        #region Instance Fields

        SimulationWorld _simWorld;
        Vec2 _lowerBoundVisibleWorld;
        Vec2 _upperBoundVisibleWorld;
        DebugDraw _debugDraw;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            // Init openGL viewport / debug drawing object.
            openGlControl.InitializeContexts();
            InitDebugDraw();

            // Populate combobox.
            PopulateSimulationWorldListBox();
        }

        #endregion

        #region Private Methods [Init/Misc]

        private void PopulateSimulationWorldListBox()
        {
            cmbSimulationWorlds.Items.Add(new ListItem(string.Empty, "Single Pole Balancing", typeof(SinglePoleBalancingWorld)));
            cmbSimulationWorlds.Items.Add(new ListItem(string.Empty, "Inverted Double Pendulum", typeof(InvertedDoublePendulumWorld)));
            cmbSimulationWorlds.Items.Add(new ListItem(string.Empty, "Walker", typeof(WalkerWorld)));
        }

        /// <summary>
        /// Initialises OpenGL graphics library. Here we set what area of the box2d world to draw and
        /// the size of the drawing window on screen.
        /// 
        /// We maintain the correct aspect ratio as follows:
        /// 
        /// 1) Vertical extents of the visible box2D world are fixed. That is, if we increase the height of the control then the world gets stretched vertically.
        /// 2) Thus to maintain aspect ratio the horizontal extents of the visible Box2D world change. That is, if we widen the control we see more of the Box2d world.
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
        /// Initialise debug drawing object (interface between Box2D world and openGL rendering).
        /// </summary>
        private void InitDebugDraw()
        {
            // Attach display window drawing routine to the box2d world.
            _debugDraw = new OpenGLDebugDraw();
            uint flags = 0;
            flags += (uint)DebugDraw.DrawFlags.Shape;
            //flags += (uint)DebugDraw.DrawFlags.Joint;
            //flags += (uint)DebugDraw.DrawFlags.Controller;
            //flags += (uint)DebugDraw.DrawFlags.CoreShape;
            //flags += (uint)DebugDraw.DrawFlags.Aabb;
            //flags += (uint)DebugDraw.DrawFlags.Obb;
            //flags += (uint)DebugDraw.DrawFlags.Pair;
            //flags += (uint)DebugDraw.DrawFlags.CenterOfMass;
            _debugDraw.Flags = (DebugDraw.DrawFlags)flags;
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

        #region GUI Wiring [Simulation World Selection]

        private void cmbSimulationWorlds_SelectedIndexChanged(object sender,EventArgs e)
        {
            // Terminate any existing world/simulation.
            TerminateSimulation();

            // Check selection.
            ListItem listItem = cmbSimulationWorlds.SelectedItem as ListItem;
            if(null == listItem)
            {   // No selection; Do nothing.
                return;
            }

            // Instantiate selected sim world.
            InitSimWorld((Type)listItem.Data);
        }

        private void InitSimWorld(Type simWorldType)
        {
            _simWorld = (SimulationWorld)Activator.CreateInstance(simWorldType);
            _simWorld.InitSimulationWorld();
            _simWorld.SetDebugDraw(_debugDraw);
            SetView();

            // Start the simulation timer.
            simTimer.Start();
        }

        private void TerminateSimulation()
        {
            if(null == _simWorld)
            {   // Do nothing.
                return;
            }

            // Stop simulation.
            simTimer.Stop();
            _simWorld = null;
            // TODO: Explicit disposal?
        }

        #endregion

        #region Private Methods [Event Handling/Simulation Ticks]

        private void simTimer_Tick(object sender,EventArgs e)
        {
            SimStep();
        }

        /// <summary>
        /// Moves the simulation forward by one timestep.
        /// </summary>
        private void SimStep()
        {
            // Clear viewport buffer.
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            // Move world forward one timestep.
            _simWorld.Step();

            // Draw the world in its new/current state.
			openGlControl.Draw();
        }

        /// <summary>
        /// Mouse interaction with the box2d world.
        /// </summary>
        private void openGlControl_MouseDown(object sender,MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Left && null != _simWorld) {
				_simWorld.MouseDown(ConvertScreenToWorld(e.X, e.Y));
            }
        }

        /// <summary>
        /// Mouse interaction with the box2d world.
        /// </summary>
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

        /// <summary>
        /// Mouse interaction with the box2d world.
        /// </summary>
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

        #endregion
    }
}
