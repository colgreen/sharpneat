using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network.IO
{
    public static class NetworkTsvReaderUtils
    {
        #region Public Static Methods

        public static IDictionary<int,string> ReadActivationFunctionSection(StreamReader sr)
        {
            var fnNameById = new Dictionary<int,string>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');
                if(fieldArr.Length != 2) {
                    throw new Exception("Invalid network tsv format.");
                }

                int id;
                if(!int.TryParse(fieldArr[0], out id)) {
                    throw new Exception("Invalid network tsv format.");
                }

                fnNameById.Add(id, fieldArr[1]);
            }
            return fnNameById;
        }

        public static IDictionary<int,int> ReadNodesSection(StreamReader sr)
        {
            var actFnIdByNodeId = new Dictionary<int,int>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');

                if(fieldArr.Length > 2) {
                    throw new Exception("Invalid network tsv format.");
                }

                int nodeId;
                if(!int.TryParse(fieldArr[0], out nodeId)) {
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

        public static IList<WeightedDirectedConnection<double>> ReadConnectionsSection(StreamReader sr)
        {
            var connList = new List<WeightedDirectedConnection<double>>();

            for(string line = ReadNextLine(sr); !string.IsNullOrEmpty(line); line = ReadNextLine(sr))
            {
                string[] fieldArr = line.Split(' ', '\t');
                if(fieldArr.Length != 3) {
                    throw new Exception("Invalid network tsv format.");
                }

                int srcNodeId;
                if(int.TryParse(fieldArr[0], out srcNodeId)) {
                    throw new Exception("Invalid network tsv format.");
                }

                int tgtNodeId;
                if(int.TryParse(fieldArr[1], out tgtNodeId)) {
                    throw new Exception("Invalid network tsv format.");
                }

                double weight;
                if(double.TryParse(fieldArr[2], out weight)) {
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
            while(null != line && line.StartsWith("#"));

            if(null != line) {
                line = line.TrimEnd();
            }
            return line;
        }

        #endregion
    }
}
