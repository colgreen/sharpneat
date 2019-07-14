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
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Tasks.FunctionRegression
{
    internal static class FuncRegressionUtils
    {
        #region Public Static Methods

        public static void Probe(
            Func<double,double> fn,
            ParamSamplingInfo paramSamplingInfo,
            double[] responseArr)
        {
            Debug.Assert(responseArr.Length == paramSamplingInfo.SampleResolution);

            double[] xArr = paramSamplingInfo.XArr;

            for(int i=0; i < xArr.Length; i++) {
                responseArr[i] = fn(xArr[i]);
            }
        }

        public static void CalcGradients(
            ParamSamplingInfo paramSamplingInfo,
            double[] yArr,
            double[] gradientArr)
        {
            // Notes.
            // The gradient at a sample point is approximated by taking the gradient of the line between the two 
            // sample points either side of that point. For the first and last sample points we take the gradient 
            // of the line between the sample point and its single adjacent sample point (as an alternative we could 
            // sample an additional point at each end that doesn't get used for the function regression evaluation.
            //
            // This approach is rather crude, but fast. A better approach might be to do a polynomial regression on 
            // the sample point and its nearest two adjacent samples, and then take the gradient of the polynomial
            // regression at the required point; obviously that would required more computational work to do so may
            // not be beneficial in the overall context of an evolutionary algorithm.
            //
            // Furthermore, the difference between this gradient approximation and the true gradient decreases with
            // increases sample density, therefore this is a reasonable approach *if* the sample density is 
            // sufficiently high.

            // Handle the end points as special cases.
            // First point.
            double[] xArr = paramSamplingInfo.XArr;
            gradientArr[0] = CalcGradient(xArr[0], yArr[0], xArr[1], yArr[1]);

            // Intermediate points.
            int width = Vector<double>.Count;
            int i=1;
            for(; i < xArr.Length - width - 1; i += width) 
            {
                // Calc a block of x deltas.
                var vecLeft = new Vector<double>(xArr, i - 1);
                var vecRight = new Vector<double>(xArr, i + 1);
                var xVecDelta = vecRight - vecLeft;

                // Calc a block of y deltas.
                vecLeft = new Vector<double>(yArr, i - 1);
                vecRight = new Vector<double>(yArr, i + 1);
                var yVecDelta = vecRight - vecLeft;

                // Divide the y's by x's to obtain the gradients.
                var gradientVec = yVecDelta / xVecDelta;

                gradientVec.CopyTo(gradientArr, i);
            }

            // Calc gradients for remaining intermediate points (if any).
            for (; i < xArr.Length - 1; i++) {
                gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i + 1], yArr[i + 1]);
            }

            // Last point.
            gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i], yArr[i]);
        }

        /// <summary>
        /// Determine the mid output value of the function (over the specified sample points) and a scaling factor
        /// to apply the to neural network response for it to be able to recreate the function (because the neural net
        /// output range is [0,1] when using the logistic function as the neuron activation function).
        /// </summary>
        /// <param name="fn">The function to be sampled.</param>
        /// <param name="paramSamplingInfo">Parameter sampling info.</param>
        /// <param name="mid">Returns the mid value of the function (halfway between min and max).</param>
        /// <param name="scale">Returns the scale of the function.</param>
        public static void CalcFunctionMidAndScale(
            Func<double,double> fn,
            ParamSamplingInfo paramSamplingInfo,
            out double mid, out double scale)
        {
            double[] xArr = paramSamplingInfo.XArr;
            double min = fn(xArr[0]);
            double max = min;

            for(int i=0; i < xArr.Length; i++)
            {
                double y = fn(xArr[i]);
                min = Math.Min(y, min);
                max = Math.Max(y, max);
            }

            // TODO: explain this (0.8 is logistic function range, 0.5 is the logistic function output value when its input is zero).
            double range = max - min;
            scale = range / 0.8;
            mid = (min + max) / 2.0;
        }

        #endregion

        #region Private Static Methods

        private static double CalcGradient(double x1, double y1, double x2, double y2)
        {
            return (y2 - y1) / (x2 - x1);
        }

        #endregion
    }
}
