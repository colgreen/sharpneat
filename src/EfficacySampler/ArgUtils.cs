using System;

namespace EfficacySampler
{
    public static class ArgUtils
    {
        public static StopCondition ReadArgs(string[] args, out string experimentId, out string filename)
        {
            if(args.Length != 4)
            {
                Console.WriteLine("Format is:");
                Console.WriteLine("  efic {experiment} secs {n} {outputfilename}");
                Console.WriteLine("  efic {experiment} gens {n} {outputfilename}");

                Console.WriteLine("");
                Console.WriteLine("  Experiment options are:  ");
                Console.WriteLine("    binary11");
                Console.WriteLine("    inverted");
                experimentId = null;
                filename = null;
                return null;
            }

            experimentId = args[0];
            StopCondition sc = ReadStopCondition(args[1], args[2]);

            // output filename
            filename = args[3];
            return sc;
        }

        private static StopCondition ReadStopCondition(string type, string valStr)
        {

            StopCondition sc = new StopCondition();
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

            int val;
            if(!int.TryParse(valStr, out val) || val <= 0)
            {
                Console.WriteLine($"Invalid stop condition value [${valStr}]");
                return null;
            }
            sc.Value = val;
            return sc;
        }
    }
}
