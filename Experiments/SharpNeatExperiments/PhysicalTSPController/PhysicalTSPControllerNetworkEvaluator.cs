using System;
using System.Collections;

using SharpNeatLib.Maths;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	///	A class that simulates the Physical Travelling Salesman (PTSP) problem as described by 
	///	Dr. Simon Lucas at the University of Essex, UK.
	///	
	///	http://cswww.essex.ac.uk/staff/sml/gecco/PTSPComp.html
	/// </summary>
	public class PhysicalTSPControllerNetworkEvaluator : INetworkEvaluator //, ISimulator
	{
		#region Constants
	
		const double _45_degrees =	Math.PI * 0.25;
		const double _90_degrees =	Math.PI * 0.5;
		const double _135_degrees =	Math.PI * 0.75;
		const double _180_degrees = Math.PI;
		const double _225_degrees = Math.PI * 1.25;
		const double _270_degrees = Math.PI * 1.5;
		const double _315_degrees = Math.PI * 1.75;
		const double _360_degrees = Math.PI * 2.0;

		const double _PI_RECIP = 1.0/(Math.PI);
		const double FITNESS_TIME_CONST = 1e4;

		#endregion

		#region Class Variables [Physics]

		const double dt = 0.1;		// 0.1 second resolution (actually sqrt(0.1) due to coding error in original java code).
		const double a = 1.0;		// Acceleration.
		const double t2 = 0.5*dt*dt;
		const double rad = 5.0;		// Radius.
		const double rad_squared = rad*rad;

		Vector2d[] accelArray = new Vector2d[]{	new Vector2d(0, 0),  // Zero acceleration.
												new Vector2d(0, -a), // North
												new Vector2d(a, 0),  // East
												new Vector2d(0, a),  // South
												new Vector2d(-a, 0), // West
												
		};

		Vector2d position;
		Vector2d velocity;

		#endregion

		#region Class Variables [Route coords / tracking]

		short[,] raw_coords_map_10 =
		{
			{97, 132},		//0
			{170, 72},		//1
			{119, 60},		//2
			{72, 10},		//3
			{61, 9},		//4
			{21, 34},		//5
			{1, 43},		//6
			{6, 170},		//7
			{175, 204},		//8
			{272, 181},		//9
		};

		short[,] raw_coords_map_20 =
		{
			
			{189, 214},		//19
			{181, 164},		//13
			{189, 124},		//18
			{236, 113},		//5
			{235, 177},		//16
			{246, 195},		//8
			{253, 225},		//11
			{289, 199},		//14
			{233, 51},		//7
			{191, 43},		//12
			{191, 28},		//4
			{143, 16},		//17
			{127, 25},		//6
			{159, 83},		//3
			{126, 123},		//10
			{72, 35},		//2
			{51, 17},		//1
			{39, 41},		//9	
			{34, 40},		//0
			{23, 64},		//15
		};

		short[,] raw_coords_map_30_g5 =
		{
			{148, 135},
			{144, 150},
			{124, 208},
			{67, 223},
			{53, 221},
			{6, 180},
			{9, 117},
			{24, 16},
			{53, 17},
			{48, 85},
			{71, 105},
			{80, 127},
			{86, 149},
			{108, 132},
			{112, 102},
			{108, 92},
			{143, 77},
			{133, 51},
			{157, 10},
			{171, 63},
			{202, 97},
			{212, 74},
			{239, 25},
			{302, 0},
			{319, 42},
			{306, 66},
			{272, 100},
			{244, 157},
			{274, 216},
			{191, 170}
		};

		/// <summary>
		/// The coordinates that must be visited in the order they are listed in the original map file.
		/// </summary>
		ShortCoord[] coords_map_30_g5;
		ShortCoord[] coords_map_10;
		ShortCoord[] coords_map_20;

		/// <summary>
		/// The coords from coords_map_30_g5 rearranged into the order we must vist them.
		/// </summary>
		Vector2d[] eval_route;
		Vector2d[] eval_route_0;
		Vector2d[] eval_route_1;
		Vector2d[] eval_route_2;
		Vector2d[] eval_route_3;
		Vector2d[] eval_route_4;
		Vector2d[] eval_route_5;
		Vector2d[] eval_route_6;
		Vector2d[] eval_route_7;
		
		const int LOOKAHEAD_POINTS = 4;

		#endregion

		#region Class Variables [Misc]
		
		FastRandom random = new FastRandom();

		#endregion

		#region Constructor

		public PhysicalTSPControllerNetworkEvaluator()
		{
			coords_map_30_g5 = BuildCoordArray(raw_coords_map_30_g5);
			coords_map_10 = BuildCoordArray(raw_coords_map_10);
			coords_map_20 = BuildCoordArray(raw_coords_map_20);

			// This sequence describes the shortest distance route as determined by concorde TSP solver. 
			// This is a likely to be a reasonably good route to take for PTSP, although not necessarily 
			// the optimal route.
			int[] sequence_0 = new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29};
			
			int[] sequence_1 = new int[]{13,25,23,4,15,18,17,2,12,5,9,20,27,3,24,28,16,22,26,11,21,6,8,19,0,7,14,10,29,1};
			int[] sequence_2 = new int[]{18,17,2,12,5,9,20,27,3,24,28,16,22,26,11,21,6,8,19,0,7,14,10,29,1,13,25,23,4,15};
			int[] sequence_3 = new int[]{16,22,26,11,21,6,8,19,0,7,14,10,29,1,13,25,23,4,15,18,17,2,12,5,9,20,27,3,24,28};

			int[] sequence_4 = new int[]{0,1,2,3,4,5,6,7,8,9};
			int[] sequence_5 = new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19};

			// Bob MacCallum's route.
			int[] sequence_6 = new int[]{20,21,22,23,24,25,26,27,28,29,0,1,2,3,4,5,6,9,10,11,12,13,14,15,16,19,18,17,8,7};
			
			// Martin Byröd's route
			int[] sequence_7 = new int[]{0,12,11,14,15,16,20,21,19,17,6,5,4,3,2,27,26,25,24,23,22,18,8,7,9,10,13,1,29,28};
			

			eval_route_0 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_0);
			eval_route_1 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_1);
			eval_route_2 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_2);
			eval_route_3 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_3);
			eval_route_4 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_10, sequence_4);
			eval_route_5 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_20, sequence_5);
			eval_route_6 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_6);
			eval_route_7 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_7);
		}

		#endregion	

		#region Private Methods [Route / tracking]

		private ShortCoord[] BuildCoordArray(short[,] coords)
		{
			int length = coords.GetLength(0);
			ShortCoord[] coordArray = new ShortCoord[length];
			
			for(int i=0; i<length; i++)
			{
				coordArray[i] = new ShortCoord(
					coords[i,0],
					coords[i,1]);
			}

			return coordArray;
		}

		#endregion

		#region Private Methods [Physics]

		private void PerformOneTimestep(int forceIdx)
		{
			// Determine acceleration to apply.
			Vector2d accel = accelArray[forceIdx];

			// Update the position given the velocity 0.1 seconds ago and the
			// acceleration 'accel'. s=ut + 0.5*a*t^2
			Vector2d tmp = velocity;
			tmp.Multiply(dt);
			position.Add(tmp);

			tmp = accel;
			tmp.Multiply(t2);
			position.Add(tmp);

			// Now update velocity  v = u + at
			accel.Multiply(1.0);  // Here we replicate bug in the oroginal java 
								// code. This should be 'dt', not 'a'.
			velocity.Add(accel);
		}

		private void ResetPhysics()
		{
			position.x = 160;
			position.y = 120;

			velocity.x = 0.0;
			velocity.y = 0.0;
		}

		#endregion

		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			// Eval route 0.
			position.x = 160; position.y = 120; velocity.x=0; velocity.y=0;
			eval_route = eval_route_0;
			double fitness = SingleEvaluation(network);
