using System;
using System.Collections;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public class CenterPlayerEx : IPlayer
	{
		#region Class Variables

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;

		// A random number generator to make the game non-deterministic.
		FastRandom random = new FastRandom();

		// A list of prospective moves from which we can randomly select. Pre-allocate for efficiency.
		ArrayList possibleMoves = new ArrayList(4);

		#endregion

		#region Constructor

		public CenterPlayerEx(BoardUnitState playerType)
		{
			this.playerType = playerType;
		}

		#endregion

		#region IPlayer Members

		public BoardUnitState PlayerType
		{
			get
			{
				return playerType;
			}
		}

		public ByteCoord GetNextMove(Board board)
		{
			BoardUnitState[,] state = board.BoardState;

		//----- Play in center if it is open.
			if(state[1,1] == BoardUnitState.Empty) 
			{
				return new ByteCoord(1,1);
			}

		//----- Play randomly.
			possibleMoves.Clear();
			for(int x=0; x<3; x++)
			{
				for(int y=0; y<3; y++)
				{
					if(state[x,y]==BoardUnitState.Empty)
						possibleMoves.Add(new ByteCoord(x,y));
				}
			}

			if(possibleMoves.Count>0)
			{
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}
			// else, Pass.
			return new ByteCoord(255,255);
		}

		#endregion
	}
}
