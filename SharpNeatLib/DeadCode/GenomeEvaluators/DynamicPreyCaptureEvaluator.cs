using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.Maths;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NetworkEvaluators;

namespace SharpNeatLib.GenomeEvaluators
{
	public class DynamicPreyCaptureEvaluator : PreyCaptureBase, IGenomeListEvaluator, INetworkListEvaluator
	{
		#region Class Variables 

		IActivationFunction	activationFn;

		const int TRIALS_PER_EVALUATION = 100;
		const double TRIALS_PER_EVALUATION_DBL = (double)TRIALS_PER_EVALUATION;

		PreyCaptureTestCase[] testCaseBuffer = new PreyCaptureTestCase[TRIALS_PER_EVALUATION];

		const int MAX_AUX_TEST_CASES = 100;
		const int MAX_AUX_TEST_CASES_TRANSFER = 30;
		CircularBuffer auxTestCaseBuffer = new CircularBuffer(MAX_AUX_TEST_CASES);
		//CircularBuffer _auxTestCaseBuffer = new CircularBuffer(60);
		Hashtable _auxTestCaseBufferTable = new Hashtable();
		ArrayList _auxTestCaseBufferList = new ArrayList();

		bool bestIsIntermediateChampion = false;
		bool searchCompleted = false;
		bool evaluationSchemeChanged = false;
		int currentDifficultyLevel=0;
		ulong evaluationCount=0;
		int maxAchievableFitness;
		double maxAchievableFitness_dbl;
		//Hashtable speciesIdTable = new Hashtable();

		int timestep;

		// A random number generator for generating initial positions and initial prey movement.
		// This is reset before each trial thus making the trial partially deterministic.
		// Construct it with a time-seeded instance for the simulation tool.
		public FastRandom initRandom = new FastRandom();

		#endregion

		#region Constructor

		public DynamicPreyCaptureEvaluator(IActivationFunction activationFn,
											int gridSize,
											double sensorRange,
											int difficultyLevel)
		: base(gridSize, sensorRange)
		{
			this.activationFn = activationFn;
			SetDifficultyLevel(difficultyLevel);

			// Generate random test cases. We will use the same test cases for each evaluation.
			for(int i=0; i<TRIALS_PER_EVALUATION; i++)
			{
				InitialisePositions();
				testCaseBuffer[i] = new PreyCaptureTestCase(_xAgent, _yAgent, _xPrey, _yPrey, initRandom.Next());
			}

			// Initialise here, but note that this value will change depending on how many 
			// test cases are in auxTestCaseBuffer.
			maxAchievableFitness = TRIALS_PER_EVALUATION;
			maxAchievableFitness_dbl = TRIALS_PER_EVALUATION_DBL;
		}

		#endregion

		#region Public Methods [Simulator Support]

		public override void InitialiseSimulation()
		{
			InitialisePositions();
			timestep=0;
		}

		public override void InitialiseSimulation(int xAgent, int yAgent, int xPrey, int yPrey)
		{
			this.xAgent = xAgent;
			this.yAgent = yAgent;
			this.xPrey = xPrey;
			this.yPrey = yPrey;

			timestep=0;
		}

		public override bool PerformSingleCycle(INetwork network)
		{
			//----- Initial Moves.
			if(timestep++ < preyInitMoves)
			{
				MoveAgent(network, true);
				MovePrey(1.0, initRandom);	
				return false;
			}

			//----- The chase proper.
			MoveAgent(network, false);
			if(IsCaptured()) return true;

			if(preySpeed > 0.0)
			{
				MovePrey(preySpeed, initRandom);      		
				if(IsCaptured()) return true;	
			}
			return false;
		}

		#endregion

		#region Private Methods [Network Evaluation]

		public bool SingleTrial(INetwork network, PreyCaptureTestCase testCase)
		{
			if(testCase==null)
			{
				InitialisePositions();
			}
			else
			{
				// TODO: Find a rnd number generator that can have its seed reset. Constructing a new Random 
				// instance here is inefficient.
				//initRandom = new RandomGenerator(testCase.RandomSeed);
				initRandom.Reinitialise(testCase.RandomSeed);

				xAgent = testCase.XAgent;
				yAgent = testCase.YAgent;
				xPrey = testCase.XPrey;
				yPrey = testCase.YPrey;
			}
			
			//----- Give prey head start at max speed.
			int i=0;
			for(; i<preyInitMoves; i++)
			{
				MoveAgent(network, true);
				MovePrey(1.0, initRandom);	
			}

			//----- Let the chase begin! mwuh ha ha!!!
			for(; i<maxTimesteps; i++)
			{
				MoveAgent(network, false);
				if(IsCaptured()) 
				{
					timestep = i;	
					return true;
				}

				if(preySpeed > 0.0)
				{
					MovePrey(preySpeed, initRandom);      		
					if(IsCaptured()) 
					{	// Not sure of this is possible. Test just in case.
						// Basically the prey just walked right into our agent. doh!
						timestep = i;	
						return true;	
					}					
				}
			}

			//----- Failed to capture prey in alloted timesteps. He'll live... for now. :)
			return false;										
		}

