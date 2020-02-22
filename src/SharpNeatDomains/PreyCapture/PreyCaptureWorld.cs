/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.PreyCapture
{
    /// <summary>
    /// The prey capture task's grid world, as defined in:
    /// 
    ///    [1] Incremental Evolution Of Complex General Behavior, Faustino Gomez and Risto Miikkulainen (1997)
    ///    http://nn.cs.utexas.edu/downloads/papers/gomez.adaptive-behavior.pdf
    ///
    /// Encapsulates the agent's sensor and motor hardware and the prey's simple stochastic movement.
    /// </summary>
    /// <remarks>
    /// Encapsulates the agent's sensor and motor hardware, and the prey's simple stochastic movement.
    /// </remarks>
    public class PreyCaptureWorld
    {
        #region Constants / Statics

        const double PiDiv4 = Math.PI / 4.0;
		const double PiDiv8 = Math.PI / 8.0;
        static readonly double[][] __atan2Lookup;

        #endregion

        #region Instance Fields

        // World parameters.
		readonly int _gridSize;         // Minimum of 9 (-> 9x9 grid). 24 is a good value here.
		readonly int _preyInitMoves;	// Number of initial moves (0 to 4).
		readonly double _preySpeed;	    // 0 to 1.
		readonly double _sensorRange;	// Agent's sensor range.
        readonly double _sensorRangeSquared;	// Agent's sensor range, squared.
        readonly int _maxTimesteps;	    // Length of time agent can survive w/out eating prey.

        // World state.
        IntPoint _preyPos;
        IntPoint _agentPos;
        
        // Random number generator.
        readonly IRandomSource _rng;

        #endregion

        #region Static Initializer

        static PreyCaptureWorld()
        {
            __atan2Lookup = new double[47][];
            for(int y=0; y < 47; y++)
            { 
                __atan2Lookup[y] = new double[47];
                for(int x=0; x < 47; x++)
                {
                    __atan2Lookup[y][x] = Math.Atan2(y-23, x-23);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs with the provided world parameter arguments.
        /// </summary>
        public PreyCaptureWorld(int gridSize, int preyInitMoves, double preySpeed, double sensorRange, int maxTimesteps)
        {
            _gridSize = gridSize;
            _preyInitMoves = preyInitMoves;
            _preySpeed = preySpeed;
            _sensorRange = sensorRange;
            _sensorRangeSquared = sensorRange * sensorRange;
            _maxTimesteps = maxTimesteps;
            _rng = RandomDefaults.CreateRandomSource();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of the square grid in terms of the length of an edge in number of squares.
        /// </summary>
        public int GridSize
        {
            get { return _gridSize; }
        }

        /// <summary>
        /// Gets the number of moves the prey is allowed to move before the agent can move.
        /// </summary>
        public int PreyInitMoves
        {
            get { return _preyInitMoves; }
        }

        /// <summary>
        /// Gets the sensor range of the agent.
        /// </summary>
        public double SensorRange
        {
            get { return _sensorRange; }
        }

        /// <summary>
        /// Gets the grid square position of the agent.
        /// </summary>
        public IntPoint AgentPosition
        {
            get { return _agentPos; }
        }

        /// <summary>
        /// Gets the grid square position of the prey.
        /// </summary>
        public IntPoint PreyPosition
        {
            get { return _preyPos; }
        }

        /// <summary>
        /// Gets the maximum number of simulation timesteps to run without the agent capturing the prey.
        /// </summary>
        public int MaxTimesteps
        {
            get { return _maxTimesteps; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs one trial of the provided agent in the world. Returns true if the agent captures the prey within
        /// the maximum number of timesteps allowed.
        /// </summary>
        public bool RunTrial(IBlackBox agent)
        {
            // Init world state.
            InitPositions();

            // Clear any prior agent state.
            agent.ResetState();

            // Prey gets a head start (simulate world as normal but agent is frozen).
            int t = 0;
            for(; t < _preyInitMoves; t++)
            {
                SetAgentInputsAndActivate(agent);
                MovePrey();
            }

            // Let the chase begin!
            for(; t < _maxTimesteps; t++)
            {
                SetAgentInputsAndActivate(agent);
                MoveAgent(agent);
                if(IsPreyCaptured()) {
                    return true;
                }

                MovePrey();
                if(IsPreyCaptured()) 
                {   // The prey walked directly into the agent.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initialise agent and prey positions. The prey is positioned randomly with at least 4 empty squares between it and a wall (in all directions).
        /// The agent is positioned randomly but such that the prey is within sensor range (distance 2 or less).
        /// </summary>
        public void InitPositions()
        {
            // Random position at least 4 units away from any wall.
            _preyPos._x = 4 + _rng.Next(_gridSize - 8);
            _preyPos._y = 4 + _rng.Next(_gridSize - 8);

            // Agent position. The angle from the prey is chosen at random, and the distance from the prey is randomly chosen between 2 and 4.
            double t = 2.0 * Math.PI * _rng.NextDouble();   // Random angle.
            double r = 2.0 + _rng.NextDouble() * 2.0;       // Distance between 2 and 4.
            _agentPos._x = _preyPos._x + (int)Math.Truncate(Math.Cos(t) * r);
            _agentPos._y = _preyPos._y + (int)Math.Truncate(Math.Sin(t) * r);
        }

        /// <summary>
        /// Determine the agent's position in the world relative to the prey and walls, and set its sensor inputs accordingly.
        /// </summary>
        /// <param name="agent"></param>
        public void SetAgentInputsAndActivate(IBlackBox agent)
        {
            // Calc prey's position relative to the agent (in polar coordinate system).
            PolarPoint relPos = CartesianToPolar(_preyPos - _agentPos);

            // Determine agent sensor input values.
            // Reset all inputs.
            agent.InputSignalArray.Reset();

            // Test if prey is in sensor range.
            if(relPos.RadialSquared <= _sensorRangeSquared)
            {
                // Determine which sensor segment the prey is within - [0,7]. There are eight segments and they are tilted 22.5 degrees (half a segment)
                // such that due North, East South and West are each in the centre of a sensor segment (rather than on a segment boundary).
                double thetaAdjusted = relPos.Theta - PiDiv8;
                if(thetaAdjusted < 0.0) thetaAdjusted += 2.0 * Math.PI;
                int segmentIdx = (int)Math.Floor(thetaAdjusted / PiDiv4);

                // Set sensor segment's input.
                agent.InputSignalArray[segmentIdx] = 1.0;
            }

            // Prey closeness detector.
            agent.InputSignalArray[8] = relPos.RadialSquared <= 4.0 ? 1.0 : 0.0;

            // Wall detectors - N,E,S,W.
            // North.
            int d = (_gridSize-1) - _agentPos._y;
            if(d <= 4) { agent.InputSignalArray[9] = (4-d) / 4.0; }

            // East.
            d = (_gridSize-1) - _agentPos._x;
            if(d <= 4) { agent.InputSignalArray[10] = (4-d) / 4.0; }

            // South.
            if(_agentPos._y <= 4) { agent.InputSignalArray[11] = (4 - _agentPos._y) / 4.0; }

            // West.
            if(_agentPos._x <= 4) { agent.InputSignalArray[12] = (4 - _agentPos._x) / 4.0; }

            // Activate agent.
            agent.Activate();
        }

        /// <summary>
        /// Allow the agent to move one square based on its decision. Note that the agent can choose to not move.
        /// </summary>
        public void MoveAgent(IBlackBox agent)
        {
            // Selected output is highest signal at or above 0.5. Tied signals result in no result.
            double maxSig = agent.OutputSignalArray[0];
            int maxSigIdx = 0;

            for(int i=1; i < 4; i++) 
            {
                double v = agent.OutputSignalArray[i];
                if(v > maxSig) 
                {
                    maxSig = v;
                    maxSigIdx = i;
                }
                else if(v == maxSig)
                {   // Tie. Two equally high outputs.
                    maxSigIdx = -1;
                }
            }

            if(-1 == maxSigIdx || maxSig < 0.1) 
            {   // No action.
                return;
            }

            switch(maxSigIdx)
            {
                case 0: // Move north.
                    if(_agentPos._y < _gridSize-1) _agentPos._y++;
                    break;
                case 1: // Move east.
                    if(_agentPos._x < _gridSize-1) _agentPos._x++;
                    break;
                case 2: // Move south.
                    if(_agentPos._y > 0) _agentPos._y--;
                    break;
                case 3: // Move west.
                    if(_agentPos._x > 0) _agentPos._x--;
                    break;
            }
        }

        /// <summary>
        /// Move the prey. The prey moves by a simple set of stochastic rules that make it more likely to move away from
        /// the agent, and more so when it is close.
        /// </summary>
        public void MovePrey()
        {
            // Determine if prey will move in this timestep. (Speed is simulated stochastically)
            if(_rng.NextDouble() > _preySpeed) {
                return;
            }

            // Determine position of agent relative to prey.
            PolarPoint relPolarPos = CartesianToPolar(_agentPos - _preyPos);

			// Calculate probabilities of moving in each of the four directions. This stochastic strategy is taken from:
            // Incremental Evolution Of Complex General Behavior, Faustino Gomez and Risto Miikkulainen (1997)
            // (http://nn.cs.utexas.edu/downloads/papers/gomez.adaptive-behavior.pdf)
            // Essentially the prey moves randomly but we bias the movements so the prey moves away from the agent, and thus 
            // generally avoids getting eaten through stupidity.
            double t = T(Math.Sqrt(relPolarPos.RadialSquared));
            double[] probs = new double[4];
            probs[0] = Math.Exp(W(relPolarPos.Theta, Math.PI / 2.0) * t * 0.33);  // North.
            probs[1] = Math.Exp(W(relPolarPos.Theta, 0) * t * 0.33);              // East.
            probs[2] = Math.Exp(W(relPolarPos.Theta, Math.PI * 1.5) * t * 0.33);  // South.
            probs[3] = Math.Exp(W(relPolarPos.Theta, Math.PI) * t * 0.33);        // West.
            
            DiscreteDistribution dist = new DiscreteDistribution(probs);
            int action = DiscreteDistribution.Sample(_rng, dist);
            switch(action)
            {
                 case 0: // Move north.
                    if(_preyPos._y < _gridSize-1) _preyPos._y++;
                    break;
                case 1: // Move east.
                    if(_preyPos._x < _gridSize-1) _preyPos._x++;
                    break;
                case 2: // Move south.
                    if(_preyPos._y > 0) _preyPos._y--;
                    break;
                case 3: // Move west.
                    if(_preyPos._x > 0) _preyPos._x--;
                    break;
            }
        }

        /// <summary>
        /// Gets a boolean that indicates if the prey has been captured.
        /// </summary>
        public bool IsPreyCaptured()
		{
            return _agentPos == PreyPosition;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The T function as defined in Appendix A of the paper referenced at the top of this class.
        /// This is a function on the distance between the agent and the prey, with it's maximum value of 15 when distance is zero,
        /// and minimum value of 1.0 when distance is greater than 15.
        /// </summary>
        /// <param name="distance">Distance between the agent and the prey.</param>
		private static double T(double distance)
		{
			if(distance <= 4.0) {
				return 15.0 - distance;
            }
            else if(distance <= 15.0) {
				return 9.0 - (distance / 2.0);
            }
            else {
				return 1.0;
            }
		}

        /// <summary>
        /// The W function as defined in Appendix A of the paper referenced at the top of this class.
        /// </summary>
        /// <param name="angleA">Angle A (radians).</param>
        /// <param name="angleB">Angle B (radians).</param>
        private static double W(double angleA, double angleB)
        {
            // Notes
            // AngleDelta() returns 0 for equal angles, and PI for angles that are separated by PI radians (180 degrees).
            // Hence this function returns zero for equal angles, and 1.0 for fully opposing angles. 
            // However, in [1] the function is described differently:
            //
            //    angle = angle between the direction of action A_i and the direction from the prey to the agent,
            //
            //    dist = distance between the prey and the agent
            //
            //    W(angle) = (180 - |angle|) / 180
            //
            // As described the function does not work as intended, i.e. the intention is to give a high probability
            // for the prey to move away from the agent, but the definition of W given does the opposite. Hence the 
            // modification here corrects the error to give a function that works as originally intended. This is a 
            // very obvious error in simulations, because it causes the prey to walk directly into the jaws of the 
            // predator!
            return (AngleDelta(angleA, angleB)) / Math.PI;
        }

        /// <summary>
        /// Gives the smallest angle between two vectors with the given angles/
        /// </summary>
        /// <param name="a">Vector a angle.</param>
        /// <param name="b">Vector b angle/</param>
        /// <returns>Smallest angle between a and b.</returns>
        private static double AngleDelta(double a, double b)
        {
            // Calc absolute difference/delta between the two angles.
            double d = Math.Abs(a-b);
            
            // If the difference is greater than 180 degrees, then we want the smaller angle between 
            // the two vectors, i.e. 360 degrees minus d.
            if(d > Math.PI)
            {
                d = (2.0 * Math.PI) - d;
            }
            return d;
        }

        private static PolarPoint CartesianToPolar(IntPoint p)
        {
            double radiusSquared = (p._x * p._x) + (p._y * p._y);
            double angle = __atan2Lookup[p._y + 23][p._x + 23];

            if(angle < 0.0) {
                angle += 2.0 * Math.PI;
            }
            return new PolarPoint(radiusSquared, angle);
        }

        #endregion
    }
}
