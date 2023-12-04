// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live graph of values by rank.
/// </summary>
internal sealed class RankGraphForm : GraphForm
{
    readonly PointPairList _ppl;

    public RankGraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string seriesName)
        : base(title, xAxisTitle, y1AxisTitle, null)
    {
        _ppl = [];
        _graphPane.XAxis.Type = AxisType.Linear;
        _graphPane.BarSettings.ClusterScaleWidth = 2f;

        BarItem barItem = _graphPane.AddBar(seriesName, _ppl, Color.LightBlue);

        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;
    }

    /// <summary>
    /// Update the graph rank data.
    /// </summary>
    /// <param name="valueByRank">The new data values, by order of rank (with rank 1 at element zero).</param>
    public void UpdateData(Span<double> valueByRank)
    {
        _ppl.Clear();

        for(int i=0; i < valueByRank.Length; i++)
            _ppl.Add(i + 1, valueByRank[i]);

        RefreshGraph();
    }

    /// <summary>
    /// Clear the rank data.
    /// </summary>
    public override void Clear()
    {
        _ppl.Clear();
        RefreshGraph();
    }
}
