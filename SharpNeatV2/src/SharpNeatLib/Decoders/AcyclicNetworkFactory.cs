using SharpNeat.Network;
/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Decoders
{
    // TODO: Work In Progress. Do Not Use.
    public class AcyclicNetworkFactory
    {
        /// <summary>
        /// Creates a AcyclicNetwork from an INetworkDefinition.
        /// </summary>
        public static AcyclicNetwork CreateFastCyclicNetwork(INetworkDefinition networkDef)
        {

            IConnectionList connectionList = networkDef.ConnectionList;
            INodeList nodeList = networkDef.NodeList;
            int connectionCount = connectionList.Count;
            int nodeCount = nodeList.Count;


            FastConnection[] fastConnectionArr = new FastConnection[connectionCount];


            // Determine the depth of each node. Bias and input nodes are defined as being at depth 0, all other nodes are defined by the
            // number of hops from the the depth 0 nodes. Where multiple paths exist (potentially with different number of hops) we take 
            // the largest number of hops as the node depth. This depth is used to divide the nodes up into depth based layers and these 
            // layers can then be used to determien teh order in which signals are propogated through the network.
            // nodeDepthArr is auto-initialised to zeros, hence we can skip setting the depth for the bias and input nodes.
            int[] nodeDepthArr = new int[nodeCount];

            // Follow connections from the depth

            



















            //IActivationFunction[] activationFnArray;
            //double[][] neuronAuxArgsArray;
            return null;
        }


    }
}
