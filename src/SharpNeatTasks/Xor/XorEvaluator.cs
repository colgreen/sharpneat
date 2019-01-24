/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Diagnostics;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Xor
{
    /// <summary>
    /// XOR logic task evaluator.
    /// 
    /// Two inputs supply the two XOR input values.
    /// 
    /// The correct response for the single output is input1 XOR input2.
    /// 
    /// Evaluation consists of querying the provided black box for all possible input combinations (2^2 = 4).
    /// </summary>
    public class XorEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        #region Public Methods

        /// <summary>
        /// Evaluate the provided IBlackBox against the Binary 11-Multiplexer problem domain and return
        /// its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            double fitness = 0.0;
            bool success = true;

            // Test case 0, 0.
            double output = Activate(box, 0.0, 0.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 1, 1.
            box.ResetState();
            output = Activate(box, 1.0, 1.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 0, 1.
            box.ResetState();
            output = Activate(box, 0.0, 1.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // Test case 1, 0.
            box.ResetState();
            output = Activate(box, 1.0, 0.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // If all four responses were correct then we add 10 to the fitness.
            if(success) {
                fitness += 10.0;
            }

            return new FitnessInfo(fitness);
        }

        #endregion

        #region Private Static Methods

        private static double Activate(
            IBlackBox<double> box,
            double in1, double in2)
        {
            // Bias input.
            box.InputVector[0] = 1.0;

            // XOR inputs.
            box.InputVector[1] = in1;
            box.InputVector[2] = in2;

            // Activate the black box.
            box.Activate();

            // Read output signal.
            double output = box.OutputVector[0];
            Debug.Assert(output >= 0.0, "Unexpected negative output.");
            return output;
        }

        #endregion
    }
}
