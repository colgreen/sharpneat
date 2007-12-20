using System;
using System.Collections;
using System.IO;
using System.Xml;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.Experiments.TicTacToe;

namespace SharpNeatLib.Experiments
{
	public class ParetoDominanceCoEvTicTacToePopulationEvaluator : IPopulationEvaluator
	{
		#region Class Variables

		IActivationFunction activationFn;
		ulong evaluationCount;

		ArrayList dominancePlayerList;

		// Store the length going into EvaluatePopulation so that we have the length that was
		// used prior to evaluation when building the EvaluatorStateMessage string.
		int dominancePlayerListCount;

		#endregion

		#region Constructor

		public ParetoDominanceCoEvTicTacToePopulationEvaluator(IActivationFunction activationFn)
		{
			this.activationFn = activationFn;
			dominancePlayerList = new ArrayList();
		}

		#endregion

		#region IPopulationEvaluator Members

		public void EvaluatePopulation(Population pop, EvolutionAlgorithm ea)
		{
			// Update this variable used for stats reporting.
			dominancePlayerListCount = dominancePlayerList.Count;

			if(dominancePlayerListCount==0)
			{	
				EvaluatePopulation_PureCoEv(pop);
				return;
			}

			// Compare each genome with each entry within dominanceNetworkList. Keep the last dominant
			// genome(decoded to INetwork) if we find any at all. Last genome is an arbitrary choice, 
			// we have to select just one and this is simpler to code / more efficient.
			INetwork newDominantNetwork = null;
			IGenome newDominantGenome = null;

			int genomeCount = pop.GenomeList.Count;
			for(int i=0; i<genomeCount; i++)
			{
				// Check for an integer tag that indicates how far along the pareto chain has been evaluated
				// against already. This will be null for a new genome.
				int dominancePlayerListStartIdx=0;
				if(pop.GenomeList[i].Tag != null)
				{	// Unbox the stored integer.
					dominancePlayerListStartIdx = (int)pop.GenomeList[i].Tag;
				}

				// Decode the genome into an INetwork.
				INetwork network = pop.GenomeList[i].Decode(activationFn);
				double fitness=0.0;

				if(dominancePlayerListStartIdx!=0)
				{	// A previously seen genome. Test against any new entries within dominancePlayerList only.
					// This genome cannot be dominant, if it was found to be dominant in the original evaluation
					// then it would have been placed into dominancePlayerList, and a genome cannot dominate itself.
					for(int j=dominancePlayerListStartIdx; j<dominancePlayerListCount; j++)
					{
						FitnessPair fitnessPair = EvaluatePlayerPair((IPlayer)dominancePlayerList[j], network);

						// Accumulate fitness scores against all of the dominant networks. Weight fitnesses to reward points
						// against later dominant networks more then earlier ones.
						fitness += fitnessPair.fitness2 * (double)(j+1);
					}

					// We add the extra fitness gained by evaluating against the networks within
					// dominanceNetworkList that weren't previously evaluated against.
					pop.GenomeList[i].Fitness += fitness;
				}
				else
				{
					// A new genome. Evaluate against all networks in dominancePlayerList and use dominanceFlag
					// to test if the genome is dominant.
					bool dominanceFlag=true;
					for(int j=0; j<dominancePlayerListCount; j++)
					{
						FitnessPair fitnessPair = EvaluatePlayerPair((IPlayer)dominancePlayerList[j], network);
						if(fitnessPair.fitness2 <= fitnessPair.fitness1)
						{	// Our genome is not dominant in this test. Reset the dominance flag.
							dominanceFlag = false;
						}

						// Accumulate fitness scores against all of the dominant networks. Weight fitnesses to reward points
						// against later dominant networks more then earlier ones.
						fitness += fitnessPair.fitness2 * (double)(j+1);
					}

					// Save the genome's fitness.
					pop.GenomeList[i].Fitness = Math.Max(fitness, EvolutionAlgorithm.MIN_GENOME_FITNESS);
					
					// Keep a ref to the genome if it was a new dominant strategy.
					if(dominanceFlag)
					{
						newDominantGenome = pop.GenomeList[i];
						newDominantNetwork = network;
					}
				}
				
				// Update evaluator stats.
				evaluationCount++;

				// Update genome fitness value & stats. 
				pop.GenomeList[i].EvaluationCount++;
				pop.GenomeList[i].Tag = dominancePlayerListCount;	// The index to start evaluating from in future.
			}

			// If a new dominant network was found then add it to dominanceNetworkList.
			if(newDominantNetwork!=null)
			{	// By convention players in the dominance chain always play as cross. LookupPlayer also 
				// requires a 'cross' player.
				NormalizedViewpointNeuralNetPlayer nvnnp = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Cross);
				nvnnp.SetNetwork(newDominantNetwork);
				dominancePlayerList.Add(nvnnp);

				// Save our dominant champ to file.
				SaveDominantGenome((NeatGenome.NeatGenome)newDominantGenome);
			}
		}


