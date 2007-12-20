using System;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Used primarily as a key into a hashtable that uniquely identifies connections
	/// by their end points.
	/// </summary>
	struct ConnectionEndpointsStruct
	{
		public uint sourceNeuronId;
		public uint targetNeuronId;

		#region Constructor

		public ConnectionEndpointsStruct(uint sourceNeuronId, uint targetNeuronId)
		{
			this.sourceNeuronId = sourceNeuronId;
			this.targetNeuronId = targetNeuronId;
		}

		#endregion

		#region Public Overrides

		public override int GetHashCode() 
		{
			// Point uses x^y far a hash, but this is actually an extremely poor hash function
			// for a pair of coordinates. Here we swpa the low and high 16 bits of one of the 
			// Id's to generate a much better hash for our (and most other likely) circumstances.
			return (int)(sourceNeuronId ^ ((targetNeuronId>>16) + (targetNeuronId<<16)));
		}

		public override bool Equals(object obj)
		{
			if(obj==null)
				return false;

			if(obj.GetType() != typeof(ConnectionEndpointsStruct))
				return false;
			
			ConnectionEndpointsStruct ces = (ConnectionEndpointsStruct)obj;
			return (sourceNeuronId==ces.sourceNeuronId) && (targetNeuronId==ces.targetNeuronId);
		}

		#endregion
	}
}
