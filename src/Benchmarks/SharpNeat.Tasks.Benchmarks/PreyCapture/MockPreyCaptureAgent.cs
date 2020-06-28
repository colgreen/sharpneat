using System;
using SharpNeat.BlackBox;

namespace SharpNeatTasks.Benchmarks.PreyCapture
{
    sealed internal class MockPreyCaptureAgent : IBlackBox<double>
    {
        readonly double[] _outputArr;
        int _state;

        #region Constructor

        public MockPreyCaptureAgent()
        {
            this.InputVector = new VectorSegment<double>(new double[14], 0, 14);;
            _outputArr = new double[4];
            this.OutputVector = new VectorSegment<double>(_outputArr, 0, 4);
        }

        #endregion

        #region IBlackBox

        public int InputCount => 14;

        public int OutputCount => 4;

        public IVector<double> InputVector { get; }

        public IVector<double> OutputVector { get; }

        public void Activate()
        {
            // Make the agent run around in a tight circle.
            _outputArr[_state] = 1.0;
            if(++_state == 4)
            {
                _state = 0;
            }
        }

        public void ResetState()
        {
            Array.Clear(_outputArr, 0, _outputArr.Length);
        }

        #endregion
    }
}
