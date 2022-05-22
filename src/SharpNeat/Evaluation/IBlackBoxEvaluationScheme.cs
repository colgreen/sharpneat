// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
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
