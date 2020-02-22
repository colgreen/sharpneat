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

namespace SharpNeat.Domains.PreyCapture
{
    /// <summary>
    /// Defines a 2D point in the polar coordinate space.
    /// </summary>
    public struct PolarPoint
    {
        /// <summary>
        /// Radial coordinate squared
        /// </summary>
        double _r;
        /// <summary>
        /// Angular coordinate (theta).
        /// </summary>
        double _t;

        #region Constructor

        /// <summary>
        /// Construct with provided coordinate values.
        /// </summary>
        /// <param name="r">Radial coordinate (distance between points) squared.</param>
        /// <param name="t">Angular coordinate (theta).</param>
        public PolarPoint(double radialSquared, double t)
        {
            _r = radialSquared;
            _t = t;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Radial coordinate.
        /// </summary>
        public double RadialSquared
        {
            get { return _r; }
        }

        /// <summary>
        /// Angular coordinate (theta).
        /// </summary>
        public double Theta
        {
            get { return _t; }
        }

        #endregion
    }
}
