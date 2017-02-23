using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using log4net.Config;
using SharpNeat.Core;
using SharpNeat.Domains.BinaryElevenMultiplexer;
using SharpNeat.Domains.Xor;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace EfficacySampler
{
    class Program
    {

        




        static void Main(string[] args)
        {
            StopCondition stopCond = ArgUtils.ReadArgs(args);
            if(null == stopCond) {
                return;
            }

            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));



            EvolutionAlgorithmHost eaHost = new EvolutionAlgorithmHost(stopCond);

            for(;;)
            {
                double fitness = eaHost.Sample();
                Console.WriteLine(fitness);
            }  
        }
    }
}
