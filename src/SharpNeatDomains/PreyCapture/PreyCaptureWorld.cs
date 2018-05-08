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
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.PreyCapture
{
    /// <summary>
    /// The prey capture task's grid world. Encapsulates agent's sensor and motor hardware and the prey's simple stochastic movement.
    /// </summary>
    public class PreyCaptureWorld
    {
        #region Constants

        const double PiDiv4 = Math.PI / 4.0;
		const double PiDiv8 = Math.PI / 8.0;

        #endregion

        #region Instance Fields

        // World parameters.
		readonly int _gridSize;         // Minimum of 9 (-> 9x9 grid). 24 is a good value here.
		readonly int _preyInitMoves;	// Number of initial moves (0 to 4).
		readonly double _preySpeed;	    // 0 to 1.
		readonly double _sensorRange;	// Agent's sensor range.
        readonly int _maxTimesteps;	    // Length of time agent can survive w/out eating prey.

        // World state.
        IntPoint _preyPos;
        IntPoint _agentPos;
        
        // Random number generator.
        XorShiftRandom _rng;

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
            _maxTimesteps = maxTimesteps;
            _rng = new XorShiftRandom();
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
            for(; t<_preyInitMoves; t++)
            {
                SetAgentInputsAndActivate(agent);
                MovePrey();
            }

            // Let the chase begin!
            for(; t<_maxTimesteps; t++)
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

            // Agent failed to capture prey in the alloted time.
            return false;
        }

        /// <summary>
        /// Initialise agent and prey positions. The prey is positioned randomly with at least 4 empty squares between it and a wall (in all directions).
        /// The agent is positioned randomly but such that the prey is within sensor range (distance 2 or less).
        /// </summary>
        public void InitPositions()
        {
            // Random pos at least 4 units away from any wall.
            _preyPos._x = 4 + _rng.Next(_gridSize - 8);
            _preyPos._y = 4 + _rng.Next(_gridSize - 8);

            // Agent position. Within range of the prey.
            double t = 2.0 * Math.PI * _rng.NextDouble();   // Random angle.
            double r = 2.0 + _rng.NextDouble() * 2.0;       // Distance between 2 and 4.
            _agentPos._x = _preyPos._x + (int)Math.Floor(Math.Cos(t) * r);
            _agentPos._y = _preyPos._y + (int)Math.Floor(Math.Sin(t) * r);
        }

        /// <summary>
        /// Determine the agent's position in the world relative to the prey and walls, and set its sensor inputs accordingly.
        /// </summary>
        /// <param name="agent"></param>
        public void SetAgentInputsAndActivate(IBlackBox agent)
        {
            // Calc prey's position relative to the agent (in polar coordinate system).
            PolarPoint relPos = PolarPoint.FromCartesian(_preyPos - _agentPos);

            // Determine agent sensor input values.
            // Reset all inputs.
            agent.InputSignalArray.Reset();

            // Test if prey is in sensor range.
            if(relPos.Radial <= _sensorRange)
            {
                // Determine which sensor segment the prey is within - [0,7]. There are eight segments and they are tilted 22.5 degrees (half a segment)
                // such that due North, East South and West are each in the center of a sensor segment (rather than on a segment boundary).
                int segmentIdx = (int)Math.Floor((relPos.Theta / PiDiv4) + PiDiv8);
                if(8==segmentIdx) {
                    segmentIdx = 0;
                }
                // Set sensor segment's input.
                agent.InputSignalArray[segmentIdx] = 1.0;
            }

            // Prey closeness detector.
            agent.InputSignalArray[8] = relPos.Radial > 2.0 ? 0.0 : 1.0;

            // Wall detectors - N,E,S,W.
            // North.
            int d = (_gridSize-1) - _agentPos._y;
            if(d < 4) { agent.InputSignalArray[9] = (4-d) / 4.0; }

            // East.
            d = (_gridSize-1) - _agentPos._x;
            if(d < 4) { agent.InputSignalArray[10] = (4-d) / 4.0; }

            // South.
            if(_agentPos._y < 4) { agent.InputSignalArray[11] = (4 - _agentPos._y) / 4.0; }

            // West.
            if(_agentPos._x < 4) { agent.InputSignalArray[12] = (4 - _agentPos._x) / 4.0; }

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

            for(int i=1; i<4; i++) 
            {
                if(agent.OutputSignalArray[i] > maxSig) 
                {
                    maxSig = agent.OutputSignalArray[i];
                    maxSigIdx = i;
                }
                else if(agent.OutputSignalArray[i] == maxSig)
                {   // Tie. Two equally high outputs.
                    maxSigIdx = -1;
                }
            }

            if(-1 == maxSigIdx || maxSig < 0.5) 
            {   // No action.
                return;
            }

            switch(maxSigIdx)
            {
                case 0: // Move north.
                    _agentPos._y = Math.Min(_agentPos._y + 1, _gridSize - 1);
                    break;
                case 1: // Move east.
                    _agentPos._x = Math.Min(_agentPos._x + 1, _gridSize - 1);
                    break;
                case 2: // Move south.
                    _agentPos._y = Math.Max(_agentPos._y - 1, 0);
                    break;
                case 3: // Move west (is the best?)
                    _agentPos._x = Math.Max(_agentPos._x - 1, 0);
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
            PolarPoint relPolarPos = PolarPoint.FromCartesian(_agentPos - _preyPos);

			// Calculate probabilities of moving in each of the four directions. This stochastic strategy is taken from:
            // Incremental Evolution Of Complex General Behavior, Faustino Gomez and Risto Miikkulainen (1997)
            // (http://nn.cs.utexas.edu/downloads/papers/gomez.adaptive-behavior.pdf)
            // Essentially the prey moves randomly but we bias the movements so the prey moves away from the agent, and thus 
            // generally avoids getting eaten through stupidity.
            double T = MovePrey_T(relPolarPos.Radial);
            double[] probs = new double[4];
            probs[0] = Math.Exp((CalcAngleDelta(relPolarPos.Theta, Math.PI/2.0) / Math.PI) * T * 0.33);    // North.
            probs[1] = Math.Exp((CalcAngleDelta(relPolarPos.Theta, 0) / Math.PI) * T * 0.33);              // East.
            probs[2] = Math.Exp((CalcAngleDelta(relPolarPos.Theta, Math.PI * 1.5) / Math.PI) * T * 0.33);  // South.
            probs[3] = Math.Exp((CalcAngleDelta(relPolarPos.Theta, Math.PI) / Math.PI) * T * 0.33);        // West.
            
            DiscreteDistribution dist = new DiscreteDistribution(probs, _rng);
            int action = dist.Sample();
            switch(action)
            {
                 case 0: // Move north.
                    _preyPos._y = Math.Min(_preyPos._y + 1, _gridSize - 1);
                    break;
                case 1: // Move east.
                    _preyPos._x = Math.Min(_preyPos._x + 1, _gridSize - 1);
                    break;
                case 2: // Move south.
                    _preyPos._y = Math.Max(_preyPos._y - 1, 0);
                    break;
                case 3: // Move west (is the best?)
                    _preyPos._x = Math.Max(_preyPos._x - 1, 0);
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

        #region Private Methods

		private double MovePrey_T(double distance)
		{
			if(distance < 5.0) {
				return 15.0 - distance;
            }
            else if(distance < 16.0) {
				return 9.0 - (distance/2.0);
            }
            else {
				return 1.0;
            }
		}

        /// <summary>
        /// Calculates minimum angle between two vectors (specified by angle only).
        /// </summary>
        private double CalcAngleDelta(double a, double b)
        {
            return Math.Min(Math.Abs(a-b), Math.Abs(b-a));
        }

        #endregion
    }
}
