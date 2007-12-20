using System;
using System.Collections;

namespace SharpNeatLib.Evolution
{
	public class ConnectionMutationParameterGroupList : CollectionBase
	{
		#region Constructors

		public ConnectionMutationParameterGroupList()
		{}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		public ConnectionMutationParameterGroupList(ConnectionMutationParameterGroupList copyFrom)
		{
			foreach(ConnectionMutationParameterGroup paramGroup in copyFrom)
				Add(new ConnectionMutationParameterGroup(paramGroup));
		}

		#endregion

		#region Indexer

		public ConnectionMutationParameterGroup this[int index]
		{
			get
			{
				return ((ConnectionMutationParameterGroup)InnerList[index]);
			}
			set
			{
				InnerList[index] = value;
			}
		}

		#endregion

		#region Public Methods

		public void Add(ConnectionMutationParameterGroup cmpg)
		{
			InnerList.Add(cmpg);
		}

		public void Remove(ConnectionMutationParameterGroup cmpg)
		{
			InnerList.Remove(cmpg);
		}

		#endregion
	}
}
