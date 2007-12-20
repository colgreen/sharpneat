using System;

namespace SharpNeatLib.Evolution
{
	public interface IIdGeneratorFactory
	{
		/// <summary>
		/// Create an IdGenerator based upon the IDs within the provided population.
		/// </summary>
		/// <param name="pop"></param>
		/// <returns></returns>
		IdGenerator CreateIdGenerator(GenomeList genomeList);
	}
}
