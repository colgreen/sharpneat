
using System;

namespace SharpNeat.Utils
{
    /// <summary>
    /// Conveniently encapsulates a single Int32, which is incremented to produce new IDs.
    /// </summary>
    public class Int32Sequence
    {
        int _next;

        #region Constructors

        /// <summary>
        /// Construct, setting the initial ID to zero.
        /// </summary>
        public Int32Sequence()
        {
            _next = 0;
        }

        /// <summary>
        /// Construct, setting the initial ID to the value provided.
        /// </summary>
        public Int32Sequence(int nextId)
        {
            _next = nextId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the next ID without incrementing (peek the ID).
        /// </summary>
        public int Peek
        {
            get { return _next; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the next ID. IDs wrap around to zero when int.MaxValue is reached. 
        /// </summary>
        public int Next()
        {
            if (_next == int.MaxValue) {   
                throw new InvalidOperationException("Last ID has been reached.");
            }
            return _next++;   
        }

        /// <summary>
        /// Resets the next ID back to zero.
        /// </summary>
        public void Reset()
        {
            _next = 0;
        }

        /// <summary>
        /// Resets the next ID to a specific value.
        /// </summary>
        public void Reset(int nextId)
        {
            _next = nextId;
        }

        #endregion
    }
}
