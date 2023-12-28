// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Recombination.Strategy.UniformCrossover;

/// <summary>
/// Helper class for building lists of connections.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
internal sealed class ConnectionGeneListBuilder<TWeight>
    where TWeight : struct
{
    // Indicates that we are building acyclic networks.
    readonly bool _isAcyclic;
    readonly CyclicConnectionCheck? _cyclicCheck;

    // Connection gene lists.
    readonly LightweightList<DirectedConnection> _connList;
    readonly LightweightList<TWeight> _weightList;

    /// <summary>
    /// Construct with the given acyclic flag and initial capacity.
    /// </summary>
    /// <param name="isAcyclic">Indicates whether we are building acyclic networks or not.</param>
    /// <param name="capacity">Initial capacity.</param>
    public ConnectionGeneListBuilder(bool isAcyclic, int capacity)
    {
        _isAcyclic = isAcyclic;
        if(_isAcyclic)
            _cyclicCheck = new CyclicConnectionCheck();

        _connList = new LightweightList<DirectedConnection>(capacity);
        _weightList = new LightweightList<TWeight>(capacity);
    }

    #region Public Methods

    /// <summary>
    /// Add a Gene to the builder, but only if the connection is not already present (as determined by its source and target ID endpoints).
    /// </summary>
    /// <param name="gene">The connection gene to add.</param>
    public void TryAddGene(in ConnectionGene<TWeight> gene)
    {
        // For acyclic networks, check if the connection gene would create a cycle in the new genome; if so then reject it.
        // Note. A cyclicity test is expensive, therefore we avoid it if at all possible.
        if(_isAcyclic && IsCyclicConnection(in gene))
            return;

        // We are free to add the gene.
        AddGene(in gene);
    }

    /// <summary>
    /// Create a new instance of <see cref="ConnectionGenes{T}"/> that contains a copy of the current connections.
    /// </summary>
    /// <returns>A new instance of <see cref="ConnectionGenes{T}"/>.</returns>
    public ConnectionGenes<TWeight> ToConnectionGenes()
    {
        return new ConnectionGenes<TWeight>(
            _connList.ToArray(),
            _weightList.ToArray());
    }

    /// <summary>
    /// Clear connections (if any).
    /// </summary>
    public void Clear()
    {
        _connList.Clear();
        _weightList.Clear();
    }

    #endregion

    #region Private Methods

    private void AddGene(in ConnectionGene<TWeight> gene)
    {
        _connList.Add(gene.Endpoints);
        _weightList.Add(gene.Weight);
    }

    private bool IsCyclicConnection(in ConnectionGene<TWeight> gene)
    {
        return _cyclicCheck!.IsConnectionCyclic(_connList.AsSpan(), in gene.Endpoints);
    }

    #endregion
}
