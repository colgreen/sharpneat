using System.Drawing;

namespace SharpNeat.Windows.App
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

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value that indicates the number of historic data points to plot at most (the time series plots are 'rolling' plots to prevent memory use increasing indefinitely).
        /// </summary>
        public int HistoryLength { get; }

        /// <summary>
        /// Gets a value that indicates which Y axis the data source should be plotted against.
        /// </summary>
        public int YAxis { get; }

        /// <summary>
        /// Gets a value that indicates the color that the data should plotted with.
        /// </summary>
        public Color Color { get; }

        readonly GetPointDelegate _getPointDelegate;

        #region Constructor

        /// <summary>
        /// Constructs a data source with the provided source details and delegate for acquiring data.
        /// </summary>
        public TimeSeriesDataSource(string name, int historyLength, int yAxis, Color color, GetPointDelegate getPointDelegate)
        {
            Name = name;
            HistoryLength = historyLength;
            YAxis = yAxis;
            Color = color;
            _getPointDelegate = getPointDelegate;
        }

        #endregion

        #region Public Methods

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
