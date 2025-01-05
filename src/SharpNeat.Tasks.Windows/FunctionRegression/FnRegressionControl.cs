// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;
using SharpNeat.Windows;
using ZedGraph;

namespace SharpNeat.Tasks.Windows.FunctionRegression;

/// <summary>
/// Task View for function regression tasks.
/// </summary>
public partial class FnRegressionControl : GenomeControl
{
    protected PointPairList _pplTarget;
    protected PointPairList _pplResponse;

    protected ZedGraphControl Zed { get => zed; }

    /// <summary>
    /// Constructs with the details of the function regression problem to be visualized.
    /// </summary>
    /// <param name="fn">The function being regressed.</param>
    /// <param name="generativeMode">Indicates that blackbox has no inputs; it will generate a waveform as a function of time.</param>
    /// <param name="paramSamplingInfo">Parameter sampling info.</param>
    /// <param name="genomeDecoder">Genome decoder.</param>
    public FnRegressionControl()
    {
        InitializeComponent();
        InitGraph(string.Empty, string.Empty, string.Empty);

        _pplTarget = [];
        _pplResponse = [];

        // Bind plot points to graph.
        zed.GraphPane.AddCurve("Target", _pplTarget, Color.Black, SymbolType.None);
        zed.GraphPane.AddCurve("Network Response", _pplResponse, Color.Red, SymbolType.None);
    }

    private void InitGraph(string title, string xAxisTitle, string yAxisTitle)
    {
        GraphPane graphPane = zed.GraphPane;
        graphPane.Title.Text = title;

        graphPane.XAxis.Title.Text = xAxisTitle;
        graphPane.XAxis.MajorGrid.IsVisible = true;

        graphPane.YAxis.Title.Text = yAxisTitle;
        graphPane.YAxis.MajorGrid.IsVisible = true;
    }
}
