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
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.BinaryThreeMultiplexer
{
    /// <summary>
    /// Binary 3-Multiplexer task.
    /// One binary input selects which of two other binary inputs to output. 
    /// </summary>
    public class BinaryThreeMultiplexerEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        const double StopFitness = 100.0;
        ulong _evalCount;
        bool _stopConditionSatisfied;

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
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the Binary 6-Multiplexer problem domain and return
        /// its fitness score.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            double fitness = 0.0;
            bool success = true;
            double output;
            ISignalArray inputArr = box.InputSignalArray;
            ISignalArray outputArr = box.OutputSignalArray;
            _evalCount++;  
            
            // 8 test cases.
            for(int i=0; i<8; i++)
            {
                // Apply bitmask to i and shift left to generate the input signals.
                // In addition we scale 0->1 to be 0.1->0.9.
                // Note. We /could/ eliminate all the boolean logic by pre-building a table of test 
                // signals and correct responses, but this is probably faster.
                int tmp = i;
                for(int j=0; j<3; j++) 
                {
                    inputArr[j] = tmp&0x1;
                    tmp >>= 1;
                }
                                
                // Activate the black box.
                box.Activate();
                if(!box.IsStateValid) 
                {   // Any black box that gets itself into an invalid state is unlikely to be
                    // any good, so let's just exit here.
                    return FitnessInfo.Zero;
                }

                // Read output signal.
                output = outputArr[0];
                Debug.Assert(output >= 0.0, "Unexpected negative output.");

                // Determine the correct answer by using highly cryptic (but fast) bit manipulation :)
                // The condition is true if the correct answer is true (1.0).
                if(((1<<(1+(i&0x1)))&i) != 0)
                {   // correct answer = true.
                    // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                    // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                    // Note. To base fitness on absolute error use: fitness += output;
                    fitness += 1.0-((1.0-output)*(1.0-output));
                    if(output<0.5) {
                        success=false;
                    }
                }
                else
                {   // correct answer = false.
                    // Assign fitness on sliding scale between 0.0 and 1.0 based on squared error.
                    // In tests squared error drove evolution significantly more efficiently in this domain than absolute error.
                    // Note. To base fitness on absolute error use: fitness += 1.0-output;
                    fitness += 1.0-(output*output);
                    if(output>=0.5) {
                        success=false;
                    }
                }

                // Reset black box state ready for next test case.
                box.ResetState();
            }

            // If the correct answer was given in each case then add a bonus value to the fitness.
            if(success) {
                fitness += 100.0;
            }

            if(fitness >= StopFitness) {
                _stopConditionSatisfied = true;
            }

            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The Binary Multiplexer problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion
    }
}
