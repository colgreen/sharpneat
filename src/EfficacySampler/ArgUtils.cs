using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficacySampler
{
    public static class ArgUtils
    {
        public static StopCondition ReadArgs(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("Format is:");
                Console.WriteLine("  sampler secs {n}");
                Console.WriteLine("  sampler gens {n}");
                return null;
            }

            StopCondition sc = new StopCondition();
            switch(args[0].ToLowerInvariant())
            {
                case "secs":
                    sc.StopConditionType = StopConditionType.ElapsedClockTime;
                    break;
                case "gens":
                    sc.StopConditionType = StopConditionType.GenerationCount;
                    break;
                default:
                    Console.WriteLine($"Invalid stop condition type [{args[0]}]");
                    return null;
            }

            int val;
            if(!int.TryParse(args[1], out val) || val <= 0)
            {
                Console.WriteLine($"Invalid stop condition value [${args[1]}]");
                return null;
            }
            sc.Value = val;
            return sc;
        }
    }
}
