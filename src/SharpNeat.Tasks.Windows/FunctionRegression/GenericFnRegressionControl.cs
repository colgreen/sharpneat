using System.Numerics;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.GenerativeFunctionRegression;

namespace SharpNeat.Tasks.Windows.FunctionRegression;

// Note. The Visual Studio UI designer/editor doesn't work with generic types, hence all generic code has been 'lifted' into this generic subclass.
public class GenericFnRegressionControl<TScalar> : FnRegressionControl
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly Func<TScalar,TScalar> _fn;
    readonly IBlackBoxProbe<TScalar> _blackBoxProbe;
    readonly TScalar[] _yArrTarget;
    readonly IGenomeDecoder<NeatGenome<TScalar>,IBlackBox<TScalar>> _genomeDecoder;

    public GenericFnRegressionControl(
        Func<TScalar, TScalar> fn,
        ParamSamplingInfo<TScalar> paramSamplingInfo,
        bool generativeMode,
        IGenomeDecoder<NeatGenome<TScalar>, IBlackBox<TScalar>> genomeDecoder)
        : base()
    {
        _fn = fn;
        _genomeDecoder = genomeDecoder;

        // Determine the mid output value of the function (over the specified sample points) and a scaling factor
        // to apply the to neural network response for it to be able to recreate the function (because the neural net
        // output range is typically in the interval [0,1], e.g. when using the logistic function activation function).
        FuncRegressionUtils<TScalar>.CalcFunctionMidAndScale(
            fn, paramSamplingInfo,
            out TScalar mid,
            out TScalar scale);

        if(generativeMode)
        {
            _blackBoxProbe = new GenerativeBlackBoxProbe<TScalar>(
                paramSamplingInfo.SampleResolution,
                mid, scale);
        }
        else
        {
            _blackBoxProbe = new BlackBoxProbe<TScalar>(
                paramSamplingInfo, mid, scale);
        }

        _yArrTarget = new TScalar[paramSamplingInfo.SampleResolution];

        // Pre-populate plot point objects.
        TScalar[] xArr = paramSamplingInfo.XArr;
        for(int i = 0; i<xArr.Length; i++)
        {
            TScalar x = xArr[i];
            _pplTarget.Add(double.CreateChecked(x), double.CreateChecked(_fn(x)));
            _pplResponse.Add(double.CreateChecked(x), 0.0);
        }
    }

    public override void OnGenomeUpdated()
    {
        base.OnGenomeUpdated();

        if(_genome is not NeatGenome<TScalar> neatGenome)
            return;

        // Decode genome.
        IBlackBox<TScalar> box = _genomeDecoder.Decode(neatGenome);

        // Probe the black box.
        _blackBoxProbe.Probe(box, _yArrTarget);

        // Update plot points.
        for(int i = 0; i < _yArrTarget.Length; i++)
            _pplResponse[i].Y = double.CreateChecked(_yArrTarget[i]);

        // Redraw graph.
        Zed.AxisChange();
        Refresh();
    }
}
