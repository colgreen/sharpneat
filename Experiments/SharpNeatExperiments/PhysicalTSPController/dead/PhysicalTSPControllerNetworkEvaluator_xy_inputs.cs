using System;

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

		short[,] raw_coords_map_30_g5 =
		{
			{144, 150},
			{9, 117},
			{112, 102},
			{202, 97},
			{71, 105},
			{143, 77},
			{274, 216},
			{124, 208},
			{191, 170},
			{133, 51},
			{53, 221},
			{272, 100},
			{108, 92},
			{24, 16},
			{67, 223},
			{80, 127},
			{302, 0},
			{108, 132},
			{86, 149},
			{148, 135},
			{157, 10},
			{244, 157},
			{319, 42},
			{48, 85},
			{212, 74},
			{53, 17},
			{306, 66},
			{171, 63},
			{239, 25},
			{6, 180}
		};

		/// <summary>
		/// The coordinates that must be visited in the order they are listed in the original map file.
		/// </summary>
		ShortCoord[] coords_map_30_g5;

		/// <summary>
		/// The coords from coords_map_30_g5 rearranged into the order we must vist them.
		/// </summary>
		Vector2d[] eval_route;
		Vector2d[] eval_route_0;
		Vector2d[] eval_route_1;
		Vector2d[] eval_route_2;
		Vector2d[] eval_route_3;


//		/// <summary>
//		/// A list of doubles that describe the angle at each point made by the previous and next points.
//		/// Element 1 describes the angle made by point 0, 1 & 2. 
//		/// The angles are pre-processed ready for use as ANN input signals.
//		/// </summary>
//		double[] route_angles_processed;
//		double[] route_angles_processed_dir;
//		double[] route_angles_processed_mag;
//
//		/// <summary>
//		/// The distances between point on the eval_route. 
//		/// Element 0 describes the distance between point 0 and 1.
//		/// The distances are pre-processed ready for use as ANN input signals.
//		/// </summary>
//		double[] route_distances_processed;
		
		/// <summary>
		/// 
		/// </summary>
		const int LOOKAHEAD_POINTS = 4;

		#endregion

		#region Class Variables [Misc]
		
		FastRandom random = new FastRandom();

		#endregion

		#region Constructor

		public PhysicalTSPControllerNetworkEvaluator()
		{
			coords_map_30_g5 = BuildCoordArray();

			// This sequence describes the shortest distance route as determined by concorde TSP solver. 
			// This is a likely to be a reasonably good route to take for PTSP, although not necessarily 
			// the optimal route.
			int[] sequence_0 = new int[]{19,0,7,14,10,29,1,13,25,23,4,15,18,17,2,12,5,9,20,27,3,24,28,16,22,26,11,21,6,8};
			int[] sequence_1 = new int[]{13,25,23,4,15,18,17,2,12,5,9,20,27,3,24,28,16,22,26,11,21,6,8,19,0,7,14,10,29,1};
			int[] sequence_2 = new int[]{18,17,2,12,5,9,20,27,3,24,28,16,22,26,11,21,6,8,19,0,7,14,10,29,1,13,25,23,4,15};
			int[] sequence_3 = new int[]{16,22,26,11,21,6,8,19,0,7,14,10,29,1,13,25,23,4,15,18,17,2,12,5,9,20,27,3,24,28};
		
			eval_route_0 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_0);
			eval_route_1 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_1);
			eval_route_2 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_2);
			eval_route_3 = PTSPUtilities.GenerateRoute_Vector2d(coords_map_30_g5, sequence_3);

//			// Pre-calculate some oft-used values.
//			int length = eval_route.Length + (LOOKAHEAD_POINTS-1);
//			route_angles_processed = new double[length];
//			route_angles_processed_mag = new double[length];
//			route_angles_processed_dir = new double[length];
//			route_distances_processed = new double[length];
//			
//			// Loop over the eval points, calculating and storing each angles and distance on the 
//			// route as we go.
//			Vector2d prevPoint;
//			Vector2d currPoint;
//			Vector2d nextPoint;
//			int eval_route_len = eval_route.Length;
//
//			for(int i=2; i<eval_route_len; i++)
//			{
//				prevPoint = eval_route[i-2];
//				currPoint = eval_route[i-1];
//				nextPoint = eval_route[i];
//
//				Vector2d prevToCurr = currPoint;
//				prevToCurr.Subtract(prevPoint);
//
//				Vector2d currToNext = nextPoint;
//				currToNext.Subtract(currPoint);
//
//				route_angles_processed[i-1] = prevToCurr.AngleWithVector(currToNext) * _PI_RECIP;
//				route_angles_processed_mag[i-1] = Math.Abs(route_angles_processed[i-1]);
//				if(route_angles_processed[i-1]<0.0)
//				{
//					route_angles_processed_dir[i-1] = -1.0;
//				}
//				else if(route_angles_processed[i-1]>0.0)
//				{
//					route_angles_processed_dir[i-1] = 1.0;
//				}
//				// else 0.0
//
//				route_distances_processed[i-2] = prevToCurr.Magnitude;
//			}
//
//			for(int i=0; i<eval_route_len-1; i++)
//			{
//				prevPoint = eval_route[i];
//				currPoint = eval_route[i+1];
//
//				Vector2d prevToCurr = currPoint;
//				prevToCurr.Subtract(prevPoint);
//
//				route_distances_processed[i] = Math.Min(1.0/prevToCurr.Magnitude, 5.0);
//			}
		}

		#endregion

