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

namespace SharpNeat.EvolutionAlgorithm.Runner;

/// <summary>
/// Evolution algorithm update event modes.
/// </summary>
public enum UpdateMode
{
    /// <summary>
    /// Do not generate any update events.
    /// </summary>
    None,
    /// <summary>
    /// Generate an update event at regular time intervals.
    /// </summary>
    Timespan,
    /// <summary>
    /// Generate an update event at regular generation intervals. (Every N generations).
    /// </summary>
    Generational
}
