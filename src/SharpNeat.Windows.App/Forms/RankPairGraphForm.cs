// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live graph of values by rank, and a secondary set of values at each rank.
/// </summary>
internal sealed class RankPairGraphForm : GraphForm
{
    readonly PointPairList _ppl;
    readonly PointPairList _ppl2;

    public RankPairGraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string seriesName,
        string seriesName2)
        : base(title, xAxisTitle, y1AxisTitle, null)
    {
        _ppl = [];
        _ppl2 = [];
        _graphPane.XAxis.Type = AxisType.Linear;

        BarItem barItem = _graphPane.AddBar(seriesName, _ppl, Color.Blue);
        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;

        barItem = _graphPane.AddBar(seriesName2, _ppl2, Color.LightBlue);
        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;
    }

    /// <summary>
    /// Update the graph rank data.
    /// </summary>
    /// <param name="valueByRank">The new data values, by order of rank (with rank 1 at element zero).</param>
    /// <param name="secondaryValues">Secondary data series.</param>
    public void UpdateData(Span<double> valueByRank, Span<double> secondaryValues)
    {
        _ppl.Clear();
        _ppl2.Clear();

        for(int i=0; i < valueByRank.Length; i++)
        {
            _ppl.Add(i + 1, valueByRank[i]);
            _ppl2.Add(i + 1, secondaryValues[i]);
        }

        RefreshGraph();
    }

    /// <summary>
    /// Clear the rank data.
    /// </summary>
    public override void Clear()
    {
        _ppl.Clear();
        _ppl2.Clear();
        RefreshGraph();
    }
}
