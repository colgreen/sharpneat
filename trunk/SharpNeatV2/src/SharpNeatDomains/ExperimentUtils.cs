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
using System.Xml;
using SharpNeat.Decoders;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using System.Threading.Tasks;

namespace SharpNeat.Domains
{
    /// <summary>
    /// Static helper methods for experiment initialization.
    /// </summary>
    public static class ExperimentUtils
    {
        /// <summary>
        /// Create a network activation scheme from the scheme setting in the provided config XML.
        /// </summary>
        /// <returns></returns>
        public static NetworkActivationScheme CreateActivationScheme(XmlElement xmlConfig, string activationElemName)
        {
            // Get root activation element.
            XmlNodeList nodeList = xmlConfig.GetElementsByTagName(activationElemName, "");
            if(nodeList.Count != 1) {
                throw new ArgumentException("Missing or invalid activation XML config setting.");
            }

            XmlElement xmlActivation = nodeList[0] as XmlElement;
            string schemeStr = XmlUtils.TryGetValueAsString(xmlActivation, "Scheme");
            switch(schemeStr)
            {
                case "FixedIters":
                    int iters = XmlUtils.GetValueAsInt(xmlActivation, "Iters");
                    return new NetworkActivationScheme(iters);
                case "Relax":
                    double deltaThreshold = XmlUtils.GetValueAsInt(xmlActivation, "Threshold");
                    int maxIters = XmlUtils.GetValueAsInt(xmlActivation, "MaxIters");
                    return new NetworkActivationScheme(deltaThreshold, maxIters);
            }
            throw new ArgumentException(string.Format("Invalid or missing ActivationScheme XML config setting [{0}]", schemeStr));
        }

        /// <summary>
        /// Create a complexity regulation strategy based on the provided XML config values.
        /// </summary>
        public static IComplexityRegulationStrategy CreateComplexityRegulationStrategy(string strategyTypeStr, int? threshold)
        {
            ComplexityCeilingType ceilingType;
            if(!Enum.TryParse<ComplexityCeilingType>(strategyTypeStr, out ceilingType)) {
                return new NullComplexityRegulationStrategy();
            }

            if(null == threshold) {
                throw new ArgumentNullException("threshold", string.Format("threshold must be provided for complexity regulation strategy type [{0}]", ceilingType));
            }

            return new DefaultComplexityRegulationStrategy(ceilingType, threshold.Value);
        }

        /// <summary>
        /// Read Parallel Extensions options from config XML.
        /// </summary>
        /// <param name="xmlConfig"></param>
        /// <returns></returns>
        public static ParallelOptions ReadParallelOptions(XmlElement xmlConfig)
        {
            // Get parallel options.
            ParallelOptions parallelOptions;
            int? maxDegreeOfParallelism = XmlUtils.TryGetValueAsInt(xmlConfig, "MaxDegreeOfParallelism");
            if(null != maxDegreeOfParallelism) {
                parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism.Value };
            } else {
                parallelOptions = new ParallelOptions();
            }
            return parallelOptions;
        }
    }
}
