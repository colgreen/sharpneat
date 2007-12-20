using System;

namespace SharpNeatLib.Experiments.TicTacToe
{
	/// <summary>
	/// Contains the high-level tic-tac-toe game routine.
	/// </summary>
	public class Game
	{
		/// <summary>
		/// The playing 'board'.
		/// </summary>
		Board board = new Board();

		/// <summary>
		/// Play a game of Tic-Tac-Toe!
		/// Returns the winning player, or null if there is a tie.
		/// </summary>
		/// <param name="networkPlayer"></param>
		/// <returns></returns>
		public IPlayer PlayGame(IPlayer player1, IPlayer player2)
		{
			ByteCoord nextMove;

			// Flag for registering a pass. If two are detcted in a row 
			// then the game is ended (draw).
			bool passFlag=false;

			board.Reset();
			for(;;)
			{
				// Player 1's move.
				nextMove = player1.GetNextMove(board);
				if(!board.MakeMove(nextMove, player1.PlayerType))
				{	// Pass.
					if(passFlag)
					{	// Two passes in a row. Tie.
						return null;
					}
					passFlag = true;
				}
				else if(board.CheckForWin())
				{	// Player 1 has won!
					return player1;
				}
				else if(board.IsBoardFull)
				{	// Tie.
					return null;
				}
				else
				{	// Reset pass flag when a valid move is made.
					passFlag=false;
				}

				// Player 2's move.
				nextMove = player2.GetNextMove(board);
				if(!board.MakeMove(nextMove, player2.PlayerType))
				{	// Pass.
					if(passFlag)
					{	// Two passes in a row. Tie.
						return null;
					}
					passFlag = true;
				}
				else if(board.CheckForWin())
				{	// Player 2 has won!
					return player2;
				}
				else if(board.IsBoardFull)
				{	// Tie.
					return null;
				}
				else
				{	// Reset pass flag when a valid move is made.
					passFlag=false;
				}
			}
		}
	}
}
