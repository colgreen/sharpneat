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

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// Represents types that decode genomes into phenomes.
    /// </summary>
    /// <typeparam name="TGenome">The genome type to be decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to.</typeparam>
    public interface IGenomeDecoder<TGenome,TPhenome>
        where TPhenome : class
    {
        /// <summary>
        /// Decodes a genome into a phenome.
        /// </summary>
        /// <remarks>
        /// Note that not all genomes have to decode successfully. That is, we support genetic representations
        /// that may produce non-viable offspring. In such cases this method can return a null.
        /// </remarks>
        TPhenome Decode(TGenome genome);
    }
}
