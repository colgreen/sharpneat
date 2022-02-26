/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Evaluation;

/// <summary>
/// A pool of phenome evaluators.
/// </summary>
/// <remarks>
/// Used when the phenome evaluators have state, which is expensive to allocate, therefore we wish to
/// maintain a pool or re-usable evaluators instead of constructing and discarding a new evaluator for
/// each evaluation (or more accurately, each Parallel.For partition).
/// </remarks>
/// <typeparam name="TPhenome">Phenome type.</typeparam>
public interface IPhenomeEvaluatorPool<TPhenome>
{
    /// <summary>
    /// Get an evaluator from the pool.
    /// </summary>
    /// <returns>An evaluator instance.</returns>
    IPhenomeEvaluator<TPhenome> GetEvaluator();

    /// <summary>
    /// Releases an evaluator back into the pool.
    /// </summary>
    /// <param name="evaluator">The evaluator to release.</param>
    void ReleaseEvaluator(IPhenomeEvaluator<TPhenome> evaluator);
}
