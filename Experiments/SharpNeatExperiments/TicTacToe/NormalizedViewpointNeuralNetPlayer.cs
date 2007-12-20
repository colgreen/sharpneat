using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments.TicTacToe
{
	/// <summary>
	/// A neural network player who's viewpoint (of the board) is 
	/// normalized. That is, board states that are rotations and or 
	/// board flips (mirrors) of each other are presented to the neural net 
	/// as a the same board state.
	/// </summary>
	public class NormalizedViewpointNeuralNetPlayer : IPlayer
	{
		#region Class Variables

		INetwork network=null;

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;

		//int originSquare=0;
		//bool rasterOrientation=false;

		#endregion

		#region Constructor

		public NormalizedViewpointNeuralNetPlayer(BoardUnitState playerType)
		{
			this.playerType = playerType;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This allows us to pass in different networks to take the place of the neural net player and therefore
		/// eliminates the need to construct this class for every network we wish to evaluate.
		/// </summary>
		/// <param name="network"></param>
		public void SetNetwork(INetwork network)
		{
			this.network = network;
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
			// Normalize the board state by rotating and/or flipping.
			int originSquare;
			bool rasterOrientation;
			board.GetNormalizedBoardOrientation(playerType, out originSquare, out rasterOrientation);
//			originSquare = 0;
//			rasterOrientation = false;


			//----- Pass the input signals into the network as described by 
			// the RasterOrientation and OriginSquare variables.
			network.ClearSignals();
			BoardUnitState[,] boardState = board.BoardState;
			int signalIdx=0;

			if(originSquare==0)
			{
				if(rasterOrientation)
				{
					for(int x=0; x<3; x++)
					{
						for(int y=0; y<3; y++, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
				else
				{
					for(int y=0; y<3; y++)
					{
						for(int x=0; x<3; x++, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
			}
			else if(originSquare==1)
			{
				if(rasterOrientation)
				{
					for(int y=0; y<3; y++)
					{
						for(int x=2; x>-1; x--, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}

				}
				else
				{
					for(int x=2; x>-1; x--)
					{
						for(int y=0; y<3; y++, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
			}
			else if(originSquare==2)
			{
				if(rasterOrientation)
				{
					for(int x=2; x>-1; x--)
					{
						for(int y=2; y>-1; y--, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
				else
				{
					for(int y=2; y>-1; y--)
					{
						for(int x=2; x>-1; x--, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
			}
			else // assume 3, any other value is invalid.
			{
				if(rasterOrientation)
				{
					for(int y=2; y>-1; y--)
					{
						for(int x=0; x<3; x++, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}
				}
				else
				{
					for(int x=0; x<3; x++)
					{
						for(int y=2; y>-1; y--, signalIdx++)
						{
							if(boardState[x,y]==BoardUnitState.Empty)
								network.SetInputSignal(signalIdx, 0.0);
							else if(boardState[x,y]==playerType)
								network.SetInputSignal(signalIdx, 1.0);
							else
								network.SetInputSignal(signalIdx, -1.0);
						}
					}

				}
			}

			//----- Activate the network.
			//network.RelaxNetwork(7, 1e-8);
			network.MultipleSteps(3);

			//----- Take the highest output as the nominated move.
			int moveX=0, moveY=0;
			double highestSignal = double.MinValue;
			signalIdx=0;

			if(originSquare==0)
			{
				if(rasterOrientation)
				{
					for(int x=0; x<3; x++)
					{
						for(int y=0; y<3; y++, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
				else
				{
					for(int y=0; y<3; y++)
					{
						for(int x=0; x<3; x++, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
			}
			else if(originSquare==1)
			{
				if(rasterOrientation)
				{
					for(int y=0; y<3; y++)
					{
						for(int x=2; x>-1; x--, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}

				}
				else
				{
					for(int x=2; x>-1; x--)
					{
						for(int y=0; y<3; y++, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
			}
			else if(originSquare==2)
			{
				if(rasterOrientation)
				{
					for(int x=2; x>-1; x--)
					{
						for(int y=2; y>-1; y--, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
				else
				{
					for(int y=2; y>-1; y--)
					{
						for(int x=2; x>-1; x--, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
			}
			else // assume 3, any other value is invalid.
			{
				if(rasterOrientation)
				{
					for(int y=2; y>-1; y--)
					{
						for(int x=0; x<3; x++, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
				else
				{
					for(int x=0; x<3; x++)
					{
						for(int y=2; y>-1; y--, signalIdx++)
						{
							double signal = network.GetOutputSignal(signalIdx);
							if(signal>highestSignal)
							{	
								// Slight modification - take the legal move with the highest signal.
								if(board.BoardState[x,y]==BoardUnitState.Empty)
								{
									moveX=x; moveY=y;
									highestSignal = signal;
								}
							}
						}
					}
				}
			}

			if(highestSignal<0.5)
			{	// If no signal is 0.5 or above then the network isn't giving a clear signal.
				// Interpret this as a pass.
				return new ByteCoord(255,255);
			}

			return new ByteCoord(moveX, moveY);
		}

		#endregion
	}
}
