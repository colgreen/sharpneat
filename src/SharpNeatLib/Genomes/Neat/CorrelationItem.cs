/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// A single comparison item resulting from the comparison of two genomes. If the CorrelationItemType
    /// is Match then both connection gene properties will be non-null, otherwise one of them will be null 
    /// and the other will hold a reference to a disjoint or excess connection gene.
    /// 
    /// Note. We generally only compare connection genes when comparing genomes. Connection genes along with
    /// their innovation IDs actually represent the complete network topology (and of course the connection weights).
    /// </summary>
    public class CorrelationItem
    {
        readonly CorrelationItemType _correlationItemType;
        readonly ConnectionGene _connectionGene1;
        readonly ConnectionGene _connectionGene2;

        #region Constructor

        /// <summary>
        /// Constructs a new CorrelationItem.
        /// </summary>
        public CorrelationItem(CorrelationItemType correlationItemType, ConnectionGene connectionGene1, ConnectionGene connectionGene2)
        {
            _correlationItemType = correlationItemType;
            _connectionGene1 = connectionGene1;
            _connectionGene2 = connectionGene2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CorrelationItemType.
        /// </summary>
        public CorrelationItemType CorrelationItemType
        {
            get { return _correlationItemType; }
        }

        /// <summary>
        /// Gets the corresponding connection gene from comparison genome 1.
        /// </summary>
        public ConnectionGene ConnectionGene1
        {
            get { return _connectionGene1; }
        }

        /// <summary>
        /// Gets the corresponding connection gene from comparison genome 2.
        /// </summary>
        public ConnectionGene ConnectionGene2
        {
            get { return _connectionGene2; }
        }

        #endregion
    }
}
