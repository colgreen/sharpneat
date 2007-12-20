using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This class represents an alternative evluation technique for the 11-multiplexer 
	/// experiment whereby evaluation is performed on a number of key input combinations
	/// to determine fitness instead of evaluating all possible input combinations.
	/// </summary>
	public class _BinaryElevenMultiplexerNetworkEvaluator : INetworkEvaluator
	{
		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			double fitness=0.0;
			bool success=true;

			// Loop address space. For each address perform 4 tests (see below),
			// thus max fitness is 8*4=32.0
			for(int a=0; a<8; a++)
			{
				double a0 = (double)(a&0x1)*0.8 + 0.1;
				double a1 = (double)(a&0x2)*0.8 + 0.1;
				double a2 = (double)(a&0x4)*0.8 + 0.1;

			//----- Test with all data lines at 0. Response should always be 0.
				// We assume the network has just been built and therefore doesn't need
				// its signals cleared.

				// Set address line values.
				network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);
				network.SetInputSignal(2, a2);

				// Set data line values.
				network.SetInputSignal(3, 0.1);
				network.SetInputSignal(4, 0.1);
				network.SetInputSignal(5, 0.1);
				network.SetInputSignal(6, 0.1);
				network.SetInputSignal(7, 0.1);
				network.SetInputSignal(8, 0.1);
				network.SetInputSignal(9, 0.1);
				network.SetInputSignal(10, 0.1);

				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				double output = network.GetOutputSignal(0);
				if(output<0.5)
					fitness+=1.0-output;
				else
					success=false;

			//----- Test with all data lines at 1. Response should always be 1.
				network.ClearSignals();

				// Set address line values.
				network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);
				network.SetInputSignal(2, a2);

				// Set data line values.
				network.SetInputSignal(3, 0.9);
				network.SetInputSignal(4, 0.9);
				network.SetInputSignal(5, 0.9);
				network.SetInputSignal(6, 0.9);
				network.SetInputSignal(7, 0.9);
				network.SetInputSignal(8, 0.9);
				network.SetInputSignal(9, 0.9);
				network.SetInputSignal(10, 0.9);

				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				output = network.GetOutputSignal(0);
				if(output>=0.5)
					fitness+=output;
				else
					success=false;

			//----- Test with addressed data line set to 1. Response should always be 1.
				network.ClearSignals();

				// Set address line values.
				network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);
				network.SetInputSignal(2, a2);

				// Set data line values.
				network.SetInputSignal(3, a==0?0.9:0.1);
				network.SetInputSignal(4, a==1?0.9:0.1);
				network.SetInputSignal(5, a==2?0.9:0.1);
				network.SetInputSignal(6, a==3?0.9:0.1);
				network.SetInputSignal(7, a==4?0.9:0.1);
				network.SetInputSignal(8, a==5?0.9:0.1);
				network.SetInputSignal(9, a==6?0.9:0.1);
				network.SetInputSignal(10, a==7?0.9:0.1);

				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				output = network.GetOutputSignal(0);
				if(output>=0.5)
					fitness+=output;
				else
					success=false;

			//----- Test with addressed data line set to 0, all other set to 1. Response should always be 0.
				network.ClearSignals();

				// Set address line values.
				network.SetInputSignal(0, a0);
				network.SetInputSignal(1, a1);
				network.SetInputSignal(2, a2);

				// Set data line values.
				network.SetInputSignal(3, a==0?0.1:0.9);
				network.SetInputSignal(4, a==1?0.1:0.9);
				network.SetInputSignal(5, a==2?0.1:0.9);
				network.SetInputSignal(6, a==3?0.1:0.9);
				network.SetInputSignal(7, a==4?0.1:0.9);
				network.SetInputSignal(8, a==5?0.1:0.9);
				network.SetInputSignal(9, a==6?0.1:0.9);
				network.SetInputSignal(10, a==7?0.1:0.9);

				// Activate network.
				network.MultipleSteps(3);

				// Get network's response.
				output = network.GetOutputSignal(0);
				if(output<0.5)
					fitness+=1.0-output;
				else
					success=false;
			}

			// If successful on the above 32 test cases then perform thourough test of all
			// 2048 possible input signal combinations. 
			if(success)
			{
				// Reset success flag. We are going to re-use it. Reset the fitness, for simplicity
				// we will re-test the above 16 test cases.
				success = true;
				fitness = 0.0;

				// 2048 test cases.
				for(int i=0; i<2048; i++)
				{
					network.ClearSignals();

					// Apply bitmask to i and shift left to generate the input signals.
					// In addition we scale 0->1 to be 0.1->0.9.
					// Note. We can eliminate all the maths by pre-building a table of test signals. 
					// Using this approach instead makes the code easier to scale up to the 11 and 20 
					// multiplexer problems.
					int tmp=i;
					for(int j=0; j<11; j++) 
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
					if(   ((1<<(3+(i&0x7)))&i)!=0)
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
					fitness+=10000.0;
			}

			return fitness;
		}

		public string EvaluatorStateMessage
		{
			get
			{	
				return string.Empty;
			}
		}

		#endregion
	}
}
