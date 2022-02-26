/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
namespace SharpNeat.Windows.App;

/// <summary>
/// Miscellaneous utility objects and methods.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Gets a <see cref="Comparer{double}"/> that can be used to sort items in descending order.
    /// </summary>
    public static Comparer<double> ComparerDesc => Comparer<double>.Create(
                delegate (double x, double y)
                {
                    if(x > y) { return -1; }
                    if(x < y) { return 1; }
                    return 0;
                });
}
