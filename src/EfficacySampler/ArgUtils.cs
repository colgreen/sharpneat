using System;

namespace EfficacySampler
{
    public static class ArgUtils
    {
        public static StopCondition? ReadArgs(
            string[] args,
            out string? experimentId,
            out string? filename)
        {
            experimentId = null;
            filename = null;
            if(args.Length == 1 && args[0] == "sysinfo")
            {
                SysInfo.DumpSystemInfo();
                return null;
            }

            if(args.Length == 4)
            {
                experimentId = args[0];
                StopCondition? sc = ReadStopCondition(args[1], args[2]);

                // output filename
                filename = args[3];
                return sc;
            }

            PrintHelp();
            return null;
        }

        #region Private Static Methods

        private static StopCondition? ReadStopCondition(string type, string valStr)
        {

            StopCondition sc = new();
            switch(type.ToLowerInvariant())
            {
                case "secs":
                    sc.StopConditionType = StopConditionType.ElapsedClockTime;
                    break;
                case "gens":
                    sc.StopConditionType = StopConditionType.GenerationCount;
                    break;
                default:
                    Console.WriteLine($"Invalid stop condition type [{type}]");
                    return null;
            }

            if(!int.TryParse(valStr, out int val) || val <= 0)
            {
                Console.WriteLine($"Invalid stop condition value [${valStr}]");
                return null;
            }
            sc.Value = val;
            return sc;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Format is:");
            Console.WriteLine("  efic {experiment} secs {n} {outputfilename}");
            Console.WriteLine("  efic {experiment} gens {n} {outputfilename}");
            Console.WriteLine("  efic sysinfo");

            Console.WriteLine("");
            Console.WriteLine("  Experiment options are:  ");
            Console.WriteLine("    binary11");
            Console.WriteLine("    sinewave");
        }

        #endregion
    }
}
