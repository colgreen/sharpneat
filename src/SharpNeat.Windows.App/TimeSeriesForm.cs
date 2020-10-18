using System.Windows.Forms;
using ZedGraph;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// Form for displaying a graph plot of time series data (e.g. best genome fitness per generation).
    /// </summary>
    public partial class TimeSeriesForm : Form
    {
        readonly GraphPane _graphPane;

        #region Constructor

        /// <summary>
        /// Construct the form with the provided details and data sources.
        /// </summary>
        public TimeSeriesForm(
            string title,
            string xAxisTitle,
            string y1AxisTitle,
            string y2AxisTitle)
        {
            InitializeComponent();

            this.Text = $"SharpNEAT Chart - {title}";

            _graphPane = zed.GraphPane;
            _graphPane.Title.Text = title;

			_graphPane.XAxis.Title.Text = xAxisTitle;
			_graphPane.XAxis.MajorGrid.IsVisible = true;

			_graphPane.YAxis.Title.Text = y1AxisTitle;
			_graphPane.YAxis.MajorGrid.IsVisible = true;

            if(y2AxisTitle is object)
            {
                _graphPane.Y2Axis.Title.Text = y2AxisTitle;
			    _graphPane.Y2Axis.MajorGrid.IsVisible = false;
                _graphPane.Y2Axis.IsVisible = true;
            }
        }

        #endregion

        #region Public Methods

        public void AddDataPoint()
        {
        }     

        #endregion
    }
}
