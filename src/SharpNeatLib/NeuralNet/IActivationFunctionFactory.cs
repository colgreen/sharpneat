/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.NeuralNet
{
    /// <summary>
    /// An interface that represents factory classes for obtaining instances of IActivationFunction<typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    public interface IActivationFunctionFactory<T> where T : struct
    {
        IActivationFunction<T> GetActivationFunction(string name);
    }
}
