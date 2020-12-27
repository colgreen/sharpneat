/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Globalization;
using System.Windows.Forms;
using log4net;

namespace SharpNeat.Windows.App
{
    public static class UIAccessUtils
    {
        private static readonly ILog __log = LogManager.GetLogger(typeof(UIAccessUtils));

        public static void SetValue(TextBox txtBox, int val)
        {
            txtBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        public static void SetValue(TextBox txtBox, double val)
        {
            txtBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        public static int GetValue(TextBox txtBox, int defaultVal)
        {
            if(int.TryParse(txtBox.Text, out int tmp)) {
                return tmp;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }

        public static double GetValue(TextBox txtBox, double defaultVal)
        {
            if(double.TryParse(txtBox.Text, out double tmp)) {
                return tmp;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }
    }
}
