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
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    internal static class AddConnectionUtils
    {
        public static int GetNodeIdFromIndex<T>(NeatGenome<T> parent, int idx)
            where T : struct
        {
            // For input/output nodes their index is their ID.
            if(idx < parent.MetaNeatGenome.InputOutputNodeCount) {
                return idx;
            }

            // All other nodes are hidden nodes; use a pre-built array of all hidden node IDs.
            return parent.HiddenNodeIdArray[idx - parent.MetaNeatGenome.InputOutputNodeCount];
        }
    }
}
