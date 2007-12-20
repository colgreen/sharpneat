using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments.TicTacToe
{
	public class NeuralNetPlayer : IPlayer
	{
		#region Class Variables

		INetwork network=null;

		/// <summary>
		/// Indicates if this player is playing as naughts or crosses.
		/// </summary>
		BoardUnitState playerType;

		#endregion

		#region Constructor

		public NeuralNetPlayer(BoardUnitState playerType)
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
			network.ClearSignals();

		//----- Pass the input signals into the network.
			BoardUnitState[,] boardState = board.BoardState;
			int signalIdx=0;

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

		//----- Activate the network.
			//network.RelaxNetwork(7, 1e-8);
			network.MultipleSteps(3);

		//----- Take the highest output as the nominated move.
			int moveX=0, moveY=0;
			double highestSignal = double.MinValue;
			signalIdx=0;
			
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
			if(highestSignal<0.5)
			{	// If no signal is 0.5 or above then the network isn't giving a clear signal.
				// Interpret this as a pass.
				return new ByteCoord(255,255);
			}

			// If is an illegal move the board will ignore it - we effectively pass on this go.
			//return !board.MakeMove(moveX, moveY, playerType);
			return new ByteCoord(moveX, moveY);
		}

		#endregion
	}
}