		/// <summary>
		/// Initialise the agent and prey positions ready for a new trial. 
		/// A non-deterministic initialisation, primarily useful for visualising a network within
		/// the prey capture simulator - the random tests give the viewer a good indication of how
		/// a network performs in a range of different starting configurations.
		/// </summary>
		private void InitialisePositions()
		{
			//----- Give agent an inital random position at least 4 units from a wall.
			_xAgent = xAgent = (int)(4.0 + initRandom.Next(gridSize-8));	
			_yAgent = yAgent = (int)(4.0 + initRandom.Next(gridSize-8));	
			
			//----- Give prey initial position just within agent's sensor range.
			double angle = (2.0 * Math.PI) * initRandom.FastNextDouble();	// 2*PI radians = 360 degrees.
			double dist = 2.0 + (2.0 * initRandom.FastNextDouble());		// Between 2 and 4.

			_xPrey = xPrey = xAgent+ (int)Math.Round(Math.Cos(angle)*dist);
			_yPrey = yPrey = yAgent+ (int)Math.Round(Math.Sin(angle)*dist);
		}

		private double EvaluateNetwork(INetwork network, int speciesId)
		{
			int fitness=0;

			// Evaluate against all test cases in the main buffer.
			for(int i=0; i<TRIALS_PER_EVALUATION; i++)
			{
				network.ClearSignals();
				if(SingleTrial(network, testCaseBuffer[i])) 
					fitness++;
			}

			// Evaluate against all(if any) tests in the aux buffer.
			int auxLength = auxTestCaseBuffer.Length;
			for(int i=0; i<auxLength; i++)
			{
				network.ClearSignals();
				if(SingleTrial(network, (PreyCaptureTestCase)auxTestCaseBuffer[i])) 
					fitness++;
			}

			// If passed all tests so far *and* a genome from this species has not already achieved this then
			// pass through up to 10000 random tests, store up to 5 failures from this genome for use in future
			// evaluation routine. We limit this to prevent the aux buffer from filling up and slowing down the 
			// simulation.
			if(fitness == maxAchievableFitness)
			{	
				int failCount=0;
				int i=0;
				while(failCount<5 && i++<10000)
				{
					int seed = initRandom.Next();
					//initRandom = new RandomGenerator(seed);
					initRandom.Reinitialise(seed);

					network.ClearSignals();
					if(!SingleTrial(network, null))
					{
						// Obtain an aux buffer for current species.
						ArrayList auxBuffer = (ArrayList)_auxTestCaseBufferTable[speciesId];
						if(auxBuffer==null)
						{
							auxBuffer = new ArrayList();
							_auxTestCaseBufferTable.Add(speciesId, auxBuffer);
						}
						auxBuffer.Add(new PreyCaptureTestCase(_xAgent, _yAgent, _xPrey, _yPrey, seed));
						failCount++;
					}
				}

				if(failCount==0)
				{
					// We need to ensure that this genome will be identified as the best by the EvolutionAlgorithm,
					// therefore we increase its fitness beyond the other genome's max fitness. Other genomes may 
					// achieve this, but that is OK so long as one of them is identified as the best at the
					// EvolutionAlgorithm level.
					fitness+=10000;
					bestIsIntermediateChampion=true;
				}

				// Notify the next evaluation round that the eval scheme has changed. Either the difficulty level 
				// will have been increased or the auxTestCaseBuffer contents will have been changed.
				evaluationSchemeChanged = true;
			}

			evaluationCount++;
			return (double)fitness;
		}

