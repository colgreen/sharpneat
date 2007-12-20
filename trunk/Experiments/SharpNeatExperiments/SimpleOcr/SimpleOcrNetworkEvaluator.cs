using System;
using System.Drawing;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class SimpleOcrNetworkEvaluator : INetworkEvaluator
	{
		char[] charArray;
		BitImageCollection bitImageCollection;

		// Store the width and height of the images in the colelction locally for efficiency.
		int width;
		int height;

		int inputNeuronCount;
		int outputNeuronCount;
		double maxFitness;

		#region Constructors

		public SimpleOcrNetworkEvaluator(char[] charArray)
		{
			Font oFont = new Font("Courier New", 8);
			this.charArray = charArray;
			bitImageCollection = new BitImageCollection(oFont, 2, 4, 0, 4, charArray);

			width = bitImageCollection[0].Width;
			height = bitImageCollection[0].Height;

			inputNeuronCount = width * height;
			outputNeuronCount = bitImageCollection.Count;

			// The network's score 10 for each character they recognise correctly.
			maxFitness = outputNeuronCount * 10;
		}

		
		public SimpleOcrNetworkEvaluator(char firstChar, char lastChar)
		{
			// Build class level currChar. Needed for ExperimentVisualization.
			int length = (lastChar-firstChar)+1;
			charArray = new char[length];

			char currChar = firstChar;
			for(int i=0; i<length; i++)
			{
				charArray[i] = currChar++;
			}
			
			// Build the BitImageCollection.
			Font oFont = new Font("Courier New", 8);
			bitImageCollection = new BitImageCollection(oFont, 2, 4, 0, 4, firstChar, lastChar);

			width = bitImageCollection[0].Width;
			height = bitImageCollection[0].Height;

			inputNeuronCount = width * height;
			outputNeuronCount = bitImageCollection.Count;

			// The networks score 10 for each character they recognise correctly.
			maxFitness = outputNeuronCount * 10;
		}

		#endregion

		#region Properties

		/// <summary>
		/// We expose this solely so that the OcrExperimentView can gain access
		/// the the characters that are associated to each BitImage.
		/// </summary>
		public char[] CharArray
		{
			get
			{
				return charArray;
			}
		}

		/// <summary>
		/// We expose this solely so that the OcrExperimentView can gain access
		/// and display the bitImages.
		/// </summary>
		public BitImageCollection BitImageCollection
		{
			get
			{
				return bitImageCollection;
			}
		}

		public int InputNeuronCount
		{
			get
			{
				return inputNeuronCount;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return outputNeuronCount;
			}
		}

		#endregion

		#region INetworkEvaluator

		public double EvaluateNetwork(SharpNeatLib.NeuralNetwork.INetwork network)
		{
			double fitness=0.0;

			// Loop through the collection of characters we wish to test against.
			int imageIdxBound = bitImageCollection.Count;
			for(int imageIdx=0; imageIdx<imageIdxBound; imageIdx++)
			{
				BitImage bitImage = bitImageCollection[imageIdx];

				// Ensure that any signals from prior activations are cleared.
				network.ClearSignals();

				// Apply the character's pixels to the input layer of the network.
				network.SetInputSignals(bitImage.SignalArray);

				// Activate the network.
				network.MultipleSteps(3);

//				// Relax the network.
//				if(!network.RelaxNetwork(5, 0.01))
//				{	// The network did not relax. Score zero for this character.
//					continue;
//				}

				// Determine which output is highest.
				int highSignalCount=0;
				int highIdx=0;
				double highSignal = double.MinValue;

				for(int i=0; i<outputNeuronCount; i++)
				{
					double signal = network.GetOutputSignal(i);
					
					if(signal>highSignal)
					{
						highSignal = signal;
						highIdx = i;
						highSignalCount=1;
					}
					else if(signal==highSignal)
					{
						highSignalCount++;
					}
				}

				if(highSignalCount>1)
				{
					// Two or more outputs share the highest signal. Therefore no output was properly selected.
					continue;
				}

				if(highIdx==imageIdx)
					fitness+=10;
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
