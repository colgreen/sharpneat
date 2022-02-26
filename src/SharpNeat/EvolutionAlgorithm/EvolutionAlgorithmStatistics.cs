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
namespace SharpNeat.EvolutionAlgorithm;

/// <summary>
/// Conveys statistics related to an <see cref="IEvolutionAlgorithm"/>.
/// </summary>
public class EvolutionAlgorithmStatistics
{
    /// <summary>
    /// The current generation number.
    /// </summary>
    public int Generation { get; set; }

    /// <summary>
    /// Indicates whether some goal fitness has been achieved and that the evolutionary algorithm search should stop.
    /// This property's value can remain false to allow the algorithm to run indefinitely.
    /// </summary>
    public bool StopConditionSatisfied { get; set; }

    /// <summary>
    /// Running evaluation count total.
    /// </summary>
    public ulong TotalEvaluationCount { get; set; }

    /// <summary>
    /// Evaluations per second.
    /// </summary>
    /// <remarks>
    /// Based on the difference in <see cref="TotalEvaluationCount"/> and <see cref="SampleTime"/>,
    /// between the last two generations.
    /// </remarks>
    public double EvaluationsPerSec { get; set; }

    /// <summary>
    /// The point in clock time that the statistics were recorded.
    /// </summary>
    public DateTime SampleTime { get; set; }
}
