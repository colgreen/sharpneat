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
// Disable comment warnings for trivial class.
#pragma warning disable 1591

using System;

namespace SharpNeat.Domains.FunctionRegression
{
    public class SineFunction : IFunction
    {
        public double GetValue(double[] args)
        {
            return (Math.Sin(args[0])*0.4)+0.5;
        }

        public int InputCount
        {
            get { return 1; }
        }
    }
}
