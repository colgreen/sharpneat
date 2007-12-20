using System;
using System.Diagnostics;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public enum BoardUnitState
	{
		Naught=-1,
		Empty=0,
		Cross=1
	}

	public class Board
	{
		BoardUnitState[,] boardState;
		int[,] tmpBoardState;

		// We can use these to quickly test if the board is full and therefore that the game is over.
		const int maxMoves=9;
		int moveCounter;

		#region Constructors

		public Board()
		{
			boardState = new BoardUnitState[3,3];
			tmpBoardState = new int[3,3];
			moveCounter=0;
		}

		#endregion

		#region Properties

		public BoardUnitState[,] BoardState
		{
			get
			{
				return boardState;
			}
		}

		/// <summary>
		/// Return true of the board is full - you must test for a win first to determine that this is a 
		/// tie.
		/// </summary>
		/// <returns></returns>
		public bool IsBoardFull
		{
			get
			{
				// Nice and quick technique for testing end-of-game state.
				return(moveCounter>=maxMoves);
			}
		}

		public bool IsBoardEmpty
		{
			get
			{
				// Nice and quick technique for testing end-of-game state.
				return(moveCounter==0);
			}
		}



		#endregion

		#region Private Methods

		private bool IsLegalMove(ByteCoord move)
		{
			if(move.x==255)
				return false;
			if(boardState[move.x,move.y]==BoardUnitState.Empty)
				return true;
			// else
			return false;
		}

		private int GetBoardStateCode(int[,] board, int originSquare, bool rasterOrientation)
		{
			if(originSquare==0)
			{	// top-left.
				if(rasterOrientation)
				{	// top-to-bottom / left-to-right.
					return	(board[0,0])
						+	(board[0,1]*3)
						+	(board[0,2]*9)
						+	(board[1,0]*27)
						+	(board[1,1]*81)
						+	(board[1,2]*243)
						+	(board[2,0]*729)
						+	(board[2,1]*2187)
						+	(board[2,2]*6561);
				}
				else
				{	
					// left-to-right / top-to-bottom.
					return	(board[0,0])
						+	(board[1,0]*3)
						+	(board[2,0]*9)
						+	(board[0,1]*27)
						+	(board[1,1]*81)
						+	(board[2,1]*243)
						+	(board[0,2]*729)
						+	(board[1,2]*2187)
						+	(board[2,2]*6561);
				}
			}
			else if(originSquare==1)
			{	// top-right.
				if(rasterOrientation)
				{	// right-to-left / top-to-bottom.
					return	(board[2,0])
						+	(board[1,0]*3)
						+	(board[0,0]*9)
						+	(board[2,1]*27)
						+	(board[1,1]*81)
						+	(board[0,1]*243)
						+	(board[2,2]*729)
						+	(board[1,2]*2187)
						+	(board[0,2]*6561);
				}
				else
				{	// top-to-bottom / right-to-left.
					return	(board[2,0])
						+	(board[2,1]*3)
						+	(board[2,2]*9)
						+	(board[1,0]*27)
						+	(board[1,1]*81)
						+	(board[1,2]*243)
						+	(board[0,0]*729)
						+	(board[0,1]*2187)
						+	(board[0,2]*6561);
				}
			}
			else if(originSquare==2)
			{	// bottom-right.
				if(rasterOrientation)
				{	// bottom-to-top / right-to-left.
					return	(board[2,2])
						+	(board[2,1]*3)
						+	(board[2,0]*9)
						+	(board[1,2]*27)
						+	(board[1,1]*81)
						+	(board[1,0]*243)
						+	(board[0,2]*729)
						+	(board[0,1]*2187)
						+	(board[0,0]*6561);
				}
				else
				{	// right-to-left / bottom-to-top.
					return	(board[2,2])
						+	(board[1,2]*3)
						+	(board[0,2]*9)
						+	(board[2,1]*27)
						+	(board[1,1]*81)
						+	(board[0,1]*243)
						+	(board[2,0]*729)
						+	(board[1,0]*2187)
						+	(board[0,0]*6561);
				}
			}
			else //if(originSquare==3)
			{	// bottom-left.
				if(rasterOrientation)
				{	// left-to-right. bottom-to-top.
					return	(board[0,2])
						+	(board[1,2]*3)
						+	(board[2,2]*9)
						+	(board[0,1]*27)
						+	(board[1,1]*81)
						+	(board[2,1]*243)
						+	(board[0,0]*729)
						+	(board[1,0]*2187)
						+	(board[2,0]*6561);
				}
				else
				{	
					// bottom-to-top. left-to-right.
					return	(board[0,2])
						+	(board[0,1]*3)
						+	(board[0,0]*9)
						+	(board[1,2]*27)
						+	(board[1,1]*81)
						+	(board[1,0]*243)
						+	(board[2,2]*729)
						+	(board[2,1]*2187)
						+	(board[2,0]*6561);
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reset the board ready for a new game. This is probably more efficient than constructing a new
		/// board each time because we don't cause the garbage collector to clean up the old board.
		/// </summary>
		public void Reset()
		{
			for(int x=0; x<3; x++)
				for(int y=0; y<3; y++)
					boardState[x,y] = BoardUnitState.Empty;

			moveCounter=0;
		}

		/// <summary>
		/// This board state has the minimum coded value. All squares are set to Naught (-1).
		/// </summary>
		public void ResetToNaught()
		{
			for(int x=0; x<3; x++)
				for(int y=0; y<3; y++)
					boardState[x,y] = BoardUnitState.Naught;

			moveCounter=0;
		}

		public void IncrementBoardCode()
		{
			// left-to-right / top-to-bottom.
			if((int)++boardState[0,0]!=2) return;
			boardState[0,0]=BoardUnitState.Naught;

			if((int)++boardState[1,0]!=2) return;
			boardState[1,0]=BoardUnitState.Naught;

			if((int)++boardState[2,0]!=2) return;
			boardState[2,0]=BoardUnitState.Naught;

			if((int)++boardState[0,1]!=2) return;
			boardState[0,1]=BoardUnitState.Naught;

			if((int)++boardState[1,1]!=2) return;
			boardState[1,1]=BoardUnitState.Naught;

			if((int)++boardState[2,1]!=2) return;
			boardState[2,1]=BoardUnitState.Naught;

			if((int)++boardState[0,2]!=2) return;
			boardState[0,2]=BoardUnitState.Naught;

			if((int)++boardState[1,2]!=2) return;
			boardState[1,2]=BoardUnitState.Naught;

			if((int)++boardState[2,2]!=2) return;
			boardState[2,2]=BoardUnitState.Naught;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="newState"></param>
		/// <returns>False if move was not accepted (illegal move).</returns>
		public bool MakeMove(ByteCoord move, BoardUnitState newState)
		{
			if(!IsLegalMove(move))
				return false;

			boardState[move.x,move.y] = newState;
			moveCounter++;
			return true;	// return success flag.
		}

		/// <summary>
		/// This method checks for a winning. If one is found then it can be assumed that the
		/// last player to make a move is the winner - because we check after every move.
		/// </summary>
		/// <returns></returns>
		public bool CheckForWin()
		{
			BoardUnitState tmpState;
			
			// Check rows for win.
			for(int y=0; y<3; y++)
			{
				tmpState = boardState[0, y];
				if(tmpState == BoardUnitState.Empty)
					continue;

				if(boardState[1,y]==tmpState && boardState[2,y]==tmpState)
				{	// Winning row found.
					return true;
				}
			}

			// Check columns for win.
			for(int x=0; x<3; x++)
			{
				tmpState = boardState[x, 0];
				if(tmpState == BoardUnitState.Empty)
					continue;

				if(boardState[x,1]==tmpState && boardState[x,2]==tmpState)
				{	// Winning column found.
					return true;
				}
			}

			// Check diagonal 1.
			tmpState = boardState[0, 0]; // top-left.
			if(tmpState != BoardUnitState.Empty)
			{
				if(boardState[1,1]==tmpState && boardState[2,2]==tmpState)
				{	// Winning diagonal found.
					return true;
				}
			}

			// Check diagonal 2.
			tmpState = boardState[2, 0]; // top-right.
			if(tmpState != BoardUnitState.Empty)
			{
				if(boardState[1,1]==tmpState && boardState[0,2]==tmpState)
				{	// Winning diagonal found.
					return true;
				}
			}

			// No winning line found.
			return false;
		}

		/// <summary>
		/// Calculate a code that represents the board's state. The code is calculated
		/// by representing the board as a base-3 number with 9 digits, one for each of
		/// the squares. 
		/// 
		/// The corner square specified by the 'originSquare' argument represents the least
		/// significant digit of the code. The next significant digit is determined by the
		/// 'rasterOrientation' argument. If rasterOrientation=0(horizontal) then the next 
		/// digit to the right or left (whichever is present) is the next most significant, 
		/// otherwise the next digit above or below is the next most significant. The order 
		/// of significance then, describes a raster pattern through the board's squares.
		/// 
		/// Each square of the argument Board holds one of three values:
		///   BoardUnitState.Naught == -1
		///   BoardUnitState.Empty ==   0
		///   BoardUnitState.Cross ==   1
		///   
		/// The board state is modified with respect to the playerType argument so that the 
		/// opponent's type is represented by -1 amd the player's type is represented by 1.
		/// 
		/// To generate a code we then add one to give the range 0 to 2 - which forms the basis of our
		/// base 3 code.
		/// </summary>
		/// <param name="playerType">The type of player (naught or cross) we are generating the code for.</param>
		/// <param name="originSquare">The corner square to  </param>
		/// <param name="rasterOrientation">0=horizontal, 1=vertical.</param>
		/// <returns></returns>
		public int GetBoardStateCode(BoardUnitState playerType, int originSquare, bool rasterOrientation)
		{
			// Copy the board state temp working array, modifying each squares state with
			// respect to playerType and casting to an int as we go.
			if(playerType==BoardUnitState.Cross)
			{
				for(int x=0; x<3; x++)
					for(int y=0; y<3; y++)
						tmpBoardState[x,y]=(int)boardState[x,y]+1;
			}
			else
			{
				for(int x=0; x<3; x++)
					for(int y=0; y<3; y++)
						tmpBoardState[x,y]=((int)boardState[x,y]*-1)+1;
			}

			// Note: This function call was written for GetNormalizedBoardOrientation() and is being re-used here.
			// This is a little inefficient, if you require faster performance then calculate the code directly off 
			// of boardState instead of copying to tmpBoardStateFirst.
			return GetBoardStateCode(tmpBoardState, originSquare, rasterOrientation);
		}

		/// <summary>
		/// Get the board orientation with the minimum board state code for the current board state.
		/// For more information on how the code is calculated see GetBoardStateCode().
		/// </summary>
		/// <param name="playerType"></param>
		/// <param name="originSquare"></param>
		/// <param name="rasterOrientation"></param>
		public void GetNormalizedBoardOrientation(BoardUnitState playerType, out int originSquare, out bool rasterOrientation)
		{
			// Copy the board state to the temp working array, modifying each square's state with
			// respect to playerType and casting to an int as we go.
			if(playerType==BoardUnitState.Cross)
			{
				for(int x=0; x<3; x++)
					for(int y=0; y<3; y++)
						tmpBoardState[x,y]=(int)boardState[x,y]+1;
			}
			else
			{
				for(int x=0; x<3; x++)
					for(int y=0; y<3; y++)
						tmpBoardState[x,y]=((int)boardState[x,y]*-1)+1;
			}

			
			int lowestCode = originSquare = int.MaxValue;
			rasterOrientation = false;

			for(int i=0; i<4; i++)
			{
				int code = GetBoardStateCode(tmpBoardState, i, false);
				if(code < lowestCode)
				{	
					lowestCode = code;
					originSquare = i;
				}
			}

			for(int i=0; i<4; i++)
			{
				int code = GetBoardStateCode(tmpBoardState, i, true);
				if(code < lowestCode)
				{	
					lowestCode = code;
					originSquare = i;
					rasterOrientation = true;
				}
			}
		}

		#endregion
	}
}
