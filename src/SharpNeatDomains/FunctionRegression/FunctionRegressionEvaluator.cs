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
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.FunctionRegression
{
    /// <summary>
    /// Function regression task.
    /// The function to be regressed is read from the config data. There is always one output.
    /// </summary>
    public class FunctionRegressionEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        /// <summary>
        /// The maximum error for the evaluator. The output at each sample point is in the range 0 to 1. Thus the error at each point has a maximum of 1.0.
        /// The error for the evaluator as a whole is the root mean square error (RMSE) over all sample points. Thus max error is always 1.0
        /// </summary>
        const double MaxError = 1.0;

        ulong _evalCount;
        bool _stopConditionSatisfied;
        ParameterSamplingInfo[] _paramSamplingInfoArr;
        IFunction _fnTask;
        double _samplePointCount;
        double _samplePointCountReciprocal;

        #region Constructor

        /// <summary>
        /// Construct a function regress evaluator with the provided parameter sampling info and function to regress.
        /// </summary>
        public FunctionRegressionEvaluator(ParameterSamplingInfo[] paramSamplingInfoArr, IFunction fnTask)
        {
            _paramSamplingInfoArr = paramSamplingInfoArr;
            _fnTask = fnTask;

            // Calculate the total number of sample points.
            int samplePointCount = 1;
            for(int i=0; i<_paramSamplingInfoArr.Length; i++) {
                samplePointCount *= _paramSamplingInfoArr[i]._sampleCount;
            }
            _samplePointCount = samplePointCount;
            _samplePointCountReciprocal = 1.0 / _samplePointCount;
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
            int paramCount = _paramSamplingInfoArr.Length;
            int paramIdxBound = paramCount - 1;

            int[] sampleIdxArr = new int[paramCount];
            int[] sampleCountArr = new int[paramCount];
            double[] paramValueArr = new double[paramCount];
            double[] paramIncrArr = new double[paramCount];

            for(int i=0; i<paramCount; i++) 
            {
                sampleCountArr[i] = _paramSamplingInfoArr[i]._sampleCount;
                paramValueArr[i] = _paramSamplingInfoArr[i]._min;
                paramIncrArr[i] = _paramSamplingInfoArr[i]._incr;
            }

            // Error accumulator.
            double errorAcc = 0.0;

            for(;;)
            {
                // Reset black box internal state.
                box.ResetState();

                // Apply function arguments to black box inputs.
                for(int i=0; i<paramCount; i++) {
                    box.InputSignalArray[i] = paramValueArr[i];
                }

                // Activate black box.
                box.Activate();

                // Get the black box's output value.
                double response = box.OutputSignalArray[0];

                // Get correct function value to compare with.
                double correctVal = _fnTask.GetValue(paramValueArr);

                // Accumulate squared error at each sample point. Abs() not required because we are squaring.
                errorAcc += (response-correctVal) * (response-correctVal);

                // Determine next sample point.
                for(int i=0; i<paramCount; i++)
                {
                    sampleIdxArr[i]++;
                    if(sampleIdxArr[i] < sampleCountArr[i]) 
                    {   // The parameter has incremented without reaching its bound. 
                        // We have the next valid sample point, so break out of the loop.
                        paramValueArr[i] += paramIncrArr[i]; 
                        break;
                    }
                        
                    // The current parameter has reached its bound. 
                    // If the *last* parameter has reached its bound then exit the outer loop.
                    if(i == paramIdxBound) {
                        goto exit;
                    }

                    // Reset the parameter and allow the inner loop to continue. This will 
                    // increment the next parameter.
                    sampleIdxArr[i] = 0;
                    paramValueArr[i] = _paramSamplingInfoArr[i]._min;
                }
            }

            exit:
            double fitness = MaxError - Math.Sqrt(errorAcc * _samplePointCountReciprocal);

            if(fitness < 0.5) {
                fitness = 0.0;
            } else {
                fitness = (fitness-0.5) * 2.0;
            }

            // Note. This is correct. Network's response is subtracted from MaxError; if all responses are correct then fitness == MaxError.
            if(fitness == MaxError) {
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

        #region Public Static Methods

        /// <summary>
        /// Get an instance of the function class for the specified function type.
        /// </summary>
        /// <param name="fnId"></param>
        /// <returns></returns>
        public static IFunction GetFunction(FunctionId fnId)
        {
            switch(fnId)
            {
                case FunctionId.Abs:
                    return new AbsFunction();    

                case FunctionId.Log:
                    return new LogFunction();

                case FunctionId.Multiplication:
                    return new MultiplicationFunction();

                case FunctionId.Sine:
                    return new SineFunction();                

                case FunctionId.SineXSquared:
                    return new SineXSquaredFunction();        
            }
            throw new ArgumentException(string.Format("Unknown FunctionId type [{0}]", fnId));
        }

        #endregion
    }
}
