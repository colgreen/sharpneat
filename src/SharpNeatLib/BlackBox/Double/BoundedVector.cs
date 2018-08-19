
namespace SharpNeat.BlackBox.Double
{
    public class BoundedVector : IVector<double>
    {
        IVector<double> _innerVec;

        #region Constructor

        public BoundedVector(IVector<double> innerVec)
        {
            _innerVec = innerVec;
        }

        #endregion

        #region Indexer / Properties
        
        /// <summary>
        /// Gets or sets the signal value at the specified index.
        /// </summary>
        public double this[int index] 
        { 
            get
            {
                // Apply bounds of [0,1].
                double y = _innerVec[index]; 
                if(y < 0.0) y = 0.0;
                else if(y > 1.0) y = 1.0;
                return y;
            }
            set => _innerVec[index] = value;
        }

        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        public int Length => _innerVec.Length;

        #endregion

        #region Methods

        /// <summary>
        /// Copies all elements from the current SignalArray to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        public void CopyTo(double[] targetArray, int targetIndex)
        {
            _innerVec.CopyTo(targetArray, targetIndex);
        }
        
        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int length)
        {
            _innerVec.CopyTo(targetArray, targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// starting from <paramref name="targetIndex"/> on the target array and <paramref name="sourceIndex"/>
        /// on the current source SignalArray.
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying begins.</param>
        /// <param name="sourceIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            _innerVec.CopyTo(targetArray, targetIndex, sourceIndex, length);
        }

        /// <summary>
        /// Copies all elements from the source array writing them into the current SignalArray starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex)
        {
            _innerVec.CopyFrom(sourceArray, targetIndex);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current SignalArray 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex, int length)
        {
            _innerVec.CopyFrom(sourceArray, targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from sourceIndex on sourceArray to the current
        /// SignalArray starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            _innerVec.CopyFrom(sourceArray, sourceIndex, targetIndex, length);
        }

        /// <summary>
        /// Reset all array elements to some default value (typically zero).
        /// </summary>
        public void Reset()
        {
            _innerVec.Reset();
        }

        #endregion
    }
}
