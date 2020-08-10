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

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a connection between two nodes. Used primarily as a key into a Dictionary that 
    /// uniquely identifies connections by their end points.
    /// </summary>
    public readonly struct DirectedConnection : IEquatable<DirectedConnection>, IComparable<DirectedConnection>
    {
        #region Auto Properties

        /// <summary>
        /// Connection source node ID.
        /// </summary>
        public int SourceId { get; }
        /// <summary>
        /// Connection target node ID.
        /// </summary>
        public int TargetId { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public DirectedConnection(int srcId, int tgtId)
        {
            this.SourceId = srcId;
            this.TargetId = tgtId;
        }

        /// <summary>
        /// Construct with the provided source and target node IDs.
        /// </summary>
        public DirectedConnection(in DirectedConnection copyFrom)
        {
            this.SourceId = copyFrom.SourceId;
            this.TargetId = copyFrom.TargetId;
        }

        #endregion

        #region IEquatable / IComparable

        /// <summary>
        /// Determines whether the specified <see cref="DirectedConnection" /> is equal to the current <see cref="DirectedConnection" />.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the objects are equal; otherwise false.</returns>
        public bool Equals(DirectedConnection other)
        {
            return (this.SourceId == other.SourceId) 
                && (this.TargetId == other.TargetId);
        }

        /// <summary>
        /// Compares this instance to a specified instance and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A directed connection to compare with.</param>
        /// <returns>A signed integer indicating result of the comparison.</returns>
        public int CompareTo(DirectedConnection other)
        {
            // Notes.
            // The comparison here uses subtraction rather than comparing IDs, this eliminates a number of branches
            // which gives better performance. The code works and is safe because the source and target node IDs are
            // always have non-negative values, and therefore have a possible range of [0, (2^31)-1]. And if we 
            // subtract the largest possible value from zero we get -(2^31)-1 which is still within the range of 
            // and Int32, i.e. the result of that subtraction does not overflow and is therefore a negative value
            // as required (to give a valid comparison result).
            int v = this.SourceId - other.SourceId;
            if(v == 0L) {
                v = this.TargetId - other.TargetId;
            }
            return v;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="DirectedConnection" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the objects are equal; otherwise false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is DirectedConnection connection && this.Equals(connection);
        }

        /// <summary>
        /// Get the hash code for the current object.
        /// </summary>
        /// <returns>The current object's hash code.</returns>
        public override int GetHashCode()
        {
            // TODO: Consider using HashCode.Combine() instead (or lift code from that method and use it directly here).
            // Variant on FNV hash taken from: http://stackoverflow.com/a/263416/15703
            unchecked
            {
                int v = (int)2166136261;
                v = (v * 16777619) ^ SourceId;
                v = (v * 16777619) ^ TargetId;
                return v;
            }
        }

        /// <summary>
        /// Determines whether two <see cref="DirectedConnection"/>s have the same value.
        /// </summary>
        /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
        /// <returns>true if the two <see cref="DirectedConnection"/>s are equal; otherwise false.</returns>
        public static bool operator ==(in DirectedConnection x, in DirectedConnection y)
        {
            return (x.SourceId == y.SourceId) 
                && (x.TargetId == y.TargetId);
        }

        /// <summary>
        /// Determines whether two <see cref="DirectedConnection"/>s have a different value.
        /// </summary>
        /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
        /// <returns>true if the two <see cref="DirectedConnection"/>s are different; otherwise false.</returns>
        public static bool operator !=(in DirectedConnection x, in DirectedConnection y)
        {
            return (x.SourceId != y.SourceId) 
                || (x.TargetId != y.TargetId);
        }

        /// <summary>
        /// Determines whether a specified <see cref="DirectedConnection"/> is less than another specified <see cref="DirectedConnection"/>.
        /// </summary>
        /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
        /// <returns>true if <paramref name="x" /> is less than <paramref name="y" />; otherwise, false.</returns>
        public static bool operator <(in DirectedConnection x, in DirectedConnection y)
        {
            return x.CompareTo(y) < 0;
        }

        /// <summary>
        /// Determines whether a specified <see cref="DirectedConnection"/> is greater than another specified <see cref="DirectedConnection"/>.
        /// </summary>
        /// <param name="x">The first <see cref="DirectedConnection"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectedConnection"/> to compare.</param>
        /// <returns>true if <paramref name="x" /> is greater than <paramref name="y" />; otherwise, false.</returns>
        public static bool operator >(in DirectedConnection x, in DirectedConnection y)
        {
            return x.CompareTo(y) > 0;
        }

        #endregion
    }
}
