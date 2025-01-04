namespace SharpNeat.Tasks.PreyCapture;

internal sealed class MockPreyCaptureAgent : IBlackBox<float>
{
    readonly float[] _outputArr;
    int _state;

    #region Constructor

    public MockPreyCaptureAgent()
    {
        Inputs = new float[14];
        _outputArr = new float[4];
        Outputs = _outputArr;
    }

    #endregion

    #region IBlackBox

    public Memory<float> Inputs { get; }

    public Memory<float> Outputs { get; }

    public void Activate()
    {
        // Make the agent run around in a tight circle.
        _outputArr[_state] = 1f;
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
