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

namespace SharpNeat.Domains
{
    /// <summary>
    /// Static helper methods for reading value from XML configuration data in DOM form.
    /// </summary>
    public class XmlUtils
    {
        /// <summary>
        /// Parse the inner text of element with the given name as an integer. If element is missing or parsing fails then
        /// throws an ArgumentException.
        /// </summary>
        public static int GetValueAsInt(XmlElement xmlParent, string elemName)
        {
            int? val = TryGetValueAsInt(xmlParent, elemName);
            if(null == val) {
                throw new ArgumentException(string.Format("Missing [{0}] configuration setting.", elemName));
            }
            return val.Value;
        }

        /// <summary>
        /// Parse the inner text of element with the given name as an integer. If element is missing or parsing fails then
        /// returns null.
        /// </summary>
        public static int? TryGetValueAsInt(XmlElement xmlParent, string elemName)
        {
            XmlElement xmlElem = xmlParent.SelectSingleNode(elemName) as XmlElement;
            if(null == xmlElem) {
                return null;
            }

            string valStr = xmlElem.InnerText;
            if(string.IsNullOrEmpty(valStr)) {
                return null;
            }

            int result;
            if(int.TryParse(valStr, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Parse the inner text of element with the given name as a double. If element is missing or parsing fails then
        /// throws an ArgumentException.
        /// </summary>
        public static double GetValueAsDouble(XmlElement xmlParent, string elemName)
        {
            double? val = TryGetValueAsDouble(xmlParent, elemName);
            if(null == val) {
                throw new ArgumentException(string.Format("Missing [{0}] configuration setting.", elemName));
            }
            return val.Value;
        }

        /// <summary>
        /// Parse the inner text of element with the given name as a double. If element is missing or parsing fails then
        /// returns null.
        /// </summary>
        public static double? TryGetValueAsDouble(XmlElement xmlParent, string elemName)
        {
            XmlElement xmlElem = xmlParent.SelectSingleNode(elemName) as XmlElement;
            if(null == xmlElem) {
                return null;
            }

            string valStr = xmlElem.InnerText;
            if(string.IsNullOrEmpty(valStr)) {
                return null;
            }

            double result;
            if(double.TryParse(valStr, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Parse the inner text of element with the given name as a boolean. If element is missing or parsing fails then
        /// throws an ArgumentException.
        /// </summary>
        public static bool GetValueAsBool(XmlElement xmlParent, string elemName)
        {
            bool? val = TryGetValueAsBool(xmlParent, elemName);
            if(null == val) {
                throw new ArgumentException(string.Format("Missing [{0}] configuration setting.", elemName));
            }
            return val.Value;
        }

        /// <summary>
        /// Parse the inner text of element with the given name as a boolean. If element is missing or parsing fails then
        /// returns null.
        /// </summary>
        public static bool? TryGetValueAsBool(XmlElement xmlParent, string elemName)
        {
            XmlElement xmlElem = xmlParent.SelectSingleNode(elemName) as XmlElement;
            if(null == xmlElem) {
                return null;
            }

            string valStr = xmlElem.InnerText;
            if(string.IsNullOrEmpty(valStr)) {
                return null;
            }

            bool result;
            if(bool.TryParse(valStr, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Read the inner text of element with the given name. If element is missing then throws an ArgumentException.
        /// </summary>
        public static string GetValueAsString(XmlElement xmlParent, string elemName)
        {
            string val = TryGetValueAsString(xmlParent, elemName);
            if(null == val) {
                throw new ArgumentException(string.Format("Missing [{0}] configuration setting.", elemName));
            }
            return val;
        }

        /// <summary>
        /// Read the inner text of element with the given name. If element is missing then returns null.
        /// </summary>
        public static string TryGetValueAsString(XmlElement xmlParent, string elemName)
        {
            XmlElement xmlElem = xmlParent.SelectSingleNode(elemName) as XmlElement;
            if(null == xmlElem) {
                return null;
            }

            string valStr = xmlElem.InnerText;
            if(string.IsNullOrEmpty(valStr)) {
                return null;
            }
            return valStr;
        }
    }
}
