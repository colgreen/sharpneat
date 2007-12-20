using System;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Different systems of determining which connection weights will be selected
	/// for mutation.
	/// </summary>
	public enum ConnectionSelectionType
	{
		/// <summary>
		/// Select a proportion of the weights in a genome.
		/// </summary>
		Proportional,

		/// <summary>
		/// Select a fixed number of weights in a genome.
		/// </summary>
		FixedQuantity
	}
}
