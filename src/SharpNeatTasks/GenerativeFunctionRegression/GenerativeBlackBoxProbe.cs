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

        public void Probe(IBlackBox<double> box, double[] responseArr)
        {
            Debug.Assert(responseArr.Length == _sampleCount);

            // Reset black box internal state.
            box.ResetState();

            // Take the require number of samples.
            for(int i=0; i < _sampleCount; i++)
            {
                // Activate the black box.
                box.Activate();

                // Get the black box's output value.
                responseArr[i] = (box.OutputVector[0] + _offset) * _scale;
            }
        }

        #endregion
    }
}
