using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class VectorCrossProductNetworkEvaluator : INetworkEvaluator
	{
		#region Structs

		struct XYZ
		{
			public double x;
			public double y;
			public double z;
		}

		#endregion

		#region Constants

		const double threesixty_degrees = Math.PI * 2.0;
		const double oneeighty_degrees = Math.PI;

		#endregion

		#region Class Variables

		double maxFitness;

		XYZ[] v1;
		XYZ[] v2;
		XYZ[] xProduct; // Cross product.

		#endregion

		#region Constructor

		public VectorCrossProductNetworkEvaluator()
		{
			BuildTestData();
			maxFitness = v1.Length*10.0;
		}

		#endregion

		#region Private Methods

		private void BuildTestData()
		{
			XYZ[] sphere = BuildVectorSphere(4);

			int testCaseCount = sphere.Length*sphere.Length;
			v1 = new XYZ[testCaseCount];
			v2 = new XYZ[testCaseCount];
			xProduct = new XYZ[testCaseCount];

			for(int i=0; i<sphere.Length; i++)
			{
				for(int j=0; j<sphere.Length; j++)
				{
					int idx=(i*sphere.Length)+j;

					v1[idx] = sphere[i];
					v2[idx] = sphere[j];
					VectorCrossProduct(sphere[i], sphere[j], ref xProduct[idx]);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectors"></param>
		/// <returns></returns>
		private XYZ[] BuildVectorSphere(int sectors)
		{
			XYZ[] spread = BuildVectorSpread_180Degree(sectors);

			// Including the two common vectors that lie on the x-axis.
			int numOfVectorsIn360DegreePlane = (spread.Length*2)+2;

			// Again, including the two common vectors that lie on the x-axis.
			int numOfVectorsInSphere = (numOfVectorsIn360DegreePlane*spread.Length) + 2;

			// Build the sphere.
			XYZ[] sphere = new XYZ[numOfVectorsInSphere];

			// Add in the two common vectors that lie on the x-axis.
			sphere[0].x = -1.0F;
			sphere[1].x = 1.0F;
			
			double angleDelta = oneeighty_degrees / sectors;
			int sphereIdx=2;
			for(int i=0; i<numOfVectorsIn360DegreePlane; i++)
			{
				for(int j=0; j<spread.Length; j++)
					sphere[sphereIdx++] = spread[j];
				
				RotateAboutXAxis(angleDelta, spread);
			}

			return sphere;
		}


		/// <summary>
		/// Build a spread of vectors from origin and in the xy plane from 
		/// 0 to 180 degrees. The two vectors that lie on the x axis are 
		/// ommitted.
		/// </summary>
		/// <param name="sectors"></param>
		/// <returns></returns>
		private XYZ[] BuildVectorSpread_180Degree(int sectors)
		{
			double angleDelta = oneeighty_degrees / sectors;
			int numOfVectors = sectors-1;
			XYZ[] spread = new XYZ[numOfVectors];

			double angle = angleDelta;
			for(int i=0; i<numOfVectors; i++)
			{
				spread[i].x = Math.Cos(angle);
				spread[i].y = Math.Sin(angle);

				angle += angleDelta;
			}

			return spread;
		}

		private void RotateAboutXAxis(double angle, XYZ[] xyz)
		{
			for(int i=0; i<xyz.Length; i++)
			{
				XYZ old = xyz[i];

				xyz[i].y = old.y*Math.Cos(angle) - old.z*Math.Sin(angle);
				xyz[i].z = old.y*Math.Sin(angle) + old.z*Math.Cos(angle);
			}
		}

		private void VectorCrossProduct(XYZ a, XYZ b, ref XYZ result)
		{
			result.x = a.y * b.z - a.z * b.y;
			result.y = a.z * b.x - a.x * b.z;
			result.z = a.x * b.y - a.y * b.x;
		}

		private double CompareVectors(XYZ v1, XYZ v2)
		{
//			const double threshold = 0.3;
//			double error = Math.Abs(v1.x-v2.x);
//			double rtn = error>=threshold ? 2.0 : Math.Pow(error,0.3);
//
//			error = Math.Abs(v1.y-v2.y);
//			rtn += error>=threshold ? 2.0 : Math.Pow(error,0.3);
//
//			error = Math.Abs(v1.z-v2.z);
//			rtn += error>=threshold ? 2.0 : Math.Pow(error,0.3);
//
//			return rtn;
//
			double rtn=Math.Pow(Math.Abs(v1.x-v2.x)/2.0, 0.5);
			rtn+=Math.Pow(Math.Abs(v1.y-v2.y)/2.0, 0.5);
			rtn+=Math.Pow(Math.Abs(v1.z-v2.z)/2.0, 0.5);

			return rtn;
		}

		#endregion

		#region INetworkEvaluator

		public double EvaluateNetwork(INetwork network)
		{
			double fitness=0.0;

			// Loop through all of the test cases.
			for(int i=0; i<v1.Length; i++)
			{
				network.ClearSignals();

				// Apply the two vectors we wish to calculate a product for
				// to the input neurons.

				// Vector 1.
				network.SetInputSignal(0, v1[i].x*0.9+0.1);
				network.SetInputSignal(1, v1[i].y*0.9+0.1);
				network.SetInputSignal(2, v1[i].z*0.9+0.1);

				// Vector 2.
				network.SetInputSignal(3, v2[i].x*0.9+0.1);
				network.SetInputSignal(4, v2[i].y*0.9+0.1);
				network.SetInputSignal(5, v2[i].z*0.9+0.1);

				// Activate the network.
				//network.RelaxNetwork(6, 1e-8);
				network.MultipleSteps(6);

				// Read the output and compare against the expect output.
				XYZ answer;
				answer.x = network.GetOutputSignal(0) * 2.0 - 1.0;
				answer.y = network.GetOutputSignal(1) * 2.0 - 1.0;
				answer.z = network.GetOutputSignal(2) * 2.0 - 1.0;

				// 
				fitness+=(3.0-CompareVectors(xProduct[i], answer))/3.0;
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
