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
using System;
using Redzen;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.FunctionRegression
{
    /// <summary>
    /// Evaluator for the Function Regression task.
    /// 
    /// One continuous valued input maps to one continuous valued output.
    ///
    /// Evaluation consists of querying the provided black box for a number of distinct values over the range of the
    /// continuous valued input, and comparing the black box response with the expected/correct response.
    /// </summary>
    public sealed class FuncRegressionEvaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        readonly Func<double,double> _fn;
        readonly ParamSamplingInfo _paramSamplingInfo;
        readonly double _gradientMseWeight;
        readonly double _yMseWeight;

        // Expected/correct response arrays.
        readonly double[] _yArrTarget;
        readonly double[] _gradientArrTarget;

        // Actual black box response arrays (reusable evaluator state).
        readonly double[] _yArr;
        readonly double[] _gradientArr;

        readonly IBlackBoxProbe _blackBoxProbe;

        #region Constructors

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="fn">The function to apply function regression to..</param>
        /// <param name="paramSamplingInfo">Parameter sampling info.</param>
        /// <param name="gradientMseWeight">Fitness weighting to apply to the gradient fitness score.</param> 
        /// <param name="yArrTarget">Array of target y values (function output values).</param>
        /// <param name="gradientArrTarget">Array of target gradient values.</param>
        /// <param name="blackBoxProbe">Black box probe. For obtaining the y value response array from an instance of <see cref="IBlackBox{T}"/>.</param>
        internal FuncRegressionEvaluator(
            Func<double,double> fn,
            ParamSamplingInfo paramSamplingInfo,
            double gradientMseWeight,
            double[] yArrTarget,
            double[] gradientArrTarget,
            IBlackBoxProbe blackBoxProbe)
        {
            _fn = fn;
            _paramSamplingInfo = paramSamplingInfo;
            _gradientMseWeight = gradientMseWeight;
            _yMseWeight = 1.0 - gradientMseWeight;

            _yArrTarget = yArrTarget;
            _gradientArrTarget = gradientArrTarget;

            // Alloc working arrays for receiving black box outputs.
            int sampleCount = _paramSamplingInfo.SampleCount;
            _yArr = new double[sampleCount];
            _gradientArr = new double[sampleCount];

            _blackBoxProbe = blackBoxProbe;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate the provided black box against the function regression task,
        /// and return its fitness score.
        /// </summary>
        /// <param name="box">The black box to evaluate.</param>
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            // Probe the black box over the full range of the input parameter.
            _blackBoxProbe.Probe(box, _yArr);

            // Calc gradients.
            FuncRegressionUtils.CalcGradients(_paramSamplingInfo, _yArr, _gradientArr);

            // Calc y position mean squared error (MSE), and apply weighting.
            double yMse = MathArrayUtils.MeanSquaredDelta(_yArr, _yArrTarget);
            yMse *= _yMseWeight;

            // Calc gradient mean squared error.
            double gradientMse = MathArrayUtils.MeanSquaredDelta(_gradientArr, _gradientArrTarget);
            gradientMse *= _gradientMseWeight;

            // Calc fitness as the inverse of MSE (higher value is fitter). 
            // Add a constant to avoid divide by zero, and to constrain the fitness range between bad and good solutions; 
            // this allows the selection strategy to select solutions that are mediocre and therefore helps preserve diversity.
            double fitness =  20.0 / (yMse + gradientMse + 0.02);

            return new FitnessInfo(fitness);
        }

        #endregion
    }
}
