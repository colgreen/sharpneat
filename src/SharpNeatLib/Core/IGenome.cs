
namespace SharpNeat.Core
{
    public interface IGenome
    {
        /// <summary>
        /// Gets the genome's unique ID. IDs are unique across all genomes created from a single 
        /// IGenomeFactory.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The generation that a genome was born/created in. Used to track genome age.
        /// </summary>
        int BirthGeneration { get; }

        /// <summary>
        /// The genome's fitness score.
        /// </summary>
        double Fitness { get; set; }
    }
}
