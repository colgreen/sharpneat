using System;

namespace SharpNeatLib.Maths
{
	/// <summary>
	/// Selected pieces of RandLib 1.3 translated into C#.
	/// See http://hpux.asknet.de/hppd/hpux/Maths/Misc/randlib-1.3/ for more info.
	/// </summary>
	public class RandLib
	{
		static Random random = new Random();


		/// <summary>
		/// Details from randlib comments...
		///		GENerate random deviate from a NORmal distribution 
		///		
		///						Function	
		///		Generates a single random deviate from a normal distribution
		///		with mean, AV, and standard deviation, SD.
		///		
		///						Arguments
		///		av --> Mean of the normal distribution.			
		///		sd --> Standard deviation of the normal distribution.
		///		JJV    (sd >= 0)
		///		
		///						Method
		///		Renames SNORM from TOMS as slightly modified by BWB to use RANF
		///		instead of SUNIF.
		///		
		///		For details see:
		///		Ahrens, J.H. and Dieter, U.
		///		Extensions of Forsythe's Method for Random
		///		Sampling from the Normal Distribution.
		///		Math. Comput., 27,124 (Oct. 1973), 927 - 937.
		/// </summary>
		/// <param name="av"></param>
		/// <param name="sd"></param>
		/// <returns></returns>
		static public double gennor(double av, double sd)
		{
			if(sd < 0.0)
				throw new MathsException("gennor() - invalid sd");
			
			return sd*snorm()+av;
		}


		static double[] a = new double[32]
		{
			0.0,3.917609E-2,7.841241E-2,0.11777,0.1573107,0.1970991,0.2372021,0.2776904,
			0.3186394,0.36013,0.4022501,0.4450965,0.4887764,0.5334097,0.5791322,
			0.626099,0.6744898,0.7245144,0.7764218,0.8305109,0.8871466,0.9467818,
			1.00999,1.077516,1.150349,1.229859,1.318011,1.417797,1.534121,1.67594,
			1.862732,2.153875
		};

		static double[] d = new double[31]
		{
			0.0,0.0,0.0,0.0,0.0,0.2636843,0.2425085,0.2255674,0.2116342,0.1999243,
			0.1899108,0.1812252,0.1736014,0.1668419,0.1607967,0.1553497,0.1504094,
			0.1459026,0.14177,0.1379632,0.1344418,0.1311722,0.128126,0.1252791,
			0.1226109,0.1201036,0.1177417,0.1155119,0.1134023,0.1114027,0.1095039
		};


		static double[] t = new double[31]
		{
			7.673828E-4,2.30687E-3,3.860618E-3,5.438454E-3,7.0507E-3,8.708396E-3,
			1.042357E-2,1.220953E-2,1.408125E-2,1.605579E-2,1.81529E-2,2.039573E-2,
			2.281177E-2,2.543407E-2,2.830296E-2,3.146822E-2,3.499233E-2,3.895483E-2,
			4.345878E-2,4.864035E-2,5.468334E-2,6.184222E-2,7.047983E-2,8.113195E-2,
			9.462444E-2,0.1123001,0.136498,0.1716886,0.2276241,0.330498,0.5847031
		};

		static double[] h = new double[31]
		{
			3.920617E-2,3.932705E-2,3.951E-2,3.975703E-2,4.007093E-2,4.045533E-2,
			4.091481E-2,4.145507E-2,4.208311E-2,4.280748E-2,4.363863E-2,4.458932E-2,
			4.567523E-2,4.691571E-2,4.833487E-2,4.996298E-2,5.183859E-2,5.401138E-2,
			5.654656E-2,5.95313E-2,6.308489E-2,6.737503E-2,7.264544E-2,7.926471E-2,
			8.781922E-2,9.930398E-2,0.11556,0.1404344,0.1836142,0.2790016,0.7010474
		};

		

		/// <summary>
		/// Details from randlib comments...
		/// 			
		///		**********************************************************************
		///                                                                      
		///                                                                      
		///		(STANDARD-)  N O R M A L  DISTRIBUTION                           
		///                                                                      
		///                                                                      
		///		**********************************************************************
		///		 **********************************************************************
		///                                                                      
		///									  FOR DETAILS SEE:                                                 
		///                                                                      
		///		AHRENS, J.H. AND DIETER, U.                            
		///		EXTENSIONS OF FORSYTHE'S METHOD FOR RANDOM             
		///		SAMPLING FROM THE NORMAL DISTRIBUTION.                 
		///									 MATH. COMPUT., 27,124 (OCT. 1973), 927 - 937.          
		///                                                                      
		///		ALL STATEMENT NUMBERS CORRESPOND TO THE STEPS OF ALGORITHM 'FL'  
		///		(M=5) IN THE ABOVE PAPER     (SLIGHTLY MODIFIED IMPLEMENTATION)  
		///                                                                      
		///		Modified by Barry W. Brown, Feb 3, 1988 to use RANF instead of   
		///		SUNIF.  The argument IR thus goes away.                          
		///                                                                      
		///		**********************************************************************
		///		THE DEFINITIONS OF THE CONSTANTS A(K), D(K), T(K) AND
		///		H(K) ARE ACCORDING TO THE ABOVEMENTIONED ARTICLE
		///																							
		/// </summary>
		/// <returns></returns>
		static public double snorm()
		{
			int i;
			double snorm, u,s,ustar,aa,w,y,tt;
			
			u = ranf();
			s = 0.0;
			if(u > 0.5) s = 1.0;
			u += (u-s);
			u = 32.0*u;
			i = (int) (u);
			if(i == 32) i = 31;
			if(i == 0) goto S100;
			/*
										START CENTER
			*/
			ustar = u-(double)i;
			aa = a[i-1];
			S40:
				if(ustar <= t[i-1]) goto S60;
			w = (ustar-t[i-1]) * h[i-1];
			S50:
				/*
											EXIT   (BOTH CASES)
				*/
				y = aa+w;
			snorm = y;
			if(s == 1.0) snorm = -y;
			return snorm;
			S60:
				/*
											CENTER CONTINUED
				*/
				u = ranf();
			w = u*(a[i]-aa);
			tt = (0.5*w+aa)*w;
			goto S80;
			S70:
				tt = u;
			ustar = ranf();
			S80:
				if(ustar > tt) goto S50;
			u = ranf();
			if(ustar >= u) goto S70;
			ustar = ranf();
			goto S40;
			S100:
				/*
											START TAIL
				*/
				i = 6;
			aa = a[31];
			goto S120;
			S110:
				aa += d[i-1];
			i += 1;
			S120:
				u += u;
			if(u < 1.0) goto S110;
			u -= 1.0;
			S140:
				w = u * d[i-1];
			tt = (0.5*w+aa)*w;
			goto S160;
			S150:
				tt = u;
			S160:
				ustar = ranf();
			if(ustar > tt) goto S50;
			u = ranf();
			if(ustar >= u) goto S150;
			u = ranf();
			goto S140;
		}

		/// <summary>
		/// A version of random.NextDouble() that avoids 0.0
		/// </summary>
		/// <returns></returns>
		static private double ranf()
		{
			double ranf;
			
			do
			{
				ranf = random.NextDouble();
			} 
			while(ranf==0.0);

			return ranf;
		}
	}
}
