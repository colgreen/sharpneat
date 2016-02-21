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

namespace SharpNeat
{
    /// <summary>
    /// General purpose exception class for use within SharpNeat.
    /// </summary>
    public class SharpNeatException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the SharpNeatException class.
        /// </summary>
        public SharpNeatException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SharpNeatException class with a specified error message. 
        /// </summary>
        public SharpNeatException(string message) : base(message)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the SharpNeatException class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception. 
        /// </summary>
        public SharpNeatException(string message, System.Exception innerException) : base(message, innerException)
        {   
        }
    }
}
