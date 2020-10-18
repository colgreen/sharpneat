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
using ZedGraph;

namespace SharpNeat.Windows.App.Forms
{
    public class FitnessTimeSeriesForm : TimeSeriesForm
    {
        const int __HistoryLength = 1_000;

        RollingPointPairList _bestPpl;
        RollingPointPairList _meanPpl;

        public FitnessTimeSeriesForm()
            : base("Fitness (Best and Mean)", "Generation", "Fitness", null)
        {
            _bestPpl = new RollingPointPairList(__HistoryLength);
            _graphPane.AddCurve("Best",  _bestPpl, Color.Red, SymbolType.None);

            _meanPpl = new RollingPointPairList(__HistoryLength);
            LineItem lineItem = _graphPane.AddCurve("Mean",  _meanPpl, Color.Black, SymbolType.None);
            lineItem.IsY2Axis = true;
        }
    }
}
