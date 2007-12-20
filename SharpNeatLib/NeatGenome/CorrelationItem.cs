using System;

namespace SharpNeatLib.NeatGenome
{

	public class CorrelationItem
	{
		CorrelationItemType correlationItemType;
		ConnectionGene connectionGene1;
		ConnectionGene connectionGene2;

		#region Constructor

		public CorrelationItem(CorrelationItemType correlationItemType, ConnectionGene connectionGene1, ConnectionGene connectionGene2)
		{
			this.correlationItemType = correlationItemType;
			this.connectionGene1 = connectionGene1;
			this.connectionGene2 = connectionGene2;
		}

		#endregion

		#region Properties

		public CorrelationItemType CorrelationItemType
		{
			get
			{
				return correlationItemType;
			}
			set
			{
				correlationItemType = value;
			}
		}

		public ConnectionGene ConnectionGene1
		{
			get
			{
				return connectionGene1;
			}
			set
			{
				connectionGene1 = value;
			}
		}

		public ConnectionGene ConnectionGene2
		{
			get
			{
				return connectionGene2;
			}
			set
			{
				connectionGene2 = value;
			}
		}

		#endregion
	}
}
