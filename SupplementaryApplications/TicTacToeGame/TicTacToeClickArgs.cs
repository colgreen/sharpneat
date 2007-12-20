using System;

namespace SharpNeat.TicTacToeGame
{
	public delegate void TicTacToeClickHandler(object source, TicTacToeClickArgs e);

	public class TicTacToeClickArgs : EventArgs
	{
		int gridX;
		int gridY;

		public TicTacToeClickArgs(int gridX, int gridY)
		{
			this.gridX = gridX;
			this.gridY = gridY;
		}

		public int GridX
		{
			get
			{
				return gridX;
			}
		}
	
		public int GridY
		{
			get
			{
				return gridY;
			}
		}
	}
}
