using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Network2.Acyclic
{



    public static class WeightedAcyclicDirectedGraphFactory<T>
        where T : struct
    {


        public static WeightedAcyclicDirectedGraph<T> Create(IList<IWeightedDirectedConnection<T>> connectionList, int inputCount, int outputCount)
        {

            


            // Determine the depth of each node in the network. 
            
            //NetworkDepthInfo netDepthInfo = AcyclicGraphDepthAnalysis.CalculateNodeDepths(networkDef);









            return null;
        }




    }




}
