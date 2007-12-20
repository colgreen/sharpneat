using System;
using System.Windows.Forms;

using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments.Views
{
	public class SimpleOcrExperimentView : AbstractExperimentView
	{
		const int widthInControls = 9;
		const int charViewWidth=41;
		const int charViewHeight=83;

		BitImageCollection bitImageCollection;
		char[] charArray;

		CharacterView[] charViewArray;

		#region Constructor

		/// <summary>
		/// Constructor for the ExperimentView. The convention is to pass in the associated IExperiment.
		/// Any information required to create an ExperimentView should be available thorugh that interface.
		/// </summary>
		/// <param name="experiment"></param>
		public SimpleOcrExperimentView(IExperiment experiment)
		{
			this.Width = 396;
			this.Height = 294;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.Fixed3D;

		//----- Create a panel that will contain all of the CharacterView controls. 
		//		This allows us to enable scrolling if the controls do not fit in the viewable part of
		//		the panel.
			Panel pnlMain = new Panel();
			pnlMain.Dock = DockStyle.Fill;
			pnlMain.AutoScroll = true;
			this.Controls.Add(pnlMain);

		//----- Create and add BitImageViewport controls to the panel.
			//SimpleOcrNetworkEvaluator evaluator = (SimpleOcrNetworkEvaluator)experiment.NetworkListEvaluator;
			SimpleOcrNetworkEvaluator evaluator = new SimpleOcrNetworkEvaluator('a','z');
			bitImageCollection = evaluator.BitImageCollection;
			charArray = evaluator.CharArray;
			charViewArray = new CharacterView[bitImageCollection.Count];

			int x=4; 
			int y=4;
			int column=0;

			for(int i=0; i<charArray.Length; i++)
			{
				CharacterView charView = new CharacterView(bitImageCollection[i], charArray[i]);
				charViewArray[i] = charView;
				charView.Width = charViewWidth;
				charView.Height = charViewHeight;
				charView.Left = x;
				charView.Top = y;
				pnlMain.Controls.Add(charView);

				if(++column>=widthInControls)
				{
					column=0;
					x=4;
					y+=(charViewHeight+1);
				}
				else
				{
					x+=(charViewWidth+1);
				}
			}

			this.Activated +=new EventHandler(SimpleOcrExperimentView_Activated);
		}

		#endregion

		#region Public Methods

		public override void RefreshView(INetwork network)
		{
			int height= bitImageCollection[0].Height;
			int width = bitImageCollection[0].Width;


			// Loop through the collection of characters we wish to test against.
			int imageIdxBound = bitImageCollection.Count;
			for(int imageIdx=0; imageIdx<imageIdxBound; imageIdx++)
			{
				CharacterView charView = charViewArray[imageIdx];
				BitImage bitImage = bitImageCollection[imageIdx];

				// Ensure that any signals from prior activations are cleared.
				network.ClearSignals();

				// Apply the character's pixels to the input layer of the network.
				int signalIdxBase=0;
				for(int y=0; y<height; y++)
				{	
					for(int x=0; x<width; x++)
						network.SetInputSignal(signalIdxBase+x, bitImage.GetPixel(x,y) ? 1.0 : 0.0);
					
					signalIdxBase+=width;
				}

				// Activate the network.
				int highSignalCount=0;
				int highIdx=0;

				network.MultipleSteps(3);

				// Determine which output is highest.
				double highSignal = double.MinValue;
				for(int i=0; i<network.OutputNeuronCount; i++)
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

				if(highSignalCount==0 || highSignalCount>1)
				{
					// Two or more outputs share the highest signal. Therefore no output was properly selected.
					charView.SetLabel("***", true);
					continue;
				}

				if(highIdx==imageIdx)
					charView.SetLabel(charArray[imageIdx].ToString(), false);
				else
					charView.SetLabel(charArray[highIdx].ToString(), true);
			}
		}

		#endregion

		#region Event Handlers

		private void SimpleOcrExperimentView_Activated(object sender, EventArgs e)
		{
			foreach(CharacterView charView in charViewArray)
			{
				charView.RepaintImage();
			}
		}

		#endregion
	}
}