//			
//			// Eval route 1.
//			position.x = 9; position.y = 117; velocity.x=0; velocity.y=-3.0;
//			eval_route = eval_route_1;
//			fitness += SingleEvaluation(network);
//
//			// Eval route 2.
//			position.x = 80; position.y = 127; velocity.x=1.0; velocity.y=2.0;
//			eval_route = eval_route_2;
//			fitness += SingleEvaluation(network);
//
//			// Eval route 3.
//			position.x = 239; position.y = 25; velocity.x=3.0; velocity.y=-0.2;
//			eval_route = eval_route_3;
//			fitness += SingleEvaluation(network);

			return fitness;
		}


		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		public double SingleEvaluation(INetwork network)
		{		
			int route_length = eval_route.Length;
			int route_idx = 0; // Next target point.
			Vector2d currentTarget = eval_route[0];
			int timestep=1;
			int timestep_lastpoint=1;
			double closestApproach = double.MaxValue;

			// Keep simulating the physics until one of the stop conditions breaks
			// the loop.
			for(;;)
			{
				//----- Set network inputs.
				network.ClearSignals();

				// Pass velocity vector to network.
				network.SetInputSignal(0, velocity.x*0.05);
				network.SetInputSignal(1, velocity.y*0.05);

				// Vector to target.
				Vector2d vectorToTarget = currentTarget;
				vectorToTarget.Subtract(position);
				network.SetInputSignal(2, vectorToTarget.x*0.02);
				network.SetInputSignal(3, vectorToTarget.y*0.02);

				// Angle made by position, currentTarget and the next target.
				// Distance T0 to T1.
				double anglewithT0_T1 = 0.0; // Default to 0.0 (straight ahead)
				double distanceT0_T1 = 200.0;	 // Default to large value.
				if(route_idx<eval_route.Length-2)
				{
					Vector2d T0_T1 = eval_route[route_idx+1];
					T0_T1.Subtract(currentTarget);

					anglewithT0_T1 = vectorToTarget.AngleWithVector(T0_T1)/Math.PI;
					distanceT0_T1 = T0_T1.Magnitude;
				}
				network.SetInputSignal(4, anglewithT0_T1);
				network.SetInputSignal(5, distanceT0_T1*0.02);

				//----- Activate the network.
				network.MultipleSteps(4);

				//----- Interpret outputs.
				double x_velocity = network.GetOutputSignal(0)*200.0-100.0;
				double y_velocity = network.GetOutputSignal(1)*200.0-100.0;

				double x_velocity_diff = x_velocity -velocity.x;
				double y_velocity_diff = y_velocity -velocity.y;

				int electedAction;
				if(Math.Abs(x_velocity_diff)<0.5 && Math.Abs(y_velocity_diff)<0.5)
				{
					electedAction = 0;
				}
				else if(Math.Abs(x_velocity_diff)>Math.Abs(y_velocity_diff))
				{
					if(x_velocity_diff<0.0)
						electedAction = 4;
					else
						electedAction = 2;
				}
				else //if(Math.Abs(y_velocity_diff)>Math.Abs(x_velocity_diff))
				{
					if(y_velocity_diff<0.0)
						electedAction = 1;
					else
						electedAction = 3;
				}


				//----- Update physics.
				PerformOneTimestep(electedAction);
				
				// Uncomment these lines to add a small amount of randomness to the saleperson's velocity changes.
//				velocity.x+=(random.NextDouble()*0.02-0.01);
//				velocity.y+=(random.NextDouble()*0.02-0.01);

				vectorToTarget = currentTarget;
				vectorToTarget.Subtract(position);
				closestApproach = Math.Min(closestApproach, vectorToTarget.Magnitude);

				//----- Route tracking.
				// Have we arrived at the target point yet? (within 5 meters of the target coord).
				if(vectorToTarget.MagnitudeSquared < rad_squared)
				{	// Arrived!
					if(route_idx==route_length-1)
					{	// Route completed.
						route_idx++;
						return (double)(1000000*(route_idx))-((timestep-1500)*20000);
					}
					currentTarget = eval_route[++route_idx];
					closestApproach = double.MaxValue;
					timestep_lastpoint = timestep;

					vectorToTarget = currentTarget;
					vectorToTarget.Subtract(position);
				}

				if((timestep>2000) || (timestep-timestep_lastpoint>150) || Math.Abs(vectorToTarget.x)>150 || Math.Abs(vectorToTarget.y)>150)
				{	// Out of time or range of target. 
					break;
				}
				timestep++;
			}

			return	(double)(1000000*(Math.Max(0,route_idx)))
				+	(200.0-(double)timestep_lastpoint) 
				+	(10000.0-closestApproach);
		}
	
		#endregion

		#region ISimulator

		int _route_idx; // Next target point.
		Vector2d _currentTarget;
		int _timestep;

		public Vector2d Position
		{
			get
			{
				return position;
			}
		}

		public Vector2d Velocity
		{
			get
			{
				return velocity;
			}
		}

		public int Timestep
		{
			get
			{
				return _timestep;
			}
		}

		public Vector2d[] Route
		{
			get
			{
				return eval_route;
			}
		}

		public int RouteIdx
		{
			get
			{
				return _route_idx;
			}	
		}

		public void Initialise()
		{
			ResetPhysics();

			// Eval route 0.
			position.x = 160; position.y = 120; velocity.x=0; velocity.y=0;
			eval_route = eval_route_0;
			
//			// Eval route 1.
//			position.x = 9; position.y = 117; velocity.x=0; velocity.y=-3.0;
//			eval_route = eval_route_1;
			
//			// Eval route 2.
//			position.x = 80; position.y = 127; velocity.x=1.0; velocity.y=2.0;
//			eval_route = eval_route_2;
			
//			// Eval route 3.
//			position.x = 239; position.y = 25; velocity.x=3.0; velocity.y=-0.2;
//			eval_route = eval_route_3;
		


			_route_idx = 0;
			_currentTarget = eval_route[0];
			_timestep=1;
		}

		public bool PerformSingleStep(INetwork network)
		{
			int route_length = eval_route.Length;

			//----- Set network inputs.
			network.ClearSignals();

			// Pass velocity vector to network.
			network.SetInputSignal(0, velocity.x*0.05);
			network.SetInputSignal(1, velocity.y*0.05);

			// Vector to target.
			Vector2d vectorToTarget = _currentTarget;
			vectorToTarget.Subtract(position);
			network.SetInputSignal(2, vectorToTarget.x*0.02);
			network.SetInputSignal(3, vectorToTarget.y*0.02);

			
			// Angle made by position, currentTarget and the next target.
			// Distance T0 to T1.
			double anglewithT0_T1 = 0.0;
			double distanceT0_T1 = 0.0;
			if(_route_idx<eval_route.Length-2)
			{
				Vector2d T0_T1 = eval_route[_route_idx+1];
				T0_T1.Subtract(_currentTarget);

				anglewithT0_T1 = vectorToTarget.AngleWithVector(T0_T1)/Math.PI;
				distanceT0_T1 = T0_T1.Magnitude;
			}
			network.SetInputSignal(4, anglewithT0_T1);
			network.SetInputSignal(5, distanceT0_T1*0.02);


			//----- Activate the network.
			network.MultipleSteps(4);

			//----- Interpret outputs.
			double x_velocity = network.GetOutputSignal(0)*200.0-100.0;
			double y_velocity = network.GetOutputSignal(1)*200.0-100.0;

			double x_velocity_diff = x_velocity -velocity.x;
			double y_velocity_diff = y_velocity -velocity.y;

			int electedAction;
			if(Math.Abs(x_velocity_diff)<0.5 && Math.Abs(y_velocity_diff)<0.5)
			{
				electedAction = 0;
			}
			else if(Math.Abs(x_velocity_diff)>Math.Abs(y_velocity_diff))
			{
				if(x_velocity_diff<0.0)
					electedAction = 4;
				else
					electedAction = 2;
			}
			else //if(Math.Abs(y_velocity_diff)>Math.Abs(x_velocity_diff))
			{
				if(y_velocity_diff<0.0)
					electedAction = 1;
				else
					electedAction = 3;
			}


			//----- Update physics.
			PerformOneTimestep(electedAction);

			// Uncomment these lines to add a small amount of randomness to the saleperson's velocity changes.
//			velocity.x+=(random.NextDouble()*0.02-0.01);
//			velocity.y+=(random.NextDouble()*0.02-0.01);

			vectorToTarget = _currentTarget;
			vectorToTarget.Subtract(position);

			//----- Route tracking.
			// Have we arrived at the target point yet? (within 5 meters of the target coord).
			if(vectorToTarget.MagnitudeSquared < rad_squared)
			{	// Arrived!
				if(_route_idx==route_length-1)
				{	// Route completed.
					return true;
				}
				_currentTarget = eval_route[++_route_idx];
				//closestApproach = double.MaxValue;
			}

			if(++_timestep>60000 || Math.Abs(vectorToTarget.x)>200 || Math.Abs(vectorToTarget.y)>200)
			{	// Out of time or range of target. 
				return true;
			}
			return false;
		}

		#endregion

	}
}
