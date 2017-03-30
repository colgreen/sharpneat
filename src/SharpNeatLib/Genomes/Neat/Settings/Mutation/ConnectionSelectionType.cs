
namespace SharpNeat.Genomes.Neat.Settings.Mutation
{
    /// <summary>
    /// Connection selection scehemes.
    /// </summary>
    public enum ConnectionSelectionType
    {
        /// <summary>
        /// Select a proportion of all weight in a genome.
        /// </summary>
        Proportion,
        /// <summary>
        /// Select a fixed number of weights in a genome.
        /// </summary>
        Count
    }
}
