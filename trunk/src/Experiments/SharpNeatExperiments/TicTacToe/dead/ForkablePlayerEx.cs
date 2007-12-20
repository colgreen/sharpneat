using System;
using System.Collections;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public class ForkablePlayerEx : IPlayer
	{
		#region Class Variables

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;
		BoardUnitState opponentType;

		// A random number generator to make the game non-deterministic.
		FastRandom random = new FastRandom();

		// A list of prospective moves from which we can randomly select. Pre-allocate for efficiency.
		ArrayList possibleMoves = new ArrayList(4);

		#endregion

		#region Constructor

		public ForkablePlayerEx(BoardUnitState playerType)
		{
			this.playerType = playerType;

			if(playerType==BoardUnitState.Cross)
				opponentType = BoardUnitState.Naught;
			else
				opponentType = BoardUnitState.Cross;
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

		public void Move(Board board)
		{
			BoardUnitState[,] state = board.BoardState;

		//----- Play randomly if the board is empty.
			if(board.IsBoardEmpty)
			{
				int x = random.Next(3);
				int y = random.Next(3);
				board.MakeMove(x, y, playerType);
				return false;
			}
		
		//----- Complete winning moves.
			if(PlayWinningMove(board, playerType))
				return false;

		//----- Block opponent's winning moves.
			if(PlayWinningMove(board, opponentType))
				return false;

		//----- Randomly play a forking move if available.
			// Examine all empty spaces and choose any one that creates two possible winning moves (a fork).
			possibleMoves.Clear();
			BoardUnitState[,] tmpState = (BoardUnitState[,])board.BoardState.Clone();
			
			for(int x=0; x<3; x++)
			{
				for(int y=0; y<3; y++)
				{
					if(tmpState[x,y]==BoardUnitState.Empty)
					{
						int numThreats = 0;
						tmpState[x,y] = playerType;

						// Horizontal rows.
						if(TestRow(tmpState[0,0], tmpState[1,0], tmpState[2,0])==2)
							numThreats++;
						if(TestRow(tmpState[0,1], tmpState[1,1], tmpState[2,1])==2)
							numThreats++;
						if(TestRow(tmpState[0,2], tmpState[1,2], tmpState[2,2])==2)
							numThreats++;
						
						// Veryical rows.
						if(TestRow(tmpState[0,0], tmpState[0,1], tmpState[0,2])==2)
							numThreats++;
						if(TestRow(tmpState[1,0], tmpState[1,1], tmpState[1,2])==2)
							numThreats++;
						if(TestRow(tmpState[2,0], tmpState[2,1], tmpState[2,2])==2)
							numThreats++;

						// Diagonals rows.
						if(TestRow(tmpState[0,0], tmpState[1,1], tmpState[2,2])==2)
							numThreats++;
						if(TestRow(tmpState[2,0], tmpState[1,1], tmpState[0,2])==2)
							numThreats++;

						if (numThreats >= 2) 
						{
							possibleMoves.Add(new ByteCoord(x,y));
						}
						// Put the space back as it was.
						tmpState[x,y] = BoardUnitState.Empty;
					}
				}
			}

			if(possibleMoves.Count>0)
			{
				ByteCoord move = (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
				board.MakeMove(move.x, move.y, playerType);
				return false;
			}

		//----- Play in center if it is open.
			if(state[1,1] == BoardUnitState.Empty) 
			{
				board.MakeMove(1, 1, playerType);
				return false;
			}

		//----- Play randomly in open corner.
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
			{
				ByteCoord move = (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
				board.MakeMove(move.x, move.y, playerType);
				return false;
			}

		//----- Play randomly (last resort).
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
				ByteCoord move = (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
				board.MakeMove(move.x, move.y, playerType);
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Count how many of the provided 3 spaces contain our player's piece.
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <param name="s3"></param>
		/// <returns></returns>
		private int TestRow(BoardUnitState s1, BoardUnitState s2, BoardUnitState s3)
		{
			if(s1==opponentType) return 0;
			if(s2==opponentType) return 0;
			if(s3==opponentType) return 0;

			int count=0;
			if(s1==playerType) count++;
			if(s2==playerType) count++;
			if(s3==playerType) count++;

			return count;
		}

		/// <summary>
		/// This method finds a winning move for the specified type of opponent. Hence it can be used to both find a winning
		/// move for out player or to find a possible winning move for the opponent - so that we can then play in that space
		/// and block the win.
		/// </summary>
		/// <param name="board"></param>
		/// <param name="type"></param>
		/// <returns>true if a move was played.</returns>
		private bool PlayWinningMove(Board board, BoardUnitState type)
		{
			BoardUnitState[,] state = board.BoardState;

			// Horizontal lines.
			if(state[0,0]==type && state[1,0]==type && state[2,0]==BoardUnitState.Empty)
			{
				board.MakeMove(2, 0, playerType);
				return true;
			}
			else if(state[0,0]==type && state[1,0]==BoardUnitState.Empty && state[2,0]==type)
			{
				board.MakeMove(1, 0, playerType);
				return true;
			}
			else if(state[0,0]==BoardUnitState.Empty && state[1,0]==type && state[2,0]==type)
			{
				board.MakeMove(0, 0, playerType);
				return true;
			}

			else if(state[0,1]==type && state[1,1]==type && state[2,1]==BoardUnitState.Empty)
			{
				board.MakeMove(2, 1, playerType);
				return true;
			}
			else if(state[0,1]==type && state[1,1]==BoardUnitState.Empty && state[2,1]==type)
			{
				board.MakeMove(1, 1, playerType);
				return true;
			}
			else if(state[0,1]==BoardUnitState.Empty && state[1,1]==type && state[2,1]==type)
			{
				board.MakeMove(0, 1, playerType);
				return true;
			}

			else if(state[0,2]==type && state[1,2]==type && state[2,2]==BoardUnitState.Empty)
			{
				board.MakeMove(2, 2, playerType);
				return true;
			}
			else if(state[0,2]==type && state[1,2]==BoardUnitState.Empty && state[2,2]==type)
			{
				board.MakeMove(1, 2, playerType);
				return true;
			}
			else if(state[0,2]==BoardUnitState.Empty && state[1,2]==type && state[2,2]==type)
			{
				board.MakeMove(0, 2, playerType);
				return true;
			}
				// Vertical lines.
			else if(state[0,0]==type && state[0,1]==type && state[0,2]==BoardUnitState.Empty)
			{
				board.MakeMove(0, 2, playerType);
				return true;
			}
			else if(state[0,0]==type && state[0,1]==BoardUnitState.Empty && state[0,2]==type)
			{
				board.MakeMove(0, 1, playerType);
				return true;
			}
			else if(state[0,0]==BoardUnitState.Empty && state[0,1]==type && state[0,2]==type)
			{
				board.MakeMove(0, 0, playerType);
				return true;
			}

			else if(state[1,0]==type && state[1,1]==type && state[1,2]==BoardUnitState.Empty)
			{
				board.MakeMove(1, 2, playerType);
				return true;
			}
			else if(state[1,0]==type && state[1,1]==BoardUnitState.Empty && state[1,2]==type)
			{
				board.MakeMove(1, 1, playerType);
				return true;
			}
			else if(state[1,0]==BoardUnitState.Empty && state[1,1]==type && state[1,2]==type)
			{
				board.MakeMove(1, 0, playerType);
				return true;
			}

			else if(state[2,0]==type && state[2,1]==type && state[2,2]==BoardUnitState.Empty)
			{
				board.MakeMove(2, 2, playerType);
				return true;
			}
			else if(state[2,0]==type && state[2,1]==BoardUnitState.Empty && state[2,2]==type)
			{
				board.MakeMove(2, 1, playerType);
				return true;
			}
			else if(state[2,0]==BoardUnitState.Empty && state[2,1]==type && state[2,2]==type)
			{
				board.MakeMove(2, 0, playerType);
				return true;
			}

				// Diagonals.
			else if(state[0,0]==type && state[1,1]==type && state[2,2]==BoardUnitState.Empty)
			{
				board.MakeMove(2, 2, playerType);
				return true;
			}
			else if(state[0,0]==type && state[1,1]==BoardUnitState.Empty && state[2,2]==type)
			{
				board.MakeMove(1, 1, playerType);
				return true;
			}
			else if(state[0,0]==BoardUnitState.Empty && state[1,1]==type && state[2,2]==type)
			{
				board.MakeMove(0, 0, playerType);
				return true;
			}

			else if(state[2,0]==type && state[1,1]==type && state[0,2]==BoardUnitState.Empty)
			{
				board.MakeMove(0, 2, playerType);
				return true;
			}
			else if(state[2,0]==type && state[1,1]==BoardUnitState.Empty && state[0,2]==type)
			{
				board.MakeMove(1, 1, playerType);
				return true;
			}
			else if(state[2,0]==BoardUnitState.Empty && state[1,1]==type && state[0,2]==type)
			{
				board.MakeMove(2, 0, playerType);
				return true;
			}

			// No move played.
			return false;
		}

		#endregion
	}
}
