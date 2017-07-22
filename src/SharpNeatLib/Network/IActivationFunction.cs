
namespace SharpNeat.Network
{
    /// <summary>
    /// Vectorized activation function.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this array/vector.</param>
    public delegate void VecFn<T>(T[] v) 
        where T : struct;

    /// <summary>
    /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.
    /// The resulting post-activation levels are written back to this array/vector.</param>
    /// <param name="startIdx">Start index.</param>
    /// <param name="endIdx">End index (exclusive).</param>
    public delegate void VecFnSegment<T>(double[] v, int startIdx, int endIdx) 
        where T : struct;

    /// <summary>
    /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector, 
    /// and post-activation levels stored in a separate supplied vector.
    /// </summary>
    /// <param name="v">A vector of pre-activation levels to pass through the function.
    /// <param name="w">A vector in which the post activation levels are stored.</param>
    /// The resulting post-activation levels are written back to this array/vector.</param>
    /// <param name="startIdx">Start index.</param>
    /// <param name="endIdx">End index (exclusive).</param>
    public delegate void VecFnSegment2<T>(double[] v, double[] w, int startIdx, int endIdx) 
        where T : struct;

    /// <summary>
    /// Neural net node activation function.
    /// </summary>
    public interface IActivationFunction<T> where T : struct
    {
        /// <summary>
        /// A unique identifier string for the function.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The activation function.
        /// </summary>
        /// <param name="x">The single pre-activation level to pass through the function.</param>
        double Fn(T x);

        /// <summary>
        /// Vectorized activation function.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this array/vector.</param>
        void Fn(T[] v);

        /// <summary>
        /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.
        /// The resulting post-activation levels are written back to this array/vector.</param>
        /// <param name="startIdx">Start index.</param>
        /// <param name="endIdx">End index (exclusive).</param>
        void Fn(T[] v, int startIdx, int endIdx);

        /// <summary>
        /// Vectorized activation function with activity limited to a defined sub-range/segment of the vector, 
        /// and post-activation levels stored in a separate supplied vector.
        /// </summary>
        /// <param name="v">A vector of pre-activation levels to pass through the function.
        /// <param name="w">A vector in which the post activation levels are stored.</param>
        /// The resulting post-activation levels are written back to this array/vector.</param>
        /// <param name="startIdx">Start index.</param>
        /// <param name="endIdx">End index (exclusive).</param>
        void Fn(T[] v, T[] w, int startIdx, int endIdx);
    }
}
