
namespace SharpNeat.Network
{
    public interface IWeightedDirectedConnection<T> : IDirectedConnection
        where T : struct
    {
        /// <summary>
        /// Connection weight
        /// </summary>
        T Weight { get; }
    }
}
