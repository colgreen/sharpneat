/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
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
        /// Probe the given black box, and record the responses in <paramref name="responseArr"/>.
        /// </summary>
        /// <param name="box">The black box to probe.</param>
        /// <param name="responseArr">Response array.</param>
        public void Probe(IBlackBox<double> box, double[] responseArr)
        {
            Debug.Assert(responseArr.Length == _paramSamplingInfo.SampleResolution);

            var inputs = box.Inputs.Span;
            double[] xArr = _paramSamplingInfo.XArrNetwork;
            for(int i=0; i < xArr.Length; i++)
            {
                // Reset black box internal state.
                box.ResetState();

                // Set bias input, and function input value.
                inputs[0] = 1.0;
                inputs[1] = xArr[i];

                // Activate the black box.
                box.Activate();

                // Get the black box's output value.
                // TODO: Review this scheme. This replicates the behaviour in SharpNEAT 2.x but not sure if it's ideal,
                // for one it depends on the output range of the neural net activation function in use.
                double output = box.Outputs[0];
                Clip(ref output);
                responseArr[i] = ((output - 0.5) * _scale) + _offset;
            }
        }

        #endregion

        #region Private Static Methods

        private static void Clip(ref double x)
        {
            if(x < 0.0) x = 0.0;
            else if(x > 1.0) x = 1.0;
        }

        #endregion
    }
}
