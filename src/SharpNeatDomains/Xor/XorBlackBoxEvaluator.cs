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

namespace SharpNeat.Domains
{
    /// <summary>
    /// A black box evaluator for the XOR logic gate problem domain. 
    /// 
    /// XOR (also known as Exclusive OR) is a type of logical disjunction on two operands that results in
    /// a value of true if and only if exactly one of the operands has a value of 'true'. A simple way 
    /// to state this is 'one or the other but not both.'.
    /// 
    /// This evaluator therefore requires that the black box to be evaluated has two inputs and one 
    /// output all using the range 0..1
    /// 
    /// In turn each of the four possible test cases are applied to the two inputs, the network is activated
    /// and the output is evaluated. If a 'false' response is required we expect an output of zero, for true
    /// we expect a 1.0. Fitness for each test case is the difference between the output and the wrong output, 
    /// thus a maximum of 1 can be scored on each test case giving a maximum of 4. In addition each outputs is
    /// compared against a threshold of 0.5, if all four outputs are on the correct side of the threshold then
    /// 10.0 is added to the total fitness. Therefore a black box that answers correctly but very close to the
    /// threshold will score just above 10, and a black box that answers correctly with perfect 0.0 and 1.0 
    /// answers will score a maximum of 14.0.
    /// 
    /// The first type of evaluation punishes for difference from the required outputs and therefore represents
    /// a smooth fitness space (we can evolve gradually towards better scores). The +10 score for 4 correct
    /// responses is 'all or nothing', in other words it is a fitness space with a large step and no indication
    /// of where the step is, which on it's own would be a poor fitness space as it required evolution to stumble
    /// on the correct network by random rather than ascending a gradient in the fitness space. If however we do 
    /// stumble on a black box that answers correctly but close to the threshold, then we would like that box to 
    /// obtain a higher score than a network with, say, 3 strong correct responses and but wrong overall. We can
    /// improve the correct box's output difference from threshold value gradually, while the box with 3 correct
    /// responses may actually be in the wrong area of the fitness space altogether - in the wrong 'ballpark'.
    /// </summary>
    public class XorBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        const double StopFitness = 10.0;
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
        /// Evaluate the provided IBlackBox against the XOR problem domain and return its fitness score.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            double fitness = 0;
            double output;
            double pass = 1.0;
            ISignalArray inputArr = box.InputSignalArray;
            ISignalArray outputArr = box.OutputSignalArray;

            _evalCount++;            

        //----- Test 0,0
            box.ResetState();

            // Set the input values
            inputArr[0] = 0.0;
            inputArr[1] = 0.0;

            // Activate the black box.
            box.Activate();
            if(!box.IsStateValid) 
            {   // Any black box that gets itself into an invalid state is unlikely to be
                // any good, so lets just bail out here.
                return FitnessInfo.Zero;
            }

            // Read output signal.
            output = outputArr[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");

            // Calculate this test case's contribution to the overall fitness score.
            //fitness += 1.0 - output; // Use this line to punish absolute error instead of squared error.
            fitness += 1.0-(output*output);
            if(output > 0.5) {
                pass = 0.0;
            }

        //----- Test 1,1
            // Reset any black box state from the previous test case.
            box.ResetState();

            // Set the input values
            inputArr[0] = 1.0;
            inputArr[1] = 1.0;

            // Activate the black box.
            box.Activate();
            if(!box.IsStateValid) 
            {   // Any black box that gets itself into an invalid state is unlikely to be
                // any good, so lets just bail out here.
                return FitnessInfo.Zero;
            }

            // Read output signal.
            output = outputArr[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");

            // Calculate this test case's contribution to the overall fitness score.
            //fitness += 1.0 - output; // Use this line to punish absolute error instead of squared error.
            fitness += 1.0-(output*output);
            if(output > 0.5) {
                pass = 0.0;
            }

        //----- Test 0,1
            // Reset any black box state from the previous test case.
            box.ResetState();

            // Set the input values
            inputArr[0] = 0.0;
            inputArr[1] = 1.0;

            // Activate the black box.
            box.Activate();
            if(!box.IsStateValid) 
            {   // Any black box that gets itself into an invalid state is unlikely to be
                // any good, so lets just bail out here.
                return FitnessInfo.Zero;
            }

            // Read output signal.
            output = outputArr[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");

            // Calculate this test case's contribution to the overall fitness score.
            // fitness += output; // Use this line to punish absolute error instead of squared error.
            fitness += 1.0-((1.0-output)*(1.0-output));
            if(output <= 0.5) {
                pass = 0.0;
            }

        //----- Test 1,0
            // Reset any black box state from the previous test case.
            box.ResetState();

            // Set the input values
            inputArr[0] = 1.0;
            inputArr[1] = 0.0;

            // Activate the black box.
            box.Activate();
            if(!box.IsStateValid) 
            {   // Any black box that gets itself into an invalid state is unlikely to be
                // any good, so lets just bail out here.
                return FitnessInfo.Zero;
            }

            // Read output signal.
            output = outputArr[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");

            // Calculate this test case's contribution to the overall fitness score.
            // fitness += output; // Use this line to punish absolute error instead of squared error.
            fitness += 1.0-((1.0-output)*(1.0-output));
            if(output <= 0.5) {
                pass = 0.0;
            }

            // If all four outputs were correct, that is, all four were on the correct side of the
            // threshold level - then we add 10 to the fitness.
            fitness += pass * 10.0;

            if(fitness >= StopFitness) {
                _stopConditionSatisfied = true;
            }

            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The XOR problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion
    }
}
