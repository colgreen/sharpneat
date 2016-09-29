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
using System;
namespace SharpNeat.Domains
{
    /// <summary>
    /// Defines a 2D point with integer Cartesian coordinates.
    /// </summary>
    public struct IntPoint
    {
        /// <summary>
        /// X-axis coordinate.
        /// </summary>
        public int _x;
        /// <summary>
        /// Y-axis coordinate.
        /// </summary>
        public int _y;

        #region Constructor

        /// <summary>
        /// Construct point with the specified coordinates.
        /// </summary>
        public IntPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        #endregion

// disable comment warnings for trivial public members.
#pragma warning disable 1591

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj is IntPoint) {
                IntPoint p = (IntPoint)obj;
                return (_x == p._x) && (_y == p._y);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _x + (17 * _y);
        }

        #endregion

        #region Operators

        public static bool operator ==(IntPoint a, IntPoint b)
        {
            return (a._x == b._x) && (a._y == b._y);
        }

        public static bool operator !=(IntPoint a, IntPoint b)
        {
            return (a._x != b._x) || (a._y != b._y);
        }

        public static IntPoint operator -(IntPoint a, IntPoint b)
        {
            return new IntPoint(a._x - b._x, a._y - b._y);
        }

        public static IntPoint operator +(IntPoint a, IntPoint b)
        {
            return new IntPoint(a._x + b._x, a._y + b._y);
        }

        #endregion

#pragma warning restore 1591

        #region Static Methods

        /// <summary>
        /// Calculate Euclidean distance between two points.
        /// </summary>
        public static double CalculateDistance(IntPoint a, IntPoint b)
        {
            double x = (a._x - b._x);
            double y = (a._y - b._y);
            return Math.Sqrt(x*x + y*y);
        }

        /// <summary>
        /// Calculate Euclidean distance between two points.
        /// </summary>
        public static double CalculateDistance(IntPoint a, int x, int y)
        {
            double xdelta = (a._x - x);
            double ydelta = (a._y - y);
            return Math.Sqrt(xdelta*xdelta + ydelta*ydelta);
        }

        #endregion

    }
}
