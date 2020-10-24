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

namespace SharpNeat.Windows.App.Forms.Rankings
{
    public class GenomeFitnessRankingForm : GraphForm
    {
        readonly PointPairList _ppl;

        #region Constructor

        public GenomeFitnessRankingForm()
            : base("Genome Fitness by Rank", "Rank", "Fitness", null)
        {
            _ppl = new PointPairList();
            _graphPane.XAxis.Type = AxisType.Linear;
            _graphPane.BarSettings.ClusterScaleWidth = 2f;

            BarItem barItem = _graphPane.AddBar("Genome Fitness by Rank", _ppl, Color.Orange);

            barItem.Bar.Fill.Type = FillType.Solid;
            barItem.Bar.Border.IsVisible = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the time series data.
        /// </summary>
        public void UpdateData(Span<double> genomeFitnessByRank)
        {
            _ppl.Clear();

            for(int i=0; i < genomeFitnessByRank.Length; i++) {
                _ppl.Add(i + 1, genomeFitnessByRank[i]);
            }

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
}