//		private void BuildRouteAuxData(Vector2d[] route,
//										double )
	

		#region Private Methods [Route / tracking]

		private ShortCoord[] BuildCoordArray()
		{
			int length = raw_coords_map_30_g5.GetLength(0);
			ShortCoord[] coordArray = new ShortCoord[length];
			
			for(int i=0; i<length; i++)
			{
				coordArray[i] = new ShortCoord(
					raw_coords_map_30_g5[i,0],
					raw_coords_map_30_g5[i,1]);
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

//			position.x = 145;
//			position.y = 140;
//			
//			velocity.x = 0.0;
//			velocity.y = 0.0;
//
//			position.x += random.NextDouble()*20.0-10.0;
//			position.y += random.NextDouble()*50.0-25.0;
//
//			velocity.x = random.NextDouble()*2.0-1.0;
//			velocity.y = random.NextDouble()*2.0-1.0;
		}

		#endregion

		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			//ResetPhysics();

//			position.x = 160; position.y = 120; velocity.x=0; velocity.y=0;
//			return SingleEvaluation(network);
			

			// Eval route 0.
			position.x = 160; position.y = 120; velocity.x=0; velocity.y=0;
			eval_route = eval_route_0;
			double fitness = SingleEvaluation(network);

			// Eval route 1.
			position.x = 9; position.y = 117; velocity.x=0; velocity.y=-3.0;
			eval_route = eval_route_1;
			fitness += SingleEvaluation(network);

			// Eval route 2.
			position.x = 80; position.y = 127; velocity.x=1.0; velocity.y=2.0;
			eval_route = eval_route_2;
			fitness += SingleEvaluation(network);

			// Eval route 3.
			position.x = 239; position.y = 25; velocity.x=3.0; velocity.y=-0.2;
			eval_route = eval_route_3;
			fitness += SingleEvaluation(network);

			return fitness * 0.25;


//			//-----
//			position.x = 130; position.y = 120; velocity.x=0; velocity.y=0;
//			double fitness = SingleEvaluation(network);
//
//			position.x = 130; position.y = 160; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 160; position.y = 120; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 160; position.y = 160; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			return fitness * 0.25;

//
//			position.x = 120; position.y = 160; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 120; position.y = 110; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 148; position.y = 110; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 170; position.y = 110; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 170; position.y = 135; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 170; position.y = 160; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			position.x = 148; position.y = 160; velocity.x=0; velocity.y=0;
//			fitness += SingleEvaluation(network);
//
//			return fitness * 0.125;
		}


		public double SingleEvaluation(INetwork network)
		{		
			int route_length = eval_route.Length;
			int route_idx = 0; // Next target point.
			Vector2d currentTarget = eval_route[0];
			double[] outputSignals = new double[7];
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
				double anglewithT0_T1 = 0.0;
				double distanceT0_T1 = 0.0;
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
				outputSignals[0] = network.GetOutputSignal(0); // No force.
				outputSignals[1] = network.GetOutputSignal(1); // North
				outputSignals[2] = network.GetOutputSignal(2); // East
				outputSignals[3] = network.GetOutputSignal(3); // South 
				outputSignals[4] = network.GetOutputSignal(4); // West

				// Search for the maximum signal.
				double maxSignal=-0.1;
				int maxSignalIdx=0;
				for(int i=0; i<5; i++)
				{
					if(outputSignals[i]>=maxSignal)
					{
						maxSignal = outputSignals[i];
						maxSignalIdx = i;
					}
				}

				// Build a list of nominated actions.
				int[] nominatedActionList = new int[5];
				int nominatedActionCount = 0;
				if(maxSignal > 0.85)
				{
					for(int i=0; i<5; i++)
					{
						if(outputSignals[i]==maxSignal)
							nominatedActionList[nominatedActionCount++] = i;
					}
				}

				// Determine which of the nominated actions to 'elect'.
				int electedAction=0;
				if(nominatedActionCount>0)
				{
					if(nominatedActionList[0]==0)
					{
						// Zero force always takes precedence.
						electedAction=0;
					}
					else if(nominatedActionCount==1)
					{
						electedAction=nominatedActionList[0];
					}
					else //nominatedActionCount>1
					{	// Pick one at 'random'.
						//electedAction=nominatedActionList[random.Next(nominatedActionCount)];
						electedAction=0;
					}
				}

				//----- Update physics.
				PerformOneTimestep(electedAction);
				velocity.x+=(random.NextDouble()*0.02-0.01);
				velocity.y+=(random.NextDouble()*0.02-0.01);


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
						return (double)(1000000*(route_idx))+(200.0-(double)timestep_lastpoint/10.0);
					}
					currentTarget = eval_route[++route_idx];
					closestApproach = double.MaxValue;
					timestep_lastpoint = timestep;

					vectorToTarget = currentTarget;
					vectorToTarget.Subtract(position);
				}

				//if((timesteps_remaining==0) || Math.Abs(vectorToTarget.x)>150 || Math.Abs(vectorToTarget.y)>150) //position.x>420 || position.x<-100 || position.y>340 || position.y<-100)
				if((timestep-timestep_lastpoint>50) || Math.Abs(vectorToTarget.x)>150 || Math.Abs(vectorToTarget.y)>150) //position.x>420 || position.x<-100 || position.y>340 || position.y<-100)
				//if(timestep>2000 || Math.Abs(vectorToTarget.x)>150 || Math.Abs(vectorToTarget.y)>150) //position.x>420 || position.x<-100 || position.y>340 || position.y<-100)
				{	// Out of time or range of target. 
					break;
				}
				timestep++;
			//	timesteps_remaining--;
			}

			// Don't reward speed if the agent can't follow the full path. This would lead
			// us into local optima.
