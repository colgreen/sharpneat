
namespace SharpNeat.Utils
{
    /// <summary>
    /// Conveniently encapsulates a single UInt32, which is incremented to produce new IDs.
    /// </summary>
    public class UInt32Sequence
    {
        uint _next;

        #region Constructors

        /// <summary>
        /// Construct, setting the initial ID to zero.
        /// </summary>
        public UInt32Sequence()
        {
            _next = 0;
        }

        /// <summary>
        /// Construct, setting the initial ID to the value provided.
        /// </summary>
        public UInt32Sequence(uint nextId)
        {
            _next = nextId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the next ID without incrementing (peek the ID).
        /// </summary>
        public uint Peek
        {
            get { return _next; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the next ID. IDs wrap around to zero when uint.MaxValue is reached. 
        /// </summary>
        public uint Next()
        {
            if (_next == uint.MaxValue) {
                _next = 0;
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
        public void Reset(uint nextId)
        {
            _next = nextId;
        }

        #endregion
    }
}
