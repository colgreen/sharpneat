/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    internal partial class ConnectionGeneListBuilder<T>
        where T : struct
    {
        #region Instance Fields

        // Indicates that we are building acyclic networks.
        readonly bool _isAcyclic;
        readonly CyclicConnectionTest? _cyclicTest;

        // Connection gene lists.
        readonly List<DirectedConnection> _connList;
        readonly List<T> _weightList;
        
        #endregion

        #region Constructor
        
        public ConnectionGeneListBuilder(bool isAcyclic, int capacity)
        {
            _isAcyclic = isAcyclic;
            if(_isAcyclic) {
                _cyclicTest = new CyclicConnectionTest();
            }

            _connList = new List<DirectedConnection>(capacity);
            _weightList = new List<T>(capacity);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a Gene to the builder, but only if the connection is not already present (as determined by its source and target ID endpoints).
        /// </summary>
        /// <param name="gene">The connection gene to add.</param>
        public void TryAddGene(in ConnectionGene<T> gene)
        {
            // For acyclic networks, check if the connection gene would create a cycle in the new genome; if so then reject it.
            // Note. A cyclicity test is expensive, therefore we avoid it if at all possible.
            if(_isAcyclic && IsCyclicConnection(in gene)) {
                return;
            }

            // We are free to add the gene.
            AddGene(in gene);
        }

        public ConnectionGenes<T> ToConnectionGenes()
        {
            return new ConnectionGenes<T>(
                _connList.ToArray(),
                _weightList.ToArray());
        }

        public void Clear()
        {
            _connList.Clear();
            _weightList.Clear();
        }

        #endregion

        #region Private Methods

        private void AddGene(in ConnectionGene<T> gene)
        {
            _connList.Add(gene.Endpoints);
            _weightList.Add(gene.Weight);
        }

        private bool IsCyclicConnection(in ConnectionGene<T> gene)
        {
            return _cyclicTest!.IsConnectionCyclic(_connList, in gene.Endpoints);
        }

        #endregion
    }
}
