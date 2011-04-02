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
using System.IO;
using System.Xml;
using SharpNeat.Utility;

namespace SharpNeat.Network
{
    /// <summary>
    /// Static class for reading and writing Network Definitions(s) to and from XML.
    /// </summary>
    public static class NetworkXmlIO
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
        const string __ElemActivationFn = "Fn";

        const string __AttrId = "id";
        const string __AttrName = "name";
        const string __AttrType = "type";
        const string __AttrSourceId = "src";
        const string __AttrTargetId = "tgt";
        const string __AttrWeight = "wght";
        const string __AttrActivationFunctionId = "fnId";
        const string __AttrProbability = "prob";

        #endregion

        #region Public Static Methods [Save to XmlDocument]

        /// <summary>
        /// Writes a single NetworkDefinition to XML within a containing 'Root' element and the activation function
        /// library that the genome is associated with.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="networkDef">The NetworkDefinition to save.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument SaveComplete(NetworkDefinition networkDef, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                WriteComplete(xw, networkDef, nodeFnIds);
            }
            return doc;
        }

        /// <summary>
        /// Writes a list of NetworkDefinition(s) to XML within a containing 'Root' element and the activation
        /// function library that the genomes are associated with.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="networkDefList">List of genomes to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument SaveComplete(IList<NetworkDefinition> networkDefList, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                WriteComplete(xw, networkDefList, nodeFnIds);
            }
            return doc;
        }

        /// <summary>
        /// Writes a single NetworkDefinition to XML.
        /// The XML is returned as a newly created XmlDocument.
        /// </summary>
        /// <param name="networkDef">The genome to save.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static XmlDocument Save(NetworkDefinition networkDef, bool nodeFnIds)
        {
            XmlDocument doc = new XmlDocument();
            using(XmlWriter xw = doc.CreateNavigator().AppendChild())
            {
                Write(xw, networkDef, nodeFnIds);
            }
            return doc;
        }

        #endregion

        #region Public Static Methods [Load from XmlDocument]

        /// <summary>
        /// Reads a list of NetworkDefinition(s) from XML that has a containing 'Root' element. The root element 
        /// also contains the activation function library that the network definitions are associated with.
        /// </summary>
        /// <param name="xmlNode">The XmlNode to read from. This can be an XmlDocument or XmlElement.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. If false then 
        /// all node activation function IDs default to 0.</param>
        public static List<NetworkDefinition> LoadCompleteGenomeList(XmlNode xmlNode, bool nodeFnIds)
        {
            using(XmlNodeReader xr = new XmlNodeReader(xmlNode))
            {
                return ReadCompleteNetworkDefinitionList(xr, nodeFnIds);
            }
        }

        /// <summary>
        /// Reads a NetworkDefinition from XML.
        /// </summary>
        /// <param name="xmlNode">The XmlNode to read from. This can be an XmlDocument or XmlElement.</param>
        /// <param name="activationFnLib">The activation function library used to decode node activation function IDs.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. They are required
        /// for HyperNEAT genomes but not for NEAT</param>
        public static NetworkDefinition ReadGenome(XmlNode xmlNode, IActivationFunctionLibrary activationFnLib, bool nodeFnIds)
        {
            using(XmlNodeReader xr = new XmlNodeReader(xmlNode))
            {
                return ReadNetworkDefinition(xr, activationFnLib, nodeFnIds);
            }
        }

        #endregion

        #region Public Static Methods [Write to XML]

        /// <summary>
        /// Writes a list of INetworkDefinition(s) to XML within a containing 'Root' element and the activation
        /// function library that the genomes are associated with.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="networkDefList">List of network definitions to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void WriteComplete(XmlWriter xw, IList<NetworkDefinition> networkDefList, bool nodeFnIds)
        {
            int count = networkDefList.Count;
            List<INetworkDefinition> tmpList = new List<INetworkDefinition>(count);
            foreach(NetworkDefinition networkDef in networkDefList) {
                tmpList.Add(networkDef);
            }
            WriteComplete(xw, tmpList, nodeFnIds);
        }

        /// <summary>
        /// Writes a list of INetworkDefinition(s) to XML within a containing 'Root' element and the activation
        /// function library that the genomes are associated with.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="networkDefList">List of network definitions to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void WriteComplete(XmlWriter xw, IList<INetworkDefinition> networkDefList, bool nodeFnIds)
        {
            if(networkDefList.Count == 0)
            {   // Nothing to do.
                return;
            }

            // <Root>
            xw.WriteStartElement(__ElemRoot);

            // Write activation function library from the first network definition 
            // (we expect all networks to use the same library).
            IActivationFunctionLibrary activationFnLib = networkDefList[0].ActivationFnLibrary;
            Write(xw, activationFnLib);

            // <Networks>
            xw.WriteStartElement(__ElemNetworks);

            // Write networks.
            foreach(INetworkDefinition networkDef in networkDefList) {
                Debug.Assert(networkDef.ActivationFnLibrary == activationFnLib);
                Write(xw, networkDef, nodeFnIds);
            }

            // </Networks>
            xw.WriteEndElement();

            // </Root>
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes a single INetworkDefinition to XML within a containing 'Root' element and the activation
        /// function library that the genome is associated with.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="networkDef">Network definition to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void WriteComplete(XmlWriter xw, INetworkDefinition networkDef, bool nodeFnIds)
        {
            // <Root>
            xw.WriteStartElement(__ElemRoot);

            // Write activation function library.
            Write(xw, networkDef.ActivationFnLibrary);

            // <Networks>
            xw.WriteStartElement(__ElemNetworks);

            // Write single network.
            Write(xw, networkDef, nodeFnIds);

            // </Networks>
            xw.WriteEndElement();

            // </Root>
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes an INetworkDefinition to XML.
        /// </summary>
        /// <param name="xw">XmlWriter to write XML to.</param>
        /// <param name="networkDef">Network definition to write as XML.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be emitted. They are required
        /// for HyperNEAT genomes but not for NEAT.</param>
        public static void Write(XmlWriter xw, INetworkDefinition networkDef, bool nodeFnIds)
        {
            xw.WriteStartElement(__ElemNetwork);

            // Emit nodes.
            xw.WriteStartElement(__ElemNodes);
            foreach(INetworkNode node in networkDef.NodeList)
            {
                xw.WriteStartElement(__ElemNode);
                xw.WriteAttributeString(__AttrType, GetNodeTypeString(node.NodeType));
                xw.WriteAttributeString(__AttrId, node.Id.ToString());
                if(nodeFnIds) {
                    xw.WriteAttributeString(__AttrActivationFunctionId, node.ActivationFnId.ToString());
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // Emit connections.
            xw.WriteStartElement(__ElemConnections);
            foreach(INetworkConnection con in networkDef.ConnectionList)
            {
                xw.WriteStartElement(__ElemConnection);
                xw.WriteAttributeString(__AttrSourceId, con.SourceNodeId.ToString());
                xw.WriteAttributeString(__AttrTargetId, con.TargetNodeId.ToString());
                xw.WriteAttributeString(__AttrWeight, con.Weight.ToString("R"));
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // </Network>
            xw.WriteEndElement();
        }

        /// <summary>
        /// Writes an activation function library to XML. This links activation function names to the 
        /// integer IDs used by network nodes, which allows us emit just the ID for each node thus 
        /// resulting in XML that is more compact compared to emitting the activation function name for
        /// each node.
        /// </summary>
        public static void Write(XmlWriter xw, IActivationFunctionLibrary activationFnLib)
        {
            xw.WriteStartElement(__ElemActivationFunctions);
            IList<ActivationFunctionInfo> fnList = activationFnLib.GetFunctionList();
            foreach(ActivationFunctionInfo fnInfo in fnList)
            {
                xw.WriteStartElement(__ElemActivationFn);
                xw.WriteAttributeString(__AttrId, fnInfo.Id.ToString());
                xw.WriteAttributeString(__AttrName, fnInfo.ActivationFunction.FunctionId);
                xw.WriteAttributeString(__AttrProbability, fnInfo.SelectionProbability.ToString("R"));
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        #endregion

        #region Public Static Methods [Read from XML]

        /// <summary>
        /// Reads a list of NetworkDefinition(s) from XML that has a containing 'Root' element. The root 
        /// element also contains the activation function library that the genomes are associated with.
        /// </summary>
        /// <param name="xr">The XmlReader to read from.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. They are required
        /// for HyperNEAT genomes but not NEAT</param>
        public static List<NetworkDefinition> ReadCompleteNetworkDefinitionList(XmlReader xr, bool nodeFnIds)
        {
            // Find <Root>.
            XmlIoUtils.MoveToElement(xr, false, __ElemRoot);

            // Read IActivationFunctionLibrray. 
            XmlIoUtils.MoveToElement(xr, true, __ElemActivationFunctions);
            IActivationFunctionLibrary activationFnLib = ReadActivationFunctionLibrary(xr);
            XmlIoUtils.MoveToElement(xr, false, __ElemNetworks);

            List<NetworkDefinition> networkDefList = new List<NetworkDefinition>();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root <Networks> element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first Network elem.
                XmlIoUtils.MoveToElement(xr, true, __ElemNetwork);
                
                // Read Network elements.
                do
                {
                    NetworkDefinition networkDef = ReadNetworkDefinition(xr, activationFnLib, nodeFnIds);
                    networkDefList.Add(networkDef);
                } 
                while(xrSubtree.ReadToNextSibling(__ElemNetwork));
            }
            return networkDefList;
        }

        /// <summary>
        /// Reads a network definition from XML. 
        /// An activation function library is required to decode the function ID at each node, typically the
        /// library is stored alongside the network definition XML and will have already been read elsewhere and
        /// passed in here.
        /// </summary>
        /// <param name="xr">The XmlReader to read from.</param>
        /// <param name="activationFnLib">The activation function library used to decode node activation function IDs.</param>
        /// <param name="nodeFnIds">Indicates if node activation function IDs should be read. They are required
        /// for HyperNEAT genomes but not NEAT</param>
        public static NetworkDefinition ReadNetworkDefinition(XmlReader xr, IActivationFunctionLibrary activationFnLib, bool nodeFnIds)
        {
            // Find <Network>.
            XmlIoUtils.MoveToElement(xr, false, __ElemNetwork);
            int initialDepth = xr.Depth;

            // Find <Nodes>.
            XmlIoUtils.MoveToElement(xr, true, __ElemNodes);
            
            // Create a reader over the <Nodes> sub-tree.
            int inputNodeCount = 0;
            int outputNodeCount = 0;
            NodeList nodeList = new NodeList();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root <Nodes> element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first node elem.
                XmlIoUtils.MoveToElement(xrSubtree, true, __ElemNode);

                // Read node elements.
                do
                {
                    NodeType nodeType = ReadAttributeAsNodeType(xrSubtree, __AttrType);
                    uint id = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrId);
                    int fnId = 0;
                    if(nodeFnIds) {
                        fnId = XmlIoUtils.ReadAttributeAsInt(xrSubtree, __AttrActivationFunctionId);
                    }

                    NetworkNode node = new NetworkNode(id, nodeType, fnId);
                    nodeList.Add(node);

                    // Track the number of input and output nodes.
                    switch(nodeType)
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
            ConnectionList connList = new ConnectionList();
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
                        uint srcId = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrSourceId);
                        uint tgtId = XmlIoUtils.ReadAttributeAsUInt(xrSubtree, __AttrTargetId);
                        double weight = XmlIoUtils.ReadAttributeAsDouble(xrSubtree, __AttrWeight);
                        NetworkConnection conn = new NetworkConnection(srcId, tgtId, weight);
                        connList.Add(conn);
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

            // Construct and return loaded network definition.
            return new NetworkDefinition(inputNodeCount, outputNodeCount, activationFnLib, nodeList, connList);
        }

        /// <summary>
        /// Reads an IActivationFunctionLibrary from the provided XmlReader.
        /// </summary>
        public static IActivationFunctionLibrary ReadActivationFunctionLibrary(XmlReader xr)
        {
            XmlIoUtils.MoveToElement(xr, false, __ElemActivationFunctions);
            
            // Create a reader over the sub-tree.
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>();
            using(XmlReader xrSubtree = xr.ReadSubtree())
            {
                // Re-scan for the root element.
                XmlIoUtils.MoveToElement(xrSubtree, false);

                // Move to first function elem.
                XmlIoUtils.MoveToElement(xrSubtree, true, __ElemActivationFn);
                
                // Read function elements.
                do
                {
                    int id = XmlIoUtils.ReadAttributeAsInt(xrSubtree, __AttrId);
                    double selectionProb = XmlIoUtils.ReadAttributeAsDouble(xrSubtree, __AttrProbability);
                    string fnName = xrSubtree.GetAttribute(__AttrName);

                    // Lookup function name.
                    IActivationFunction activationFn = GetActivationFunction(fnName);

                    // Add new function to our list of functions.
                    ActivationFunctionInfo fnInfo = new ActivationFunctionInfo(id, selectionProb, activationFn);
                    fnList.Add(fnInfo);
                }
                while(xrSubtree.ReadToNextSibling(__ElemActivationFn));
            }

            // If we have read library items then ensure that their selection probabilities are normalized.
            if(fnList.Count != 0) {
                NormalizeSelectionProbabilities(fnList);
            }            
            return new DefaultActivationFunctionLibrary(fnList);
        }

        #endregion

        #region Public Static Methods [Low-level XML Parsing]

        /// <summary>
        /// Read the named attribute and parse its string value as a NodeType.
        /// </summary>
        public static NodeType ReadAttributeAsNodeType(XmlReader xr, string attrName)
        {
            string valStr = xr.GetAttribute(attrName);
            return GetNodeType(valStr);
        }

        /// <summary>
        /// Gets the NodeType for the specified node type string.
        /// Note. we use our own type strings in place of Enum.ToString() to provide more compact XML.
        /// </summary>
        public static NodeType GetNodeType(string type)
        {
            switch(type)
            {
                case "bias":
                    return NodeType.Bias;
                case "in":
                    return NodeType.Input;
                case "out":
                    return NodeType.Output;
                case "hid":
                    return NodeType.Hidden;
            }
            throw new InvalidDataException(string.Format("Unknown node type [{0}]", type));
        }

        /// <summary>
        /// Gets the node type string for the specified NodeType.
        /// Note. we use our own type strings in place of Enum.ToString() to provide more compact XML.
        /// </summary>
        public static string GetNodeTypeString(NodeType nodeType)
        {
            switch(nodeType)
            {
                case NodeType.Bias:
                    return "bias";
                case NodeType.Input:
                    return "in";
                case NodeType.Output:
                    return "out";
                case NodeType.Hidden:
                    return "hid";
            }
            throw new ArgumentException(string.Format("Unexpected NodeType [{0}]", nodeType));
        }

        /// <summary>
        /// Gets an IActivationFunction from its short name.
        /// </summary>
        public static IActivationFunction GetActivationFunction(string name)
        {
            switch(name)
            {
                case "BipolarGaussian":
                    return BipolarGaussian.__DefaultInstance;
                case "BipolarSigmoid":
                    return BipolarSigmoid.__DefaultInstance;
                case "Linear":
                    return Linear.__DefaultInstance;
                case "Sine":
                    return Sine.__DefaultInstance;
                case "Absolute":
                    return Absolute.__DefaultInstance;
                case "AbsoluteRoot":
                    return AbsoluteRoot.__DefaultInstance;
                case "Gaussian":
                    return Gaussian.__DefaultInstance;
                case "InverseAbsoluteSigmoid":
                    return InverseAbsoluteSigmoid.__DefaultInstance;
                case "PlainSigmoid":
                    return PlainSigmoid.__DefaultInstance;
                case "ReducedSigmoid":
                    return ReducedSigmoid.__DefaultInstance;
                case "SteepenedSigmoid":
                    return SteepenedSigmoid.__DefaultInstance;
                case "SteepenedSigmoidApproximation":
                    return SteepenedSigmoidApproximation.__DefaultInstance;
                case "StepFunction":
                    return StepFunction.__DefaultInstance;
            }
            throw new ArgumentException(string.Format("Unexpected activation function [{0}]", name));
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Normalize the selection probabilities of the provided ActivationFunctionInfo items.
        /// </summary>
        private static void NormalizeSelectionProbabilities(IList<ActivationFunctionInfo> fnList)
        {
            double total = 0.0;
            int count = fnList.Count;
            for(int i=0; i<count; i++) {
                total += fnList[i].SelectionProbability;
            }
            if(Math.Abs(total - 1.0) < 0.0001)
            {   // Probabilities already normalized to within acceptable limits (from rounding errors).
                return;
            }

            // Normalize the probabilities. Note that ActivationFunctionInfo is immutable therefore
            // we replace the existing items.
            for(int i=0; i<count; i++) 
            {
                ActivationFunctionInfo item = fnList[i];
                fnList[i] = new ActivationFunctionInfo(item.Id, item.SelectionProbability/total, item.ActivationFunction);
            }
        }

        #endregion
    }
}
