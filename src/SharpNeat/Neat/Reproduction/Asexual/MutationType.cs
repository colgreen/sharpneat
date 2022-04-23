// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Genome mutation types.
/// </summary>
public enum MutationType
{
    ConnectionWeight = 0,
    AddNode = 1,
    AddConnection = 2,
    DeleteConnection = 3
}
