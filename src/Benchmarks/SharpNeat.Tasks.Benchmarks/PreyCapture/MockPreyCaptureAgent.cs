namespace SharpNeat.Tasks.Benchmarks.PreyCapture;

internal sealed class MockPreyCaptureAgent : IBlackBox<double>
{
    readonly double[] _outputArr;
    int _state;

    #region Constructor

    public MockPreyCaptureAgent()
    {
        Inputs = new double[14];
        _outputArr = new double[4];
        Outputs = _outputArr;
    }

    #endregion

    #region IBlackBox

    public Memory<double> Inputs { get; }

    public Memory<double> Outputs { get; }

    public void Activate()
    {
        // Make the agent run around in a tight circle.
        _outputArr[_state] = 1.0;
        if(++_state == 4)
        {
            _state = 0;
        }
    }

    public void Reset()
    {
        Array.Clear(_outputArr, 0, _outputArr.Length);
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases both managed and unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion
}
