using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using SharpNeatLib.Experiments.TicTacToe;

namespace SharpNeat.TicTacToeGame
{
	public class TicTacToeControl : System.Windows.Forms.UserControl
	{
		Board board;
		Brush brushBackground = new SolidBrush(Color.White);
		Pen penBlack = new Pen(Color.Black, 2.0F);

		// Distance between the grid lines. Used to draw the lines and to calculate which
		// grid square the user is clicking.
		int xIncrement;
		int yIncrement;

		#region Events

		public event TicTacToeClickHandler GridClick;

		#endregion

		#region Windows Component designer variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor / Disposal

		public TicTacToeControl(Board board)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Store a reference to the tic-tac-toe board.
			this.board = board;
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
			// TicTacToeControl
			// 
			this.Name = "TicTacToeControl";
			this.Size = new System.Drawing.Size(184, 184);
			this.Resize += new System.EventHandler(this.TicTacToeControl_Resize);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TicTacToeControl_MouseDown);

		}
		#endregion

		#region Properties

//		public Board Board
//		{
//			get
//			{
//				return board;
//			}
//		}

		#endregion

		#region Painting Routines

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

		//----- Paint grid.
			xIncrement = Width / 3;
			yIncrement = Height / 3;

			int xCurrent = xIncrement;
			g.DrawLine(penBlack, xCurrent, 0, xCurrent, Height);

			xCurrent += xIncrement;
			g.DrawLine(penBlack, xCurrent, 0, xCurrent, Height);

			int yCurrent = yIncrement;
			g.DrawLine(penBlack, 0, yCurrent, Width, yCurrent);

			yCurrent += yIncrement;
			g.DrawLine(penBlack, 0, yCurrent, Width, yCurrent);

		//----- Paint the noughts and crosses.	
			int xBase = Width/6;
			int yBase = Height/6;

			// Don't touch the edges of the grid.
			int xWidth = (int)((float)xIncrement * 0.5F);
			int yHeight = (int)((float)yIncrement * 0.5F);
			int xOriginDelta = xWidth/2;
			int yOriginDelta = yHeight/2;

			for(int row=0; row<3; row++)
			{
				for(int col=0; col<3; col++)
				{
					xCurrent = xBase + col*xIncrement;
					yCurrent = yBase + row*yIncrement;

					switch(board.BoardState[col,row])
					{
						case BoardUnitState.Naught:
						{
							g.DrawEllipse(penBlack, 
								xCurrent-xOriginDelta,
								yCurrent-yOriginDelta,
								xWidth, yHeight);
							break;
						}
						case BoardUnitState.Cross:
						{
							g.DrawLine(penBlack, 
								xCurrent-xOriginDelta, yCurrent-yOriginDelta,
								xCurrent+xOriginDelta, yCurrent+yOriginDelta);
					
							g.DrawLine(penBlack, 
								xCurrent+xOriginDelta, yCurrent-yOriginDelta,
								xCurrent-xOriginDelta, yCurrent+yOriginDelta);
							break;
						}
					}
				}
			}
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.FillRectangle(brushBackground, e.ClipRectangle);
		}
		
		#endregion

		#region Event Handlers

		private void TicTacToeControl_Resize(object sender, System.EventArgs e)
		{
			Refresh();
		}

		private void TicTacToeControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Determine which grid square the user clicked and trigger an event.
			int gridX = e.X / xIncrement;
			int gridY = e.Y / yIncrement;

			if(GridClick!=null)
				GridClick(this, new TicTacToeClickArgs(gridX, gridY));
		}

		#endregion
	}
}
