// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.IO.Models;

/// <summary>
/// Represents an activation function line in a 'net' file.
/// </summary>
public class ActivationFnLine
{
    /// <summary>
    /// Activation function ID.
    /// </summary>
    /// <remarks>Function IDs are integers and are always defined in a continuous sequence starting at zero.</remarks>
    public int Id { get; }

    /// <summary>
    /// Activation function code.
    /// </summary>
    /// <remarks>
    /// The code is a string identifier such as 'ReLU' or 'Logistic'.
    /// These code correspond with the local class names (i.e. not including the namespace) of the activation function implementations.</remarks>
    public string Code { get; }

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <param name="code">Code.</param>
    public ActivationFnLine(int id, string code)
    {
        this.Id = id;
        this.Code = code;
    }
}
