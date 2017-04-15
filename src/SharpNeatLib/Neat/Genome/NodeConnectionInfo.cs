using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Genome
{
    public struct NodeConnectionInfo
    {
        public readonly uint[] SrcNodes;
        public readonly uint[] TgtNodes;

        public NodeConnectionInfo(uint[] srcNodes, uint[] tgtNodes)
        {
            this.SrcNodes = srcNodes;
            this.TgtNodes = tgtNodes;
        }
    }
}
