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
using System.Numerics;

namespace SharpNeat.NeuralNets.Double.ActivationFunctions.Vectorized
{
    /// <summary>
    /// A sigmoid formed by two sub-sections of the y=x^2 curve.
    /// 
    /// The extremes are implemented as per the leaky ReLU, i.e. there is a linear slop to 
    /// ensure there is at least a gradient to follow at the extremes.
    /// </summary>
    public class QuadraticSigmoid : IActivationFunction<double>
    {
        public string Id => "QuadraticSigmoid";

        public double Fn(double x)
        {
            const double t = 0.999;
            const double a = 0.00001;

            // Calc abs(x) and sign(x) with just a single conditional branch 
            // (calling those functions individually results in two conditional branches).
            double sign = 1;
            if(x < 0)
            {
                x = x * -1;  
                sign = -1;
            }

            double y = 0;
            if(x < t) {
                y = t - ((x - t) * (x - t));
            }
            else //if (x >= t) 
            {
                y = t + (x - t) * a;
            }

            return (y * sign * 0.5) + 0.5;
        }

        public void Fn(double[] v)
        {
            Fn(v, v, 0, v.Length);
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            Fn(v, v, startIdx, endIdx);
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            // Init constants.
            var vec_t = new Vector<double>(0.999);
            var vec_a = new Vector<double>(0.00001);
            var vec_half = new Vector<double>(0.5);
            int width = Vector<double>.Count;

            int i=startIdx;
            for(; i <= endIdx-width; i += width)
            {
                // Load values into a vector.
                var vec = new Vector<double>(v, i);

                // Determine the absolute value of each element.
                var vec_abs = Vector.Abs(vec);

                // Determine the sign of each element (true indicates a non-negative value).
                var vec_sign_flag = Vector.Equals(vec, vec_abs);
                var vec_sign = Vector.ConditionalSelect(vec_sign_flag, Vector<double>.One, new Vector<double>(-1));

                // Handle abs values in the interval [0,t)
                var vec_x_minus_t = vec_abs - vec_t;
                var vec_inner = vec_t - (vec_x_minus_t * vec_x_minus_t);

                // Handle abs values outside of the interval [0,t).
                var vec_outer = vec_t + vec_x_minus_t * vec_a;

                // Select a value from vec_inner or vec_outer.
                var vec_select_inner = Vector.LessThan(vec_abs, vec_t);
                var vec_y = Vector.ConditionalSelect(vec_select_inner, vec_inner, vec_outer);

                // Apply final calc stage.
                vec_y = (vec_y * vec_sign * vec_half) + vec_half;

                // Copy the final result back into v.
                vec_y.CopyTo(w, i);
            }

            // Handle vectors with lengths not an exact multiple of vector width.
            for(; i < endIdx; i++) {
                w[i]= Fn(v[i]);
            }
        }
    }
}
