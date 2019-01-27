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
using SharpNeat.BlackBox;

namespace SharpNeat.Tasks.FunctionRegression
{
    /// <summary>
    /// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
    /// </summary>
    public interface IBlackBoxProbe
    {
        /// <summary>
        /// Probe the given black box, and record the responses in <paramref name="responseArr"/>
        /// </summary>
        /// <param name="box">The black box to probe.</param>
        /// <param name="responseArr">Response array.</param>
        void Probe(IBlackBox<double> box, double[] responseArr);
    }
}
