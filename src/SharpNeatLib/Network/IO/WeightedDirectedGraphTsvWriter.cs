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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.NeuralNets;
using static SharpNeat.Network.IO.NetworkTsvWriterUtils;

namespace SharpNeat.Network.IO
{
    /// <summary>
    /// Writes a WeightedDirectedGraph to file in a tab separated file (tsv) based format.
    /// </summary>
    public static class WeightedDirectedGraphTsvWriter
    {
        #region Public Static Methods

        public static void Save(WeightedDirectedGraph<double> digraph, string activationFnName, string path)
        {
            using(var sw = new StreamWriter(path)) {
                Write(digraph, activationFnName, sw);
            }
        }

        public static void Write(WeightedDirectedGraph<double> digraph, string activationFnName, Stream strm)
        {
            using(StreamWriter sw = new StreamWriter(strm)) {
                Write(digraph, activationFnName, sw);
            }
        }

        public static void Write(WeightedDirectedGraph<double> digraph, string activationFnName, StreamWriter sw)
        {
            WriteActivationFunctionsSection(activationFnName, sw);
            WriteNodesSection(digraph, sw);
            WriteConnectionsSection(digraph.ConnectionArray, digraph.WeightArray, sw);
        }

        #endregion
    }
}
