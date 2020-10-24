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
    public class FitnessTimeSeriesForm : TimeSeriesForm
    {
        const int __HistoryLength = 1_000;
        readonly RollingPointPairList _bestPpl;
        readonly RollingPointPairList _meanPpl;

        #region Constructor

        public FitnessTimeSeriesForm()
            : base("Fitness (Best and Mean)", "Generation", "Fitness", null)
        {
            _bestPpl = new RollingPointPairList(__HistoryLength);
            _graphPane.AddCurve("Best",  _bestPpl, Color.Red, SymbolType.None);

            _meanPpl = new RollingPointPairList(__HistoryLength);
            LineItem lineItem = _graphPane.AddCurve("Mean",  _meanPpl, Color.Black, SymbolType.None);
            lineItem.IsY2Axis = true;
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
            _bestPpl.Add(eaStats.Generation, popStats.BestFitness.PrimaryFitness);
            _meanPpl.Add(eaStats.Generation, popStats.MeanFitness);
            RefreshGraph();
        }

        /// <summary>
        /// Clear the time series data.
        /// </summary>
        public override void Clear()
        {
            _bestPpl.Clear();
            _meanPpl.Clear();
            RefreshGraph();
        }

        #endregion
    }
}
