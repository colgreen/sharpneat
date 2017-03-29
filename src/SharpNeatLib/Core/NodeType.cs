using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
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
