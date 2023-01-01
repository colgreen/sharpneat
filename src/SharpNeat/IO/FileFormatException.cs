// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.IO;

/// <summary>
/// Thrown when a file has an invalid format.
/// </summary>
public class FileFormatException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileFormatException"/> class.
    /// </summary>
    /// <param name="message">A message describing why this exception was thrown.</param>
    public FileFormatException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileFormatException"/> class.
    /// </summary>
    /// <param name="message">A message describing why this exception was thrown.</param>
    /// <param name="innerException">The inner exception that caused this ConfigurationException to be thrown,
    /// if any.</param>
    public FileFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
