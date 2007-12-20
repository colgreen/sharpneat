using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments.TicTacToe
{
	/// <summary>
	/// This class accepts an INetwork on its constructor which it uses to build a lookup table
	/// of INetwork responses for each of the 19,683 possible board states. This table is therefore
	/// 19,683*8 bytes in size.
	/// </summary>
	public class LookupPlayer : IPlayer
	{
		#region Class Variables

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;

		ByteCoord[] lookupArray = new ByteCoord[19683];

		#endregion

		#region Constructor

		public LookupPlayer(BoardUnitState playerType, INetwork network)
		{
			this.playerType = playerType;
			BuildLookupArray(network);
		}

		#endregion

		#region Private

		private void BuildLookupArray(INetwork network)
		{
			int index=0;
			for(int a=-1; a<2; a++)
			{
			for(int b=-1; b<2; b++)
			{
			for(int c=-1; c<2; c++)
			{
			for(int d=-1; d<2; d++)
			{
			for(int e=-1; e<2; e++)
			{
			for(int f=-1; f<2; f++)
			{
			for(int g=-1; g<2; g++)
			{
			for(int h=-1; h<2; h++)
			{
			for(int i=-1; i<2; i++, index++)
			{
				// Apply the 9 board square state to the network.
				network.ClearSignals();
				network.SetInputSignal(0, (double)a);
				network.SetInputSignal(1, (double)b);
				network.SetInputSignal(2, (double)c);
				network.SetInputSignal(3, (double)d);
				network.SetInputSignal(4, (double)e);
				network.SetInputSignal(5, (double)f);
				network.SetInputSignal(6, (double)g);
				network.SetInputSignal(7, (double)h);
				network.SetInputSignal(8, (double)i);

				// Activate the network.
				network.MultipleSteps(3);

				//----- Take the highest output as the nominated move.
				int moveX=0, moveY=0;
				double highestSignal = double.MinValue;
				int signalIdx=0;
				for(int y=0; y<3; y++)
				{
					for(int x=0; x<3; x++, signalIdx++)
					{
						double signal = network.GetOutputSignal(signalIdx);
						if(signal>highestSignal)
						{	
							// Slight modification - take the legal move with the highest signal.
							bool squareIsEmpty=false;
							switch(signalIdx)
							{
								case 0:
									squareIsEmpty = a==0;
									break;
								case 1:
									squareIsEmpty = b==0;
									break;
								case 2:
									squareIsEmpty = c==0;
									break;
								case 3:
									squareIsEmpty = d==0;
									break;
								case 4:
									squareIsEmpty = e==0;
									break;
								case 5:
									squareIsEmpty = f==0;
									break;
								case 6:
									squareIsEmpty = g==0;
									break;
								case 7:
									squareIsEmpty = h==0;
									break;
								case 8:
									squareIsEmpty = i==0;
									break;
							}

							if(squareIsEmpty)
							{
								moveX=x; moveY=y;
								highestSignal = signal;
							}
						}
					}
				}	
				if(highestSignal<0.5)
				{	// If no signal is 0.5 or above then the network isn't giving a clear signal.
					// Interpret this as a pass. Use coord 255,255 to signal a pass.
					lookupArray[index]=new ByteCoord(255,255);
				}
				else
				{	// Store the move in the lookup table.
					lookupArray[index]=new ByteCoord((byte)moveX, (byte)moveY);
				}
			}
			}
			}
			}
			}
			}
			}
			}
			}
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
			// Lookup our response in lookupArray. Access the board squares in memory order 
			// to speed up access.
			if(this.playerType==BoardUnitState.Naught)
			{	// For each dimension we obtain an index by converting the opponents
				// BoardUnitState to -1 and add 1.
				return	lookupArray[	((int)board.BoardState[0,0]*-6561)
					+	((int)board.BoardState[1,0]*-2187)
					+	((int)board.BoardState[2,0]*-729)
					+	((int)board.BoardState[0,1]*-243)
					+	((int)board.BoardState[1,1]*-81)
					+	((int)board.BoardState[2,1]*-27)
					+	((int)board.BoardState[0,2]*-9)
					+	((int)board.BoardState[1,2]*-3)
					+	  (int)board.BoardState[2,2]
					+   9840
					];

				// The above code is an optimised version of this commented out code, which in turn 
				// was already optimised to factor out 9 multiplication operations!
//				return	lookupArray[	(((int)board.BoardState[0,0]*-6561)+6561)
//									+	(((int)board.BoardState[1,0]*-2187)+2187)
//									+	(((int)board.BoardState[2,0]*-729)+729)
//									+	(((int)board.BoardState[0,1]*-243)+243)
//									+	(((int)board.BoardState[1,1]*-81)+81)
//									+	(((int)board.BoardState[2,1]*-27)+27)
//									+	(((int)board.BoardState[0,2]*-9)+9)
//									+	(((int)board.BoardState[1,2]*-3)+3)
//									+	  (int)board.BoardState[2,2]				
//									];

			}
			else
			{	// Add 1 to the BoardUnitState
				return	lookupArray[  ((int)board.BoardState[0,0]+1)*6561
									+ ((int)board.BoardState[1,0]+1)*2187
									+ ((int)board.BoardState[2,0]+1)*729
									+ ((int)board.BoardState[0,1]+1)*243
									+ ((int)board.BoardState[1,1]+1)*81
									+ ((int)board.BoardState[2,1]+1)*27
									+ ((int)board.BoardState[0,2]+1)*9
									+ ((int)board.BoardState[1,2]+1)*3
									+ ((int)board.BoardState[2,2]+1)					
									];
			}	
		}

		#endregion
	}
}