		private void SetDifficultyLevel(int level)
		{
			switch(level)
			{
				case 0 :
					preyInitMoves = 0;
					preySpeed = 0.0;
					maxTimesteps = 6;
					break;
			
				case 1 :
					preyInitMoves = 1;
					preySpeed = 0.0;
					maxTimesteps = 8;
					break;

				case 2 :
					preyInitMoves = 2;
					preySpeed = 0.0;
					maxTimesteps = 10;
					break;

				case 3 :
					preyInitMoves = 3;
					preySpeed = 0.0;
					maxTimesteps = 12;
					break;

				case 4 :
					preyInitMoves = 4;
					preySpeed = 0.0;
					maxTimesteps = 14;
					break;

				case 5 :
					preyInitMoves = 4;
					preySpeed = 0.1;
					maxTimesteps = 16;
					break;

				case 6 :
					preyInitMoves = 4;
					preySpeed = 0.2;
					maxTimesteps = 17;
					break;

				case 7 :
					preyInitMoves = 4;
					preySpeed = 0.3;
					maxTimesteps = 19;
					break;

				case 8 :
					preyInitMoves = 4;
					preySpeed = 0.4;
					maxTimesteps = 20;
					break;

				case 9 :
					preyInitMoves = 4;
					preySpeed = 0.6;
					maxTimesteps = 28;
					break;

				case 10 :
					preyInitMoves = 4;
					preySpeed = 0.8;
					maxTimesteps = 47;
					break;

				case 11 :
					preyInitMoves = 4;
					preySpeed = 1.0;
					maxTimesteps = 72;
					break;	

				default :
					preyInitMoves = 4;
					preySpeed = 1.0;
					maxTimesteps = 72;
					break;	
			}	
			currentDifficultyLevel = level;
		}

		#endregion

		#region Private Methods [MovePrey]

		private void MovePrey(double preySpeed, FastRandom oRandom)
		{
			if(oRandom.NextDouble() > preySpeed) return;	// Do nothing this time.

			double angle, dist;

			// Obtain heading and distance of prey relative to agent.
			GetPreyPosition(out angle, out dist);	

			// Calculate probabilities of moving in each of the four directions.
			double p0,p1,p2,p3;
			double T = MovePrey_T(dist);
			p0= Math.Exp(0.33 * MovePrey_W(AngleDelta(angle,Math.PI/2.0)) * T);	// North.
			p1= Math.Exp(0.33 * MovePrey_W(AngleDelta(angle,1.5*Math.PI)) * T);	// South.
			p2= Math.Exp(0.33 * MovePrey_W(AngleDelta(angle,0.0)) * T);			// East.
			p3= Math.Exp(0.33 * MovePrey_W(AngleDelta(angle,Math.PI)) * T);		// West.

			// This is effectively a roullete wheel selected of one of the four direction.
			double pTotal= p0+p1+p2+p3;
			double pSelect= pTotal*oRandom.NextDouble();
			double accumulator=p0;
			int action=0;
			
			if(pSelect<= accumulator) action=0;
			else if(pSelect<= (accumulator+=p1)) action=1;
			else if(pSelect<= (accumulator+=p2)) action=2;
			else if(pSelect<= (accumulator+=p3)) action=3;

			switch(action)
			{
				case 0:	// North.
					if(yPrey < gridSize-1) yPrey++;
					break;
				case 1: // South.
					if(yPrey > 0) yPrey--;
					break;
				case 2: // East.
					if(xPrey < gridSize-1) xPrey++;
					break;
				case 3: // West.
					if(xPrey > 0) xPrey--;
					break;
			}	
		}

		private double MovePrey_W(double angle)
		{
			return (Math.PI-angle) / Math.PI;
		}

		private double MovePrey_T(double distance)
		{
			if(distance <=4.0)
				return 15.0-distance;
			else if(distance <=15.0)
				return 9.0-distance/2.0;
			else
				return 1.0;
		}

		#endregion

		#region IGenomeListEvaluator

