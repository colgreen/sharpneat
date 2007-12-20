using System;
using System.Collections;

namespace SharpNeatLib.NeuralNetwork
{

	public class ConnectionList : CollectionBase
	{
		public Connection this[int index]
		{
			get
			{
				return ((Connection)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(Connection connection)
		{
			return (List.Add(connection));
		}

	}
}
