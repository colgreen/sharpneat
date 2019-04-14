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
    public class GenerativeBlackBoxProbe : IBlackBoxProbe
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
        public GenerativeBlackBoxProbe(ParamSamplingInfo paramSamplingInfo, double offset, double scale)
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

            // Reset black box internal state.
            box.ResetState();

            int sampleCount = _paramSamplingInfo._sampleCount;
            for(int i=0; i < sampleCount; i++)
            {
                // Activate the black box.
                box.Activate();

                // Get the black box's output value.
                responseArr[i] = ((box.OutputSignalArray[0] - 0.5) * _scale) + _offset;
            }
        }

        #endregion
    }
}
