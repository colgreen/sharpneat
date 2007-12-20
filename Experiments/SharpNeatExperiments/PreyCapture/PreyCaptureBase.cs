using System;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Maths;

namespace SharpNeatLib.Experiments
{

	public abstract class PreyCaptureBase
	{
		//----- Constants.
		protected const double pi_4 = Math.PI/4.0;
		protected const double pi_8 = Math.PI/8.0;

		// ------------------------------------ Environment parameters.
		protected int gridSize;			// Minimum of 9. 24 good value here.
		protected int preyInitMoves;	// Number of initial moves. (0->4)
		protected double preySpeed;		// 0->1.
		protected int maxTimesteps;		// AKA length of time agent can survive w/out eating prey.
		protected double sensorRange;				//

		// ------------------------------------ Run-time data.
		double[] actions = new double[4];	// Used in moveAgent().
		protected int xAgent, yAgent;		// Agent's coordinates.	
		protected int xPrey, yPrey;			// Prey's coordinates.
		protected int _xAgent, _yAgent, _xPrey, _yPrey;

		//------------------------------------- Other stuff.
		protected FastRandom random = new FastRandom();

		#region Constructor

		public PreyCaptureBase(	int gridSize, double sensorRange)
		{
			this.gridSize =			Math.Max(gridSize , 9);	// Enforce minimum of 9.
			this.sensorRange =		sensorRange;
		}

		public PreyCaptureBase(	int gridSize,
			int preyInitMoves, 
			double preySpeed,
			int maxTimesteps,
			double sensorRange)
		{
			this.gridSize =			Math.Max(gridSize , 9);	// Enforce minimum of 9.
			this.preyInitMoves =	preyInitMoves;		
			this.preySpeed =		preySpeed;			
			this.maxTimesteps =		maxTimesteps;
			this.sensorRange =		sensorRange;
		}

		#endregion

		#region Properties

		public int GridSize
		{
			get	{	return gridSize;	}
		}

		public int AgentX
		{
			get	{	return xAgent;	}
		}

		public int AgentY
		{
			get	{	return yAgent;	}
		}

		public int PreyX
		{
			get	{	return xPrey;	}
		}

		public int PreyY
		{
			get	{	return yPrey;	}
		}

		public double SensorRange
		{
			get	{	return sensorRange;	}
		}

