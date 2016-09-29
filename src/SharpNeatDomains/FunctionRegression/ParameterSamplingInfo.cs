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
    public struct ParameterSamplingInfo
    {
        /// <summary>Sample range minimum.</summary>
        public double _min;
        /// <summary>Sample range maximum.</summary>
        public double _max;
        /// <summary>Intra sample increment.</summary>
        public double _incr;
        /// <summary>Sample count.</summary>
        public int _sampleCount;

        /// <summary>
        /// Construct with the provided parameter info.
        /// </summary>
        public ParameterSamplingInfo(double min, double max, int sampleCount)
        {
            Debug.Assert(sampleCount>=3, "Sample count must be >= 3");
            _min = min;
            _max = max;
            _incr = (max-min) / (sampleCount-1);
            _sampleCount = sampleCount;
        }
    }
}
