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
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live graph of values by rank.
/// </summary>
public class RankGraphForm : GraphForm
{
    readonly PointPairList _ppl;

    #region Constructor

    public RankGraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string seriesName)
        : base(title, xAxisTitle, y1AxisTitle, null)
    {
        _ppl = new PointPairList();
        _graphPane.XAxis.Type = AxisType.Linear;
        _graphPane.BarSettings.ClusterScaleWidth = 2f;

        BarItem barItem = _graphPane.AddBar(seriesName, _ppl, Color.LightBlue);

        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;
    }

    #endregion

    #region Public Methods

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
    /// Clear the time series data.
    /// </summary>
    public override void Clear()
    {
        _ppl.Clear();
        RefreshGraph();
    }

    #endregion
}
