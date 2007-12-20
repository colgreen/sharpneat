using System;

namespace SharpNeatLib.Evolution
{
	public class IdGenerator
	{
		uint nextGenomeId;
		uint nextInnovationId;		
		
		#region Constructors

		public IdGenerator()
		{
			this.nextGenomeId = 0;
			this.nextInnovationId = 0;
		}

		public IdGenerator(uint nextGenomeId, uint nextInnovationId)
		{
			this.nextGenomeId = nextGenomeId;
			this.nextInnovationId = nextInnovationId;
		}

		#endregion

		#region Properties

		public uint NextGenomeId
		{
			get
			{
				if(nextGenomeId==uint.MaxValue)
					nextGenomeId=0;
				return nextGenomeId++;
			}
		}

		public uint NextInnovationId
		{
			get
			{
				if(nextInnovationId==uint.MaxValue)
					nextInnovationId=0;
				return nextInnovationId++;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Used primarilty by the GenomeFactory so that the same innovation ID's are used for input & output nodes
		/// for all of the initial population.
		/// </summary>
		public void ResetNextInnovationNumber()
		{
			nextInnovationId=0;
		}

		#endregion
	}
}
