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
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.FunctionRegression
{
    public class BlackBoxProbe : IBlackBoxProbe
    {
        readonly ParamSamplingInfo _paramSamplingInfo;
        readonly double _offset;
        readonly double _scale;

        #region Constructor

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="paramSamplingInfo">Parameter sampling info.</param>
        /// <param name="offset">Offset to apply to each neural network output response.</param>
        /// <param name="scale">Scaling factor to apply to each neural network output response.</param>
        public BlackBoxProbe(ParamSamplingInfo paramSamplingInfo, double offset, double scale)
        {
            _paramSamplingInfo = paramSamplingInfo;
            _offset = offset;
            _scale = scale;
        }

        #endregion

        #region Public Methods

        public void Probe(IBlackBox box, double[] responseArr)
        {
            Debug.Assert(responseArr.Length == _paramSamplingInfo._sampleCount);

            double[] xArr = _paramSamplingInfo._xArrNetwork;
            for(int i=0; i < xArr.Length; i++)
            {
                // Reset black box internal state.
                box.ResetState();

                // Apply function argument to black box input, and activate.
                box.InputSignalArray[0] = xArr[i];
                box.Activate();

                // Get the black box's output value.
                responseArr[i] = ((box.OutputSignalArray[0]-0.5) * _scale) + _offset;
            }
        }

        #endregion
    }
}
