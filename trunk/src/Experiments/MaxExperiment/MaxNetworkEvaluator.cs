using System;
using System.Collections;
using System.IO;

namespace SharpNeatLib.Experiments
{
	public class MaxNetworkEvaluator : INetworkEvaluator
	{
		#region Class Variables

		/// <summary>
		/// The number of neurons in the input layers. 
		/// </summary>
		int inputNeuronCount;
		string dataFileUrl;
		double[][] testDataTable;

		/// <summary>
		/// The maximum value on each row.
		/// </summary>
		double[] maxValueArray;

		#endregion

		#region Constructor

		public MaxNetworkEvaluator(int inputNeuronCount, string dataFileUrl)
		{
			this.inputNeuronCount = inputNeuronCount;
			this.dataFileUrl = dataFileUrl;
			LoadTestData(dataFileUrl, inputNeuronCount);
		}	

		#endregion

		#region Private Methods

		private void LoadTestData(string dataFileUrl, int rowWidth)
		{
			StreamReader sr = new StreamReader(dataFileUrl);
			ArrayList rowList = new ArrayList();
		
			string line=null;
			while((line=sr.ReadLine())!=null)
			{
				string[] rowDataStringArray = line.Split(new char[]{' '});

				// Get the row width from the first record.
				if(rowDataStringArray.Length!=rowWidth)
					throw new ApplicationException("Data file row width does not match input neuron count.");

				// Create a new row array and read the string data into it.
				double[] rowDataArray = new double[rowWidth];

				for(int i=0; i<rowWidth; i++)
					rowDataArray[i] = double.Parse(rowDataStringArray[i]);

				rowList.Add(rowDataArray);
			}

			// Copy data into a new 2D array.
			testDataTable = new double[rowList.Count][];
			maxValueArray = new double[rowList.Count];
			for(int row=0; row<rowList.Count; row++)
			{
				testDataTable[row] = (double[])rowList[row];

				// Pre-determine the highest value for each row. We don't store the index of the highest value
				// because more then one column may contain the maximum value.
				double maxValue = testDataTable[row][0];

				for(int col=1; col<testDataTable[row].Length; col++)
					maxValue = Math.Max(maxValue, testDataTable[row][col]);

				maxValueArray[row] = maxValue;
			}
		}

		#endregion

		#region INetworkEvaluator Members

		public double EvaluateNetwork(SharpNeatLib.NeuralNetwork.INetwork network)
		{
			double fitness=0.0;

			// Test each of the data rows in turn.
			for(int row=0; row<testDataTable.Length; row++)
			{
				// Reset any old signals in the network.
				network.ClearSignals();

				// Apply the input signals to the input neurons.
				network.SetInputSignals(testDataTable[row]);

				// Activate the network.
				network.MultipleSteps(2);

				// The single output value indicates which input is the highest.
				int highValueIndex = Math.Min(	(int)Math.Floor(network.GetOutputSignal(0) * inputNeuronCount), 
												inputNeuronCount-1);
				
				// Assign 1.0 for a correct answer. 0 for everything else. Note that more than one input
				// value can have the maximum signal value, therefore we test the value rather then the
				// index (which otherwise would have been faster).
				if(Math.Abs(testDataTable[row][highValueIndex]-maxValueArray[row]) < double.Epsilon)
					fitness++;
			}
			return fitness;
		}

		public string EvaluatorStateMessage
		{
			get
			{
				return "MaxFitness=" + testDataTable.Length.ToString();
			}
		}

		#endregion
	}
}
