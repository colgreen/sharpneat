/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;

namespace SharpNeat.Network
{
    public static class DirectedGraphUtils
    {
        public static DictionaryNodeIdMap CompileNodeIdMap_InputOutputCount_HiddenNodeIdArr(
            int inputOutputCount, int[] hiddenNodeIdArr)
        {
            // Build dictionary of hidden node new ID/index keyed by old ID.
            // Note. the new IDs start immediately after the last input/output node ID (defined by inputOutputCount).
            var hiddenNodeIdxById = new Dictionary<int,int>(hiddenNodeIdArr.Length);

            for(int i=0, nodeIdx = inputOutputCount; i < hiddenNodeIdArr.Length; i++, nodeIdx++) {
                hiddenNodeIdxById.Add(hiddenNodeIdArr[i], nodeIdx);
            }

            return new DictionaryNodeIdMap(inputOutputCount, hiddenNodeIdxById);
        }
    }
}
