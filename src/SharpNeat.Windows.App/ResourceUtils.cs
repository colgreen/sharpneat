// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Reflection;

namespace SharpNeat.Windows.App;

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
