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
using SharpNeat.Network;

namespace SharpNeat.Neat
{
    internal static class IOConnectionUtils
    {
        public static bool IsInputOutputConnection(in DirectedConnection key, int inputCount, int outputCount)
        {
            return (key.SourceId < inputCount) && (key.TargetId >= inputCount && key.TargetId < inputCount + outputCount);
        }
    }
}
