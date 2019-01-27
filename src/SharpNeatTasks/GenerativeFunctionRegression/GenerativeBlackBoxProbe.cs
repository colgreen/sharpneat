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

namespace SharpNeat.Tasks.GenerativeFunctionRegression
{
    /// <summary>
    /// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
    /// </summary>
    public sealed class GenerativeBlackBoxProbe
    {
        readonly int _sampleCount;
        readonly double _offset;
        readonly double _scale;

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="sampleCount">The number of generative samples to take.</param>
        /// <param name="offset">Offset to apply to each black box output response.</param>
        /// <param name="scale">Scaling factor to apply to each black box output response.</param>
        public GenerativeBlackBoxProbe(int sampleCount, double offset, double scale)
        {
            _sampleCount = sampleCount;
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
            Debug.Assert(responseArr.Length == _sampleCount);

            // Reset black box internal state.
            box.ResetState();

            // Take the require number of samples.
            for(int i=0; i < _sampleCount; i++)
            {
                // Set bias input.
                box.InputVector[0] = 1.0;

                // Activate the black box.
                box.Activate();

                // Get the black box's output value.
                // TODO: Review. Note this scheme is different to the one in SharpNEAT 2.x
                responseArr[i] = (box.OutputVector[0] + _offset) * _scale;
            }
        }

        #endregion
    }
}
