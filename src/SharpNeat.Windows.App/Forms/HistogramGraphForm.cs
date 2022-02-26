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
using System.Drawing;
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live histogram.
/// </summary>
public class HistogramGraphForm : GraphForm
{
    readonly PointPairList _ppl;

    #region Constructor

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
        _graphPane.BarSettings.ClusterScaleWidth = 2f;

        BarItem barItem = _graphPane.AddBar("Frequency", _ppl, Color.LightBlue);

        barItem.Bar.Fill.Type = FillType.Solid;
        barItem.Bar.Border.IsVisible = true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Update the graph histogram data.
    /// </summary>
    /// <param name="xdata">The X data values.</param>
    /// <param name="ydata">The Y data values.</param>
    public void UpdateData(Span<double> xdata, Span<double> ydata)
    {
        if(xdata.Length != ydata.Length) { throw new ArgumentException("x and y data spans have different lengths."); }

        _ppl.Clear();

        for(int i=0; i < xdata.Length; i++)
            _ppl.Add(xdata[i], ydata[i]);

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
