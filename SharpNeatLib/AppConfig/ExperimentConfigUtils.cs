using System;
using System.Configuration;

namespace SharpNeatLib.AppConfig
{
	public class ExperimentConfigUtils
	{
		const string EXPERIMENT_CATALOG_NAME = "experimentCatalog";

		public static ExperimentConfigInfo[] ReadExperimentConfigCatalog()
		{
			return (ExperimentConfigInfo[])ConfigurationSettings.GetConfig(EXPERIMENT_CATALOG_NAME);
		}
	}
}
