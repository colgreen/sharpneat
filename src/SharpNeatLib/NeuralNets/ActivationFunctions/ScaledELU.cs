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

namespace SharpNeat.NeuralNets
{
    /// <summary>
    /// Scaled Exponential Linear Unit (SELU).
    /// 
    /// From:
    ///     Self-Normalizing Neural Networks
    ///     https://arxiv.org/abs/1706.02515
    /// 
    /// Original source code (including parameter values):
    ///     https://github.com/bioinf-jku/SNNs/blob/master/selu.py
    ///    
    /// </summary>
    public class ScaledELU : IActivationFunction<double>
    {
        public string Id => "ScaledELU";

        public double Fn(double x)
        {
            double alpha = 1.6732632423543772848170429916717;
            double scale = 1.0507009873554804934193349852946;

            double y;
            if(x >= 0) {
                y = scale*x;
            } 
            else {
                y = scale*(alpha*Math.Exp(x)) - alpha;
            }

            return y;
        }

        public void Fn(double[] v)
        {
            // Naive implementation.
            for(int i=0; i<v.Length; i++) {
                v[i]= Fn(v[i]);
            }
        }

        public void Fn(double[] v, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i<endIdx; i++) {
                v[i]= Fn(v[i]);
            }
        }

        public void Fn(double[] v, double[] w, int startIdx, int endIdx)
        {
            // Naive implementation.
            for(int i=startIdx; i<endIdx; i++) {
                w[i]= Fn(v[i]);
            }
        }
    }
}
