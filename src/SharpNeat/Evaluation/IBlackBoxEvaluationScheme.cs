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
using SharpNeat.BlackBox;

namespace SharpNeat.Evaluation;

/// <summary>
/// Black box evaluation scheme, i.e. an <see cref="IPhenomeEvaluationScheme{TPhenome}"/> in which the
/// phenome type is an <see cref="IBlackBox{T}"/>.
/// </summary>
/// <typeparam name="T">Black box numeric data type.</typeparam>
public interface IBlackBoxEvaluationScheme<T> : IPhenomeEvaluationScheme<IBlackBox<T>>
    where T : struct
{
    /// <summary>
    /// The number of black box inputs expected/required by the black box evaluation scheme.
    /// </summary>
    int InputCount { get; }

    /// <summary>
    /// The number of black box inputs expected/required by the black box evaluation scheme.
    /// </summary>
    int OutputCount { get; }
}
