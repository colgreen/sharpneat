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

namespace SharpNeat.Drawing.Graph.Painting
{
    /// <summary>
    /// For tracking connection points on nodes when drawing backwards directed connections, i.e. when the
    /// connection target node is vertically higher than the source node.
    /// </summary>
    public struct ConnectionPointInfo
    {
        /// <summary>
        /// Running connection count for top left of node.
        /// </summary>
        public int UpperLeft;

        /// <summary>
        /// Running connection count for top right of node.
        /// </summary>
        public int UpperRight;

        /// <summary>
        /// Running connection count for bottom left of node.
        /// </summary>
        public int LowerLeft;

        /// <summary>
        /// Running connection count for bottom right of node.
        /// </summary>
        public int LowerRight;
    }
}
