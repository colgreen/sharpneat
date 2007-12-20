using System;
using System.Collections;
using SharpNeatLib.Maths;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public class CompetentLoserPlayer : IPlayer
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

		// Here we store proespective moves. We classify the moves depending on how many open 
		// lines they participate in. e.g. the the center square can participate in 4 possible 
		// winning lines if the opponent has not yet placed any pieces on the board.
		ArrayList prospectiveMovesFour=new ArrayList(1);	// There can only ever be a max of one of these! (the center).
		ArrayList prospectiveMovesThree = new ArrayList(4);
		ArrayList prospectiveMovesTwo = new ArrayList(4);
		ArrayList prospectiveMovesOne = new ArrayList(4);
		
		#endregion

		#region Constructor

		public CompetentLoserPlayer(BoardUnitState playerType)
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

		public ByteCoord GetNextMove(Board board)
		{
			BoardUnitState[,] state = board.BoardState;

		//----- Play randomly if the board is empty.
			if(board.IsBoardEmpty)
			{
				return new ByteCoord(random.Next(3), random.Next(3));
			}
		
		//----- Complete winning moves.
			ByteCoord move = PlayWinningMove(board, playerType);
			if(move.x!=255)
				return move;

		//----- Block opponent's winning moves.
			move = PlayWinningMove(board, opponentType);
			if(move.x!=255)
				return move;

		//----- Randomly play a forking move if available.
			// Simultaneously keep a record of the moves that participate in open lines. If no forking or 
			// forking block moves exist we can then use this data to decide what move to play.

			// Clear the arraylists of any previous data.
			prospectiveMovesFour.Clear();
			prospectiveMovesThree.Clear();
			prospectiveMovesTwo.Clear();
			prospectiveMovesOne.Clear();

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
						int openLines = 0;
						int lineScore;
						tmpState[x,y] = playerType;

						// Horizontal rows.
						lineScore = TestRow(tmpState[0,0], tmpState[1,0], tmpState[2,0]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						lineScore = TestRow(tmpState[0,1], tmpState[1,1], tmpState[2,1]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						lineScore = TestRow(tmpState[0,2], tmpState[1,2], tmpState[2,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

							
						// Vertical columns.
						lineScore = TestRow(tmpState[0,0], tmpState[0,1], tmpState[0,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						lineScore = TestRow(tmpState[1,0], tmpState[1,1], tmpState[1,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						lineScore = TestRow(tmpState[2,0], tmpState[2,1], tmpState[2,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						// Diagonals rows.
						lineScore = TestRow(tmpState[0,0], tmpState[1,1], tmpState[2,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						lineScore = TestRow(tmpState[2,0], tmpState[1,1], tmpState[0,2]);
						if(lineScore>0)
						{
							openLines++;
							if(lineScore==2) numThreats++;
						}

						if(openLines>0)
						{
							ByteCoord coord = new ByteCoord(x,y);

							switch(openLines)
							{
								case 1:
									prospectiveMovesOne.Add(coord);
									break;
								case 2:
									prospectiveMovesTwo.Add(coord);
									break;
								case 3:
									prospectiveMovesThree.Add(coord);
									break;
								case 4:
									prospectiveMovesFour.Add(coord);
									break;

							}

							// Put this test inside the openLines test for efficiency. No point in testing
							// if that test failed.
							if (numThreats >= 2) 
							{
								possibleMoves.Add(coord);
							}
						}

						// Put the space back as it was, ready for testing the next space.
						tmpState[x,y] = BoardUnitState.Empty;
					}
				}
			}

			if(possibleMoves.Count>0)
			{
				// A forking move is available.
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}

		//----- Pre-empt opponent fork.
			possibleMoves.Clear();

			if( (state[1,1]==playerType) && (state[0,0]==opponentType) && (state[1,0]==BoardUnitState.Empty)
				&& (state[2,0]==BoardUnitState.Empty) && (state[0,1]==BoardUnitState.Empty) && (state[2,1]==BoardUnitState.Empty)
				&& (state[0,2]==BoardUnitState.Empty) && (state[1,2]==BoardUnitState.Empty) && (state[2,2]==opponentType))
			{
				possibleMoves.Add(new ByteCoord(1,0));
			}

			if( (state[1,1]==playerType) && (state[0,0]==BoardUnitState.Empty) && (state[1,0]==BoardUnitState.Empty)
				&& (state[2,0]==opponentType) && (state[0,1]==BoardUnitState.Empty) && (state[2,1]==BoardUnitState.Empty)
				&& (state[0,2]==opponentType) && (state[1,2]==BoardUnitState.Empty) && (state[2,2]==BoardUnitState.Empty))
			{
				possibleMoves.Add(new ByteCoord(1,0));
			}

			if( (state[0,0]==BoardUnitState.Empty) && (state[1,1]!=BoardUnitState.Empty)
				&& ((state[1,0]==opponentType) || (state[0,1]==opponentType)))
			{
				possibleMoves.Add(new ByteCoord(0,0));
			}

			if( (state[2,2]==BoardUnitState.Empty) && (state[1,1]!=BoardUnitState.Empty)
				&& ((state[2,1]==opponentType) || (state[1,2]==opponentType)))
			{
				possibleMoves.Add(new ByteCoord(2,2));
			}

			if(possibleMoves.Count>0)
			{
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}


		//----- Select a move with a good number of prospective winning lines (open lines) - if there are any.
			if(prospectiveMovesFour.Count>0)
			{
				if(prospectiveMovesThree.Count>0)
				{	// Give the better '4 lines' move a 50% chance of selection.
					if(random.NextDouble()<0.5)
					{	// We know that the move is in the center, that is the only square with 4 possible open lines.
						return new ByteCoord(1,1);
					} 
					else
					{
						// Play one of the moves with 3 open lines.
						return (ByteCoord)prospectiveMovesThree[random.Next(prospectiveMovesThree.Count)];
					}
				}
				else
				{	// Play the 4-line move.
					return new ByteCoord(1,1);
				}
			}
			else if(prospectiveMovesThree.Count>0)
			{
				if(prospectiveMovesTwo.Count>0)
				{	// Give the better '3 lines' move a 50% chance of selection.
					if(random.NextDouble()<0.5)
					{	
						// Play one of the moves with 3 open lines.
						return (ByteCoord)prospectiveMovesThree[random.Next(prospectiveMovesThree.Count)];
					} 
					else
					{	
						// Play one of the moves with 2 open lines.
						return (ByteCoord)prospectiveMovesTwo[random.Next(prospectiveMovesTwo.Count)];
					}
				}
				else
				{
					// Play one of the moves with 3 open lines.
					return (ByteCoord)prospectiveMovesThree[random.Next(prospectiveMovesThree.Count)];
				}
			}
			else if(prospectiveMovesTwo.Count>0)
			{
				// Play one of the moves with 2 open lines.
				return (ByteCoord)prospectiveMovesTwo[random.Next(prospectiveMovesTwo.Count)];
			}
			else if(prospectiveMovesOne.Count>0)
			{
				// Play one of the moves with 2 open lines.
				return (ByteCoord)prospectiveMovesOne[random.Next(prospectiveMovesOne.Count)];
			}

//		//----- Play in center if it is open.
//			if(state[1,1] == BoardUnitState.Empty) 
//			{
//				board.MakeMove(1, 1, playerType);
//				return;
//			}

//		//----- Play randomly in open corner.
//			possibleMoves.Clear();
//
//			if(state[0,0]==BoardUnitState.Empty)
//				possibleMoves.Add(new Coord(0,0));
//			if(state[2,0]==BoardUnitState.Empty)
//				possibleMoves.Add(new Coord(2,0));
//			if(state[0,2]==BoardUnitState.Empty)
//				possibleMoves.Add(new Coord(0,2));
//			if(state[2,2]==BoardUnitState.Empty)
//				possibleMoves.Add(new Coord(2,2));
//
//			if(possibleMoves.Count>0)
//			{
//				Coord move = (Coord)possibleMoves[random.Next(possibleMoves.Count)];
//				board.MakeMove(move.x, move.y, playerType);
//				return;
//			}

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
				return (ByteCoord)possibleMoves[random.Next(possibleMoves.Count)];
			}
			// else, Pass.
			return new ByteCoord(255,255);
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
			if(s1==opponentType) return -1;
			if(s2==opponentType) return -1;
			if(s3==opponentType) return -1;

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
		private ByteCoord PlayWinningMove(Board board, BoardUnitState type)
		{
			BoardUnitState[,] state = board.BoardState;

			// Horizontal lines.
			if(state[0,0]==type && state[1,0]==type && state[2,0]==BoardUnitState.Empty)
			{
				return new ByteCoord(2, 0);
			}
			else if(state[0,0]==type && state[1,0]==BoardUnitState.Empty && state[2,0]==type)
			{
				return new ByteCoord(1, 0);
			}
			else if(state[0,0]==BoardUnitState.Empty && state[1,0]==type && state[2,0]==type)
			{
				return new ByteCoord(0, 0);
			}

			else if(state[0,1]==type && state[1,1]==type && state[2,1]==BoardUnitState.Empty)
			{
				return new ByteCoord(2, 1);
			}
			else if(state[0,1]==type && state[1,1]==BoardUnitState.Empty && state[2,1]==type)
			{
				return new ByteCoord(1, 1);
			}
			else if(state[0,1]==BoardUnitState.Empty && state[1,1]==type && state[2,1]==type)
			{
				return new ByteCoord(0, 1);
			}

			else if(state[0,2]==type && state[1,2]==type && state[2,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(2, 2);
			}
			else if(state[0,2]==type && state[1,2]==BoardUnitState.Empty && state[2,2]==type)
			{
				return new ByteCoord(1, 2);
			}
			else if(state[0,2]==BoardUnitState.Empty && state[1,2]==type && state[2,2]==type)
			{
				return new ByteCoord(0, 2);
			}
				// Vertical lines.
			else if(state[0,0]==type && state[0,1]==type && state[0,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(0, 2);
			}
			else if(state[0,0]==type && state[0,1]==BoardUnitState.Empty && state[0,2]==type)
			{
				return new ByteCoord(0, 1);
			}
			else if(state[0,0]==BoardUnitState.Empty && state[0,1]==type && state[0,2]==type)
			{
				return new ByteCoord(0, 0);
			}

			else if(state[1,0]==type && state[1,1]==type && state[1,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(1, 2);
			}
			else if(state[1,0]==type && state[1,1]==BoardUnitState.Empty && state[1,2]==type)
			{
				return new ByteCoord(1, 1);
			}
			else if(state[1,0]==BoardUnitState.Empty && state[1,1]==type && state[1,2]==type)
			{
				return new ByteCoord(1, 0);
			}

			else if(state[2,0]==type && state[2,1]==type && state[2,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(2, 2);
			}
			else if(state[2,0]==type && state[2,1]==BoardUnitState.Empty && state[2,2]==type)
			{
				return new ByteCoord(2, 1);
			}
			else if(state[2,0]==BoardUnitState.Empty && state[2,1]==type && state[2,2]==type)
			{
				return new ByteCoord(2, 0);
			}

				// Diagonals.
			else if(state[0,0]==type && state[1,1]==type && state[2,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(2, 2);
			}
			else if(state[0,0]==type && state[1,1]==BoardUnitState.Empty && state[2,2]==type)
			{
				return new ByteCoord(1, 1);
			}
			else if(state[0,0]==BoardUnitState.Empty && state[1,1]==type && state[2,2]==type)
			{
				return new ByteCoord(0, 0);
			}

			else if(state[2,0]==type && state[1,1]==type && state[0,2]==BoardUnitState.Empty)
			{
				return new ByteCoord(0, 2);
			}
			else if(state[2,0]==type && state[1,1]==BoardUnitState.Empty && state[0,2]==type)
			{
				return new ByteCoord(1, 1);
			}
			else if(state[2,0]==BoardUnitState.Empty && state[1,1]==type && state[0,2]==type)
			{
				return new ByteCoord(2, 0);
			}

			// No move played.
			return new ByteCoord(255,255);
		}

		#endregion
	}
}