		public bool IsCaptured()
		{
			if(xAgent==xPrey)
				if(yAgent==yPrey)
					return true;

			return false;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Purely for interactive simulations rather than for evaluation.
		/// </summary>
		abstract public void InitialiseSimulation();

		abstract public void InitialiseSimulation(int xAgent, int yAgent, int xPrey, int yPrey);

		/// <summary>
		/// Purely for interactive simulations rather than for evaluation.
		/// </summary>
		abstract public bool PerformSingleStep(INetwork network);

		#endregion

		#region Protected Methods [MoveAgent]

		/// <summary>
		///
		/// </summary>
		/// <param name="network"></param>
		/// <param name="frozen"></param>
		/// <returns>False if the network was bad e.g. could not be relaxed.</returns>
		protected void MoveAgent(INetwork network, bool frozen)
		{
			double i1=0.0,i2=0.0,i3=0.0,i4=0.0;	// Input signals from sector sensors.
			double i5=0.0,i6=0.0,i7=0.0,i8=0.0;			
			double C;							// Closeness input.
			double N,S,E,W;						// Wall detector inputs.
		
			//----- Determine angle & distance of prey relative to agent.	
			double angle, distance;
			GetPreyPosition(out angle, out distance); 

			//-----  Set sector inputs & C (Closeness flag).
			if(distance<=2.0) C=1.0; else C=0.0;

			if(distance<=sensorRange)
			{	
				if(angle < pi_8) i8=1.0;
				else if(angle < 3.0*pi_8) i7=1.0;
				else if(angle < 5.0*pi_8) i6=1.0;
				else if(angle < 7.0*pi_8) i5=1.0;
				else if(angle < 9.0*pi_8) i4=1.0;
				else if(angle < 11.0*pi_8) i3=1.0;
				else if(angle < 13.0*pi_8) i2=1.0;
				else if(angle < 15.0*pi_8) i1=1.0;
				else i8=1.0;
			}

			//----- Set wall detector inputs.
			N=(5.0-Math.Min(5,gridSize-yAgent))/4.0;
			S=(4.0-Math.Min(4,yAgent))/4.0;
			E=(4.0-Math.Min(4,xAgent))/4.0;
			W=(5.0-Math.Min(5,gridSize-xAgent))/4.0;

			//----- Apply inputs to network & obtain response.
			network.SetInputSignal(0,i1);
			network.SetInputSignal(1,i2);
			network.SetInputSignal(2,i3);
			network.SetInputSignal(3,i4);
			network.SetInputSignal(4,i5);
			network.SetInputSignal(5,i6);
			network.SetInputSignal(6,i7);
			network.SetInputSignal(7,i8);
			network.SetInputSignal(8,C);
			network.SetInputSignal(9,N);
			network.SetInputSignal(10,S);
			network.SetInputSignal(11,E);
			network.SetInputSignal(12,W);
	
			network.MultipleSteps(3);	// Arbitrary number. 2-4 seems resonable for a small, simple network.

			// Network relaxation isn't really applicable to this kind of problem. 
			// There's no harm in experimenting though if you want to try it.
			//			if(!network.RelaxNetwork(20, 0.05))
			//			{	// Don't move agent if the network could not be relaxed. The network may relax in subsequent timesteps/trials
			//				// so just leave the agent sitting where it is and carry on.
			//				return;
			//			}

			if(frozen) return; // Don't allow agent to move. Only observe.

			//----- Get output response and determine which neuron (if any) has the highest output value.
			
			//----- Strategy to only move if there is a single output above 0.5.
			//			bool bNorth, bEast, bSouth, bWest;
			//			int signalCount=0;
			//
			//			if(network.GetOutputSignal(0)>=0.5)
			//			{
			//				bNorth = true;
			//				signalCount++;
			//			}
			//			else
			//				bNorth=false;
			//
			//			if(network.GetOutputSignal(1)>=0.5)
			//			{
			//				bEast = true;
			//				signalCount++;
			//			}
			//			else
			//				bEast=false;
			//
			//			if(network.GetOutputSignal(2)>=0.5)
			//			{
			//				bSouth = true;
			//				signalCount++;
			//			}
			//			else
			//				bSouth=false;
			//
			//			if(network.GetOutputSignal(3)>=0.5)
			//			{
			//				bWest = true;
			//				signalCount++;
			//			}
			//			else
			//				bWest=false;
			//
			//			if(signalCount!=1)
			//				return;
			//
			//			//----- Move the agent.
			//			if(bNorth)
			//			{	// North.
			//				if(yAgent < gridSize-1) yAgent++;	
			//			}	
			//			else if(bEast)
			//			{	// East.
			//				if(xAgent < gridSize-1) xAgent++;
			//			}
			//			else if(bSouth)
			//			{	// South.
			//				if(yAgent > 0) yAgent--;
			//			}
			//			else 
			//			{	// West.
			//				if(xAgent > 0) xAgent--;
			//			}


			//----- Strategy to select the stogest signal. If there is no clear candidate then do nothing (indecision!).
			double aNorth = actions[0] = network.GetOutputSignal(0);
			double aEast = actions[1] = network.GetOutputSignal(1);
			double aSouth = actions[2] = network.GetOutputSignal(2);
			double aWest = actions[3] = network.GetOutputSignal(3);

			Array.Sort(actions);
			double selectedValue = actions[3];
			if(actions[2]==selectedValue) return;	// No one signal stands out, so do nothing.

			//----- Move the agent.
			if(selectedValue==aNorth)
			{	// North.
				if(yAgent < gridSize-1) yAgent++;	
			}	
			else if(selectedValue==aEast)
			{	// East.
				if(xAgent < gridSize-1) xAgent++;
			}
			else if(selectedValue==aSouth)
			{	// South.
				if(yAgent > 0) yAgent--;
			}
			else 
			{	// West.
				if(xAgent > 0) xAgent--;
			}
		}

		#endregion

		#region Protected Methods [General]

		protected double AngleDelta(double a1, double a2)
		{
			double tmp=Math.Abs(a1-a2);
			if(tmp>Math.PI) tmp=2.0*Math.PI - tmp;
			return tmp;
		}

		protected void GetPreyPosition(out double angle, out double distance)
		{
			double a, o;	// Adjacent, opposite.

			//----- Calculate angle of prey from agent.
			a=(double)xPrey - xAgent;								
			o=(double)yPrey - yAgent;

			if(a==0.0)
			{
				if(o<0.0) angle=1.5*Math.PI; else angle=Math.PI/2.0;
			}
			else if(o==0.0)
			{
				if(a<0.0) angle=Math.PI; else angle=0.0;
			}
			else
			{
				// TODO: check use of Atan. Also, Atan2 can be used to eleiminate the quadrant testing.
				angle= Math.Abs(Math.Atan(o/a));	

				// Test quadrants & modify angle accordingly.
				if((a<0.0) && (o>=0.0)) angle=Math.PI-angle;		// Cartesian quadrant 2.
				if((a<0.0) && (o<0.0)) angle+=Math.PI;				// Cartesian quadrant 3.
				if((a>=0.0) && (o<0.0)) angle=2.0*Math.PI-angle;	// Cartesian quadrant 4.
			}

			//----- Determine distance of prey from agent (pythagoras' rule).
			distance= Math.Sqrt(Math.Pow(o,2.0) + Math.Pow(a,2.0));					
		}

		#endregion
	}
}
