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
    public class FunctionProbe
    {
        readonly ParamSamplingInfo _paramSamplingInfo;

        #region Constructor

        public FunctionProbe(ParamSamplingInfo paramSamplingInfo)
        {
            _paramSamplingInfo = paramSamplingInfo;
        }

        #endregion

        #region Public Methods

        public void Probe(IFunction fn, double[] responseArr)
        {
            Debug.Assert(responseArr.Length == _paramSamplingInfo._sampleCount);

            double[] xArr = _paramSamplingInfo._xArr;

            for(int i=0; i < xArr.Length; i++) {
                responseArr[i] = fn.GetValue(xArr[i]);
            }
        }

        #endregion
    }
}
