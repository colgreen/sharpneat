using System;
using System.Collections;

namespace SharpNeatLib.Experiments.TicTacToe
{
	/// <summary>
	/// Plays randomly in first open side space. If no sides are open, plays randomly in first open
	/// corner. If no corners are open, plays in center.
	/// </summary>
	public class BadPlayerEx : IPlayer
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

		public BadPlayerEx(BoardUnitState playerType)
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
			possibleMoves.Clear();

		//----- Test for free side space.
			if(state[1,0]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(1,0));
			
			if(state[0,1]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(0,1));
			
			if(state[2,1]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(2,1));
			
			if(state[1,2]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(1,2));

			if(possibleMoves.Count>0)
			{	// Pick one at random.
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}

		//----- Test for free corner.
			possibleMoves.Clear();
			if(state[0,0]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(0,0));
			
			if(state[2,0]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(2,0));
			
			if(state[0,2]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(0,2));
			
			if(state[2,2]==BoardUnitState.Empty)
				possibleMoves.Add(new ByteCoord(2,2));
			

			if(possibleMoves.Count>0)
			{	// Pick one at random.
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}

			// The only space left is the centre! We know this is free otherwise the game would have ended.
			return new ByteCoord(1,1);
		}

		#endregion
	}
}
