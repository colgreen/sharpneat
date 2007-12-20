using System;
using System.Collections;

namespace SharpNeatLib.NetworkVisualization
{
	public class ModelConnectionList : CollectionBase
	{
		public ModelConnection this[int index]
		{
			get
			{
				return ((ModelConnection)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(ModelConnection connection)
		{
			return (List.Add(connection));
		}
	}
}
