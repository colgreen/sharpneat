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
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace SharpNeat.Utility
{
    /// <summary>
    /// Static helper methods for XML IO.
    /// </summary>
    public static class XmlIoUtils
    {
        #region Public Static Methods [Low-level XML Parsing]

        /// <summary>
        /// Read from the XmlReader until we encounter an element. If the name doesn't match
        /// elemName then throw an exception, else return normally.
        /// 
        /// </summary>
        public static void MoveToElement(XmlReader xr, bool skipCurrent, string elemName)
        {
            string localName = MoveToElement(xr, skipCurrent);            
            if(localName != elemName)
            {   // No element or unexpected element.
                throw new InvalidDataException(string.Format("Expected element [{0}], encountered [{1}]", elemName, localName));
            }
        }

        /// <summary>
        /// Read from the XmlReader until we encounter an element. 
        /// Return the Local name of the element or null if no element was found before 
        /// the end of the input.
        /// </summary>
        public static string MoveToElement(XmlReader xr, bool skipCurrent)
        {
            // Optionally skip the current node.
            if(skipCurrent) 
            {
                if(!xr.Read())
                {   // EOF.
                    return null;
                }
            }

            // Keep reading until we encounter an element.
            do
            {
                if(XmlNodeType.Element == xr.NodeType) {
                    return xr.LocalName;
                }
            } while(xr.Read());

            // No element encountered.
            return null;
        }

        /// <summary>
        /// Read the named attribute and parse its string value as an integer.
        /// </summary>
        public static int ReadAttributeAsInt(XmlReader xr, string attrName)
        {
            string valStr = xr.GetAttribute(attrName);
            int val;
            if(!int.TryParse(valStr, out val)) {
                throw new InvalidDataException(string.Format("Failed to parse string as integer [{0}]", valStr));
            }
            return val;
        }

        /// <summary>
        /// Read the named attribute and parse its string value as a uint.
        /// </summary>
        public static uint ReadAttributeAsUInt(XmlReader xr, string attrName)
        {
            string valStr = xr.GetAttribute(attrName);
            uint val;
            if(!uint.TryParse(valStr, out val)) {
                throw new InvalidDataException(string.Format("Failed to parse string as integer [{0}]", valStr));
            }
            return val;
        }

        /// <summary>
        /// Read the named attribute and parse its string value as a double.
        /// </summary>
        public static double ReadAttributeAsDouble(XmlReader xr, string attrName)
        {
            string valStr = xr.GetAttribute(attrName);
            double val;
            if(!double.TryParse(valStr, out val)) {
                throw new InvalidDataException(string.Format("Failed to parse string as double [{0}]", valStr));
            }
            return val;
        }

        /// <summary>
        /// Read the named attribute and parse its string value as an array of doubles.
        /// </summary>
        public static double[] ReadAttributeAsDoubleArray(XmlReader xr, string attrName)
        {
            string valStr = xr.GetAttribute(attrName);
            if(string.IsNullOrEmpty(valStr)) {
                return null;
            }

            // Parse comma separated values.
            string[] strArr = valStr.Split(',');
            double[] dblArr = new double[strArr.Length];
            for(int i=0; i<strArr.Length; i++) {
                if(!double.TryParse(strArr[i], out dblArr[i])) {
                    throw new InvalidDataException(string.Format("Failed to parse string as comma separated double array [{0}]", valStr));
                }    
            }
            return dblArr;
        }

        /// <summary>
        /// Writes a double array as a comma separated list of values.
        /// </summary>
        public static void WriteAttributeString(XmlWriter xw, string attrName, double[] arr)
        {
            if(null == arr || arr.Length == 0) {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(arr[0].ToString("R"));
            for(int i=1; i<arr.Length; i++)
            {
                sb.Append(',');
                sb.Append(arr[i].ToString("R"));
            }
            xw.WriteAttributeString(attrName, sb.ToString());
        }

        #endregion

    }
}
