/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Random;
using SharpNeat.BlackBox;

namespace SharpNeat.Tasks.PreyCapture
{
    /// <summary>
    /// The prey capture task's grid world, as defined in:
    /// 
    ///    Incremental Evolution Of Complex General Behavior, Faustino Gomez and Risto Miikkulainen (1997)
    ///    http://nn.cs.utexas.edu/downloads/papers/gomez.adaptive-behavior.pdf
    /// 
    /// Encapsulates the agent's sensor and motor hardware and the prey's simple stochastic movement.
    /// </summary>
    /// <remarks>
    /// Encapsulates the agent's sensor and motor hardware, and the prey's simple stochastic movement.
    /// </remarks>
    public sealed class PreyCaptureWorld
    {
        static readonly float[][] __atan2Lookup;

        #region Instance Fields

        // World parameters.
		readonly int _gridSize;         // Minimum of 9 (-> 9x9 grid). 24 is a good value here.
		readonly int _preyInitMoves;	// Number of initial moves (0 to 4).
		readonly float _preySpeed;	    // 0 to 1.
		readonly float _sensorRange;	// Agent's sensor range.
        readonly float _sensorRangeSqr; // Agent's sensor range, squared.
        readonly int _maxTimesteps;	    // Length of time agent can survive w/out eating prey.

        // Random number generator.
        readonly IRandomSource _rng;

        // World state.
        Int32Point _preyPos;
        Int32Point _agentPos;

        #endregion

        #region Static Initializer

