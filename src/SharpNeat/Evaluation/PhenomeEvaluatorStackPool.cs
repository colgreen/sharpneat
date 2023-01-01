// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using Redzen.Collections;

namespace SharpNeat.Evaluation;

/// <summary>
/// A pool of phenome evaluators, in which the pool is implemented with a stack structure with thread synchronised access to the stack.
/// </summary>
/// <typeparam name="TPhenome">Phenome type.</typeparam>
public sealed class PhenomeEvaluatorStackPool<TPhenome> : IPhenomeEvaluatorPool<TPhenome>
{
    readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
    readonly LightweightStack<IPhenomeEvaluator<TPhenome>> _evaluatorStack;

    // Note. Do not make this readonly; it's a mutable struct.
    SpinLock _spinLock;

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
    /// <param name="initialPoolSize">Initial pool size.</param>
    public PhenomeEvaluatorStackPool(
        IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
        int initialPoolSize)
    {
        if(!phenomeEvaluationScheme.EvaluatorsHaveState)
            throw new InvalidOperationException("A stateless evaluation scheme does not require an evaluator pool; just use a single evaluator instance concurrently.");

        _phenomeEvaluationScheme = phenomeEvaluationScheme;

        // Create the stack with the spare capacity; to avoid re-alloc overhead if we require additional capacity.
        _evaluatorStack = new LightweightStack<IPhenomeEvaluator<TPhenome>>(initialPoolSize * 2);

        // Pre-populate with evaluators.
        for(int i=0; i < initialPoolSize; i++)
            _evaluatorStack.Push(phenomeEvaluationScheme.CreateEvaluator());

        // Enable thread tracking only if the debugger is attached; it adds non-trivial overhead to Enter/Exit.
        _spinLock = new SpinLock(Debugger.IsAttached);
    }

    #endregion

    #region IPhenomeEvaluatorPool

    /// <summary>
    /// Get an evaluator from the pool.
    /// </summary>
    /// <returns>An evaluator instance.</returns>
    public IPhenomeEvaluator<TPhenome> GetEvaluator()
    {
        bool lockTaken = false;
        try
        {
            // Wait for the sync lock.
            _spinLock.Enter(ref lockTaken);

            // Take an evaluator from the pool (if any available).
            if(_evaluatorStack.Count > 0)
                return _evaluatorStack.Pop();
        }
        finally
        {
            // Release the sync lock.
            if(lockTaken) _spinLock.Exit(false);
        }

        // If the pool is empty then create a new evaluator instance; this should get released
        // back into the pool, thus increasing the total number of evaluators being managed by the pool.
        return _phenomeEvaluationScheme.CreateEvaluator();
    }

    /// <summary>
    /// Releases an evaluator back into the pool.
    /// </summary>
    /// <param name="evaluator">The evaluator to release.</param>
    public void ReleaseEvaluator(IPhenomeEvaluator<TPhenome> evaluator)
    {
        bool lockTaken = false;
        try
        {
            // Wait for the sync lock.
            _spinLock.Enter(ref lockTaken);

            // Put the evaluator into the pool.
            _evaluatorStack.Push(evaluator);
        }
        finally
        {
            // Release the sync lock.
            if(lockTaken) _spinLock.Exit(false);
        }
    }

    #endregion
}
