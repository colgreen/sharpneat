using System;
using SharpNeat.BlackBox;

namespace SharpNeat.Tasks.Tests.PreyCapture
{
    internal class MockPreyCaptureAgent : IBlackBox<double>
    {
        readonly double[] _inputArr;
        readonly double[] _outputArr;

        #region Constructor

        public MockPreyCaptureAgent()
        {
            _inputArr = new double[14];
            this.InputVector = new VectorSegment<double>(_inputArr, 0, 14);;
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
        }

        public void ResetState()
        {
            Array.Clear(_outputArr, 0, _outputArr.Length);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {}

        #endregion
    }
}