		public void EvaluateGenomeList(GenomeList genomeList)
		{
		//----- Update auxTestCaseBuffer with test cases that stored from previous evaluation round.
			// Move the aux buffers (per species) into a temp list.
			foreach(ArrayList auxBuffer in _auxTestCaseBufferTable.Values)
				_auxTestCaseBufferList.Add(auxBuffer);

			// Randomly pluck test cases from _auxTestCaseBufferList until we have MAX_AUX_TEST_CASES
			// or the list is exhausted.
			int cases=0;
			while(cases<MAX_AUX_TEST_CASES_TRANSFER && _auxTestCaseBufferList.Count>0)
			{
				// Pick an aux buffer at random;
				int listIdx = (int)(Utilities.NextDouble() * (double)_auxTestCaseBufferList.Count);
				ArrayList auxBuffer = (ArrayList)_auxTestCaseBufferList[listIdx];

				// Randomly get a test case from the auxbuffer and remove it. Randomizing increases chances
				// of getting a mix of test cases from different genomes.
				int caseIdx = (int)(Utilities.NextDouble() * (double)auxBuffer.Count); 
				auxTestCaseBuffer.Enqueue(auxBuffer[caseIdx]);
				auxBuffer.RemoveAt(caseIdx);

				// If the auxBuffer is exhautsed then remove it from _auxTestCaseBufferList.
				if(auxBuffer.Count==0)
					_auxTestCaseBufferList.RemoveAt(listIdx);

				cases++;
			}
			// Release any remaining objects in _auxTestCaseBufferTable and auxTestCaseBufferList.
			_auxTestCaseBufferList.Clear();
			_auxTestCaseBufferTable.Clear();

			// Update maxAchievableFitness
			maxAchievableFitness = TRIALS_PER_EVALUATION + auxTestCaseBuffer.Length;
			maxAchievableFitness_dbl = TRIALS_PER_EVALUATION_DBL + (double)(auxTestCaseBuffer.Length);



			// Test champion flag from previous evaluation/generation.
			if(bestIsIntermediateChampion)
			{	// Reset the flag and increment difficulty level (if not already at the max difficulty).
				bestIsIntermediateChampion=false;
				
				if(currentDifficultyLevel<11)
					SetDifficultyLevel(++currentDifficultyLevel);
			}

			int genomeListCount = genomeList.Count;
			if(evaluationSchemeChanged)
			{	// The scheme has changed so we should evaluate all genomes because any old fitness values
				// will relate to the old evaluation scheme.

				// Reset flag. Note that it may get set back to true during this round of evaluations below.
				evaluationSchemeChanged = false;

				// Loop through the genomes, decode and evaluate them in turn.
				for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
				{
					IGenome genome = genomeList[genomeIdx];

					INetwork network = genome.Decode(activationFn);
					if(network==null) 
						genome.Fitness = GenomeEvaluatorUtilities.MIN_GENOME_FITNESS;
					else
						genome.Fitness = Math.Max(GenomeEvaluatorUtilities.MIN_GENOME_FITNESS, EvaluateNetwork(network, genome.SpeciesId));
					
					// Reset these genome level statistics.
					genome.TotalFitness = genome.Fitness;
					genome.EvaluationCount = 1;
				}
			}
			else
			{
				// Only evaluate new genomes that have not already been evaluated. This is OK because the 
				// evaluation scheme is deterministic and will therefore always give the same fitness for 
				// a given genome.
				for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
				{	
					IGenome genome = genomeList[genomeIdx];
					if(genome.EvaluationCount>0)
						continue;

					INetwork network = genome.Decode(activationFn);
					if(network==null) 
						genome.Fitness = GenomeEvaluatorUtilities.MIN_GENOME_FITNESS;
					else
						genome.Fitness = Math.Max(GenomeEvaluatorUtilities.MIN_GENOME_FITNESS, EvaluateNetwork(network, genome.SpeciesId));
					
					// Update genome level statistics.
					genome.TotalFitness += genome.Fitness;
					genome.EvaluationCount++;
				}
			}
		}

		#endregion

		#region INetworkListEvaluator

		double[] fitnessValueArray = new double[0];
		public double[] EvaluateNetworkList(NetworkList networkList)
		{
			return null;
		}

		/// <summary>
		/// The total number of evaluations performed.
		/// </summary>
		public ulong EvaluationCount
		{
			get
			{
				return evaluationCount;
			}
		}
		
		/// <summary>
		/// Indicates that the evaluation scheme has changed. This flag will generally be set immediately
		/// whereas the EvaluatorStateMessage will not reflect the change until after the following round
		/// of evaluation. This means that we can use EvaluatorStateMessage to determine which scheme was
		/// used to obtain the current set of fitness values, while at the same time providing a flag to 
		/// notify us of a change in teh evaluation scheme.
		/// </summary>
		public bool EvaluationSchemeChanged
		{
			get
			{
				return evaluationSchemeChanged;
			}
		}

		/// <summary>
		/// A human readable message that describes the state of the evaluator. This is useful if the
		/// evaluator has several modes (e.g. difficulty levels in incremenetal evolution) and we want 
		/// to let the user know what mode the evaluator is in.
		/// </summary>
		public string EvaluatorStateMessage
		{
			get
			{
				return "difficulty(" + currentDifficultyLevel.ToString() + "),maxAchievableFitness(" + maxAchievableFitness.ToString() + ")";
			}
		}

		/// <summary>
		/// Indicates that the current best genome is a champion at the current level of difficulty.
		/// If there is only one difficulty level then the 'SearchCompleted' flag should also be set.
		/// </summary>
		public bool BestIsIntermediateChampion
		{
			get
			{
				return bestIsIntermediateChampion;
			}
		}

		/// <summary>
		/// Indicates that the best solution meets the evaluator's end criteria.
		/// </summary>
		public bool SearchCompleted
		{
			get
			{
				return searchCompleted;
			}
		}

		/// <summary>
		/// The number of input neurons required for the domain.
		/// </summary>
		public int InputNeuronCount
		{
			get
			{
				return 13;
			}
		}

		/// <summary>
		/// The number of output neurons required for the domain.
		/// </summary>
		public int OutputNeuronCount
		{
			get
			{
				return 4;
			}
		}

		#endregion	
	}
}
