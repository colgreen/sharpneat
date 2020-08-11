using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SharpNeat.Experiments;

namespace SharpNeat.Windows.App
{
    internal static class AppUtils
    {

        public static List<INeatExperimentFactory> ScanAssembliesForNeatExperiments()
        {
            var list = new List<INeatExperimentFactory>();            
            string[] taskAssemblyFilenames = Directory.GetFiles(Environment.CurrentDirectory, "*.Tasks.dll");

            foreach(string taskAssemblyFilename in taskAssemblyFilenames) {
                ScanAssembliesForNeatExperiments(list, taskAssemblyFilename);
            }

            //list.Sort((x, y) => x.
            //    );


            return list;
        }

        private static void ScanAssembliesForNeatExperiments(
            IList<INeatExperimentFactory> targetList,
            string assemblyFilename)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFilename);
            foreach(Type type in assembly.DefinedTypes)
            {
                if(typeof(INeatExperimentFactory).IsAssignableFrom(type))
                {
                    INeatExperimentFactory experimentfactory = (INeatExperimentFactory)Activator.CreateInstance(type);
                    targetList.Add(experimentfactory);

                }
            }
        }
    }
}
