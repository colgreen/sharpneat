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
namespace SharpNeat.Core
{
    /// <summary>
    /// Generic interface for classes that decode genomes into phenomes.
    /// </summary>
    /// <typeparam name="TGenome">The genome type to be decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to.</typeparam>
    public interface IGenomeDecoder<TGenome,TPhenome>
    {
        /// <summary>
        /// Decodes a genome into a phenome. Note that not all genomes have to decode successfully. That is, we 
        /// support genetic representations that may produce non-viable offspring. In such cases this method
        /// can return a null.
        /// </summary>
        TPhenome Decode(TGenome genome);
    }
}
