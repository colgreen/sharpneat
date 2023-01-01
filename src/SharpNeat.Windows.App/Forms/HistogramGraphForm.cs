// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Numerics;
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live histogram.
/// </summary>
internal sealed class HistogramGraphForm : GraphForm
{
    readonly PointPairList _ppl;

    /// <summary>
    /// Construct with the given titles.
    /// </summary>
    /// <param name="title">Graph title.</param>
    /// <param name="xAxisTitle">X-axis title.</param>
    /// <param name="y1AxisTitle">Y-axis title.</param>
    /// <param name="y2AxisTitle">Y2-axis title (optional).</param>
    public HistogramGraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string y2AxisTitle)
        : base(title, xAxisTitle, y1AxisTitle, y2AxisTitle)
    {
        _ppl = new PointPairList();
        _graphPane.XAxis.Type = AxisType.Linear;
        _graphPane.BarSettings.MinClusterGap = 0;

        BarItem barItem = _graphPane.AddBar(null, _ppl, Color.LightBlue);

        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;
    }

    /// <summary>
    /// Update the graph histogram data.
    /// </summary>
    /// <param name="xdata">The X data values.</param>
    /// <param name="ydata">The Y data values.</param>
    public void UpdateData(
        HistogramData histogramData)
    {
        _ppl.Clear();

        Span<HistogramBin> bins = histogramData.GetBinSpan();

        for(int i=0; i < bins.Length; i++)
        { 
            _ppl.Add(
                (bins[i].LowerBound + bins[i].UpperBound) * 0.5,
                bins[i].Frequency);
        }

        RefreshGraph();
    }

    /// <summary>
    /// Clear the histogram data.
    /// </summary>
    public override void Clear()
    {
        _ppl.Clear();
        RefreshGraph();
    }
}
