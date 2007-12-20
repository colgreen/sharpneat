using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpNeatLib.Experiments;


namespace SharpNeat.PreyCaptureSimulator
{
	public class PreyCaptureGrid : System.Windows.Forms.UserControl
	{
		#region Class variables
//		[DllImport("User32")] 
//		private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam); 
//		private const int WM_SETREDRAW = 0xB; 

		int gridSize=0;
		System.Windows.Forms.Button[,] grid = null;
		PreyCaptureBase preyCapture=null;
		Color gridColor = Color.Bisque;
		Color agentColor = Color.Red;
		Color preyColor = Color.Green;

		Point agentPosition, preyPosition;

		#endregion

		#region Class Designer variables
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Dispose

		public PreyCaptureGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// PreyCaptureGrid
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "PreyCaptureGrid";
			this.Size = new System.Drawing.Size(208, 224);

		}
		#endregion

		#region Public methods

		public void Initialise(PreyCaptureBase preyCapture)
		{
			if(this.preyCapture != null)
				if(gridSize!=preyCapture.GridSize)
					throw new Exception("Cannot initialise PreyCapture object with incompatible grid size.");


			this.preyCapture = preyCapture;
			this.gridSize = preyCapture.GridSize;

			if(grid==null)
			{
				grid = new Button[gridSize,gridSize];

				// Build the grid.

				for(int x=0; x<gridSize; x++)
					for(int y=0; y<gridSize; y++)
					{
						Button button = new Button();
						button.BackColor = Color.Bisque;

						grid[x,y] = button;
						this.Controls.Add(button);
					}

				agentPosition = new Point(0,0);
				preyPosition = new Point(0,0); 
			}
			DrawGrid();
		}

		


		public void RefreshGrid()
		{
			if(gridSize==0)	return;

			for(int x=0; x<gridSize; x++)
			{
				for(int y=0; y<gridSize; y++)
				{
					if(IsAgentAtCoord(x,y))
						grid[x,y].BackColor = agentColor;
					else if(IsPreyAtCoord(x,y))
						grid[x,y].BackColor = preyColor;
					else
					{
						double angle, distance;
						GetRelativePosition(x, y, out angle, out distance);
						if(distance<=preyCapture.SensorRange)
						{
							const double pi_8 = Math.PI/8.0;
							Color color = Color.White;

							if((angle>= 15.0*pi_8) && (angle<2.0*Math.PI))		color = Color.LightCyan;
							else if((angle>= 0.0) && (angle<pi_8))				color = Color.LightCyan;
							else if((angle>=pi_8) && (angle<3.0*pi_8))			color = Color.LightSkyBlue;
							else if((angle>=3.0*pi_8) && (angle<5.0*pi_8))		color = Color.LightCyan;
							else if((angle>=5.0*pi_8) && (angle<7.0*pi_8))		color = Color.LightSkyBlue;
							else if((angle>=7.0*pi_8) && (angle<9.0*pi_8))		color = Color.LightCyan;
							else if((angle>=9.0*pi_8) && (angle<11.0*pi_8))		color = Color.LightSkyBlue;
							else if((angle>=11.0*pi_8 ) && (angle<13.0*pi_8))	color = Color.LightCyan;
							else												color = Color.LightSkyBlue;
							grid[x,y].BackColor = color;
							

						}
						else
						{	
							grid[x,y].BackColor = gridColor;
						}



					}
//						if(IsCoordWithAgentSensorRange(x,y))
//						grid[x,y].BackColor = Color.LightGray;
//					else
//						grid[x,y].BackColor = gridColor;
				}
			}

//			// Paint over old occupied cells.
//			grid[agentPosition.X, agentPosition.Y].BackColor = gridColor;
//			grid[preyPosition.X, preyPosition.Y].BackColor = gridColor;
//
//			agentPosition.X = preyCapture.AgentX;
//			agentPosition.Y = preyCapture.AgentY;
//			preyPosition.X = preyCapture.PreyX;
//			preyPosition.Y = preyCapture.PreyY;
//
//			// Draw the agent & the prey at their new positions.
//			grid[agentPosition.X, agentPosition.Y].BackColor = agentColor;
//			grid[preyPosition.X, preyPosition.Y].BackColor = preyColor;
		}

		private bool IsAgentAtCoord(int x, int y)
		{
			return (preyCapture.AgentX==x && preyCapture.AgentY==y);
		}

		private bool IsPreyAtCoord(int x, int y)
		{
			return (preyCapture.PreyX==x && preyCapture.PreyY==y);
		}

		private bool IsCoordWithAgentSensorRange(int x, int y)
		{
			//----- Calculate angle of prey from agent.
			double a=(double)x - preyCapture.AgentX;								
			double o=(double)y - preyCapture.AgentY;

			//----- Determine distance of prey from agent (pythagoras' rule).
			double distance= Math.Sqrt(Math.Pow(o,2.0) + Math.Pow(a,2.0));	
			return distance <= preyCapture.SensorRange;
		}


		#endregion

		#region Private methods

		private void DrawGrid()
		{
			if(gridSize==0) return;

//			SendMessage(this.Handle, WM_SETREDRAW, 0, 0); 

			int xIncrement = this.Width / gridSize;
			int yIncrement = this.Height / gridSize;

			int xCurrent = 2;
			int yCurrent = (yIncrement*(gridSize-1)) + 2;

			for(int xIdx=0; xIdx<gridSize; xIdx++)
			{
				for(int yIdx=0; yIdx<gridSize; yIdx++)
				{
					Button button = grid[xIdx,yIdx];
					button.BackColor = Color.Bisque;
					button.Height = yIncrement-1;
					button.Width = xIncrement-1;

					button.Top = yCurrent;
					button.Left = xCurrent;

					yCurrent-=yIncrement;
				}
				yCurrent = (yIncrement*(gridSize-1)) + 2;
				xCurrent+=xIncrement;
			}

//			SendMessage(this.Handle, WM_SETREDRAW, 1, 0); 
//			this.Invalidate(true); 
			RefreshGrid();
		}

		/// <summary>
		/// Determine the relative (polar coords) position of the given grid (cartesian coords) position from the agent. 
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="distance"></param>
		void GetRelativePosition(int x, int y, out double angle, out double distance)
		{
			double a, o;	// Adjacent, opposite.

			//----- Calculate angle of prey from agent.
			a=(double)x - preyCapture.AgentX;								
			o=(double)y - preyCapture.AgentY;

			if(a==0.0)
			{
				if(o<0.0) angle=1.5*Math.PI; else angle=Math.PI/2.0;
			}
			else if(o==0.0)
			{
				if(a<0.0) angle=Math.PI; else angle=0.0;
			}
			else
			{
				angle= Math.Abs(Math.Atan(o/a));	

				// Test quadrants & modify angle accordingly.
				if((a<0.0) && (o>=0.0)) angle=Math.PI-angle;		// Cartesian quadrant 2.
				if((a<0.0) && (o<0.0)) angle+=Math.PI;				// Cartesian quadrant 3.
				if((a>=0.0) && (o<0.0)) angle=2.0*Math.PI-angle;	// Cartesian quadrant 4.
			}

			//----- Determine distance of prey from agent (pythagoras' rule).
			distance= Math.Sqrt(Math.Pow(o,2.0) + Math.Pow(a,2.0));					
		}

		#endregion
	}
}
