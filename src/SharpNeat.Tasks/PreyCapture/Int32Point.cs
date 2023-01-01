// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Tasks.PreyCapture;

/// <summary>
/// An integer Cartesian coordinate.
/// </summary>
public struct Int32Point
{
    /// <summary>
    /// The x-axis coordinate.
    /// </summary>
    public int X;
    /// <summary>
    /// The y-axis coordinate.
    /// </summary>
    public int Y;

    /// <summary>
    /// Construct with the provided Cartesian coordinate components.
    /// </summary>
    /// <param name="x">The x-axis coordinate.</param>
    /// <param name="y">The y-axis coordinate.</param>
    public Int32Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    #region Overrides [Object Equality]

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Int32Point" />.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the objects are equal; otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Int32Point point
            && X == point.X
            && Y == point.Y;
    }

    /// <summary>
    /// Get the hash code for the current object.
    /// </summary>
    /// <returns>The current object's hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    #endregion

    #region Operators

    /// <summary>
    /// Determines whether two <see cref="Int32Point"/>s have the same value.
    /// </summary>
    /// <param name="a">The first <see cref="Int32Point"/> to compare.</param>
    /// <param name="b">The second <see cref="Int32Point"/> to compare.</param>
    /// <returns>true if the two <see cref="Int32Point"/>s are equal; otherwise false.</returns>
    public static bool operator ==(Int32Point a, Int32Point b)
    {
        return (a.X == b.X) && (a.Y == b.Y);
    }

    /// <summary>
    /// Determines whether two <see cref="Int32Point"/>s have a different value.
    /// </summary>
    /// <param name="a">The first <see cref="Int32Point"/> to compare.</param>
    /// <param name="b">The second <see cref="Int32Point"/> to compare.</param>
    /// <returns>true if the two <see cref="Int32Point"/>s are different; otherwise false.</returns>
    public static bool operator !=(Int32Point a, Int32Point b)
    {
        return (a.X != b.X) || (a.Y != b.Y);
    }

    /// <summary>
    /// Subtract point b from point a, using pointwise subtraction of the point coordinates.
    /// </summary>
    /// <param name="a">The <see cref="Int32Point"/> to subtract from.</param>
    /// <param name="b">The <see cref="Int32Point"/> to subtract from point a.</param>
    /// <returns>The result of the subtraction.</returns>
    public static Int32Point operator -(Int32Point a, Int32Point b)
    {
        return new Int32Point(a.X - b.X, a.Y - b.Y);
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// <param name="a">Point A.</param>
    /// <param name="b">Point B.</param>
    /// <returns>The Euclidean distance between the two points.</returns>
    public static double CalculateDistance(Int32Point a, Int32Point b)
    {
        double xdelta = (a.X - b.X);
        double ydelta = (a.Y - b.Y);
        return Math.Sqrt((xdelta*xdelta) + (ydelta*ydelta));
    }

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// <param name="a">Point A.</param>
    /// <param name="x">Point B x coordinate.</param>
    /// <param name="y">Point B y coordinate.</param>
    /// <returns>The Euclidean distance between the two points.</returns>
    public static double CalculateDistance(Int32Point a, int x, int y)
    {
        double xdelta = (a.X - x);
        double ydelta = (a.Y - y);
        return Math.Sqrt((xdelta*xdelta) + (ydelta*ydelta));
    }

    #endregion
}
