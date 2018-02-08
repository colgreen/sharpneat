using System;
using System.IO;
using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.RAM;
using SystemInfoLibrary.OperatingSystem;

namespace EfficacySampler
{
    public class SysInfo
    {
        public static void DumpSystemInfo()
        {
            OperatingSystemInfo osInfo = OperatingSystemInfo.GetOperatingSystemInfo();
            Console.WriteLine("");
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"OS Name: {osInfo.Name}");
            Console.WriteLine($"Architecture: {osInfo.Architecture}");
            Console.WriteLine($".NET Framework: {GetCurrentFrameworkVersionBasedOnWindowsRegistry()} (CLR {osInfo.FrameworkVersion})");
            Console.WriteLine("");

            // CPUs
            int idx = 0;
            foreach(CPUInfo cpuInfo in osInfo.Hardware.CPUs)
            {
                Console.WriteLine($"CPU {idx}");
                Console.WriteLine($"   Brand: {cpuInfo.Brand}");
                Console.WriteLine($"   Name: {cpuInfo.Name}");
                Console.WriteLine($"   Architecture: {cpuInfo.Architecture}");
                Console.WriteLine($"   Cores: {cpuInfo.Cores}");
                Console.WriteLine($"   Frequency: {cpuInfo.Frequency}");                    
                idx++;
            }

            RAMInfo ramInfo = osInfo.Hardware.RAM;
            Console.WriteLine("");
            Console.WriteLine($"RAM {ramInfo.Total:#,###} kilobytes");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("");
        }


        private static string GetCurrentFrameworkVersionBasedOnWindowsRegistry()
        {   
            // Bonkers logic for determining what version of the .NET Framework is installed.
            // Taken from: https://github.com/dotnet/BenchmarkDotNet/blob/c6bbda4e1eb69218e7981d8aa2b0d59344c5ebaf/src/BenchmarkDotNet.Core/Toolchains/CsProj/CsProjClassicNetToolchain.cs

            using (var ndpKey = Microsoft.Win32.RegistryKey
                .OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null)
                {
                    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                    // magic numbers come from https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx
                    if (releaseKey >= 461308 && Directory.Exists(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.1"))
                        return "4.7.1";
                    if (releaseKey >= 460798 && Directory.Exists(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7"))
                        return "4.7";
                    if (releaseKey >= 394802 && Directory.Exists(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"))
                        return "4.6.2";
                    if (releaseKey >= 394254 && Directory.Exists(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1"))
                        return "4.6.1";
                }
                return "Unknown";
            }
        }

    }
}
