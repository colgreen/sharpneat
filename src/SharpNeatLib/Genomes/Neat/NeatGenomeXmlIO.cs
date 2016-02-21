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
using System.Globalization;
using System.Text;
using System.Xml;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Static class for reading and writing NeatGenome(s) to and from XML.
    /// </summary>
    public static class NeatGenomeXmlIO
    {
        #region Constants [XML Strings]

        const string __ElemRoot = "Root";
        const string __ElemNetworks = "Networks";
        const string __ElemNetwork = "Network";
        const string __ElemNodes = "Nodes";
        const string __ElemNode = "Node";
        const string __ElemConnections = "Connections";
        const string __ElemConnection = "Con";
        const string __ElemActivationFunctions = "ActivationFunctions";

        const string __AttrId = "id";
        const string __AttrBirthGeneration = "birthGen";
        const string __AttrFitness = "fitness";
        const string __AttrType = "type";
        const string __AttrSourceId = "src";
        const string __AttrTargetId = "tgt";
        const string __AttrWeight = "wght";
        const string __AttrActivationFunctionId = "fnId";
        const string __AttrAuxState = "aux";

        #endregion

        #region Public Static Methods [Save to XmlDocument]

        /// <summary>
        /// Writes a single NeatGenome to XML within a containing 'Root' element and the activation function
        /// library that the genome is associated with.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="genome">The genome to save.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument SaveComplete(NeatGenome genome, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                WriteComplete(xw, genome, nodeFnIds);
            }
            return doc;
        }

        /// <summary>
        /// Writes a list of NeatGenome(s) to XML within a containing 'Root' element and the activation
        /// function library that the genomes are associated with.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="genomeList">List of genomes to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument SaveComplete(IList<NeatGenome> genomeList, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                WriteComplete(xw, genomeList, nodeFnIds);
            }
            return doc;
        }

        /// <summary>
        /// Writes a single NeatGenome to XML.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="genome">The genome to save.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument Save(NeatGenome genome, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                Write(xw, genome, nodeFnIds);
            }
            return doc;
        }

        #endregion

        #region Public Static Methods [Load from XmlDocument]

        /// <summary>
        /// Loads a list of NeatGenome(s) from XML that has a containing 'Root' element. The root element 
        /// also contains the activation function library that the network definitions are associated with.
        /// </summary>
        /// <param name="xmlNode">The XmlNode to read from. This can be an XmlDocument or XmlElement.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. If false then 
        /// all node activation function IDs default to 0.</param>
        /// <param name="genomeFactory">A NeatGenomeFactory object to construct genomes against.</param>
        public static List<NeatGenome> LoadCompleteGenomeList(XmlNode xmlNode, bool nodeFnIds, NeatGenomeFactory genomeFactory)
        {
            using(XmlNodeReader xr = new XmlNodeReader(xmlNode))
            {
                return ReadCompleteGenomeList(xr, nodeFnIds, genomeFactory);
            }
        }

        /// <summary>
        /// Reads a NeatGenome from XML.
        /// </summary>
        /// <param name="xmlNode">The XmlNode to read from. This can be an XmlDocument or XmlElement.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. They are required
        /// for HyperNEAT genomes but not for NEAT</param>
        public static NeatGenome LoadGenome(XmlNode xmlNode, bool nodeFnIds)
        {
            using(XmlNodeReader xr = new XmlNodeReader(xmlNode))
            {
                return ReadGenome(xr, nodeFnIds);
            }
        }

        #endregion

        #region Public Static Methods [Write to XML]

        /// <summary>
        /// Writes a list of NeatGenome(s) to XML within a containing 'Root' element and the activation
        /// function library that the genomes are associated with.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="genomeList">List of genomes to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void WriteComplete(XmlWriter xw, IList<NeatGenome> genomeList, bool nodeFnIds)
        {
            if(genomeList.Count == 0)
            {   // Nothing to do.
                return;
            }

            // <Root>
            xw.WriteStartElement(__ElemRoot);

            // Write activation function library from the first genome.
            // (we expect all genomes to use the same library).
            IActivationFunctionLibrary activationFnLib = genomeList[0].ActivationFnLibrary;
            NetworkXmlIO.Write(xw, activationFnLib);

            // <Networks>
            xw.WriteStartElement(__ElemNetworks);

            // Write genomes.
            foreach(NeatGenome genome in genomeList) {
                Debug.Assert(genome.ActivationFnLibrary == activationFnLib);
                Write(xw, genome, nodeFnIds);
            }

            // </Networks>
            xw.WriteEndElement();

            // </Root>
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a single NeatGenome to XML within a containing 'Root' element and the activation
        /// function library that the genome is associated with.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="genome">Genome to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void WriteComplete(XmlWriter xw, NeatGenome genome, bool nodeFnIds)
        {
            // <Root>
            xw.WriteStartElement(__ElemRoot);

            // Write activation function library.
            NetworkXmlIO.Write(xw, genome.ActivationFnLibrary);

            // <Networks>
            xw.WriteStartElement(__ElemNetworks);

            // Write single genome.
            Write(xw, genome, nodeFnIds);

            // </Networks>
            xw.WriteEndElement();

            // </Root>
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a NeatGenome to XML.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="genome">Genome to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void Write(XmlWriter xw, NeatGenome genome, bool nodeFnIds)
        {
            xw.WriteStartElement(__ElemNetwork);
            xw.WriteAttributeString(__AttrId, genome.Id.ToString(NumberFormatInfo.InvariantInfo));
            xw.WriteAttributeString(__AttrBirthGeneration, genome.BirthGeneration.ToString(NumberFormatInfo.InvariantInfo));
            xw.WriteAttributeString(__AttrFitness, genome.EvaluationInfo.Fitness.ToString("R", NumberFormatInfo.InvariantInfo));

            // Emit nodes.
            StringBuilder sb = new StringBuilder();
            xw.WriteStartElement(__ElemNodes);
            foreach(NeuronGene nGene in genome.NeuronGeneList)
            {
                xw.WriteStartElement(__ElemNode);
                xw.WriteAttributeString(__AttrType, NetworkXmlIO.GetNodeTypeString(nGene.NodeType));
                xw.WriteAttributeString(__AttrId, nGene.Id.ToString(NumberFormatInfo.InvariantInfo));
                if(nodeFnIds) 
                {	// Write activation fn ID.
                    xw.WriteAttributeString(__AttrActivationFunctionId, nGene.ActivationFnId.ToString(NumberFormatInfo.InvariantInfo));

                    // Write aux state as comma separated list of real values.
                    XmlIoUtils.WriteAttributeString(xw, __AttrAuxState, nGene.AuxState);
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // Emit connections.
            xw.WriteStartElement(__ElemConnections);
            foreach(ConnectionGene cGene in genome.ConnectionList)
            {
                xw.WriteStartElement(__ElemConnection);
                xw.WriteAttributeString(__AttrId, cGene.InnovationId.ToString(NumberFormatInfo.InvariantInfo));
                xw.WriteAttributeString(__AttrSourceId, cGene.SourceNodeId.ToString(NumberFormatInfo.InvariantInfo));
                xw.WriteAttributeString(__AttrTargetId, cGene.TargetNodeId.ToString(NumberFormatInfo.InvariantInfo));
                xw.WriteAttributeString(__AttrWeight, cGene.Weight.ToString("R", NumberFormatInfo.InvariantInfo));
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // </Network>
            xw.WriteEndElement();
        }

        #endregion

        #region Public Static Methods [Read from XML]

        /// <summary>
        /// Reads a list of NeatGenome(s) from XML that has a containing 'Root' element. The root 
        /// element also contains the activation function library that the genomes are associated with.
        /// </summary>
        /// <param name="xr">The XmlReader to read from.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. If false then 
        /// all node activation function IDs default to 0.</param>
        /// <param name="genomeFactory">A NeatGenomeFactory object to construct genomes against.</param>
        public static List<NeatGenome> ReadCompleteGenomeList(XmlReader xr, bool nodeFnIds, NeatGenomeFactory genomeFactory)
        {
            // Find <Root>.
            XmlIoUtils.MoveToElement(xr, false, __ElemRoot);

            // Read IActivationFunctionLibrary. This library is not used, it is compared against the one already present in the 
            // genome factory to confirm that the loaded genomes are compatible with the genome factory.
            XmlIoUtils.MoveToElement(xr, true, __ElemActivationFunctions);
            IActivationFunctionLibrary activationFnLib = NetworkXmlIO.ReadActivationFunctionLibrary(xr);
            XmlIoUtils.MoveToElement(xr, false, __ElemNetworks);

            // Read genomes.
            List<NeatGenome> genomeList = new List<NeatGenome>();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root <Networks> element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first Network elem.
                XmlIoUtils.MoveToElement(xrSubtree, true, __ElemNetwork);
                
                // Read Network elements.
                do
                {
                    NeatGenome genome = ReadGenome(xrSubtree, nodeFnIds);
                    genomeList.Add(genome);
                } 
                while(xrSubtree.ReadToNextSibling(__ElemNetwork));
            }

            // Check for empty list.
            if(genomeList.Count == 0) {
                return genomeList;
            }

            // Get the number of inputs and outputs expected by the genome factory.
            int inputCount = genomeFactory.InputNeuronCount;
            int outputCount = genomeFactory.OutputNeuronCount;

            // Check all genomes have the same number of inputs & outputs.
            // Also track the highest genomeID and innovation ID values; we need these to construct a new genome factory.
            uint maxGenomeId = 0;
            uint maxInnovationId = 0;

            foreach(NeatGenome genome in genomeList)
            {
                // Check number of inputs/outputs.
                if(genome.InputNeuronCount != inputCount || genome.OutputNeuronCount != outputCount) {
                    throw new SharpNeatException(string.Format("Genome with wrong number of inputs and/or outputs, expected [{0}][{1}] got [{2}][{3}]",
                                                               inputCount, outputCount, genome.InputNeuronCount, genome.OutputNeuronCount));
                }

                // Track max IDs.
                maxGenomeId = Math.Max(maxGenomeId, genome.Id);

                // Node and connection innovation IDs are in the same ID space.
                foreach(NeuronGene nGene in genome.NeuronGeneList) {
                    maxInnovationId = Math.Max(maxInnovationId, nGene.InnovationId);
                }

                // Register connection IDs.
                foreach(ConnectionGene cGene in genome.ConnectionGeneList) {
                    maxInnovationId = Math.Max(maxInnovationId, cGene.InnovationId);
                }
            }

            // Check that activation functions in XML match that in the genome factory.
            IList<ActivationFunctionInfo> loadedActivationFnList = activationFnLib.GetFunctionList();
            IList<ActivationFunctionInfo> factoryActivationFnList = genomeFactory.ActivationFnLibrary.GetFunctionList();
            if(loadedActivationFnList.Count != factoryActivationFnList.Count) {
                throw new SharpNeatException("The activation function library loaded from XML does not match the genome factory's activation function library.");
            }

            for(int i=0; i<factoryActivationFnList.Count; i++) 
            {
                if(    (loadedActivationFnList[i].Id != factoryActivationFnList[i].Id)
                    || (loadedActivationFnList[i].ActivationFunction.FunctionId != factoryActivationFnList[i].ActivationFunction.FunctionId)) {
                    throw new SharpNeatException("The activation function library loaded from XML does not match the genome factory's activation function library.");
                }
            }

            // Initialise the genome factory's genome and innovation ID generators.
            genomeFactory.GenomeIdGenerator.Reset(Math.Max(genomeFactory.GenomeIdGenerator.Peek, maxGenomeId+1));
            genomeFactory.InnovationIdGenerator.Reset(Math.Max(genomeFactory.InnovationIdGenerator.Peek, maxInnovationId+1));

            // Retrospecitively assign the genome factory to the genomes. This is how we overcome the genome/genomeFactory
            // chicken and egg problem.
            foreach(NeatGenome genome in genomeList) {
                genome.GenomeFactory = genomeFactory;
            }

            return genomeList;
        }

        /// <summary>
        /// Reads a NeatGenome from XML.
        /// </summary>
        /// <param name="xr">The XmlReader to read from.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. They are required
        /// for HyperNEAT genomes but not for NEAT</param>
        public static NeatGenome ReadGenome(XmlReader xr, bool nodeFnIds)
        {
            // Find <Network>.
            XmlIoUtils.MoveToElement(xr, false, __ElemNetwork);
            int initialDepth = xr.Depth;

            // Read genome ID attribute if present. Otherwise default to zero; it's the caller's responsibility to 
            // check IDs are unique and in-line with the genome factory's ID generators.
            string genomeIdStr = xr.GetAttribute(__AttrId);
            uint genomeId;
            uint.TryParse(genomeIdStr, out genomeId);

            // Read birthGeneration attribute if present. Otherwise default to zero.
            string birthGenStr = xr.GetAttribute(__AttrBirthGeneration);
            uint birthGen;
            uint.TryParse(birthGenStr, out birthGen);

            // Find <Nodes>.
            XmlIoUtils.MoveToElement(xr, true, __ElemNodes);
            
            // Create a reader over the <Nodes> sub-tree.
            int inputNodeCount = 0;
            int outputNodeCount = 0;
            NeuronGeneList nGeneList = new NeuronGeneList();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root <Nodes> element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first node elem.
                XmlIoUtils.MoveToElement(xrSubtree, true, __ElemNode);

                // Read node elements.
                do
                {
                    NodeType neuronType = NetworkXmlIO.ReadAttributeAsNodeType(xrSubtree, __AttrType);
                    uint id = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrId);
                    int functionId = 0;
                    double[] auxState = null;
                    if(nodeFnIds) 
                    {	// Read activation fn ID.
                        functionId = XmlIoUtils.ReadAttributeAsInt(xrSubtree, __AttrActivationFunctionId);

                        // Read aux state as comma seperated list of real values.
                        auxState = XmlIoUtils.ReadAttributeAsDoubleArray(xrSubtree, __AttrAuxState);
                    }

                    NeuronGene nGene = new NeuronGene(id, neuronType, functionId, auxState);
                    nGeneList.Add(nGene);

                    // Track the number of input and output nodes.
                    switch(neuronType)
                    {
                        case NodeType.Input:
                            inputNodeCount++;
                            break;
                        case NodeType.Output:
                            outputNodeCount++;
                            break;
                    }
                } 
                while(xrSubtree.ReadToNextSibling(__ElemNode));
            }

            // Find <Connections>.
            XmlIoUtils.MoveToElement(xr, false, __ElemConnections);

            // Create a reader over the <Connections> sub-tree.
            ConnectionGeneList cGeneList = new ConnectionGeneList();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root <Connections> element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first connection elem.
                string localName = XmlIoUtils.MoveToElement(xrSubtree, true);
                if(localName == __ElemConnection)
                {   // We have at least one connection.
                    // Read connection elements.
                    do
                    {
                        uint id = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrId);
                        uint srcId = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrSourceId);
                        uint tgtId = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrTargetId);
                        double weight = XmlIoUtils.ReadAttributeAsDouble(xrSubtree, __AttrWeight);
                        ConnectionGene cGene = new ConnectionGene(id, srcId, tgtId, weight);
                        cGeneList.Add(cGene);
                    } 
                    while(xrSubtree.ReadToNextSibling(__ElemConnection));
                }
            }

            // Move the reader beyond the closing tags </Connections> and </Network>.
            do
            {
                if (xr.Depth <= initialDepth) {
                    break;
                }
            }
            while(xr.Read());

            // Construct and return loaded NeatGenome.
            return new NeatGenome(null, genomeId, birthGen, nGeneList, cGeneList, inputNodeCount, outputNodeCount, true);
        }

        #endregion
    }
}
