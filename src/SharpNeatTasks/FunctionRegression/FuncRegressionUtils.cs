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
            Debug.Assert(responseArr.Length == paramSamplingInfo.SampleCount);

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
            // TODO: Can this be vectorized?

            // Handle the end points as special cases.
            // First point.
            double[] xArr = paramSamplingInfo.XArr;
            gradientArr[0] = CalcGradient(xArr[0], yArr[0], xArr[1], yArr[1]);

            // Intermediate points.
            for(int i=1; i < xArr.Length - 1; i++) {
                gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i + 1], yArr[i + 1]);
            }

            // Last point.
            int lastIdx = xArr.Length - 1;
            gradientArr[lastIdx] = CalcGradient(xArr[lastIdx - 1], yArr[lastIdx - 1], xArr[lastIdx], yArr[lastIdx]);
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

        public static double CalcMeanSquaredError(double[] a, double[] b)
        {
            Debug.Assert(a.Length == b.Length);

            double total = 0.0;

            if(Vector.IsHardwareAccelerated)
            {
                int width = Vector<double>.Count;
                var sumVec = new Vector<double>(0.0);

                // Loop over vector sized segments, calc the squared error for each, and accumulate in sumVec.
                int idx=0;
                for(; idx <= a.Length - width; idx += width)
                {
                    var aVec = new Vector<double>(a, idx);
                    var bVec = new Vector<double>(b, idx);

                    var cVec = aVec - bVec;
                    sumVec += cVec * cVec;
                }

                // Sum the elements of sumVec.
                for(int j=0; j < width; j++){
                    total += sumVec[j];
                }

                // Handle remaining elements (if any).
                for(; idx < a.Length; idx++)
                {
                    double err = a[idx] - b[idx];
                    total += err * err;
                }
            }
            else
            {
                // Calc sum(squared error).
                for(int i=0; i < a.Length; i++)
                {
                    double err = a[i] - b[i];
                    total += err * err;
                }
            }

            // Calculate mean squared error (MSE).
            return total / a.Length;
        }

        #endregion

        #region Private Static Methods

        private static double CalcGradient(double x1, double y1, double x2, double y2)
        {
            double ydiff = y2 - y1;
            if(ydiff == 0.0) {
                return 0.0;
            }
            return ydiff / (x2 - x1);
        }

        #endregion
    }
}
