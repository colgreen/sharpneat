
namespace SharpNeat.Core
{
    /// <summary>
    /// Generic interface for classes that decode genomes into phenomes.
    /// </summary>
    /// <typeparam name="TGenome">The genome type to be decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to.</typeparam>
    public interface IGenomeDecoder<TGenome,TPhenome>
    {
        /// <summary>
        /// Decodes a genome into a phenome. Note that not all genomes have to decode successfully. That is, we 
        /// support genetic representations that may produce non-viable offspring. In such cases this method
        /// can return a null.
        /// </summary>
        TPhenome Decode(TGenome genome);
    }
}
