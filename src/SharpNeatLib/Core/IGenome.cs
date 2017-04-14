
namespace SharpNeat.Core
{
    public interface IGenome
    {
        /// <summary>
        /// Gets the genome's unique ID. IDs are unique across all genomes created from a single 
        /// IGenomeFactory.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// The generation that a genome was born/created in. Used to track genome age.
        /// </summary>
        uint BirthGeneration { get; }

        /// <summary>
        /// An array of auxiliary objects associated with a genome.
        /// </summary>
        /// <remarks>Additional information attached to a genome by various strategy classes. 
        /// Each strategy must register with the genome factory to acquire a slot number,
        /// which is the index into this array in each genome that is allocated to that strategy.</remarks>
        object[] AuxObjects { get; }

        /// <summary>
        /// The genome's fitness information.
        /// </summary>
        FitnessInfo FitnessInfo { get; set; }
    }
}
