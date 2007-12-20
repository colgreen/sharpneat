using System;

namespace SharpNeatLib.Experiments
{
	public struct Vector2d
	{
		const double _2_PI = 2.0*Math.PI;

		public double x;
		public double y;

		public Vector2d(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public void Set(Vector2d v) 
		{
			this.x = v.x;
			this.y = v.y;
		}

		public void Add(Vector2d v) 
		{
			this.x += v.x;
			this.y += v.y;
		}

		public void Subtract(Vector2d v) 
		{
			this.x -= v.x;
			this.y -= v.y;
		}

		public void Multiply(double factor) 
		{
			this.x *= factor;
			this.y *= factor;
		}

		/// <summary>
		/// The angle relative to north (0 degrees).
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public double AngleWithNorth
		{
			get
			{
				double a = Math.Atan2(x, y);
				if(a<0) return a+_2_PI;
				return a;
			}
		}

        /// <summary>
        /// Return the angle of the argument vector relative to the current
        /// vector. This a directed angle no greater than 180 degrees. So positive
        /// angles are to the right, negative to the left. 
        /// An angle of 180 degrees (pi radians, and the opposite direction) will
        /// be returned as a positive 180 degrees.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double AngleWithVector(Vector2d v)
        {   //TODO: Review this! 
            double a1 = Math.Atan2(y, x);
            double a2 = Math.Atan2(v.y, v.x);
            double a3 = a2-a1; ;

            if(a3>Math.PI)
            {
                return a3-_2_PI;
            }
            else if(a3<-Math.PI)
            {
                return a3+_2_PI;
            }
            return a3;
        }

		public double Magnitude
		{
			get
			{
				return Math.Sqrt(x*x + y*y);
			}
		}

		public double MagnitudeSquared
		{
			get
			{
				return x*x + y*y;
			}
		}

		/// <summary>
		/// Bearing in radians. North=0 degrees, East=90, South=180, West=270.
		/// </summary>
		public double Bearing
		{
			get
			{
				return Math.Atan2(x,y);
			}
		}


	}
}
