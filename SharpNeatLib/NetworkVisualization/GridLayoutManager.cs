using System;
using System.Drawing;

namespace SharpNeatLib.NetworkVisualization
{
	public class GridLayoutManager : ILayoutManager
	{
		const int MARGIN_X = 0;
		const int MARGIN_Y = 0;
		const double JIGGLE_PROPORTION = 0.3;

		Size bounds;

		public void Layout(NetworkModel nm, Size areaSize)
		{
			// Use this to keep track of the coordinate bounds of the network (maxX, maxY).
			bounds = new Size(0,0);

		//----- Determine how many layers we are going arrange the neurons into.
			int inputCount = nm.InputNeuronList.Count;
			int outputCount = nm.OutputNeuronList.Count;
			int hiddenCount = nm.MasterNeuronList.Count - (inputCount+outputCount);
			float heightWidthRatio = (float)areaSize.Height/(float)areaSize.Width;

			// Default to 2 (input and output layers).
			int numLayers=2;
			int numHiddenLayers=0;
			if(hiddenCount>0)
			{
				// Arrange as if there no input/output layers.
				double sqrtHidden = Math.Sqrt(hiddenCount);
				numHiddenLayers = (int)Math.Floor(sqrtHidden * heightWidthRatio);

				// Now factor in the input/output layers.
				numHiddenLayers = Math.Max(1, numHiddenLayers-2);
				numLayers = 2+numHiddenLayers;
			}

		//----- Arrange the neurons.
			int heightLayers = areaSize.Height - 2*MARGIN_Y;
			int yIncrement = areaSize.Height / (numLayers+1);
			int yCurrent = MARGIN_Y + yIncrement;
			double yIncrementHalfed = yIncrement / 2.0;

			// Input layer. Place all input neurons in one layer.
			int widthLayer = areaSize.Width - 2*MARGIN_X;
			int xIncrement = widthLayer / (inputCount+1);
			int xCurrent = MARGIN_X + xIncrement;

			foreach(ModelNeuron modelNeuron in nm.InputNeuronList)
			{	
				modelNeuron.Position = new Point(xCurrent, yCurrent);
				modelNeuron.HasPositionInfo = true;
				UpdateModelBounds(modelNeuron);
				xCurrent+=xIncrement;
			}
			// Increment yCurrent, ready for the next layer.
			yCurrent+=yIncrement;
				
			// Hidden layers.
			if(numLayers>0)
			{
				// Keep track of which neuron is being positioned.
				int neuronIdx=0;

				// calculate the max number of neurons in any hidden layer.
				int layerNeuronsMax = (int)Math.Ceiling((float)hiddenCount / (float)numHiddenLayers);

				// Keep track of how many neurons remain to be positioned.
				int neuronsRemaining = hiddenCount;
				for(int hiddenLayer=0; hiddenLayer<numHiddenLayers; hiddenLayer++)
				{	
					// Calculate the number of neurons in this layer.
					int layerNeuronCount = Math.Min(neuronsRemaining, layerNeuronsMax);

					// Position neurons in this layer.
					xIncrement = widthLayer / (layerNeuronCount+1);
					xCurrent = MARGIN_X + xIncrement;

					for(int i=0; i<layerNeuronCount; i++)
					{
						ModelNeuron modelNeuron = nm.HiddenNeuronList[neuronIdx++];
						modelNeuron.Position = new Point(	
								xCurrent,
								yCurrent);
								//yCurrent + (int)(((random.NextDouble()*(double)yIncrement)-yIncrementHalfed)*JIGGLE_PROPORTION));
						modelNeuron.HasPositionInfo = true;
						UpdateModelBounds(modelNeuron);

						xCurrent+=xIncrement;
					}
					// Increment yCurrent, ready for the next layer.
					yCurrent+=yIncrement;
					
					neuronsRemaining-=layerNeuronCount;
				}
			}

			// Output layer.
			xIncrement = widthLayer / (outputCount+1);
			xCurrent = MARGIN_X + xIncrement;

			foreach(ModelNeuron modelNeuron in nm.OutputNeuronList)
			{	
				modelNeuron.Position = new Point(	
						xCurrent,
						yCurrent);
						//yCurrent + (int)(((random.NextDouble()*(double)yIncrement)-yIncrementHalfed)*JIGGLE_PROPORTION));
				modelNeuron.HasPositionInfo = true;
				UpdateModelBounds(modelNeuron);

				xCurrent+=xIncrement;
			}

			nm.Bounds = bounds;
		}

		private void UpdateModelBounds(ModelNeuron neuron)
		{
			if(neuron.Position.X > bounds.Width)
				bounds.Width = neuron.Position.X;

			if(neuron.Position.Y > bounds.Height)
				bounds.Height = neuron.Position.Y;
		}
	}
}
