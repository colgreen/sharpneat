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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Decoders
{
    /// <summary>
    /// Static factory for creating CyclicNetwork's from INetworkDefinition's.
    /// </summary>
    public class FastCyclicNetworkFactory
    {
        #region Public Static Methods

        /// <summary>
        /// Creates a CyclicNetwork from an INetworkDefinition.
        /// </summary>
        public static FastCyclicNetwork CreateFastCyclicNetwork(INetworkDefinition networkDef,
                                                                NetworkActivationScheme activationScheme)
        {
            FastConnection[] fastConnectionArray;
            IActivationFunction[] activationFnArray;
            double[][] neuronAuxArgsArray;
            InternalDecode(networkDef, 
                           activationScheme.RelaxingActivation ? activationScheme.MaxTimesteps : activationScheme.TimestepsPerActivation,
                           out fastConnectionArray, out activationFnArray, out neuronAuxArgsArray);

            // Construct neural net.
            if(activationScheme.RelaxingActivation)
            {
                return new FastRelaxingCyclicNetwork(fastConnectionArray,
                                                     activationFnArray, 
                                                     neuronAuxArgsArray,
                                                     networkDef.NodeList.Count,
                                                     networkDef.InputNodeCount,
                                                     networkDef.OutputNodeCount,
                                                     activationScheme.MaxTimesteps,
                                                     activationScheme.SignalDeltaThreshold);
            }

            return new FastCyclicNetwork(fastConnectionArray,
                                         activationFnArray,
                                         neuronAuxArgsArray,
                                         networkDef.NodeList.Count,
                                         networkDef.InputNodeCount,
                                         networkDef.OutputNodeCount,
                                         activationScheme.TimestepsPerActivation);
        }

        #endregion

        #region Private Static Methods

        private static void InternalDecode(INetworkDefinition networkDef,
                                           int timestepsPerActivation,
                                           out FastConnection[] fastConnectionArray,
                                           out IActivationFunction[] activationFnArray,
                                           out double[][] neuronAuxArgsArray)
        {
            // Create an array of FastConnection(s) that represent the connectivity of the network.
            fastConnectionArray = CreateFastConnectionArray(networkDef);

            // TODO: Test/optimize heuristic - this is just back of envelope maths.
            // A rough heuristic to decide if we should sort fastConnectionArray by source neuron index.
            // The principle here is that each activation loop will be about 2x faster (unconfirmed) if we sort 
            // fastConnectionArray, but sorting takes about n*log2(n) operations. Therefore the decision to sort
            // depends on length of fastConnectionArray and _timestepsPerActivation.
            // Another factor here is that small networks will fit into CPU caches and therefore will not appear
            // to speed up - however the unsorted data will 'scramble' CPU caches more than they otherwise would 
            // have and thus may slow down other threads (so we just keep it simple).
            double len = fastConnectionArray.Length;
            double timesteps = timestepsPerActivation;
            if((len > 2) && (((len * Math.Log(len,2)) + ((timesteps * len)/2.0)) < (timesteps * len)))
            {   // Sort fastConnectionArray by source neuron index.
                Array.Sort(fastConnectionArray, delegate(FastConnection x, FastConnection y)
                {   // Use simple/fast diff method.
                    return x._srcNeuronIdx - y._srcNeuronIdx;
                });
            }

            // Construct an array of neuron activation functions. Skip bias and input neurons as
            // these don't have an activation function (because they aren't activated).
            INodeList nodeList = networkDef.NodeList;
            int nodeCount = nodeList.Count;
            IActivationFunctionLibrary activationFnLibrary = networkDef.ActivationFnLibrary;
            activationFnArray = new IActivationFunction[nodeCount];
            neuronAuxArgsArray = new double[nodeCount][];

            for(int i=0; i<nodeCount; i++) {
                activationFnArray[i] = activationFnLibrary.GetFunction(nodeList[i].ActivationFnId);
                neuronAuxArgsArray[i] = nodeList[i].AuxState;
            }
        }

        /// <summary>
        /// Create an array of FastConnection(s) representing the connectivity of the provided INetworkDefinition.
        /// </summary>
        private static FastConnection[] CreateFastConnectionArray(INetworkDefinition networkDef)
        {
            // We vary the decode logic depending on the size of the genome. The most CPU intensive aspect of
            // decoding is the conversion of the neuron IDs at connection endpoints into neuron indexes. For small
            // genomes we simply use the BinarySearch() method on NeuronGeneList for each lookup; Each lookup is
            // an operation with O(log n) time complexity. Thus for C connections and N neurons the number of operations
            // to perform all lookups is approximately = 2*C*Log(N)
            //
            // For larger genomes we invest time in building a Dictionary that maps neuron IDs to their indexes, this on the
            // basis that the time invested will be more than recovered in time saved performing lookups; The time complexity
            // of a dictionary lookup is near constant O(1). Thus number of operations is approximately = O(2*C*1) + the time
            // required to build the dictionary which is approximately O(N).
            //
            // Therefore the choice of lookup type is based on which of these two expressions gives the lowest value. 
            //
            //      Binary search.      LookupOps = 2 * C * Log2(N) * x
            //      Dictionary Search.  LookupOps = (N * y) + (2 * C * z)
            //
            // Where x, y and z are constants that adjust for the relative speeds of the lookup and dictionary building operations.
            // Note that the actual time required to perform these separate algorithms is actually a far more complex problem, and 
            // for modern CPUs exact times cannot be calculated because of large memory caches and superscalar architecture that
            // makes execution times in a real environment effectively non-deterministic. Thus these calculations are a rough 
            // guide/heuristic that estimate which algorithm will perform best. The constants can be found experimentally but will
            // tend to vary depending on factors such as CPU and memory architecture, .Net framework version and what other tasks the
            // CPU is currently doing which may affect our utilisation of memory caches.
            // TODO: Experimentally determine reasonably good values for constants x,y and z in some common real world runtime platform.
            IConnectionList connectionList = networkDef.ConnectionList;
            INodeList nodeList = networkDef.NodeList;
            int connectionCount = connectionList.Count;
            int nodeCount = nodeList.Count;

            FastConnection[] fastConnectionArray = new FastConnection[connectionCount];

            if((2.0 * connectionCount * Math.Log(nodeCount, 2.0)) < ((2.0 * connectionCount) + nodeCount))
            {
                // Binary search requires items to be sorted.
                Debug.Assert(nodeList.IsSorted());

                // Loop the connections and lookup the neuron IDs for each connection's end points using a binary search
                // on nGeneList. This is probably the quickest approach for small numbers of lookups.
                for(int i=0; i<connectionCount; i++)
                {   
                    INetworkConnection conn = connectionList[i];
                    fastConnectionArray[i]._srcNeuronIdx = nodeList.BinarySearch(conn.SourceNodeId);
                    fastConnectionArray[i]._tgtNeuronIdx = nodeList.BinarySearch(conn.TargetNodeId);
                    fastConnectionArray[i]._weight = conn.Weight;

                    // Check that the correct neuron indexes were found.
                    Debug.Assert( 
                           nodeList[fastConnectionArray[i]._srcNeuronIdx].Id == conn.SourceNodeId
                        && nodeList[fastConnectionArray[i]._tgtNeuronIdx].Id == conn.TargetNodeId);
                }
            }
            else
            {
                // Build dictionary of neuron indexes keyed on neuron innovation ID.
                Dictionary<uint,int> neuronIndexDictionary = new Dictionary<uint,int>(nodeCount);
                for(int i=0; i<nodeCount; i++) {
                    // ENHANCEMENT: Check if neuron innovation ID requires further manipulation to make a good hash code.
                    neuronIndexDictionary.Add(nodeList[i].Id, i);
                }

                // Loop the connections and lookup the neuron IDs for each connection's end points using neuronIndexDictionary.
                // This is probably the quickest approach for large numbers of lookups.
                for(int i=0; i<connectionCount; i++)
                {   
                    INetworkConnection conn = connectionList[i];
                    fastConnectionArray[i]._srcNeuronIdx = neuronIndexDictionary[conn.SourceNodeId];
                    fastConnectionArray[i]._tgtNeuronIdx = neuronIndexDictionary[conn.TargetNodeId];
                    fastConnectionArray[i]._weight = conn.Weight;

                    // Check that the correct neuron indexes were found.
                    Debug.Assert( 
                           nodeList[fastConnectionArray[i]._srcNeuronIdx].Id == conn.SourceNodeId
                        && nodeList[fastConnectionArray[i]._tgtNeuronIdx].Id == conn.TargetNodeId);
                }
            }

            return fastConnectionArray;
        }

        #endregion
    }
}
