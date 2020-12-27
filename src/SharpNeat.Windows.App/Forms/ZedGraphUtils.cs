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
using ZedGraph;

namespace SharpNeat.Windows.App.Forms
{
    internal static class ZedGraphUtils
    {
        /// <summary>
        /// Apply a consistent style to the provided ZedGraph line item.
        /// </summary>
        /// <param name="lineItem">Th eline item.</param>
        public static void ApplyLineStyle(LineItem lineItem)
        {
            lineItem.Line.IsAntiAlias = true;
            lineItem.Line.Width = 2f;
        }
    }
}