//			return timestep
//					+ (10000.0-closestApproach*10.0)
//					+ running_fitness;
			return	(double)(1000000*(Math.Max(0,route_idx)))
				+	(200.0-(double)timestep_lastpoint/10.0) 
				+	(10000.0-closestApproach*10.0);
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
//			
//			// Eval route 3.
//			position.x = 239; position.y = 25; velocity.x=3.0; velocity.y=-0.2;
//			eval_route = eval_route_3;
			


			_route_idx = 0;
			_currentTarget = eval_route[0];
			_timestep=1;
		}

		public bool PerformSingleStep(INetwork network)
		{
			double[] outputSignals = new double[7];
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
			network.SetInputSignal(5, distanceT0_T1*0.05);


			//----- Activate the network.
			network.MultipleSteps(4);

			//----- Interpret outputs.
			outputSignals[0] = network.GetOutputSignal(0); // No force.
			outputSignals[1] = network.GetOutputSignal(1); // North
			outputSignals[2] = network.GetOutputSignal(2); // East
			outputSignals[3] = network.GetOutputSignal(3); // South 
			outputSignals[4] = network.GetOutputSignal(4); // West

			// Search for the maximum signal.
			double maxSignal=-0.1;
			int maxSignalIdx=0;
			for(int i=0; i<5; i++)
			{
				if(outputSignals[i]>=maxSignal)
				{
					maxSignal = outputSignals[i];
					maxSignalIdx = i;
				}
			}

			// Build a list of nominated actions.
			int[] nominatedActionList = new int[5];
			int nominatedActionCount = 0;
			if(maxSignal > 0.85)
			{
				for(int i=0; i<5; i++)
				{
					if(outputSignals[i]==maxSignal)
						nominatedActionList[nominatedActionCount++] = i;
				}
			}

			// Determine which of the nominated actions to 'elect'.
			int electedAction=0;
			if(nominatedActionCount>0)
			{
				if(nominatedActionList[0]==0)
				{
					// Zero force always takes precedence.
					electedAction=0;
				}
				else if(nominatedActionCount==1)
				{
					electedAction=nominatedActionList[0];
				}
				else //nominatedActionCount>1
				{	// Pick one at 'random'.
					//electedAction=nominatedActionList[random.Next(nominatedActionCount)];
					electedAction=0;
				}
			}


			//----- Update physics.
			PerformOneTimestep(electedAction);
			velocity.x+=(random.NextDouble()*0.02-0.01);
			velocity.y+=(random.NextDouble()*0.02-0.01);

//			electedDir = electedDir==4?0:electedDir+1;
//			System.Diagnostics.Debug.WriteLine(electedDir);


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

			if(++_timestep>60000 || Math.Abs(vectorToTarget.x)>150 || Math.Abs(vectorToTarget.y)>150) //position.x>420 || position.x<-100 || position.y>340 || position.y<-100)
			{	// Out of time or range of target. 
				return true;
			}
			return false;
		}

		#endregion

	}
}
