using System;
using System.Collections;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Evaluators
{
	/// <summary>
	/// Although the trials are random the random number generator is reset for each trial and therefore
	/// the same set of 'random' trials are performed each time. Therefore despite the name this domain
	/// is actually deterministic for trials with 0 prey speed. 
	/// 
	/// Once the prey starts moving then it's movements are affected by the agents movement and therefore
	/// the domain becomes non-deterministic, the start positions and initial movemtn of the prey are always
	/// deterministic however.
	/// </summary>
	public class DynamicPreyCaptureEvaluator : PreyCaptureBase, INetworkEvaluator
	{
		int timestep;

		// A random number generator for generating initial positions and initial prey movement.
		// This is reset before each trial thus making the trial partially deterministic.
		// Construct it with a time-seeded instance for the simulation tool.
		Random initRandom; // = new Random();
		int randomSeed;
		
		const int TRIALS_PER_EVALUATION = 100;
		const int MAX_TEST_CASES = 50;
		const int ELITE_TEST_CASES = 10;

		ArrayList tmpTestCaseBuffer = new ArrayList(TRIALS_PER_EVALUATION);
		ArrayList testCaseBuffer = new ArrayList(2*TRIALS_PER_EVALUATION);
		double testCaseMinFitness=0;

		long testCaseCount=0;

		#region Constructor

		public DynamicPreyCaptureEvaluator(	int gridSize,
											int preyInitMoves, 
											double preySpeed,
											int maxTimesteps,
											double sensorRange)
			: base(gridSize, preyInitMoves, preySpeed, maxTimesteps, sensorRange)
		{
		}

		public DynamicPreyCaptureEvaluator( int gridSize, double sensorRange, int difficultyLevel)
		: base(gridSize, sensorRange)
		{
			SetDifficultyLevel(difficultyLevel);
		}

		#endregion

		#region Public Methods

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
				MovePrey(preySpeed, random);      		
				if(IsCaptured()) return true;	
			}
			return false;
		}

		#endregion

		#region Private Methods 

		private bool SingleTrial(INetwork network, PreyCaptureTestCase testCase)
		{
			if(testCase==null)
			{
				// Use the current tickcount as a good source for a seed. Store the number so that 
				// we can store it in a test case object later if necessary.
				randomSeed = Environment.TickCount;
				initRandom = new Random(randomSeed);

				InitialisePositions();
			}
			else
			{
				initRandom = new Random(testCase.RandomSeed);

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
			double angle = (2.0 * Math.PI) * initRandom.NextDouble();	// 2*PI radians = 360 degrees.
			double dist = 2.0 + (2.0 * initRandom.NextDouble());		// Between 2 and 4.

			_xPrey = xPrey = xAgent+ (int)Math.Round(Math.Cos(angle)*dist);
			_yPrey = yPrey = yAgent+ (int)Math.Round(Math.Sin(angle)*dist);
		}

		#endregion

		#region Private Methods [MovePrey]

		private void MovePrey(double preySpeed, Random oRandom)
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

		#region INetworkEvaluator

		bool isDeterministic;

		public override double Evaluate(INetwork network)
		{
			double fitness=0;
	
			// Test the known difficult test cases from testCaseBuffer. This is variable
			// length with max size MAX_TEST_CASES. 
			int testCaseBufferLength = testCaseBuffer.Count;
			for(int i=0; i<testCaseBufferLength; i++)
			{
				network.clearSignals();
				if(SingleTrial(network, (PreyCaptureTestCase)testCaseBuffer[i])) 
				{
					if(i<10)
						fitness+=10.0;
					else if(i<20)
						fitness+=5.0;
					else if(i<30)
						fitness+=4.0;
					else if(i<40)
						fitness+=3.0;
					else 
						fitness+=2.0;
				}
			}

			// The remainder of tests are purely random (between 50 and 100 tests here).
			for(int i=testCaseBufferLength; i<TRIALS_PER_EVALUATION; i++)
			{
				network.clearSignals();
				if(SingleTrial(network, null)) 
				{
					fitness++;
				}
				else
				{	// Store failed test cases in the tmpTestCaseBuffer.
					tmpTestCaseBuffer.Add(new PreyCaptureTestCase(_xAgent, _yAgent, _xPrey, _yPrey, randomSeed, testCaseCount++));
				}
			}

			// Update testCaseBuffer. 
			int tmpBound = tmpTestCaseBuffer.Count;
			if(tmpBound>0)
			{
				if((fitness>=testCaseMinFitness) || (testCaseBufferLength<MAX_TEST_CASES))
				{
					// Set the fitness property of each new test case.
					for(int i=0; i<tmpBound; i++)
					{
						PreyCaptureTestCase testCase = (PreyCaptureTestCase)tmpTestCaseBuffer[i];
						testCase.Fitness = fitness;
					}
					// Add test cases to testCaseBuffer.
					testCaseBuffer.AddRange(tmpTestCaseBuffer);

					// Sort testCaseBuffer.
					testCaseBuffer.Sort();

					// Trim tets cases from the end of the buffer.
					int delta = testCaseBuffer.Count - MAX_TEST_CASES;
					if(delta>0)
						testCaseBuffer.RemoveRange(MAX_TEST_CASES, delta);

					if(testCaseBuffer.Count>0)
						testCaseMinFitness = ((PreyCaptureTestCase)testCaseBuffer[testCaseBuffer.Count-1]).Fitness;
					// else it doesn't matter. 

				}
				tmpTestCaseBuffer.Clear();
			}

			return fitness;
		}

		public void NotifyGenerationEnd()
		{
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

		/// <summary>
		/// Inidicates to EvolutionAlgorithm that this evaluator is deterministic and that therefore
		/// genomes will not need re-evaluating.
		/// </summary>
		public bool IsDeterministic
		{
			get
			{
				return isDeterministic;
			}
		}

		/// <summary>
		/// Maximum fitness that can be obtained with this evaluator.
		/// </summary>
		public double MaxFitness
		{
			get
			{
				return 290.0D;
			}
		}

		#region Incremental Evolution Support

		int currentDifficultyLevel;

		/// <summary>
		/// Not implemented. The XOR evaluator does not support incremental evoloution.
		/// </summary>
		public void IncrementDifficultyLevel()
		{
			currentDifficultyLevel = Math.Min(MaxDifficultyLevel, currentDifficultyLevel+1);
			SetDifficultyLevel(currentDifficultyLevel);
		}

		/// <summary>
		/// Not implemented. The XOR evaluator does not support incremental evoloution.
		/// </summary>
		public void ResetDifficultyLevel()
		{
			SetDifficultyLevel(0);
		}

		/// <summary>
		/// Some evaluators may choose to use a parameterised difficulty level. EvolutionAlgorithm will
		/// increment this level when the best fitness reaches DifficultyFitnessThreshold. 
		/// </summary>
		public int MaxDifficultyLevel
		{
			get
			{
				return 11;
			}
		}

		/// <summary>
		/// Some evaluators may choose to use a parameterised difficulty level. EvolutionAlgorithm will
		/// increment this level when the best fitness reaches DifficultyFitnessThreshold. 
		/// </summary>
		public int CurrentDifficultyLevel
		{
			get
			{
				return currentDifficultyLevel;
			}
		}

		/// <summary>
		/// Some evaluators may choose to use a parameterised difficulty level. EvolutionAlgorithm will
		/// increment this level when the best fitness reaches DifficultyFitnessThreshold. 
		/// </summary>
		public double DifficultyFitnessThreshold
		{
			get
			{
				return 290.0D;
			}
		}

		private void SetDifficultyLevel(int level)
		{
			switch(level)
			{
				case 0 :
					isDeterministic=true;
					preyInitMoves = 0;
					preySpeed = 0.0;
					maxTimesteps = 6;
					break;
			
				case 1 :
					isDeterministic=true;
					preyInitMoves = 1;
					preySpeed = 0.0;
					maxTimesteps = 8;
					break;

				case 2 :
					isDeterministic=true;
					preyInitMoves = 2;
					preySpeed = 0.0;
					maxTimesteps = 10;
					break;

				case 3 :
					isDeterministic=true;
					preyInitMoves = 3;
					preySpeed = 0.0;
					maxTimesteps = 12;
					break;

				case 4 :
					isDeterministic=true;
					preyInitMoves = 4;
					preySpeed = 0.0;
					maxTimesteps = 14;
					break;

				case 5 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.1;
					maxTimesteps = 16;
					break;

				case 6 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.2;
					maxTimesteps = 17;
					break;

				case 7 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.3;
					maxTimesteps = 19;
					break;

				case 8 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.4;
					maxTimesteps = 20;
					break;

				case 9 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.6;
					maxTimesteps = 28;
					break;

				case 10 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 0.8;
					maxTimesteps = 47;
					break;

				case 11 :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 1.0;
					maxTimesteps = 72;
					break;	

				default :
					isDeterministic=false;
					preyInitMoves = 4;
					preySpeed = 1.0;
					maxTimesteps = 72;
					break;	
			}	

			
			
			// Trim test cases back to the elite. By keeping an elite we lessen the possibility that difficult 
			// to discover traits will be lost in the early generations of the next difficulty level.
			int delta = testCaseBuffer.Count - ELITE_TEST_CASES;
			if(delta>0)
				testCaseBuffer.RemoveRange(ELITE_TEST_CASES, delta);

			// No need to determine the actual min here because it will be overwritten in the next call to Evaluate().
			testCaseMinFitness = 0;
		}

		#endregion

		#endregion
	}
}
