// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using ZedGraph;

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// Form for displaying a live graph.
/// </summary>
public partial class GraphForm : Form
{
    readonly protected GraphPane _graphPane;

    #region Constructor

    /// <summary>
    /// Construct with the given titles.
    /// </summary>
    /// <param name="title">Graph title.</param>
    /// <param name="xAxisTitle">X-axis title.</param>
    /// <param name="y1AxisTitle">Y-axis title.</param>
    /// <param name="y2AxisTitle">Y2-axis title (optional).</param>
    public GraphForm(
        string title,
        string xAxisTitle,
        string y1AxisTitle,
        string y2AxisTitle)
    {
        InitializeComponent();

        this.Text = $"SharpNEAT - {title}";

        _graphPane = zed.GraphPane;
        _graphPane.Title.Text = title;

        _graphPane.XAxis.Title.Text = xAxisTitle;
        _graphPane.XAxis.MajorGrid.IsVisible = true;

        _graphPane.YAxis.Title.Text = y1AxisTitle;
        _graphPane.YAxis.MajorGrid.IsVisible = true;

        if(y2AxisTitle is not null)
        {
            _graphPane.Y2Axis.Title.Text = y2AxisTitle;
            _graphPane.Y2Axis.MajorGrid.IsVisible = false;
            _graphPane.Y2Axis.IsVisible = true;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clear the graph data.
    /// </summary>
    public virtual void Clear()
    {
        // Note. This method could be defined as abstract, but that would prevent the Window Forms UI designer from working;
        // therefore instead it is defined as virtual with no implementation.
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Recalc the axis scales based on the current data, and call Refresh() to redraw the graph.
    /// </summary>
    protected void RefreshGraph()
    {
        _graphPane.AxisChange();
        zed.Refresh();
    }

    #endregion
}
