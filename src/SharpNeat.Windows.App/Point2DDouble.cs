// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App;

internal struct Point2DDouble
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point2DDouble(double x, double y)
    {
        X = x;
        Y = y;
    }
}
