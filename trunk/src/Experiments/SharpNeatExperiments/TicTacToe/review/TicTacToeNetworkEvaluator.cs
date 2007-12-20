using System;
using SharpNeatLib.NeuralNetwork;

using SharpNeatLib.Experiments.TicTacToe

namespace SharpNeatLib.NetworkEvaluators
{
	public class TicTacToeNetworkEvaluator : INetworkListEvaluator
	{
		Board board;
		IPlayer[] opponents = new IPlayer[5];
		//		double[] opponentWinWeight = new double[] {1.0, 1.5, 2.0, 5.0, 5.0};
		//		double[] opponentLooseWeight = new double[] {5.0, 4.5, 4.0, 1.0, 1.0};
		double[] opponentWinWeight = new double[] {1.0, 1.0, 1.0, 1.0, 1.0};
		double[] opponentLooseWeight = new double[] {1.0, 1.0, 1.0, 1.0, 1.0};

		NeuralNetPlayer neuralnetPlayer;

		#region Constructor

		public TicTacToeNetworkEvaluator()
		{
			board = new Board();
			neuralnetPlayer = new NeuralNetPlayer(BoardUnitState.Cross);

			opponents[0] = new BadPlayer(BoardUnitState.Naught);
			opponents[1] = new RandomPlayer(BoardUnitState.Naught);
			opponents[2] = new CenterPlayer(BoardUnitState.Naught);
			//opponents[3] = new ForkablePlayer(BoardUnitState.Naught);
			opponents[3] = new CompetentLoserPlayer(BoardUnitState.Naught);
			opponents[4] = new BestPlayer(BoardUnitState.Naught);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the winning player, or null if there is a tie.
		/// </summary>
		/// <param name="networkPlayer"></param>
		/// <returns></returns>
		private IPlayer PlayGame(IPlayer player1, IPlayer player2)
		{
			board.Reset();

			for(;;)
			{
				player1.Move(board);
				if(board.CheckForWin())
				{	// player 1 has won.
					return player1;
				}
				else if(board.IsBoardFull)
				{	// Tie.
					return null;
				}

				player2.Move(board);
				if(board.CheckForWin())
				{	// player 2 has won.
					return player2;
				}
				else if(board.IsBoardFull)
				{	// Tie.
					return null;
				}
			}
		}

		#endregion

		#region Private Methods

		private double EvaluateNetwork(INetwork network)
		{
			neuralnetPlayer.SetNetwork(network);
			double fitness=0.0;

			// 5 opponents * 50 rounds of games * each way = 500 games. (theoretical max score 2500).
			// Actual max is less because BestPlayer cannot be beaten.
			for(int i=0; i<opponents.Length; i++)
			{
				IPlayer opponentPlayer = opponents[i];
				double winWeight = opponentWinWeight[i];
				double looseWeight = opponentLooseWeight[i];

				
				for(int j=0; j<50; j++)
				{
					// Let the cpu player go first.
					IPlayer winner = PlayGame(opponentPlayer, neuralnetPlayer);
					if(winner==null)
						fitness+=winWeight*2.0;
					else if(winner==neuralnetPlayer)
						fitness+=winWeight*5.0;
					else
						fitness-=looseWeight*10;

					// Let the neuralnet player go first.
					winner = PlayGame(neuralnetPlayer, opponentPlayer);
					if(winner==null)
						fitness+=winWeight*2.0;
					else if(winner==neuralnetPlayer)
						fitness+=winWeight*5.0;
					else
						fitness-=looseWeight*10;
				}
			}

			// If the score is negative then return 0.
			return Math.Max(fitness, 0.0);
		}

		#endregion

		#region INetworkListEvaluator Members

		double[] fitnessValueArray = new double[0];
		ulong evaluationCount=0;

		public double[] EvaluateNetworkList(NetworkList networkList)
		{
			// Ensure a fitnessValueArray of appropriate length exists. We attempt to re-use this
			// if possible for increased performance.
			int networkListCount = networkList.Count;
			if(fitnessValueArray.Length < networkListCount)
			{
				fitnessValueArray = new double[networkListCount];
			}

			// Evaluate each of the networks in the list.
			for(int i=0; i<networkListCount; i++)
			{	
				fitnessValueArray[i] = EvaluateNetwork(networkList[i]);
				evaluationCount++;
			}

			return fitnessValueArray;
		}

		public ulong EvaluationCount
		{
			get
			{
				return evaluationCount;
			}
		}

		public bool EvaluationSchemeChanged
		{
			get
			{
				return false;
			}
		}

		public string EvaluatorStateMessage
		{
			get
			{
				return string.Empty;
			}
		}

		public bool BestIsIntermediateChampion
		{
			get
			{
				return false;
			}
		}

		public bool SearchCompleted
		{
			get
			{
				return false;
			}
		}

		public int InputNeuronCount
		{
			get
			{
				return 9;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 9;
			}
		}

		#endregion
	}
}
