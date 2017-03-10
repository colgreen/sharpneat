/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
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
                throw new ArgumentException($"Missing [{elemName}] configuration setting.");
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
                throw new ArgumentException($"Missing [{elemName}] configuration setting.");
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
                throw new ArgumentException($"Missing [{elemName}] configuration setting.");
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
                throw new ArgumentException($"Missing [{elemName}] configuration setting.");
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