        static PreyCaptureWorld()
        {
            __atan2Lookup = new float[47][];
            for(int y=0; y < 47; y++)
            { 
                __atan2Lookup[y] = new float[47];
                for(int x=0; x < 47; x++)
                {
                    __atan2Lookup[y][x] = MathF.Atan2(y-23, x-23);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs with the provided world parameter arguments.
        /// </summary>
        public PreyCaptureWorld(int gridSize, int preyInitMoves, float preySpeed, float sensorRange, int maxTimesteps)
        {
            _gridSize = gridSize;
            _preyInitMoves = preyInitMoves;
            _preySpeed = preySpeed;
            _sensorRange = sensorRange;
            _sensorRangeSqr = sensorRange * sensorRange;
            _maxTimesteps = maxTimesteps;
            _rng = RandomDefaults.CreateRandomSource();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs one trial of the provided agent in the world. Returns true if the agent captures the prey within
        /// the maximum number of timesteps allowed.
        /// </summary>
        public bool RunTrial(IBlackBox<double> agent)
        {
            // Initialise world state.
            InitPositions();

            // Clear any prior agent (neural network) state.
            agent.ResetState();

            // The prey gets a head start. Here we simulate the world as normal for a number of timesteps, during which
            // the agent's sensors receive inputs as normal, but the it is prevented from moving.
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

            // Agent failed to capture prey in the allotted time.
            return false;
        }

        /// <summary>
        /// Initialise agent and prey positions. The prey is positioned randomly with at least 4 empty squares between it and a wall (in all directions).
        /// The agent is positioned randomly but such that the prey is within sensor range (distance 2 or less).
        /// </summary>
        public void InitPositions()
        {
            // Random position at least 4 units away from any wall.
            _preyPos.X = 4 + _rng.Next(_gridSize - 8);
            _preyPos.Y = 4 + _rng.Next(_gridSize - 8);

            // Agent position. The angle from the prey is chosen at random, and the distance from the prey is randomly chosen between 2 and 4.
            float t = 2f * MathF.PI * _rng.NextFloat();   // Random angle.
            float r = MathF.FusedMultiplyAdd(2f,  _rng.NextFloat(), 2f);    // A distance between 2 and 4.
            _agentPos.X = _preyPos.X + (int)MathF.Truncate(MathF.Cos(t) * r);
            _agentPos.Y = _preyPos.Y + (int)MathF.Truncate(MathF.Sin(t) * r);
        }

        /// <summary>
        /// Determine the agent's position in the world relative to the prey and walls, and set its sensor inputs accordingly.
        /// </summary>
        /// <param name="agent"></param>
        public void SetAgentInputsAndActivate(IBlackBox<double> agent)
        {
            const float PI_over_8 = MathF.PI / 8f;
            const float Four_over_PI = 4f / MathF.PI;
            const float Quarter = 1f / 4f;

            // Calc prey's position relative to the agent, in polar coordinates.
            CartesianToPolar(
                _preyPos - _agentPos,
                out int relPosRadiusSqr,
                out float relPosAzimuth);

            // Determine agent sensor input values.
            // Reset all inputs.
            IVector<double> inputVec = agent.InputVector;
            inputVec.Reset();

            // Bias input.
            inputVec[0] = 1.0;

            // Test if prey is in sensor range.
            if(relPosRadiusSqr <= _sensorRangeSqr)
            {
                // Determine which sensor segment the prey is within - [0,7]. There are eight segments and they are tilted 22.5 degrees (half a segment)
                // such that due North, East South and West are each in the centre of a sensor segment (rather than on a segment boundary).
                float thetaAdjusted = relPosAzimuth - PI_over_8;
                if(thetaAdjusted < 0f) thetaAdjusted += 2f * MathF.PI;
                int segmentIdx = 1 + (int)MathF.Floor(thetaAdjusted * Four_over_PI);

                // Set sensor segment's input.
                inputVec[segmentIdx] = 1.0;
            }

            // Prey closeness detector.
            inputVec[9] = relPosRadiusSqr <= 4 ? 1.0 : 0.0;

            // Wall detectors - N,E,S,W.
            // North.
            int d = (_gridSize-1) - _agentPos.Y;
            if(d <= 4) { inputVec[10] = (4-d) * Quarter; }

            // East.
            d = (_gridSize-1) - _agentPos.X;
            if(d <= 4) { inputVec[11] = (4-d) * Quarter; }

            // South.
            if(_agentPos.Y <= 4) { inputVec[12] = (4 - _agentPos.Y) * Quarter; }

            // West.
            if(_agentPos.X <= 4) { inputVec[13] = (4 - _agentPos.X) * Quarter; }

            // Activate agent.
            agent.Activate();
        }

        /// <summary>
        /// Allow the agent to move one square based on its decision. Note that the agent can choose to not move.
        /// </summary>
        public void MoveAgent(IBlackBox<double> agent)
        {
            IVector<double> outputVec = agent.OutputVector;

            // Selected output is highest signal at or above 0.5. Tied signals result in no result.
            double maxSig = outputVec[0];
            int maxSigIdx = 0;

            for(int i=1; i < 4; i++) 
            {
                double v = outputVec[i];
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
                    if(_agentPos.Y < _gridSize-1) _agentPos.Y++;
                    break;
                case 1: // Move east.
                    if(_agentPos.X < _gridSize-1) _agentPos.X++;
                    break;
                case 2: // Move south.
                    if(_agentPos.Y > 0) _agentPos.Y--;
                    break;
                case 3: // Move west.
                    if(_agentPos.X > 0) _agentPos.X--;
                    break;
            }
        }

        /// <summary>
        /// Move the prey. The prey moves by a simple set of stochastic rules that make it more likely to move away from
        /// the agent with increased proximity to the agent.
        /// </summary>
        public void MovePrey()
        {
            // Determine if prey will move in this timestep. (Speed is simulated stochastically)
            if(_preySpeed == 1f || _rng.NextFloat() < _preySpeed)
            { 
                // Determine position of agent relative to prey, in polar coordinates.
                CartesianToPolar(
                    _agentPos - _preyPos,
                    out int relPosRadiusSqr,
                    out float relPosAzimuth);

                // Calculate the probability of moving in each of the four directions (north, east, south, west).
		        // This stochastic strategy is taken from the paper referenced at the top of this class.
                // Essentially, the prey moves randomly he movements are biased such that the prey is more likely to move away the agent the nearer it is to the agent , and thus 
                // generally avoids getting eaten 'by accident'.
                float t = T(MathF.Sqrt(relPosRadiusSqr)) * 0.33f;

                Span<float> probs = stackalloc float[4];
                probs[0] = Exp(W(relPosAzimuth, MathF.PI * 0.5f) * t);  // North.
                probs[1] = Exp(W(relPosAzimuth, 0f) * t);               // East.
                probs[2] = Exp(W(relPosAzimuth, MathF.PI * 1.5f) * t);  // South.
                probs[3] = Exp(W(relPosAzimuth, MathF.PI) * t);         // West.

                // Normalise the probabilities, such they sum to 1.0 (approximately, due to limitation of floating point arithmetic).
                NormaliseProbabilities(probs);

                // Sample from the discrete distribution.
                int action = Sample(probs);

                // Move in the chosen direction.
                switch(action)
                {
                    case 0: // Move north.
                        if(_preyPos.Y < _gridSize-1) _preyPos.Y++;
                        break;
                    case 1: // Move east.
                        if(_preyPos.X < _gridSize-1) _preyPos.X++;
                        break;
                    case 2: // Move south.
                        if(_preyPos.Y > 0) _preyPos.Y--;
                        break;
                    case 3: // Move west.
                        if(_preyPos.X > 0) _preyPos.X--;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a boolean that indicates if the prey has been captured.
        /// </summary>
        public bool IsPreyCaptured()
		{
            return _agentPos == _preyPos;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The T function as defined in Appendix A of the paper referenced at the top of this class.
        /// This is a function on the distance between the agent and the prey, with it's maximum value of 15 when distance is zero,
        /// and minimum value of 1.0 when distance is greater than 15.
        /// </summary>
        /// <param name="distance">Distance between the agent and the prey.</param>
		private static float T(float distance)
		{
			if(distance <= 4f) {
				return 15f - distance;
            }
            else if(distance <= 15f) {
				return 9f - (distance * 0.5f);
            }
            else {
				return 1f;
            }
		}

        /// <summary>
        /// The W function as defined in Appendix A of the paper referenced at the top of this class.
        /// </summary>
        /// <param name="angleA">Angle A (radians).</param>
        /// <param name="angleB">Angle B (radians).</param>
        private static float W(float angleA, float angleB)
        {
            const float One_over_PI = 1f / MathF.PI;

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
            return AngleDelta(angleA, angleB) * One_over_PI;
        }

        /// <summary>
        /// Gives the smallest angle between two vectors with the given angles/
        /// </summary>
        /// <param name="a">Vector a angle.</param>
        /// <param name="b">Vector b angle/</param>
        /// <returns>Smallest angle between a and b.</returns>
        private static float AngleDelta(float a, float b)
        {
            // Calc absolute difference/delta between the two angles.
            float d = Math.Abs(a-b);
            
            // If the difference is greater than 180 degrees, then we want the smaller angle between 
            // the two vectors, i.e. 360 degrees minus d.
            if(d > MathF.PI)
            {
                d = (2f * MathF.PI) - d;
            }
            return d;
        }

        /// <summary>
        /// Convert the given integer Cartesian coordinate to a polar coordinate.
        /// </summary>
        /// <param name="p">The integer Cartesian coordinate to convert.</param>
        /// <param name="radiusSqr">Returns the square of the radius, as an integer. I.e. the distance from the pole, squared.</param>
        /// <param name="azimuth">Returns the azimuth; the anticlockwise angle from the polar axis.</param>
        /// <remarks>
        /// The radius coordinate is returned as the *square* of the radius. This is because normally we want to compare the radius
        /// with some threshold, and it's faster to avoid a square root operation to calculate the radius, and instead to compare 
        /// squared radii.
        /// 
        /// By convention the polar axis is horizontal and to the right, and a positive azimuth represents an anticlockwise 
        /// rotation. Therefore an azimuth of zero degrees represents due east (note. this same angle is never represented by 
        /// +360 degrees), and an angle of +90 degrees represents due north.
        /// </remarks>
        private static void CartesianToPolar(Int32Point p, out int radiusSqr, out float azimuth)
        {
            radiusSqr = p.X * p.X + p.Y * p.Y;
            azimuth = __atan2Lookup[p.Y + 23][p.X + 23];
            if(azimuth < 0f) {
                azimuth += 2f * MathF.PI;
            }
        }

        /// <summary>
        /// Returns e raised to the specified power.
        /// </summary>
        /// <remarks>
        /// This approximate function is customised for use in the prey capture task, and gives reasonable results over the interval [0, 2*PI].
        /// As such this function should not be used elsewhere as a general purpose approximation for exp().
        /// </remarks>
        /// <param name="x">A number specifying a power.</param>
        /// <returns>The number e raised to the power x</returns>
        static float Exp(float x) 
        {
            // This variable should be 1/32, however, it has been optimised for values of x in the interval [0, 2*PI]
            const float __1_over_29_5 = 1f / 29.5f;
            x = MathF.FusedMultiplyAdd(x, __1_over_29_5, 1f);
            x *= x;
            x *= x;
            x *= x;
            x *= x;
            x *= x;
            return x;
        }

        #endregion

        #region Private Static Methods [Discrete Distribution Sampling]

        private int Sample(Span<float> probs)
        {
            // Obtain a random threshold value by sampling uniformly from interval [0,1).
            float thresh = _rng.NextFloat();

            // Loop through the discrete probabilities, accumulating as we go and stopping once 
            // the accumulator is greater than the random sample.
            float acc = 0f;
            for(int i=0; i < probs.Length; i++)
            {
                acc += probs[i];
                if(acc > thresh) {
                    return i;
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. acc == thresh. 
            // For simplicity just return the last option, and accept any small bias this might result in.
            return probs.Length-1;
        }

        private static void NormaliseProbabilities(Span<float> probs)
        {
            float totalReciprocal = 1f / (probs[0] + probs[1] + probs[2] + probs[3]);
            probs[0] *= totalReciprocal;
            probs[1] *= totalReciprocal;
            probs[2] *= totalReciprocal;
            probs[3] *= totalReciprocal;
        }

        #endregion
    }
}
