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

namespace SharpNeat.Windows.App.Forms
{
    /// <summary>
    /// Form for displaying a live graph of values by rank, and a secondary set of values at each rank.
    /// </summary>
    public class RankPairGraphForm : GraphForm
    {
        readonly PointPairList _ppl;
        readonly PointPairList _ppl2;

        #region Constructor

        public RankPairGraphForm(
            string title,
            string xAxisTitle,
            string y1AxisTitle,
            string seriesName,
            string seriesName2)
            : base(title, xAxisTitle, y1AxisTitle, null)
        {
            _ppl = new PointPairList();
            _ppl2 = new PointPairList();
            _graphPane.XAxis.Type = AxisType.Linear;

            BarItem barItem = _graphPane.AddBar(seriesName, _ppl, Color.Blue);
            barItem.Bar.Fill.Type = FillType.Solid;
            barItem.Bar.Border.IsVisible = true;

            barItem = _graphPane.AddBar(seriesName2, _ppl2, Color.LightBlue);
            barItem.Bar.Fill.Type = FillType.Solid;
            barItem.Bar.Border.IsVisible = true;
        }

        #endregion

        #region Public Methods

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
        /// Clear the time series data.
        /// </summary>
        public override void Clear()
        {
            _ppl.Clear();
            _ppl2.Clear();
            RefreshGraph();
        }

        #endregion
    }
}
