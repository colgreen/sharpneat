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
    /// Data source for the SummaryGraphForm.
    /// </summary>
    public class SummaryDataSource
    {
        /// <summary>Delegate for obtaining an array of plot points.</summary>
        public delegate Point2DDouble[] GetDataPointArrayDelegate();

        string _name;
        int _yAxis;
        Color _color;
        GetDataPointArrayDelegate _getPointArrayDelegate;

        #region Constructor

        /// <summary>
        /// Constructs a data source with the provided source details and delegate for acquiring data.
        /// </summary>
        public SummaryDataSource(string name, int yAxis, Color color, GetDataPointArrayDelegate getPointArrayDelegate)
        {
            _name = name;
            _yAxis = yAxis;
            _color = color;
            _getPointArrayDelegate = getPointArrayDelegate;
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
        /// Gets the data to be plotted.
        /// </summary>
        public Point2DDouble[] GetPointArray()
        {
            return _getPointArrayDelegate();
        }

        #endregion
    }
}
