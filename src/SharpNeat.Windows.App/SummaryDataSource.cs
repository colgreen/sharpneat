// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App;

/// <summary>
/// Data source for the SummaryGraphForm.
/// </summary>
internal sealed class SummaryDataSource
{
    // A function for obtaining an array of plot points.
    readonly Func<Point2DDouble[]> _getPointArrayFn;

    /// <summary>
    /// Constructs a data source with the provided source details and delegate for acquiring data.
    /// </summary>
    public SummaryDataSource(
        string name, int yAxis, Color color,
        Func<Point2DDouble[]> getPointArrayFn)
    {
        Name = name;
        YAxis = yAxis;
        Color = color;
        _getPointArrayFn = getPointArrayFn;
    }

    /// <summary>
    /// Gets the name of the data source.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value that indicates which Y axis the data source should be plotted against.
    /// </summary>
    public int YAxis { get; }

    /// <summary>
    /// Gets a value that indicates the color that the data should plotted with.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the data to be plotted.
    /// </summary>
    public Point2DDouble[] GetPointArray()
    {
        return _getPointArrayFn();
    }
}
