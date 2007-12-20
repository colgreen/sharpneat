using System;

namespace NeatParameterOptimizer
{
	public class NeatParametersWrapperFactory
	{
		public static NeatParametersWrapperList CreateNeatParametersWrapperList(NeatParametersWrapper seed, MetaNeatParameters mnp, int length)
		{
			NeatParametersWrapperList oList = new NeatParametersWrapperList();
			for(int i=0; i<length; i++)
			{
				NeatParametersWrapper oNew = new NeatParametersWrapper(seed);
				oNew.Mutate_ResetParameters(mnp);
				oList.Add(oNew);
			}
			return oList;
		}
	}
}
