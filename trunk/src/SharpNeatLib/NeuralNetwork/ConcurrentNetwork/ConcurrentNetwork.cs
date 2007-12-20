using System;

namespace SharpNeatLib.NeuralNetwork
{
	/// <summary>
	/// A network that simulates a network in real-time. That is, each neuron in the network 
	/// calculates its accumulated input and output from the previous timestep's outputs.
	/// Each neuron then switches to the new 'next timestep' state in unison.
	/// 
	/// This is opposed to an activation traversal network where the output signal updates
	/// are updated by a traversal algorithm that follows the network's connections.
	/// </summary>
	public class ConcurrentNetwork : AbstractNetwork
	{
		#region Constructor
		
		public ConcurrentNetwork(NeuronList neuronList)
		: base(neuronList)
		{
		}

		#endregion

		#region INetwork 

		public override void SingleStep()
		{
			int loopBound = masterNeuronList.Count;
			for(int j=0; j<loopBound; j++)
				masterNeuronList[j].Recalc();
			
			for(int j=0; j<loopBound; j++)
				masterNeuronList[j].UseRecalculatedValue();
		}

		public override void MultipleSteps(int numberOfSteps)
		{
			for(int i=0; i<numberOfSteps; i++)
				SingleStep();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSteps">The number of timesteps to run the network before we give up.</param>
		/// <param name="maxAllowedSignalDelta"></param>
		/// <returns>False if the network did not relax. E.g. due to oscillating signals.</returns>
		public override bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta)
		{
			// Perform at least one step.
			SingleStep();

			// Now perform steps until the network is relaxed or maxSteps is reached.
			int loopBound;
			bool isRelaxed=false;
			for(int i=0; i<maxSteps && !isRelaxed; i++)
			{	
				isRelaxed=true;	// Assume true.

				// foreach syntax is 30% slower then this!
				loopBound = masterNeuronList.Count;
				for(int j=0; j<loopBound; j++)
				{
					Neuron neuron = masterNeuronList[j];
					neuron.Recalc();

					// If this flag is set then keep testing neurons. Otherwise there is no need to
					// keep testing.
					if(isRelaxed)
					{
						if(neuron.NeuronType == NeuronType.Hidden || neuron.NeuronType == NeuronType.Output)
						{
							if(neuron.OutputDelta > maxAllowedSignalDelta)
								isRelaxed=false;
						}
					}
				}

				for(int j=0; j<loopBound; j++)
					masterNeuronList[j].UseRecalculatedValue();
			}

			return isRelaxed;
		}

		#endregion
	}
}
