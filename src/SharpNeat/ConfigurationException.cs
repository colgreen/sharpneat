// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat;

/// <summary>
/// Represents a problem with configuration data.
/// </summary>
public class ConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message describing why this exception was thrown.</param>
    public ConfigurationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">A message describing why this exception was thrown.</param>
    /// <param name="innerException">The inner exception that caused this ConfigurationException to be thrown,
    /// if any.</param>
    public ConfigurationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
