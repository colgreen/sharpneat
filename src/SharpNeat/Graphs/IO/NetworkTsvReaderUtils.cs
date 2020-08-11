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

namespace SharpNeat.Graphs.IO
{
    // TODO: Consider if this is dead code.

    /// <summary>
    /// Static utility methods for reading networks stored in a tab separated value (tsv) format.
    /// </summary>
    public static class NetworkTsvReaderUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Read an activation functions section.
        /// </summary>
        /// <param name="sr">The stream reader to read from.</param>
        /// <returns>A dictionary of the activation names keyed by ID.</returns>
        public static IDictionary<int,string> ReadActivationFunctionSection(StreamReader sr)
        {
            var fnNameById = new Dictionary<int,string>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');
                if(fieldArr.Length != 2) {
                    throw new Exception("Invalid network tsv format.");
                }

                if(!int.TryParse(fieldArr[0], out int id)) {
                    throw new Exception("Invalid network tsv format.");
                }

                fnNameById.Add(id, fieldArr[1]);
            }
            return fnNameById;
        }

        /// <summary>
        /// Read an nodes section.
        /// </summary>
        /// <param name="sr">The stream reader to read from.</param>
        /// <returns>A dictionary of activation function IDs keyed by node ID.</returns>
        public static IDictionary<int,int> ReadNodesSection(StreamReader sr)
        {
            var actFnIdByNodeId = new Dictionary<int,int>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');

                if(fieldArr.Length > 2) {
                    throw new Exception("Invalid network tsv format.");
                }

                if(!int.TryParse(fieldArr[0], out int nodeId)) {
                    throw new Exception("Invalid network tsv format.");
                }

                int actFnId = 0;
                if(fieldArr.Length == 2)
                {
                    if(!int.TryParse(fieldArr[1], out actFnId)) {
                        throw new Exception("Invalid network tsv format.");
                    }
                }
                actFnIdByNodeId.Add(nodeId, actFnId);
            }

            return actFnIdByNodeId;
        }

        /// <summary>
        /// Read an connections section.
        /// </summary>
        /// <param name="sr">The stream reader to read from.</param>
        /// <returns>A list of <see cref="WeightedDirectedConnection{Double}"/></returns>
        public static IList<WeightedDirectedConnection<double>> ReadConnectionsSection(StreamReader sr)
        {
            var connList = new List<WeightedDirectedConnection<double>>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');
                if(fieldArr.Length != 3) {
                    throw new Exception("Invalid network tsv format.");
                }

                if(int.TryParse(fieldArr[0], out int srcNodeId)) {
                    throw new Exception("Invalid network tsv format.");
                }

                if(int.TryParse(fieldArr[1], out int tgtNodeId)) {
                    throw new Exception("Invalid network tsv format.");
                }

                if(double.TryParse(fieldArr[2], out double weight)) {
                    throw new Exception("Invalid network tsv format.");
                }

                var conn = new WeightedDirectedConnection<double>(srcNodeId, tgtNodeId, weight);
                connList.Add(conn);
            }
            return connList;
        }

        #endregion

        #region Private Static Methods

        private static string ReadNextLine(StreamReader sr)
        {
            // Skip comment lines.
            string line;
            do {
                line = sr.ReadLine();
            }
            while(line is object && line.StartsWith("#"));

            if(line is object) {
                line = line.TrimEnd();
            }
            return line;
        }

        #endregion
    }
}
