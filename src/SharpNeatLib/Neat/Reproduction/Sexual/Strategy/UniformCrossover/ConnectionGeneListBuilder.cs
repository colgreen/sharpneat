using System.Collections.Generic;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    internal partial class ConnectionGeneListBuilder<T>
        where T : struct
    {
        #region Instance Fields

        // Indicates that we are building acyclic networks.
        readonly bool _isAcyclic;
        readonly CyclicConnectionTestWithIds _cyclicTest;

        // Connection gene lists.
        List<DirectedConnection> _connList;
        List<T> _weightList;
        
        #endregion

        #region Constructor
        
        public ConnectionGeneListBuilder(bool isAcyclic, int capacity)
        {
            _isAcyclic = isAcyclic;
            if(_isAcyclic) {
                _cyclicTest = new CyclicConnectionTestWithIds();
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
        /// <param name="isSecondaryGene">Indicates if the gene is from the secondary parent.</param>
        public void TryAddGene(ConnectionGene<T> gene, bool isSecondaryGene)
        {
            // Test for acyclic networks only...
            // Check if the connection gene would create a cycle in the new genome; if so then reject it.
            // Notes. 
            // Only genes from the secondary parent can create a cycle; the genes in both parents describe
            // an acyclic graph, thus the only means of creating a cycle here is when we add a connection to
            // a parent (from the other parent) that it previously did not have.
            if(isSecondaryGene && _isAcyclic && IsCyclicConnection(gene)) {
                return;
            }

            // We are free to add the gene.
            AddGene(gene);
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

        private void AddGene(ConnectionGene<T> gene)
        {
            _connList.Add(gene.Endpoints);
            _weightList.Add(gene.Weight);
        }

        private bool IsCyclicConnection(ConnectionGene<T> gene)
        {
            return _cyclicTest.IsConnectionCyclic(_connList, gene.Endpoints);
        }

        #endregion
    }
}
