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
using System;

namespace SharpNeat.Tasks.PreyCapture
{
    /// <summary>
    /// An integer Cartesian coordinate.
    /// </summary>
    internal struct Int32Point
    {
        /// <summary>
        /// The x-axis coordinate.
        /// </summary>
        public int X;
        /// <summary>
        /// The y-axis coordinate.
        /// </summary>
        public int Y;

        #region Constructor

        /// <summary>
        /// Construct with the provided Cartesian coordinate components.
        /// </summary>
        /// <param name="x">The x-axis coordinate.</param>
        /// <param name="y">The y-axis coordinate.</param>
        public Int32Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        #endregion

        #region Overrides [Object Equality]

        public override bool Equals(object obj)
        {
            return obj is Int32Point point 
                && X == point.X 
                && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X,Y);
        }

        #endregion

        #region Operators

        public static bool operator ==(Int32Point a, Int32Point b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Int32Point a, Int32Point b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static Int32Point operator -(Int32Point a, Int32Point b)
        {
            return new Int32Point(a.X - b.X, a.Y - b.Y);
        }

        #endregion
    }
}
