using System;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public interface IPlayer
	{
		/// <summary>
		/// Naught or Cross.
		/// </summary>
		BoardUnitState PlayerType
		{
			get;
		}

		/// <summary>
		/// Ask the player for her next move.
		/// </summary>
		/// <param name="board"></param>
		/// <returns>The move the player wishes to make. Returns null to pass.</returns>
		ByteCoord GetNextMove(Board board);
	}
}
