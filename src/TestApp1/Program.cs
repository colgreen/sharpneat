using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.EA;
using SharpNeat.EA.Controllers;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace TestApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //EAFactory factory = new EAFactory();


            //DefaultEvolutionAlgorithm<NeatGenome<double>> ea = factory.CreateDefaultEvolutionAlgorithm();
            //EvolutionAlgorithmController eaController = new EvolutionAlgorithmController(ea);



            //ea.PerformOneGeneration();

            //EvolutionAlgorithmController eaController = new EvolutionAlgorithmController()

            

            Random rng = new Random(0);
            int len = 1000;

            int[] srcIdArr = CreateRandomInt32Array(rng, len);
            int[] tgtIdArr = CreateRandomInt32Array(rng, len);
            double[] weightArr = CreateRandomDoubleArray(rng, len);
            ConnectionIdArrays connIdArrays = new ConnectionIdArrays(srcIdArr, tgtIdArr);

            int[] srcIdArr2 = (int[])srcIdArr.Clone();
            int[] tgtIdArr2 = (int[])tgtIdArr.Clone();
            double[] weightArr2 = (double[])weightArr.Clone();
            ConnectionIdArrays connIdArrays2 = new ConnectionIdArrays(srcIdArr2, tgtIdArr2);
            
            //ConnectionSorter.Sort(connIdArrays, weightArr);
            //ConnectionSorter2.Sort(connIdArrays2, weightArr2);

            Compare(srcIdArr, srcIdArr2);
            Compare(tgtIdArr, tgtIdArr2);
            Compare(weightArr, weightArr2);
        }


        private static void Compare<T>(T[] x, T[] y) where T : struct
        {
            if(x.Length != y.Length) {
                throw new Exception();
            }

            for(int i=0; i < x.Length; i++)
            {
                if(!x[i].Equals(y[i])) {
                    throw new Exception();
                }
            }
        }


        private static int[] CreateRandomInt32Array(Random rng, int length)
        {
            int[] arr = new int[length];
            for(int i=0; i<length; i++) {
                arr[i] = rng.Next();
            }
            return arr;
        }

        private static double[] CreateRandomDoubleArray(Random rng, int length)
        {
            double[] arr = new double[length];
            for(int i=0; i<length; i++) {
                arr[i] = rng.NextDouble();
            }
            return arr;
        }

    }
}
