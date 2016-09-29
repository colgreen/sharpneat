
/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Drawing;
using SharpNeat.Utility;

namespace SharpNeatGUI
{
    /// <summary>
    /// Data source for the TimeSeriesGraphForm.
    /// </summary>
    public class TimeSeriesDataSource
    {
        /// <summary>
        /// Default number of historic data points to store for rolling time series plots.
        /// </summary>
        public const int DefaultHistoryLength = 1000;
        /// <summary>
        /// Delegate for obtaining the current plot point.
        /// </summary>
        public delegate Point2DDouble GetPointDelegate();

        string _name;
        int _historyLength;
        int _yAxis;
        Color _color;
        GetPointDelegate _getPointDelegate;

        #region Constructor

        /// <summary>
        /// Constructs a data source with the provided source details and delegate for acquiring data.
        /// </summary>
        public TimeSeriesDataSource(string name, int historyLength, int yAxis, Color color, GetPointDelegate getPointDelegate)
        {
            _name = name;
            _historyLength = historyLength;
            _yAxis = yAxis;
            _color = color;
            _getPointDelegate = getPointDelegate;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a value that indicates the number of historic data points to plot at most (the time series plots are 'rolling' plots to prevent memory use increasing indefinitely).
        /// </summary>
        public int HistoryLength
        {
            get { return _historyLength; }
        }

        /// <summary>
        /// Gets a value that indicates which Y axis the data source should be plotted against.
        /// </summary>
        public int YAxis
        {
            get { return _yAxis; }
        }

        /// <summary>
        /// Gets a value that indicates the color that the data should plotted with.
        /// </summary>
        public Color Color
        {
            get { return _color; }
        }

        /// <summary>
        /// Gets a new data point to be plotted.
        /// </summary>
        public Point2DDouble GetPoint()
        {
            return _getPointDelegate();
        }

        #endregion
    }
}
