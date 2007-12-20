using System;
using System.Collections;

namespace SharpNeatLib
{
	public class UniqueCyclicBuffer : CyclicBuffer
	{
		Hashtable bufferTable;

		public UniqueCyclicBuffer(int size) : base(size)
		{
			bufferTable = new Hashtable(size);
		}

		public override void Enqueue(object value)
		{
			// Enforce uniquess of contained objects.
			if(bufferTable.Contains(value))
				return;

			base.Enqueue (value);
		}
	}
}
