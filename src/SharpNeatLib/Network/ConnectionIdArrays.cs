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
using System.Diagnostics;

namespace SharpNeat.Network
{
    public readonly struct ConnectionIdArrays
    {
        public readonly int[] _sourceIdArr;
        public readonly int[] _targetIdArr;

        public ConnectionIdArrays(int[] srcIdArr, int[] tgtIdArr)
        {
            Debug.Assert(srcIdArr.Length == tgtIdArr.Length);
            _sourceIdArr = srcIdArr;
            _targetIdArr = tgtIdArr;
        }

        public int Length => _sourceIdArr.Length;
    }
}
