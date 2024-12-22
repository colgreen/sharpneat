// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// Connections genes represented as a structure of arrays (see https://en.wikipedia.org/wiki/AOS_and_SOA).
/// Element i of each array represents a value relating to the 'i'th gene.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public sealed class ConnectionGenes<TWeight>
    where TWeight : unmanaged
{
    /// <summary>
    /// Array of directed connections; this describes the network structure.
    /// </summary>
    public readonly DirectedConnection[] _connArr;
    /// <summary>
    /// Array of connection weights.
    /// </summary>
    public readonly TWeight[] _weightArr;

    #region Constructors

    /// <summary>
    /// Construct uninitialised arrays with the given length.
    /// </summary>
    /// <param name="length">Number of connection genes to allocate space for.</param>
    public ConnectionGenes(int length)
    {
        _connArr = new DirectedConnection[length];
        _weightArr = new TWeight[length];
    }

    /// <summary>
    /// Construct with the pre-built arrays.
    /// </summary>
    /// <param name="connArr">Array of directed connections; this described the network structure.</param>
    /// <param name="weightArr">Array of connection weights.</param>
    public ConnectionGenes(
        DirectedConnection[] connArr,
        TWeight[] weightArr)
    {
        Debug.Assert(connArr is not null);
        Debug.Assert(weightArr is not null);
        Debug.Assert(connArr.Length == weightArr.Length);

        _connArr = connArr;
        _weightArr = weightArr;
    }

    #endregion

    #region Properties / Indexer

    /// <summary>
    /// Gets the number of connection genes.
    /// </summary>
    public int Length => _connArr.Length;

    /// <summary>
    /// Connection gene indexer.
    /// </summary>
    /// <param name="idx">Index of the gene to get or set.</param>
    public (int srcIdx, int tgtIdx, TWeight weight) this[int idx]
    {
        get => (_connArr[idx].SourceId, _connArr[idx].TargetId, _weightArr[idx]);

        set
        {
            _connArr[idx] = new DirectedConnection(value.srcIdx, value.tgtIdx);
            _weightArr[idx] = value.weight;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sort the connection genes by sourceID then targetID.
    /// </summary>
    public void Sort()
    {
        Array.Sort(_connArr, _weightArr);
    }

    #endregion
}
