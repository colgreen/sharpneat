using System.Runtime.InteropServices;
using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.RAM;
using SystemInfoLibrary.OperatingSystem;

namespace EfficacySampler;

public class SysInfo
{
    public static void DumpSystemInfo()
    {
        OperatingSystemInfo osInfo = OperatingSystemInfo.GetOperatingSystemInfo();
        Console.WriteLine("");
        Console.WriteLine("---------------------------------");
        Console.WriteLine($"OS Name: {osInfo.Name}");
        Console.WriteLine($"Architecture: {osInfo.Architecture}");
        Console.WriteLine($"dotnet RuntimeInformation.FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
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
}

