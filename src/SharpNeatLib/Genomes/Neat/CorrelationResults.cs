/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// The results from comparing two NEAT genomes and correlating their connection genes.
    /// </summary>
    public class CorrelationResults
    {
        readonly CorrelationStatistics _correlationStatistics = new CorrelationStatistics();
        readonly List<CorrelationItem> _correlationItemList;

        #region Constructor

        /// <summary>
        /// Cosntructs with a specified initial correlation item list capacity.
        /// </summary>
        public CorrelationResults(int itemListCapacity)
        {
            _correlationItemList = new List<CorrelationItem>(itemListCapacity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the statistics for the genome comparison.
        /// </summary>
        public CorrelationStatistics CorrelationStatistics
        {
            get { return _correlationStatistics; }
        }

        /// <summary>
        /// Gets the list of correlation items from the genome comparison.
        /// </summary>
        public List<CorrelationItem> CorrelationItemList
        {
            get { return _correlationItemList; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs an integrity check on the correlation items.
        /// Returns true if the test is passed.
        /// </summary>
        public bool PerformIntegrityCheck()
        {
            long prevInnovationId = -1;

            foreach(CorrelationItem item in _correlationItemList)
            {
                if(item.CorrelationItemType==CorrelationItemType.Match)
                {
                    if(item.ConnectionGene1==null || item.ConnectionGene2==null) {
                        return false;
                    }

                    if((item.ConnectionGene1.InnovationId != item.ConnectionGene2.InnovationId)
                    || (item.ConnectionGene1.SourceNodeId != item.ConnectionGene2.SourceNodeId)
                    || (item.ConnectionGene1.TargetNodeId != item.ConnectionGene2.TargetNodeId)) {
                        return false;
                    }

                    // Innovation ID's should be in order and not duplicated.
                    if(item.ConnectionGene1.InnovationId <= prevInnovationId) {
                        return false;
                    }
                    prevInnovationId = item.ConnectionGene1.InnovationId;
                }
                else // Disjoint or excess gene.
                {
                    if((item.ConnectionGene1==null && item.ConnectionGene2==null)
                    || (item.ConnectionGene1!=null && item.ConnectionGene2!=null))
                    {   // Precisely one gene should be present.
                        return false;
                    }
                    if(item.ConnectionGene1 != null)
                    {
                        if(item.ConnectionGene1.InnovationId <= prevInnovationId) {
                            return false;
                        }
                        prevInnovationId = item.ConnectionGene1.InnovationId;
                    }
                    else // ConnectionGene2 is present.
                    {
                        if(item.ConnectionGene2.InnovationId <= prevInnovationId) {
                            return false;
                        }
                        prevInnovationId = item.ConnectionGene2.InnovationId;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
