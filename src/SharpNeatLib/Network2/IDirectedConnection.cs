
namespace SharpNeat.Network2
{
    public interface IDirectedConnection
    {
        /// <summary>
        /// Connection source node ID.
        /// </summary>
        int SourceId { get; }
        /// <summary>
        /// Connection target node ID.
        /// </summary>
        int TargetId { get; }
    }
}
