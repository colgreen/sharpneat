/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.PreyCapture
{
    /// <summary>
    /// Evaulator for the prey capture task.
    /// </summary>
    public class PreyCaptureEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        readonly int _trialsPerEvaluation;

        // World parameters.
		readonly int _gridSize;         // Minimum of 9 (-> 9x9 grid). 24 is a good value here.
		readonly int _preyInitMoves;	// Number of initial moves (0 to 4).
		readonly double _preySpeed;	    // 0 to 1.
		readonly double _sensorRange;	// Agent's sensor range.
        readonly int _maxTimesteps;	    // Length of time agent can survive w/out eating prey.

        ulong _evalCount;
        bool _stopConditionSatisfied;

        #region Constructor

        /// <summary>
        /// Construct with the provided task parameter arguments.
        /// </summary>
        public PreyCaptureEvaluator(int trialsPerEvaluation, int gridSize, int preyInitMoves, double preySpeed, double sensorRange, int maxTimesteps)
        {
            _trialsPerEvaluation = trialsPerEvaluation;
            _gridSize = gridSize;
            _preyInitMoves = preyInitMoves;
            _preySpeed = preySpeed;
            _sensorRange = sensorRange;
            _maxTimesteps = maxTimesteps;
        }

        #endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the XOR problem domain and return its fitness score.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            // Create grid based world.
            PreyCaptureWorld world = new PreyCaptureWorld(_gridSize, _preyInitMoves, _preySpeed, _sensorRange, _maxTimesteps);

            // Perform multiple independent trials.
            int fitness = 0;
            for(int i=0; i<_trialsPerEvaluation; i++)
            {
                // Run trial and count how many trials end with the agent catching the prey.
                if(world.RunTrial(box)) {
                    fitness++;
                }
            }

            // Track number of evaulations and test stop condition.
            _evalCount++;
            if(fitness == _trialsPerEvaluation) {
                _stopConditionSatisfied = true;
            }
            
            // return fitness score.
            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion
    }
}
