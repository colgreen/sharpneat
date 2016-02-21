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
using System.Collections.Generic;
using SharpNeat.Network;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Decoders
{
    /// <summary>
    /// Static factory for creating CyclicNetwork's from INetworkDefinition's.
    /// </summary>
    public class CyclicNetworkFactory
    {
        #region Public Static Methods

        /// <summary>
        /// Creates a CyclicNetwork from an INetworkDefinition.
        /// </summary>
        public static CyclicNetwork CreateCyclicNetwork(INetworkDefinition networkDef,
                                                           NetworkActivationScheme activationScheme)
        {
            List<Neuron> neuronList;
            List<Connection> connectionList;
            InternalDecode(networkDef, out neuronList, out connectionList);

            // Construct neural net.
            if(activationScheme.RelaxingActivation)
            {
                return new RelaxingCyclicNetwork(neuronList,
                                                 connectionList,
                                                 networkDef.InputNodeCount,
                                                 networkDef.OutputNodeCount,
                                                 activationScheme.MaxTimesteps,
                                                 activationScheme.SignalDeltaThreshold);
            }

            return new CyclicNetwork(neuronList,
                                     connectionList,
                                     networkDef.InputNodeCount,
                                     networkDef.OutputNodeCount,
                                     activationScheme.TimestepsPerActivation);
        }

        #endregion

        #region Private Static Methods

        private static void InternalDecode(INetworkDefinition networkDef,
                                           out List<Neuron> neuronList,
                                           out List<Connection> connectionList)
        {
            // Build a list of neurons.
            INodeList nodeDefList = networkDef.NodeList;
            int nodeCount = nodeDefList.Count;
            neuronList = new List<Neuron>(nodeCount);

            // A dictionary of neurons keyed on their innovation ID.
            Dictionary<uint,Neuron> neuronDictionary = new Dictionary<uint,Neuron>(nodeCount);

            // Loop neuron genes.
            IActivationFunctionLibrary activationFnLib = networkDef.ActivationFnLibrary;

            for(int i=0; i<nodeCount; i++) 
            {   // Create a Neuron, add it to the neuron list and add an entry into neuronDictionary -
                // required for next loop.
                INetworkNode nodeDef = nodeDefList[i];

                // Note that we explicitly translate between the two NeuronType enums even though
                // they define the same types and could therefore be cast from one to the other.
                // We do this to keep genome and phenome classes completely separated and also to 
                // prevent bugs - e.g. if one of the enums is changed then TranslateNeuronType() will 
                // need to be modified to prevent exceptions at runtime. Otherwise a silent bug may 
                // be introduced.
                Neuron neuron = new Neuron(nodeDef.Id,
                                           nodeDef.NodeType,
                                           activationFnLib.GetFunction(nodeDef.ActivationFnId),
                                           nodeDef.AuxState);
                neuronList.Add(neuron);
                neuronDictionary.Add(nodeDef.Id, neuron);
            }

            // Build a list of connections.
            IConnectionList connectionDefList = networkDef.ConnectionList;
            int connectionCount = connectionDefList.Count;
            connectionList = new List<Connection>(connectionCount);

            // Loop connection genes.
            for(int i=0; i<connectionCount; i++)
            {
                INetworkConnection connDef = connectionDefList[i];
                connectionList.Add(
                        new Connection(neuronDictionary[connDef.SourceNodeId], 
                                       neuronDictionary[connDef.TargetNodeId],
                                       connDef.Weight));
            }
        }

        #endregion
    }
}
