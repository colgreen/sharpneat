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
    /// The function to be regressed is read from the config data.
    /// </summary>
    public class FnRegressionEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        IFunction _fn;
        ParamSamplingInfo _paramSamplingInfo;
        IBlackBoxProbe _blackBoxProbe;
        readonly double[] _yArrTarget;
        readonly double[] _gradientArrTarget;

        ulong _evalCount;
        bool _stopConditionSatisfied;        
            
        #region Constructor

        /// <summary>
        /// Construct a function regression evaluator with the provided parameter sampling info and function to regress.
        /// </summary>
        public FnRegressionEvaluator(IFunction fn, ParamSamplingInfo paramSamplingInfo)
            : this(fn, paramSamplingInfo, CreateBlackBoxProbe(fn, paramSamplingInfo))
        {
        }

        /// <summary>
        /// Construct a function regression evaluator with the provided parameter sampling info and function to regress.
        /// </summary>
        public FnRegressionEvaluator(IFunction fn, ParamSamplingInfo paramSamplingInfo, IBlackBoxProbe blackBoxProbe)
        {
            _fn = fn;
            _paramSamplingInfo = paramSamplingInfo;
            _blackBoxProbe = blackBoxProbe;

            // Predetermine target responses.
            int sampleCount = _paramSamplingInfo._sampleCount;
            _yArrTarget = new double[sampleCount];
            _gradientArrTarget = new double[sampleCount];

            FunctionProbe fnProbe = new FunctionProbe(paramSamplingInfo);
            fnProbe.Probe(fn, _yArrTarget);
            FnRegressionUtils.CalcGradients(paramSamplingInfo, _yArrTarget, _gradientArrTarget);
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
            int sampleCount = _paramSamplingInfo._sampleCount;
            // TODO: We can avoid memory alloc here by allocating at construction time, but this requires modification of ParallelGenomeListEvaluator to utilise mulitple evaluators (one per thread).
            double[] yArr = new double[sampleCount];
            double[] gradientArr = new double[sampleCount];

            // Probe the black box over the full range of the input parameter.
            _blackBoxProbe.Probe(box, yArr);

            // Calc gradients.
            FnRegressionUtils.CalcGradients(_paramSamplingInfo, yArr, gradientArr);

            // Calc y position mean squared error.
            double yMse = FnRegressionUtils.CalcMeanSquaredError(yArr, _yArrTarget);

            // Calc gradient mean squared error.
            double gradientMse = FnRegressionUtils.CalcMeanSquaredError(gradientArr, _gradientArrTarget);

            // Calc fitness as the inverse of mse (higher value is fitter). 
            // Add a constant to avoid divide by zero, and to constrain the fitness range between bad and good solutions; 
            // this allows the selection straregy to select solutions that are mediocre and therefore helps preserve diversity.
            double fitness =  1.0 / (yMse + gradientMse + 0.01);

            // Test for stopping condition (near perfect response).
            if(fitness >= 100.0) {
                _stopConditionSatisfied = true;
            }
            _evalCount++;
            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {   
        }

        #endregion

        #region Private Static Methods

        private static BlackBoxProbe CreateBlackBoxProbe(IFunction fn, ParamSamplingInfo paramSamplingInfo)
        {
            // Determine the mid output value of the function (over the specified sample points) and a scaling factor
            // to apply the to neural netwkrk response for it to be able to recreate the function (because the neural net
            // output range is [0,1] when using the logistic function as the neurn activation function).
            double scale, mid;
            FnRegressionUtils.CalcFunctionMidAndScale(fn, paramSamplingInfo, out mid, out scale);

            var blackBoxProbe = new BlackBoxProbe(paramSamplingInfo, mid, scale);
            return blackBoxProbe;
        }

        #endregion
    }
}
