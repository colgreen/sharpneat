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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SharpNeat.Drawing.Graph
{
    /// <summary>
    /// An <see cref="IGraphLayoutScheme"/> that arranges/positions nodes into layers, based on their depth in the network.
    /// </summary>
    public class DepthLayoutScheme : IGraphLayoutScheme
    {
        /// <summary>
        /// Layout nodes based on their depth within the network.
        /// 
        /// Note 1.
        /// Input nodes are defined as being at layer zero, and we position them in their own layer at the 
        /// top of the layout area. Any other type of node (hidden or output) not connected to is also
        /// defined as being at layer zero, if such nodes exist then we place them into their own layout 
        /// layer to visually separate them from the input nodes.
        /// 
        /// Note 2.
        /// Output nodes can be at a range of depths, but for clarity we position them all in their own layer 
        /// at the bottom of the layout area. A hidden node can have a depth greater than or equal to one or 
        /// more of the output nodes, to handle this case neatly we ensure that the output nodes are always 
        /// in a layer by themselves by creating an additional layer in the layout if necessary.
        ///
        /// Note 3.
        /// Hidden nodes are positioned into layers between the inputs and outputs based on their depth.
        /// 
        /// Note 4. 
        /// Empty layers are not possible in the underlying network because for there to be a layer N node it 
        /// must have a connection from a layer N-1 node. However, in cyclic networks the output nodes can be
        /// source nodes, but we also always paint output nodes in their own layout layer at the bottom of the
        /// layout area. Therefore if the only node at a given depth is an output node then the layout layer 
        /// can be empty. To handle this neatly we check for empty layout layers before invoking the layout logic.
        /// </summary>
        /// <param name="viewModel">The graph view model to be laid out.</param>
        /// <param name="layoutArea">The area to layout nodes within.</param>
        public void Layout(DirectedGraphViewModel viewModel, Size layoutArea)
        {
            // TODO: Implement.


            throw new NotImplementedException();
        }
    }
}