		/// <summary>
		/// Compare each genome with all others (play 2 games, 1 each way).
		/// </summary>
		/// <param name="pop"></param>
		private void EvaluatePopulation_PureCoEv(Population pop)
		{
			int genomeCount = pop.GenomeList.Count;

			// Use the fitnessReductionFactor to reduce the magnitude of the fitness values. Their
			// relative values will remain the same and so the search algorithm is not effected, however
			// by keeping the fitness scores low we do not register a high fitness score which effect the
			// invocation of pruning phases when we switch to the pareto evaluation scheme after generation one.
			// Pareto evaluation typically gives a lower fitness score. This is a crude technique but should
			// be quite adequate.
			double fitnessReductionFactor = 1.0/(double)genomeCount;

			// Compare each genome in the population with all others. This is time consuming but
			// each game executes extremely quickly, and so this strategy is viable for tic-tac-toe.

			// Decode all of the genomes into an array of INetwork objects.
			INetwork[] networkList = new INetwork[genomeCount];
			for(int i=0; i<genomeCount; i++)
			{
				networkList[i] = pop.GenomeList[i].Decode(activationFn);
			}

			// Play each network against every other network. Accumulate total fitness within fitnessArray,
			// this is more efficient than retrieving genome.Fitness from randomly distributed points
			// within main memory - attempt to keep loop within CPU caches.
			double[] fitnessArray = new double[genomeCount];
			for(int i=0; i<genomeCount-1; i++)
			{
				for(int j=i+1; j<genomeCount; j++)
				{

					FitnessPair fitnessPair = EvaluatePlayerPair(networkList[i], networkList[j]);
					fitnessArray[i] += fitnessPair.fitness1 * fitnessReductionFactor;
					fitnessArray[j] += fitnessPair.fitness2 * fitnessReductionFactor;

					// Update master evaluation counter.
					evaluationCount++;
				}
			}

			// Transfer fitness values into genomes.
			double bestFitness = Double.MinValue;
			int bestIdx=-1;
			for(int i=0; i<genomeCount; i++)
			{	// Note. This should be the first generation, so set genome stats directly to save time (no addition required).
				pop.GenomeList[i].Fitness = Math.Max(fitnessArray[i], EvolutionAlgorithm.MIN_GENOME_FITNESS);
				pop.GenomeList[i].TotalFitness = fitnessArray[i];
				pop.GenomeList[i].EvaluationCount=1;

				// Store the index of the genome with the best fitness. If there are more than one champ
				// then we just take the last one.
				if(pop.GenomeList[i].Fitness > bestFitness)
				{
					bestFitness = pop.GenomeList[i].Fitness;
					bestIdx = i;
				}
			}

			// The generation champ become the first entry in dominanceNetworkList where it
			// represents our pareto front. 
			// By convention players in the dominance chain always play as cross. LookupPlayer also 
			// requires a 'cross' player.
			NormalizedViewpointNeuralNetPlayer nvnnp = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Cross);
			nvnnp.SetNetwork(networkList[bestIdx]);
			dominancePlayerList.Add(nvnnp);

