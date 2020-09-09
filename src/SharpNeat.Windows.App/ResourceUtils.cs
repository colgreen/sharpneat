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
using System.IO;
using System.Reflection;

namespace SharpNeat.Windows.App
{
    internal static class ResourceUtils
    {
        public static string ReadStringResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using(Stream inputStream = assembly.GetManifestResourceStream(name))
            using(var sr = new StreamReader(inputStream))
            {
                return sr.ReadToEnd();
            }
        }

        public static Bitmap ReadBitmapResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using(Stream inputStream = assembly.GetManifestResourceStream(name))
            using(var sr = new StreamReader(inputStream))
            {
                return new Bitmap(inputStream);
            }
        }
    }
}
