/* ***************************************************************************
 * HSLColor Class.
 * 
 * Richard Newman.
 * 
 * http://richnewman.wordpress.com/hslcolor-class/
 */
using System;
using System.Drawing;

namespace SharpNeat.Drawing
{
    /// <summary>
    /// For working with the Hue, Saturation, Luminosity (HSL) colour model.
    /// </summary>
    public class HSLColor
    {
        private const double scale = 240.0;

        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double _hue = 1.0;
        private double _saturation = 1.0;
        private double _luminosity = 1.0;

        #region Properties

        /// <summary>
        /// Hue.
        /// </summary>
        public double Hue
        {
            get { return _hue * scale; }
            set { _hue = Math.Clamp(value / scale, 0.0, 1.0); }
        }

        /// <summary>
        /// Saturation.
        /// </summary>
        public double Saturation
        {
            get { return _saturation * scale; }
            set { _saturation = Math.Clamp(value / scale, 0.0, 1.0); }
        }

        /// <summary>
        /// Luminosity.
        /// </summary>
        public double Luminosity
        {
            get { return _luminosity * scale; }
            set { _luminosity = Math.Clamp(value / scale, 0.0, 1.0); }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HSLColor() { }

        /// <summary>
        /// Construct with the given <see cref="Color"/>.
        /// </summary>
        /// <param name="color"></param>
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }

        /// <summary>
        /// Construct with the given colour, using the RGB colour model.
        /// </summary>
        /// <param name="red">Red.</param>
        /// <param name="green">Green.</param>
        /// <param name="blue">Blue.</param>
        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }

        /// <summary>
        /// Construct with the given colour, using the HSL colour model.
        /// </summary>
        /// <param name="hue">Hue.</param>
        /// <param name="saturation">Saturation.</param>
        /// <param name="luminosity">Luminosity.</param>
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assign a colour using the RGB colour model.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public void SetRGB(int red, int green, int blue)
        {
            HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
            this._hue = hslColor._hue;
            this._saturation = hslColor._saturation;
            this._luminosity = hslColor._luminosity;
        }

        #endregion

        #region Public Static Methods [Operators - Casts to/from System.Drawing.Color]

        /// <summary>
        /// Cast from <see cref="HSLColor"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="hslColor">The HSL color to cast from.</param>
        public static implicit operator Color(HSLColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor._luminosity != 0)
            {
                if (hslColor._saturation == 0)
                { 
                    r = g = b = hslColor._luminosity;
                }
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0 * hslColor._luminosity - temp2;
  
                    r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor._hue);
                    b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        /// <summary>
        /// Cast from <see cref="Color"/> to <see cref="HSLColor"/>.
        /// </summary>
        /// <param name="color">The RGB color to cast from.</param>
        public static implicit operator HSLColor(Color color)
        {
            HSLColor hslColor = new HSLColor
            {
                _hue = color.GetHue() / 360.0, // we store hue as [0,1] as opposed to 0-360. 
                _luminosity = color.GetBrightness(),
                _saturation = color.GetSaturation()
            };
            return hslColor;
        }

        #endregion

        #region Private Static Methods

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }

        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor._luminosity < 0.5)  //<=??
                temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
            else
                temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
            return temp2;
        }
  
        #endregion
     }
}
