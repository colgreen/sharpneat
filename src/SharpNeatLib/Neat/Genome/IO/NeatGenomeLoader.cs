using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Redzen.Sorting;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome.IO
{
    /// <summary>
    /// Abstract base class for NeatGenome loading.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public abstract class NeatGenomeLoader<T> where T : struct
    {
        #region Instance Fields

        readonly MetaNeatGenome<T> _metaNeatGenome;
        readonly INeatGenomeBuilder<T> _genomeBuilder;

        readonly List<DirectedConnection> _connList;
        readonly List<T> _weightList;

        StreamReader _sr;
        int _lineIdx;
        int _genomeId;

        #if DEBUG
        /// <summary>
        /// Indicates if a call to Load() is currently in progress. 
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Constructors

        protected NeatGenomeLoader(
            MetaNeatGenome<T> metaNeatGenome) 
            : this(metaNeatGenome, 8)
        {}

        protected NeatGenomeLoader(
            MetaNeatGenome<T> metaNeatGenome,
            int connCountEstimate)
        {
            _metaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
            _genomeBuilder = NeatGenomeBuilderFactory<T>.Create(metaNeatGenome);

            _connList = new List<DirectedConnection>(connCountEstimate);
            _weightList = new List<T>(connCountEstimate);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a genome from the specified file.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        /// <returns>The loaded genome.</returns>
        public NeatGenome<T> Load(string path)
        {
            using(var sr = new StreamReader(path)) {
                return Load(sr);
            }
        }
        /// <summary>
        /// Load a genome from the given stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <returns>The loaded genome.</returns>
        /// <remarks>This method does not close the Stream.</remarks>
        public NeatGenome<T> Load(Stream stream)
        {
            if(null == stream) throw new ArgumentNullException(nameof(stream));

            using(var sr = new StreamReader(stream, Encoding.UTF8, true, 1024, true)) {
                return Load(sr);
            }
        }
        /// <summary>
        /// Load a genome from the given stream.
        /// </summary>
        /// <param name="sr"></param>
        /// <returns>The loaded genome.</returns>
        /// <remarks>This method does not close the StreamReader.</remarks>
        public NeatGenome<T> Load(StreamReader sr)
        {
            if(null == sr) throw new ArgumentNullException(nameof(sr));

            #if DEBUG
            // Check for attempts to re-enter this method.
            if(1 == Interlocked.CompareExchange(ref _reentranceFlag, 1, 0)) {
                throw new InvalidOperationException("Attempt to re-enter non-reentrant method.");
            }
            #endif

            try
            {
                _sr = sr;    
                return LoadInner(sr);
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private NeatGenome<T> LoadInner(StreamReader sr)
        {
            // Reset line counter.
            _lineIdx = 0;

            // Read node counts.
            int inputCount = ReadInt32Line();
            int outputCount = ReadInt32Line();
            ReadEndOfSection();
            ValidateNodeCounts(inputCount, outputCount);

            // Read connections.
            ReadConnections();

            // Copy connection data into arrays, and sort into the correct order (for a NeatGenome).
            DirectedConnection[] connArr = _connList.ToArray();
            T[] weightArr = _weightList.ToArray();
            Array.Sort(connArr, weightArr);

            // Create a genome object and return.
            var connGenes = new ConnectionGenes<T>(connArr, weightArr);
            return _genomeBuilder.Create(_genomeId++, 0, connGenes);
        }

        private void ReadConnections()
        {
            for(;;)
            {
                // Read a line.
                string line = ReadNextLine();

                // Stop reading connections if we detect the end of the section, or end of the file.
                if(string.IsNullOrEmpty(line)) {
                    return;
                }

                // Parse the connection fields.
                string[] fields = line.Split(' ', '\t');
                if(fields.Length != 3) {
                    throw new IOException($"Invalid connection. Line [{_lineIdx}].");
                }

                if(!TryParseInt32(fields[0], out int srcId)) {
                    throw new IOException($"Invalid connection source ID format. Line [{_lineIdx}].");
                }

                if(!TryParseInt32(fields[1], out int tgtId)) {
                    throw new IOException($"Invalid connection target ID format. Line [{_lineIdx}].");
                }

                if(!TryParseWeight(fields[2], out T weight)) {
                    throw new IOException($"Invalid connection weight format. Line [{_lineIdx}].");
                }

                // Further validation.
                // A target ID should never refer to an input node.
                if(tgtId < _metaNeatGenome.InputNodeCount) {
                    throw new IOException($"Invalid connection target ID. Refers to an input node. Line [{_lineIdx}].");
                }

                // Store the connection.
                _connList.Add(new DirectedConnection(srcId, tgtId));
                _weightList.Add(weight);
            }
        }

        private void ValidateNodeCounts(
            int inputCount, int outputCount)
        {
            // Validate node counts.
            if(_metaNeatGenome.InputNodeCount != inputCount) {
                throw new IOException($"Incorrect input count. Line [{_lineIdx}].");
            }
            
            if(_metaNeatGenome.OutputNodeCount != outputCount) {
                throw new IOException($"Incorrect output count. Line [{_lineIdx}].");
            }
        }

        private void Cleanup()
        {
            _connList.Clear();
            _weightList.Clear();

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion

        #region Private Methods [Low Level]

        private int ReadInt32Line()
        {
            string line = ReadNonEmptyLine();
            if(!TryParseInt32(line, out int val)) {
                throw new IOException("Invalid integer format. Line [{_lineIdx}].");
            }
            return val;
        }

        private void ReadEndOfSection()
        {
            string line = ReadNextLine();
            if(line != string.Empty) {
                throw new IOException("End of section expected. Line [{_lineIdx}].");
            }
        }

        private string ReadNonEmptyLine()
        {
            string line = ReadNextLine();
            if(string.IsNullOrEmpty(line)) {
                throw new IOException($"Expected non-empty line. Line [{_lineIdx}].");
            }
            return line;
        }

        private string ReadNextLine()
        {
            for(;;)
            {
                // Read the next line.
                string line = _sr.ReadLine();
                _lineIdx++;

                // Return the line if it is empty (end of a section), null (end of file), 
                // or is not a comment line (starts with a hash character).
                if(string.IsNullOrEmpty(line) || line[0] != '#') {
                    return line;
                }
            }
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract bool TryParseWeight(string str, out T weight);

        #endregion

        #region Private Static Methods

        private static bool TryParseInt32(string str, out int val)
        {
            return int.TryParse(
                str,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out val);
        }

        #endregion
    }
}
