/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
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
using System.Drawing;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// Interface for types that position nodes of an IOGraph within a specified 2D layout area.
    /// </summary>
    public interface ILayoutManager
    {
        /// <summary>
        /// Position/layout the nodes of an IOGraph within a specified 2D layout area.
        /// </summary>
        /// <param name="graph">The network/graph structure to be layed out.</param>
        /// <param name="layoutArea">The area the structrue is to be layed out on.</param>
        void Layout(IOGraph graph, Size layoutArea);
    }
}
