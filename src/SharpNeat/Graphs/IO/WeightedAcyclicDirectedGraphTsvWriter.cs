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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Network.Acyclic;
using SharpNeat.NeuralNets;
using static SharpNeat.Network.IO.NetworkTsvWriterUtils;

namespace SharpNeat.Graphs.IO
{
    /// <summary>
    /// Writes a WeightedAcyclicDirectedGraph to file in a tab separated value (tsv) format.
    /// </summary>
    public static class WeightedAcyclicDirectedGraphTsvWriter
    {
        #region Public Static Methods

        /// <summary>
        /// Save an acyclic directed graph to a file.
        /// </summary>
        /// <param name="digraph">The directed graph to save.</param>
        /// <param name="activationFnName">The single activation function to save with the graph.</param>
        /// <param name="path">The file path to save to.</param>
        public static void Save(WeightedAcyclicDirectedGraph<double> digraph, string activationFnName, string path)
        {
            using(var sw = new StreamWriter(path)) {
                Write(digraph, activationFnName, sw);
            }
        }

        /// <summary>
        /// Write an acyclic directed graph to a stream.
        /// </summary>
        /// <param name="digraph">The directed graph to save.</param>
        /// <param name="activationFnName">The single activation function to save with the graph.</param>
        /// <param name="strm">The stream to write to.</param>
        public static void Write(WeightedAcyclicDirectedGraph<double> digraph, string activationFnName, Stream strm)
        {
            using(StreamWriter sw = new StreamWriter(strm)) {
                Write(digraph, activationFnName, sw);
            }
        }

        /// <summary>
        /// Write an acyclic directed graph to a stream writer.
        /// </summary>
        /// <param name="digraph">The directed graph to save.</param>
        /// <param name="activationFnName">The single activation function to save with the graph.</param>
        /// <param name="sw">The stream writer to write to.</param>
        public static void Write(WeightedAcyclicDirectedGraph<double> digraph, string activationFnName, StreamWriter sw)
        {
            WriteActivationFunctionsSection(activationFnName, sw);
            WriteNodesSection(digraph, sw);
            WriteConnectionsSection(digraph.ConnectionArray, digraph.WeightArray, sw);
        }

        #endregion
    }
}
