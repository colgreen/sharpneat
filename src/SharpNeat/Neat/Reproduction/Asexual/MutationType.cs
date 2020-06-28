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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SharpNeat.Neat.Reproduction.Asexual
{
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
}
