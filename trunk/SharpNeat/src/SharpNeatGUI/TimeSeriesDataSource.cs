using System.Drawing;
/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
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
