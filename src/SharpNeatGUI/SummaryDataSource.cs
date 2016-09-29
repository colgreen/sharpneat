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
