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
/// Represents an evaluator of <typeparamref name="TPhenome"/> instances.
/// </summary>
/// <typeparam name="TPhenome">Phenome input/output signal data type.</typeparam>
public interface IPhenomeEvaluator<TPhenome>
{
    /// <summary>
    /// Evaluate a single phenome and return its fitness score or scores.
    /// </summary>
    /// <param name="phenome">The phenome to evaluate.</param>
    /// <returns>An instance of <see cref="FitnessInfo"/> that conveys the phenome's fitness scores/data.</returns>
    FitnessInfo Evaluate(TPhenome phenome);
}
