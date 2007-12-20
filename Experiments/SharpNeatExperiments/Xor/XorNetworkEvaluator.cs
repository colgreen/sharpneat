using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class XorNetworkEvaluator : INetworkEvaluator
	{
		#region Public Methods

		public double EvaluateNetwork(INetwork network)
		{
			double fitness=0;
			double output;
			bool pass=true;

			//----- Test 0,0
			network.SetInputSignal(0, 0.0);
			network.SetInputSignal(1, 0.0);
            if(!network.RelaxNetwork(10, 0.01))
            {	// Any networks that don't relax are unlikely to be any good to us, 
                // so lets just bail out here.
                return 0.0;
            }
			output = network.GetOutputSignal(0);
			fitness += 1.0 - output;
			if(output>=0.5) pass=false;
			
			//----- Test 1,1
            network.ClearSignals();
			network.SetInputSignal(0, 1.0);
			network.SetInputSignal(1, 1.0);
            if(!network.RelaxNetwork(10, 0.01))
            {	// Any networks that don't relax are unlikely to be any good to us, 
                // so lets just bail out here.
                return 0.0;
            }
			output = network.GetOutputSignal(0);
			fitness += 1.0 - output;
			if(output>=0.5) pass=false;

			//----- Test 0,1
			network.ClearSignals();
			network.SetInputSignal(0, 0.0);
			network.SetInputSignal(1, 1.0);
            if(!network.RelaxNetwork(10, 0.01))
            {	// Any networks that don't relax are unlikely to be any good to us, 
                // so lets just bail out here.
                return 0.0;
            }
			output = network.GetOutputSignal(0);
			fitness += output;
			if(output<0.5) pass=false;

			//----- Test 1,0
			network.ClearSignals();
			network.SetInputSignal(0, 1.0);
			network.SetInputSignal(1, 0.0);

            if(!network.RelaxNetwork(10, 0.01))
            {	// Any networks that don't relax are unlikely to be any good to us, 
                // so lets just bail out here.
                return 0.0;
            }
			output = network.GetOutputSignal(0);
			fitness += output;
			if(output<0.5) pass=false;

			if(pass) fitness +=10.0;

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
