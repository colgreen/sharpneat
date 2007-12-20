using System;
using System.Collections;

namespace SharpNeatLib.NeatGenome
{
	public class CorrelationResults
	{
		CorrelationStatistics correlationStatistics = new CorrelationStatistics();
		ArrayList correlationItemList = new ArrayList();	

		#region Properties

		public CorrelationStatistics CorrelationStatistics
		{
			get
			{
				return correlationStatistics;
			}
		}

		public ArrayList CorrelationItemList
		{
			get
			{
				return correlationItemList;
			}
		}

		#endregion

		public bool PerformIntegrityCheck()
		{
			long prevInnovationId=-1;

			foreach(CorrelationItem correlationItem in correlationItemList)
			{
				if(correlationItem.CorrelationItemType==CorrelationItemType.MatchedConnectionGenes)
				{
					if(correlationItem.ConnectionGene1==null || correlationItem.ConnectionGene2==null)
						return false;

					if(		(correlationItem.ConnectionGene1.InnovationId != correlationItem.ConnectionGene2.InnovationId)
						||	(correlationItem.ConnectionGene1.SourceNeuronId != correlationItem.ConnectionGene2.SourceNeuronId)
						||	(correlationItem.ConnectionGene1.TargetNeuronId != correlationItem.ConnectionGene2.TargetNeuronId))
						return false;

					// Innovation ID's should be in order and not duplicated.
					if(correlationItem.ConnectionGene1.InnovationId<=prevInnovationId)
						return false;

					prevInnovationId = correlationItem.ConnectionGene1.InnovationId;
				}
				else // Disjoint or excess gene.
				{
					if(		(correlationItem.ConnectionGene1==null && correlationItem.ConnectionGene2==null)
						||	(correlationItem.ConnectionGene1!=null && correlationItem.ConnectionGene2!=null))
					{	// Precisely one gene should be present.
						return false;
					}
					if(correlationItem.ConnectionGene1!=null)
					{
						if(correlationItem.ConnectionGene1.InnovationId<=prevInnovationId)
							return false;

						prevInnovationId = correlationItem.ConnectionGene1.InnovationId;
					}
					else // ConnectionGene2 is present.
					{
						if(correlationItem.ConnectionGene2.InnovationId<=prevInnovationId)
							return false;

						prevInnovationId = correlationItem.ConnectionGene2.InnovationId;
					}
				}
			}
			return true;
		}
	}
}
