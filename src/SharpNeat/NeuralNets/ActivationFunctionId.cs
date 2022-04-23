// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.NeuralNets;

/// <summary>
/// The set of neural network activation functions provided as standard in SharpNEAT.
/// </summary>
public enum ActivationFunctionId
{
    /// <summary>
    /// The ArcSinH function (inverse hyperbolic sine function).
    /// </summary>
    ArcSinH,
    /// <summary>
    /// The ArcTan function (inverse tangent function).
    /// </summary>
    ArcTan,
    /// <summary>
    /// Leaky rectified linear activation unit (ReLU).
    /// </summary>
    LeakyReLU,
    /// <summary>
    /// Leaky rectified linear activation unit (ReLU).
    /// </summary>
    LeakyReLUShifted,
    /// <summary>
    /// The logistic function.
    /// </summary>
    Logistic,
    /// <summary>
    /// The logistic function with a steepened slope, and implemented using a fast to compute approximation of exp().
    /// </summary>
    LogisticApproximantSteep,
    /// <summary>
    /// The logistic function with a steepened slope.
    /// </summary>
    LogisticSteep,
    /// <summary>
    /// max(-1, x,) function.
    /// </summary>
    MaxMinusOne,
    /// <summary>
    /// Null activation function. Returns zero regardless of input.
    /// </summary>
    NullFn,
    /// <summary>
    /// A very close approximation of the logistic function that avoids use of exp() and is therefore
    /// typically much faster to compute, while giving an almost identical sigmoid curve.
    /// </summary>
    PolynomialApproximantSteep,
    /// <summary>
    /// A sigmoid formed by two sub-sections of the y=x^2 curve.
    /// </summary>
    QuadraticSigmoid,
    /// <summary>
    /// Rectified linear activation unit (ReLU).
    /// </summary>
    ReLU,
    /// <summary>
    /// Scaled Exponential Linear Unit (SELU).
    /// </summary>
    ScaledELU,
    /// <summary>
    /// The softsign sigmoid.
    /// </summary>
    SoftSignSteep,
    /// <summary>
    /// S-shaped rectified linear activation unit (SReLU).
    /// </summary>
    SReLU,
    /// <summary>
    /// S-shaped rectified linear activation unit (SReLU). Shifted on the x-axis so that x=0 gives y=0.5, in keeping with the logistic sigmoid.
    /// </summary>
    SReLUShifted,
    /// <summary>
    /// TanH function (hyperbolic tangent function).
    /// </summary>
    TanH
}
