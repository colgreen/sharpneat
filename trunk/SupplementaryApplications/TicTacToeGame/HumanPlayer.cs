using System;
using SharpNeatLib;
using SharpNeatLib.Experiments.TicTacToe;

namespace SharpNeat.TicTacToeGame
{
	public class HumanPlayer : IPlayer
	{
		#region Class Variables

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;

		#endregion

		#region Constructor

		public HumanPlayer(BoardUnitState playerType)
		{
			this.playerType = playerType;
		}

		#endregion

		#region Properties

		public BoardUnitState PlayerType
		{
			get
			{
				return playerType;
			}
		}

		#endregion

		#region IPlayer Members

		public ByteCoord GetNextMove(Board board)
		{
			// No implementation required.
			return new ByteCoord(255,255);
		}

		#endregion
	}
}
