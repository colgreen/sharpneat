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
using System.Diagnostics;

namespace SharpNeat.Domains.FunctionRegression
{
    public static class FnRegressionUtils
    {
        #region Public Static Methods

        public static void CalcFunctionMidAndScale(IFunction fn, ParamSamplingInfo paramSamplingInfo, out double mid, out double scale)
        {
            double[] xArr = paramSamplingInfo._xArr;
            double min = fn.GetValue(xArr[0]);
            double max = min;

            for(int i=0; i<xArr.Length; i++)
            {
                double y = fn.GetValue(xArr[i]);
                min = Math.Min(y, min);
                max = Math.Max(y, max);
            }

            // TODO: explain this (0.8 is logistic function range, 0.5 is the logistic function output value when its input is zero).
            double range = max - min;
            scale = range / 0.8;
            mid = ((min+max) / 2.0) - 0.5;
        }

        public static void CalcGradients (ParamSamplingInfo paramSamplingInfo, double[] yArr, double[] gradientArr)
        {
            // Handle the end points as special cases.
            // First point.
            double[] xArr = paramSamplingInfo._xArr;
            gradientArr[0] = CalcGradient(xArr[0], yArr[0], xArr[1], yArr[1]);

            // Intermediate points.
            for(int i=1; i< xArr.Length-1; i++) {
                gradientArr[i] = CalcGradient(xArr[i-1], yArr[i-1], xArr[i+1], yArr[i+1]);
            }

            // Last point.
            int lastIdx = xArr.Length-1;
            gradientArr[lastIdx] = CalcGradient(xArr[lastIdx-1], yArr[lastIdx-1], xArr[lastIdx], yArr[lastIdx]);
        }

        public static double CalcMeanSquaredError(double[] a, double[] b)
        {
            Debug.Assert(a.Length == b.Length);

            // Calc sum(squared error).
            double total = 0.0;
            for(int i=0; i<a.Length; i++)
            {
                double err = a[i] - b[i];
                total += err * err;
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
            return ydiff / (x2-x1);
        }

        #endregion
    }
}
