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
using System.Drawing;
using SharpNeat.EvolutionAlgorithm;
using ZedGraph;

namespace SharpNeat.Windows.App.Forms
{
    public class EvalsPerSecTimeSeriesForm : TimeSeriesForm
    {
        const int __HistoryLength = 1_000;
        readonly RollingPointPairList _ppl;

        #region Constructor

        public EvalsPerSecTimeSeriesForm()
            : base("Evaluations per second", "Generation", "Evaluations", null)
        {
            _ppl = new RollingPointPairList(__HistoryLength);
            _graphPane.AddCurve("Evaluations",  _ppl, Color.Red, SymbolType.None);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the time series data.
        /// </summary>
        /// <param name="eaStats">Evolution algorithm statistics object.</param>
        /// <param name="popStats">Population statistics object.</param>
        public override void UpdateData(
            EvolutionAlgorithmStatistics eaStats,
            PopulationStatistics popStats)
        {
            _ppl.Add(eaStats.Generation, eaStats.EvaluationsPerSec);
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
