using System;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public class TicTacToeUtilities
	{



//		public static int GetBoardStateCode(int[,] board, int originSquare, bool rasterOrientation)
//		{
//			if(originSquare==0)
//			{	// top-left.
//				if(rasterOrientation)
//				{	// left-to-right / top-to-bottom.
//					return	(board[0,0])
//						+	(board[1,0]*3)
//						+	(board[2,0]*9)
//						+	(board[0,1]*27)
//						+	(board[1,1]*81)
//						+	(board[2,1]*243)
//						+	(board[0,2]*729)
//						+	(board[1,2]*2187)
//						+	(board[2,2]*6561);
//				}
//				else
//				{	// top-to-bottom / left-to-right.
//					return	(board[0,0])
//						+	(board[0,1]*3)
//						+	(board[0,2]*9)
//						+	(board[1,0]*27)
//						+	(board[1,1]*81)
//						+	(board[1,2]*243)
//						+	(board[2,0]*729)
//						+	(board[2,1]*2187)
//						+	(board[2,2]*6561);
//
//				}
//			}
//			else if(originSquare==1)
//			{	// top-right.
//				if(rasterOrientation)
//				{	// right-to-left / top-to-bottom.
//					return	(board[2,0])
//						+	(board[1,0]*3)
//						+	(board[0,0]*9)
//						+	(board[2,1]*27)
//						+	(board[1,1]*81)
//						+	(board[0,1]*243)
//						+	(board[2,2]*729)
//						+	(board[1,2]*2187)
//						+	(board[0,2]*6561);
//				}
//				else
//				{	// top-to-bottom / right-to-left.
//					return	(board[2,0])
//						+	(board[2,1]*3)
//						+	(board[2,2]*9)
//						+	(board[1,0]*27)
//						+	(board[1,1]*81)
//						+	(board[1,2]*243)
//						+	(board[0,0]*729)
//						+	(board[0,1]*2187)
//						+	(board[0,2]*6561);
//				}
//			}
//			else if(originSquare==2)
//			{	// bottom-right.
//				if(rasterOrientation)
//				{	// right-to-left / bottom-to-top.
//					return	(board[2,2])
//						+	(board[1,2]*3)
//						+	(board[0,2]*9)
//						+	(board[2,1]*27)
//						+	(board[1,1]*81)
//						+	(board[0,1]*243)
//						+	(board[2,0]*729)
//						+	(board[1,0]*2187)
//						+	(board[0,0]*6561);
//				}
//				else
//				{	// bottom-to-top / right-to-left.
//					return	(board[2,2])
//						+	(board[2,1]*3)
//						+	(board[2,0]*9)
//						+	(board[1,2]*27)
//						+	(board[1,1]*81)
//						+	(board[1,0]*243)
//						+	(board[0,2]*729)
//						+	(board[0,1]*2187)
//						+	(board[0,0]*6561);
//				}
//			}
//			else //if(originSquare==3)
//			{	// bottom-left.
//				if(rasterOrientation)
//				{	// left-to-right. bottom-to-top.
//					return	(board[0,2])
//						+	(board[1,2]*3)
//						+	(board[2,2]*9)
//						+	(board[0,1]*27)
//						+	(board[1,1]*81)
//						+	(board[2,1]*243)
//						+	(board[0,0]*729)
//						+	(board[1,0]*2187)
//						+	(board[2,0]*6561);
//				}
//				else
//				{	// bottom-to-top. left-to-right.
//					return	(board[0,2])
//						+	(board[0,1]*3)
//						+	(board[0,0]*9)
//						+	(board[1,2]*27)
//						+	(board[1,1]*81)
//						+	(board[1,0]*243)
//						+	(board[2,2]*729)
//						+	(board[2,1]*2187)
//						+	(board[2,0]*6561);
//				}
//			}
//		}


//		// Allocate this variable once only. It will be heavily re-used by GetBoardStateCode().
//		static int[,] tmpBoard = new int[3,3];
//
//		/// <summary>
//		/// Calculate a code that represents the board's state. The code is calculated
//		/// by representing the board as a base-3 number with 9 digits, one for each of
//		/// the squares. 
//		/// 
//		/// The corner square specified by the 'originSquare' argument represents the least
//		/// significant digit of the code. The next significant digit is determined by the
//		/// 'rasterOrientation' argument. If rasterOrientation=0(horizontal) then the next 
//		/// digit to the right or left (whichever is present) is the next most significant, 
//		/// otherwise the next digit above or below is the next most significant. The order 
//		/// of significance then, describes a raster pattern through the board's squares.
//		/// 
//		/// Each square of the argument Board holds one of three values:
//		///   BoardUnitState.Naught == -1
//		///   BoardUnitState.Empty ==   0
//		///   BoardUnitState.Cross ==   1
//		///   
//		/// The board state is modified with respect to the playerType argument so that the 
//		/// opponent's type is represented by -1 amd the players type is represented by 1.
//		/// 
//		/// To generate a code we then add one to give the range 0 to 2 - which forms the basis of our
//		/// base 3 code.
//		///   
//		/// </summary>
//		/// <param name="board"></param>
//		/// <param name="playerType">The type of player (naught or cross) we are generating the code for.</param>
//		/// <param name="originSquare">The corner square to  </param>
//		/// <param name="rasterOrientation">0=horizontal, 1=vertical.</param>
//		/// <returns></returns>
//		public static int GetBoardStateCode(Board board, BoardUnitState playerType, int originSquare, bool rasterOrientation)
//		{
//			// Copy the board state temp working array, modifying each squares state with
//			// respect to playerType and casting to an int as we go.
//			if(playerType==BoardUnitState.Cross)
//			{
//				for(int x=0; x<3; x++)
//					for(int y=0; y<3; y++)
//						tmpBoard[x,y]=(int)board.BoardState[x,y]+1;
//			}
//			else
//			{
//				for(int x=0; x<3; x++)
//					for(int y=0; y<3; y++)
//						tmpBoard[x,y]=((int)board.BoardState[x,y]*-1)+1;
//			}
//
//			return GetBoardStateCode(tmpBoard, originSquare, rasterOrientation);
//		}
//
//
//		public static void GetNormalizedBoardOrientation(Board board, BoardUnitState playerType, out int originSquare, out bool rasterOrientation)
//		{
//			int lowestCode = -1;
//
//			// Copy the board state temp working array, modifying each square's state with
//			// respect to playerType and casting to an int as we go.
//			if(playerType==BoardUnitState.Cross)
//			{
//				for(int x=0; x<3; x++)
//					for(int y=0; y<3; y++)
//						tmpBoard[x,y]=(int)board.BoardState[x,y]+1;
//			}
//			else
//			{
//				for(int x=0; x<3; x++)
//					for(int y=0; y<3; y++)
//						tmpBoard[x,y]=((int)board.BoardState[x,y]*-1)+1;
//			}
//
//			for(int i=0; i<4; i++)
//			{
//				int code = GetBoardStateCode(tmpBoard, i, false);
//				if(code < lowestCode)
//				{	
//					lowestCode = code;
//					originSquare = i;
//					rasterOrientation = false;
//				}
//
//				code = GetBoardStateCode(tmpBoard, i, true);
//				if(code < lowestCode)
//				{	
//					lowestCode = code;
//					originSquare = i;
//					rasterOrientation = true;
//				}
//			}




//		}
	}
}