			// Save our dominant champ to file.
			SaveDominantGenome((NeatGenome.NeatGenome)pop.GenomeList[bestIdx]);
		}

		public ulong EvaluationCount
		{
			get
			{
				return evaluationCount;
			}
		}

		public string EvaluatorStateMessage
		{
			get
			{
				return "dominanceChain(" + dominancePlayerListCount.ToString() + ")";
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

		#endregion

		#region Private Members [Evaluation]

		const double P1_SCORE_WIN = 2.0;
		const double P1_SCORE_DRAW = 1.0;
		const double P1_SCORE_LOSE = 0.0;

		const double P2_SCORE_WIN = 4.0;
		const double P2_SCORE_DRAW = 2.0;
		const double P2_SCORE_LOSE = 0.0;

		/// <summary>
		/// An instance of a Tic-Tac-Toe game, this includes a playing surface(board).
		/// </summary>
		Game game = new Game();

		/// <summary>
		/// Some prebuilt players to save on (re)construction time.
		/// </summary>
		NormalizedViewpointNeuralNetPlayer nvnnp1 = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Cross);
		NormalizedViewpointNeuralNetPlayer nvnnp2 = new NormalizedViewpointNeuralNetPlayer(BoardUnitState.Naught);

		private FitnessPair EvaluatePlayerPair(INetwork n1, INetwork n2)
		{
			nvnnp1.SetNetwork(n1);
			nvnnp2.SetNetwork(n2);
			return EvaluatePlayerPair(nvnnp1, nvnnp2);
		}

		public FitnessPair EvaluatePlayerPair(IPlayer p1, INetwork n2)
		{
			nvnnp2.SetNetwork(n2);
			return EvaluatePlayerPair(p1, nvnnp2);
		}

		public FitnessPair EvaluatePlayerPair(IPlayer p1, IPlayer p2)
		{
			FitnessPair fitnessPair = new FitnessPair();

			// Play one game each way.
			IPlayer player1_tmp = p1;
			IPlayer player2_tmp = p2;

			for(;;)
			{
				IPlayer winner = game.PlayGame(player1_tmp, player2_tmp);
				if(winner==p1)
				{
					if(winner==player1_tmp)
					{
						fitnessPair.fitness1 += P1_SCORE_WIN;
						fitnessPair.fitness2 += P2_SCORE_LOSE;
					}
					else
					{
						fitnessPair.fitness1 += P2_SCORE_WIN;
						fitnessPair.fitness2 += P1_SCORE_LOSE;
					}
				}
				else if(winner==p2)
				{
					if(winner==player1_tmp)
					{
						fitnessPair.fitness1 += P2_SCORE_LOSE;
						fitnessPair.fitness2 += P1_SCORE_WIN;
					}
					else
					{
						fitnessPair.fitness1 += P1_SCORE_LOSE;
						fitnessPair.fitness2 += P2_SCORE_WIN;
					}
				}
				else
				{	// Returned null, draw.
					if(p1==player1_tmp)
					{
						fitnessPair.fitness1 += P1_SCORE_DRAW;
						fitnessPair.fitness2 += P2_SCORE_DRAW;
					}
					else
					{
						fitnessPair.fitness1 += P2_SCORE_DRAW;
						fitnessPair.fitness2 += P1_SCORE_DRAW;
					}
				}

				// More efficient way of ending the loop than counting.
				if(p1==player2_tmp)
					return fitnessPair;

				// Swap players over for second round.
				player1_tmp = p2;
				player2_tmp = p1;
			}
		}

		#endregion
		
		#region Private Methods [Miscellaneous]

		private void SaveDominantGenome(NeatGenome.NeatGenome genome)
		{						
			string filename = "ParetoDominanceTicTacToe_Dominant_" + dominancePlayerList.Count.ToString()
							+ '_' + DateTime.Now.ToString("yyMMdd_hhmmss")
							+ ".xml";
		
			//----- Write the genome to an XmlDocument.
			XmlDocument doc = new XmlDocument();
			XmlGenomeWriterStatic.Write(doc, genome);
			FileInfo oFileInfo = new FileInfo(filename);

			try
			{	// Ignore any exceptions.
				doc.Save(oFileInfo.FullName);
			}
			catch
			{}
		}

		#endregion
	}
}
