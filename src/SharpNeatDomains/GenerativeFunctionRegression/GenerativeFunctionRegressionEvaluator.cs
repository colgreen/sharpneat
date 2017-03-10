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
using SharpNeat.Core;
using SharpNeat.Domains.FunctionRegression;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.GenerativeFunctionRegression
{
    public class GenerativeFunctionRegressionEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        ParameterSamplingInfo _paramSamplingInfo;
        IFunction _fnTask;
        double _samplePointCountReciprocal;

        #region Constructor

        /// <summary>
        /// Construct a function regression evaluator with the provided parameter sampling info and function to regress.
        /// </summary>
        public GenerativeFunctionRegressionEvaluator(ParameterSamplingInfo paramSamplingInfo, IFunction fnTask)
        {
            _paramSamplingInfo = paramSamplingInfo;
            _fnTask = fnTask;
            _samplePointCountReciprocal = 1.0 / paramSamplingInfo._sampleCount;
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
            _evalCount++;

            double[] paramValueArr = new double[1];
            paramValueArr[0] = _paramSamplingInfo._min;
            double paramIncr = _paramSamplingInfo._incr;

            // Reset black box internal state.
            box.ResetState();

            // Error accumulator.
            double errorAcc = 0.0;

            for(int i=0; i<_paramSamplingInfo._sampleCount; i++)
            {
                //// Apply function arguments to black box inputs.
                //for(int i=0; i<paramCount; i++) {
                //    box.InputSignalArray[i] = paramValueArr[i];
                //}

                // Activate black box.
                box.Activate();

                // Get the black box's output value.
                double response = box.OutputSignalArray[0];

                // Get correct function value to compare with.
                double correctVal = _fnTask.GetValue(paramValueArr);

                // Accumulate squared error at each sample point. Abs() not required because we are squaring.
                double err = response-correctVal;
                errorAcc += err * err;

                // Determine next sample point.
                paramValueArr[0] += paramIncr;
            }

            // Note. The output at each sample point is in the range 0 to 1, thus the error at each point has a maximum of 1.0.
            // The error for the evaluator as a whole is the root mean square error (RMSE) over all sample points; thus max fitness is 1.0.
            double fitness = 1.0 - Math.Sqrt(errorAcc * _samplePointCountReciprocal);
            if(fitness == 1.0) {
                _stopConditionSatisfied = true;
            }

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
