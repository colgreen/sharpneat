/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
namespace SharpNeat.Network
{
    /// <summary>
    /// Enum of network node types.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Bias node. Output is fixed to 1.0
        /// </summary>
        Bias,
        /// <summary>
        /// Input node.
        /// </summary>
        Input,
        /// <summary>
        /// Output node.
        /// </summary>
        Output,
        /// <summary>
        /// Hidden node.
        /// </summary>
        Hidden
    }
}
