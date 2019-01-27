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
using SharpNeat.BlackBox;

namespace SharpNeat.Tasks.FunctionRegression
{
    /// <summary>
    /// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
    /// </summary>
    public sealed class BlackBoxProbe : IBlackBoxProbe
    {
        readonly ParamSamplingInfo _paramSamplingInfo;
        readonly double _offset;
        readonly double _scale;

        #region Constructor

        /// <summary>
        /// Construct a new instance.
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

        /// <summary>
        /// Probe the given black box, and record the responses in <paramref name="responseArr"/>
        /// </summary>
        /// <param name="box">The black box to probe.</param>
        /// <param name="responseArr">Response array.</param>
        public void Probe(IBlackBox<double> box, double[] responseArr)
        {
            Debug.Assert(responseArr.Length == _paramSamplingInfo.SampleCount);

            double[] xArr = _paramSamplingInfo.XArrNetwork;
            for(int i=0; i < xArr.Length; i++)
            {
                // Reset black box internal state.
                box.ResetState();

                // Apply function argument to black box input, and activate.
                box.InputVector[0] = xArr[i];
                box.Activate();

                // Get the black box's output value.
                responseArr[i] = (box.OutputVector[0] + _offset) * _scale;
            }
        }

        #endregion
    }
}
