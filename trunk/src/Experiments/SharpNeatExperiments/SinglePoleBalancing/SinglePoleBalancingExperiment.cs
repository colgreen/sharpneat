using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class SinglePoleBalancingExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public SinglePoleBalancingExperiment()
		{
		}

		#endregion

		#region IExperiment Members
		
		/// <summary>
		/// This method is called immediately following instantiation of an experiment. It is used
		/// to pass in a hashtable of string key-value pairs from the 'experimentParameters' 
		/// block of the experiment configuration block within the application config file.
		/// 
		/// If no parameters where specified then an empty Hashtable is used.
		/// </summary>
		/// <param name="parameterTable"></param>
		public void LoadExperimentParameters(Hashtable parameterTable)
		{
		}

		public IPopulationEvaluator PopulationEvaluator
		{
			get
			{
				if(populationEvaluator==null)
					ResetEvaluator(activationFunction);

				return populationEvaluator;
			}
		}

		public void ResetEvaluator(IActivationFunction activationFn)
		{
			populationEvaluator = new SingleFilePopulationEvaluator(new SinglePoleBalancingNetworkEvaluator(), activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 4;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 1;
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				// Experimentally determined value. This gives some speciation from the outset.
				np.compatibilityThreshold = 1.8F; 

				// No hidden nodes are required to solve single pole balancing. Also the network is
				// only activated once per simulation timestep and so hidden nodes are essentially 
				// useless anyway - though not completely because network state is not reset per timestep.
				np.pMutateAddNode = 0.0;

				return np;
			}
		}

		public IActivationFunction SuggestedActivationFunction
		{
			get
			{
				return activationFunction;
			}
		}

		public AbstractExperimentView CreateExperimentView()
		{
			return null;
		}

		public string ExplanatoryText
		{
			get
			{
				return @"Here we simulate balancing a single pole in 2D. The pole is attached to a cart with
a hinge, the cart sits on a track with a fixed length. This simulation defines the
cart's position and velocity on track along with the pole's current angle and angular
velocity.
A single trial runs until the pole angle passes a threshold(+-12 degrees) or the cart runs off the track
or maxTimesteps is reached(100000 = success).
	
Ported from the C/C++ code in NEAT by K.Stanley (http://www.cs.utexas.edu/users/kstanley/neat.html)
that code in turn was taken from the pole simulator written by Richard Sutton and Charles Anderson.

This simulator uses normalized, continous inputs instead of discretizing the input space.";
			}
		}

		#endregion
	}
}
