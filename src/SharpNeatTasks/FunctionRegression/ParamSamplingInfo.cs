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
using System.Diagnostics;

namespace SharpNeat.Tasks.FunctionRegression
{
    /// <summary>
    /// Parameter sampling info.
    /// Describes the value range to sample, the number of samples within that range, and the increment between samples.
    /// </summary>
    public struct ParamSamplingInfo
    {
        /// <summary>Sample range minimum.</summary>
        public double Min { get; set; }

        /// <summary>Sample range maximum.</summary>
        public double Max { get; set; }

        /// <summary>Intra sample increment.</summary>
        public double Incr { get; set; }

        /// <summary>Sample count.</summary>
        public int SampleCount { get; set; }

        /// <summary>X positions of the sample points.</summary>
        public double[] XArr { get; set; }

        /// <summary>X positions of the sample points in the neural net input space (i.e. scaled from 0 to 1)</summary>
        public double[] XArrNetwork { get; set; }

        /// <summary>
        /// Construct with the provided parameter sampling info.
        /// </summary>
        public ParamSamplingInfo(double min, double max, int sampleCount)
        {
            Debug.Assert(sampleCount>=3, "Sample count must be >= 3");
            this.Min = min;
            this.Max = max;
            double incr = this.Incr = (max-min) / (sampleCount - 1);
            this.SampleCount = sampleCount;

            double incrNet = 1.0 / (sampleCount - 1);
            double x = min;
            double xNet = 0;
            this.XArr = new double[sampleCount];
            this.XArrNetwork = new double[sampleCount];

            for(int i=0; i < sampleCount; i++, x += incr, xNet += incrNet)
            {
                this.XArr[i] = x;
                this.XArrNetwork[i] = xNet;
            }
        }
    }
}
