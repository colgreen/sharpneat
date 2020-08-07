/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Drawing;

namespace SharpNeat.Drawing
{
    /// <summary>
    /// Represents types that paint to a viewport.
    /// </summary>
    public interface IViewportPainter
    {
        /// <summary>
        /// Paints onto the specified viewport.
        /// </summary>
        void Paint(Graphics g, Rectangle viewportArea, float zoomFactor);
    }
}
