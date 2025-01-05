// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Drawing;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.GenerativeFunctionRegression;
using SharpNeat.Windows;
using ZedGraph;

namespace SharpNeat.Tasks.Windows.FunctionRegression;

/// <summary>
/// Task View for function regression tasks.
/// </summary>
public partial class FnRegressionControl : GenomeControl
{
    readonly Func<float,float> _fn;
    readonly IBlackBoxProbe<float> _blackBoxProbe;
    readonly float[] _yArrTarget;
    readonly IGenomeDecoder<NeatGenome<float>,IBlackBox<float>> _genomeDecoder;
    readonly PointPairList _pplTarget;
    readonly PointPairList _pplResponse;

    /// <summary>
    /// Constructs with the details of the function regression problem to be visualized.
    /// </summary>
    /// <param name="fn">The function being regressed.</param>
    /// <param name="generativeMode">Indicates that blackbox has no inputs; it will generate a waveform as a function of time.</param>
    /// <param name="paramSamplingInfo">Parameter sampling info.</param>
    /// <param name="genomeDecoder">Genome decoder.</param>
    public FnRegressionControl(
        Func<float, float> fn,
        ParamSamplingInfo<float> paramSamplingInfo,
        bool generativeMode,
        IGenomeDecoder<NeatGenome<float>,IBlackBox<float>> genomeDecoder)
    {
        InitializeComponent();
        InitGraph(string.Empty, string.Empty, string.Empty);

        _fn = fn;
        _genomeDecoder = genomeDecoder;

        // Determine the mid output value of the function (over the specified sample points) and a scaling factor
        // to apply the to neural network response for it to be able to recreate the function (because the neural net
        // output range is typically in the interval [0,1], e.g. when using the logistic function activation function).
        FuncRegressionUtils<float>.CalcFunctionMidAndScale(
            fn, paramSamplingInfo,
            out float mid,
            out float scale);

        if(generativeMode)
        {
            _blackBoxProbe = new GenerativeBlackBoxProbe<float>(
                paramSamplingInfo.SampleResolution,
                mid, scale);
        }
        else
        {
            _blackBoxProbe = new BlackBoxProbe<float>(
                paramSamplingInfo, mid, scale);
        }

        _yArrTarget = new float[paramSamplingInfo.SampleResolution];

        // Pre-build plot point objects.
        _pplTarget = [];
        _pplResponse = [];

        float[] xArr = paramSamplingInfo.XArr;
        for(int i=0; i<xArr.Length; i++)
        {
            float x = xArr[i];
            _pplTarget.Add(x, _fn(x));
            _pplResponse.Add(x, 0.0);
        }

        // Bind plot points to graph.
        zed.GraphPane.AddCurve("Target", _pplTarget, Color.Black, SymbolType.None);
        zed.GraphPane.AddCurve("Network Response", _pplResponse, Color.Red, SymbolType.None);
    }

    public override void OnGenomeUpdated()
    {
        base.OnGenomeUpdated();

        if(_genome is not NeatGenome<float> neatGenome)
            return;

        // Decode genome.
        IBlackBox<float> box = _genomeDecoder.Decode(neatGenome);

        // Probe the black box.
        _blackBoxProbe.Probe(box, _yArrTarget);

        // Update plot points.
        for(int i=0; i < _yArrTarget.Length; i++)
            _pplResponse[i].Y = _yArrTarget[i];

        // Redraw graph.
        zed.AxisChange();
        Refresh();
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
