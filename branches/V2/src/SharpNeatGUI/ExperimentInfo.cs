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
using System.Xml;

namespace SharpNeatGUI
{
    /// <summary>
    /// Stores experiment class info. Used by problem domain drop down combobox in GUI to hold details of
    /// how to instantiate selected experiments.
    /// </summary>
    public class ExperimentInfo
    {
        string _name;
        string _assemblyPath;
        string _className;
        XmlElement _xmlConfig;

        #region Constructor

        /// <summary>
        /// Constructs with the provided experiment info.
        /// </summary>
        public ExperimentInfo(string name, string assemblyPath, string className, XmlElement xmlConfig)
        {
            _name = name;
            _assemblyPath = assemblyPath;
            _className = className;
            _xmlConfig = xmlConfig;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Experiment name (for display purposes).
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The assembly that contains the experiment's code.
        /// </summary>
        public string AssemblyPath
        {
            get { return _assemblyPath; }
        }

        /// <summary>
        /// Name of the class implementing INeatExperiment for this experiment.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// Optional XML configuration for the experiment. Originates from the experiments.xml file.
        /// </summary>
        public XmlElement XmlConfig
        {
            get { return _xmlConfig; }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Read an experiments XML file and return the parsed experiments as a list of ExperimentInfo objects.
        /// </summary>
        public static List<ExperimentInfo> ReadExperimentXml(string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            return ReadExperimentXml(xmlDoc);
        }

        /// <summary>
        /// Read an experiments XML file and return the parsed experiments as a list of ExperimentInfo objects.
        /// </summary>
        public static List<ExperimentInfo> ReadExperimentXml(XmlDocument xmlDoc)
        {
            XmlNodeList experimentNodeList = xmlDoc.SelectNodes("/Experiments/Experiment");
            List<ExperimentInfo> experimentInfoList = new List<ExperimentInfo>(experimentNodeList.Count);

            foreach(XmlElement expElem in experimentNodeList)
            {
                string name = expElem.GetAttribute("name");
                XmlElement assemblyPathElem = expElem.SelectSingleNode("AssemblyPath") as XmlElement;
                XmlElement classNameElem = expElem.SelectSingleNode("ClassName") as XmlElement;
                XmlElement xmlConfig = expElem.SelectSingleNode("Config") as XmlElement;
                if(null == assemblyPathElem) {
                    throw new ApplicationException("Experiment XML has missing AssemblyPath element");
                }
                if(null == classNameElem) {
                    throw new ApplicationException("Experiment XML has missing ClassName element");
                }

                ExperimentInfo expInfo = new ExperimentInfo(name, assemblyPathElem.InnerText.Trim(), classNameElem.InnerText.Trim(), xmlConfig);
                experimentInfoList.Add(expInfo);
            }
            return experimentInfoList;
        }

        #endregion
    }
}
