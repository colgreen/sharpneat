using System;
using System.Collections;
using System.Reflection;

namespace SharpNeatLib.NeuralNetwork
{
	public class ActivationFunctionFactory
	{
		public static Hashtable activationFunctionTable = new Hashtable();

		public static IActivationFunction GetActivationFunction(string functionId)
		{
			IActivationFunction activationFunction = (IActivationFunction)ActivationFunctionFactory.activationFunctionTable[functionId];
			if(activationFunction==null)
			{
				activationFunction = CreateActivationFunction(functionId);
				activationFunctionTable.Add(functionId, activationFunction);
			}
			return activationFunction;
		}

		private static IActivationFunction CreateActivationFunction(string functionId)
		{
			// For now the function ID is the name of a class that implements IActivationFunction.
			string className = typeof(ActivationFunctionFactory).Namespace + '.' + functionId;
			return (IActivationFunction)Assembly.GetExecutingAssembly().CreateInstance(className);
		}
	}
}
