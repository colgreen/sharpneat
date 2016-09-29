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
