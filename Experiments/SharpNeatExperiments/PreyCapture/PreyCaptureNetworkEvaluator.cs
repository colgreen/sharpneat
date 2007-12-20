using System;
using System.Collections;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Maths;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// A network evaluator for the prey capture domain. This implementation evaluates networks against
	/// TRIALS_PER_EVALUATION random test cases.
	/// Although the trials are random the random number generator is reset for each trial and therefore
	/// the same set of 'random' trials are performed each time. Therefore despite the name this domain
	/// is actually deterministic for trials with 0 prey speed. 
	/// 
	/// Once the prey starts moving then it's movements are affected by the agents movement and therefore
	/// the domain becomes non-deterministic, the start positions and initial movement of the prey are always
	/// deterministic however.
	/// </summary>
	public class PreyCaptureNetworkEvaluator : PreyCaptureBase, INetworkEvaluator, ISimulator
	{
		const int TRIALS_PER_EVALUATION = 100;
		public const double TRIALS_PER_EVALUATION_DBL = (double)TRIALS_PER_EVALUATION;

		PreyCaptureTestCase[] testCaseBuffer = new PreyCaptureTestCase[TRIALS_PER_EVALUATION];

		int currentDifficultyLevel=0;

		int timestep;

		// A random number generator for generating initial positions and initial prey movement.
		// This is reset before each trial thus making the trial partially deterministic.
		// Construct it with a time-seeded instance for the simulation tool.
		FastRandom initRandom = new FastRandom();

		#region Constructor

		public PreyCaptureNetworkEvaluator(	int gridSize,
											int preyInitMoves, 
											double preySpeed,
											int maxTimesteps,
											double sensorRange)
			: base(gridSize, preyInitMoves, preySpeed, maxTimesteps, sensorRange)
		{
		}

		public PreyCaptureNetworkEvaluator( int gridSize, double sensorRange, int difficultyLevel)
		: base(gridSize, sensorRange)
		{
			SetDifficultyLevel(difficultyLevel);

			// Generate random test cases. We will use the same test cases for each evaluation.
			for(int i=0; i<TRIALS_PER_EVALUATION; i++)
			{
				InitialisePositions();
				testCaseBuffer[i] = new PreyCaptureTestCase(_xAgent, _yAgent, _xPrey, _yPrey, initRandom.Next());
			}
		}

		#endregion

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

		#region ISimulator

		public void Initialise_Random()
		{
			InitialisePositions();
			timestep=0;
		}

		public override bool PerformSingleStep(INetwork network)
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

		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			int fitness=0;
	
			for(int i=0; i<TRIALS_PER_EVALUATION; i++)
			{
				network.ClearSignals();
				if(SingleTrial(network, testCaseBuffer[i])) 
					fitness++;
			}

			return (double)fitness;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		#endregion

		#region Public Methods [Miscellaneous]

		public void SetDifficultyLevel(int level)
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

		#region Private Methods 

		public bool SingleTrial(INetwork network, PreyCaptureTestCase testCase)
		{
			if(testCase==null)
			{
				InitialisePositions();
			}
			else
			{
				initRandom = new FastRandom(testCase.RandomSeed);

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
	}
}
