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
using System.Diagnostics;

namespace SharpNeat.Domains.FunctionRegression
{
    /// <summary>
    /// Parameter sampling info. Describes the value range to sample, the number of samples within
    /// that range and the increment between samples.
    /// </summary>
    public struct ParamSamplingInfo
    {
        /// <summary>Sample range minimum.</summary>
        public readonly double _min;
        /// <summary>Sample range maximum.</summary>
        public readonly double _max;
        /// <summary>Intra sample increment.</summary>
        public readonly double _incr;
        /// <summary>Sample count.</summary>
        public readonly int _sampleCount;
        /// <summary>X positions of the sample points.</summary>
        public readonly double[] _xArr;
        /// <summary>X positions of the sample points in the neural net input space (i.e. scaled from 0 to 1)</summary>
        public readonly double[] _xArrNetwork;

        /// <summary>
        /// Construct with the provided parameter info.
        /// </summary>
        public ParamSamplingInfo(double min, double max, int sampleCount)
        {
            Debug.Assert(sampleCount>=3, "Sample count must be >= 3");
            _min = min;
            _max = max;
            _incr = (max-min) / (sampleCount-1);
            _sampleCount = sampleCount;

            double incrNet = 1.0 / (sampleCount-1);
            double x = min;
            double xNet = 0;
            _xArr = new double[sampleCount];
            _xArrNetwork = new double[sampleCount];
            for(int i=0; i<sampleCount; i++, x += _incr, xNet += incrNet)
            {
                _xArr[i] = x;
                _xArrNetwork[i] = xNet;
            }
        }
    }
}
