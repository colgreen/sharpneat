using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This class represents an alternative evluation technique for the 6-multiplexer 
	/// experiment whereby evaluation is performed on a number of key input combinations
	/// to determine fitness instead of evaluating all possible input combinations.
	/// </summary>
	public class BinarySixMultiplexerNetworkEvaluator : INetworkEvaluator
	{
		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			double fitness=0.0;
			bool success=true;

			// Loop address space. For each address test network with all data lines at 0, and
			// again with the addressed line at 1. Allocate maximum of 1.0 for each correct answer,
			// thus max fitness is 8.0.
			for(int a=0; a<4; a++)
			{
				double a0 = (double)(a&0x1)*0.8 + 0.1;
				double a1 = (double)(a&0x2)*0.8 + 0.1;

			//----- Test with all data lines at 0. Response should always be 0.
			//		Set address line values.
                network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);

				// Data lines are 0 to start with so don't bother setting them.
				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				double output = network.GetOutputSignal(0);
				if(output<0.5)
					fitness+=1.0-output;
				else
					success=false;

			//----- Test with addressed data line set to 1. Response should always be 1.
				network.ClearSignals();

				// Set address line values.
				network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);

				// Set data line values.
				network.SetInputSignal(2, a==0?0.9:0.1);
				network.SetInputSignal(3, a==1?0.9:0.1);
				network.SetInputSignal(4, a==2?0.9:0.1);
				network.SetInputSignal(5, a==3?0.9:0.1);

				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				output = network.GetOutputSignal(0);
				if(output>=0.5)
					fitness+=output;
				else
					success=false;
			}

			// If successful on the above 8 test cases then perform thourough test of all
			// 64 possible input signal combinations. 
			if(success)
			{
				// Reset success flag. We are going to re-use it. Reset the fitness, for simplicity
				// we will re-test the above 8 test cases.
				success = true;
				fitness = 0.0;

				// 64 test cases.
				for(int i=0; i<64; i++)
				{
					network.ClearSignals();

					// Apply bitmask to i and shift left to generate the input signals.
					// In addition we scale 0->1 to be 0.1->0.9.
					// Note. We can eliminate all the maths by pre-building a table of test signals. 
					// Using this approach instead makes the code easier to scale up to the 11 and 20 
					// multiplexer problems.
					int tmp=i;
					for(int j=0; j<6; j++) 
					{
						network.SetInputSignal(j, ((tmp&0x1)*0.8)+0.1);
						tmp >>= 1;
					}
								
					// Activate the network.
					network.MultipleSteps(3);

					// Get network's answer.
					double output = network.GetOutputSignal(0);

					// Determine the correct answer by using highly cryptic bit manipulation :)
					// The condition is true if the correct answer is true (1.0).
					if(   ((1<<(2+(i&0x3)))&i)!=0)
					{	// correct answer = true.
						// Assign fitness on sliding scale between 0.0 and 1.0.
						fitness += output;
						if(output<0.5)
							success=false;
					}
					else
					{	// correct answer = false.
						// Assign fitness on sliding scale between 0.0 and 1.0.
						fitness += 1.0-output;
						if(output>=0.5)
							success=false;
					}
				}

				if(success)
					fitness+=1000.0;
			}

			return fitness;
		}

		#endregion
	}
}
