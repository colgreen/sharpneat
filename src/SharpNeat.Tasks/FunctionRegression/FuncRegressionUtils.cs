﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Static utility methods related to the function regression family of tasks.
/// </summary>
/// <typeparam name="TScalar">Function data type.</typeparam>
public static class FuncRegressionUtils<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    #region Public Static Methods

    /// <summary>
    /// Probe the given function by taking samples of it at a number of discrete sample points.
    /// </summary>
    /// <param name="fn">The function to probe/sample.</param>
    /// <param name="paramSamplingInfo">Sampling metadata.</param>
    /// <param name="responseArr">An array to store the sample results within.</param>
    public static void Probe(
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        TScalar[] responseArr)
    {
        Debug.Assert(responseArr.Length == paramSamplingInfo.SampleResolution);

        TScalar[] xArr = paramSamplingInfo.XArr;

        for(int i=0; i < xArr.Length; i++)
            responseArr[i] = fn(xArr[i]);
    }

    /// <summary>
    /// Calculate an approximate gradient of a given function, at a number of discrete sample points.
    /// </summary>
    /// <param name="paramSamplingInfo">Sampling metadata.</param>
    /// <param name="yArr">The function output/result at a number of discrete sample points.</param>
    /// <param name="gradientArr">An array to store the calculated gradients within.</param>
    public static void CalcGradients(
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        TScalar[] yArr,
        TScalar[] gradientArr)
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
        TScalar[] xArr = paramSamplingInfo.XArr;
        gradientArr[0] = CalcGradient(xArr[0], yArr[0], xArr[1], yArr[1]);

        // Intermediate points.
        int width = Vector<TScalar>.Count;
        int i=1;
        for(; i < xArr.Length - width - 1; i += width)
        {
            // Calc a block of x deltas.
            var vecLeft = new Vector<TScalar>(xArr, i - 1);
            var vecRight = new Vector<TScalar>(xArr, i + 1);
            var xVecDelta = vecRight - vecLeft;

            // Calc a block of y deltas.
            vecLeft = new Vector<TScalar>(yArr, i - 1);
            vecRight = new Vector<TScalar>(yArr, i + 1);
            var yVecDelta = vecRight - vecLeft;

            // Divide the y's by x's to obtain the gradients.
            var gradientVec = yVecDelta / xVecDelta;

            gradientVec.CopyTo(gradientArr, i);
        }

        // Calc gradients for remaining intermediate points (if any).
        for(; i < xArr.Length - 1; i++)
        {
            gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i + 1], yArr[i + 1]);
        }

        // Last point.
        gradientArr[i] = CalcGradient(xArr[i - 1], yArr[i - 1], xArr[i], yArr[i]);
    }

    static readonly TScalar Two = TScalar.CreateChecked(2);
    static readonly TScalar PointEight = TScalar.CreateChecked(0.8);

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
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        out TScalar mid, out TScalar scale)
    {
        TScalar[] xArr = paramSamplingInfo.XArr;
        TScalar min = fn(xArr[0]);
        TScalar max = min;

        for(int i=0; i < xArr.Length; i++)
        {
            TScalar y = fn(xArr[i]);
            min = TScalar.Min(y, min);
            max = TScalar.Max(y, max);
        }

        // TODO: explain this (0.8 is logistic function range, 0.5 is the logistic function output value when its input is zero).
        TScalar range = max - min;
        scale = range / PointEight;
        mid = (min + max) / Two;
    }

    #endregion

    #region Private Static Methods

    private static TScalar CalcGradient(TScalar x1, TScalar y1, TScalar x2, TScalar y2)
    {
        return (y2 - y1) / (x2 - x1);
    }

    #endregion
}
