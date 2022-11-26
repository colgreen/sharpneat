// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using log4net;

namespace SharpNeat.Windows.App;

internal static class UIAccessUtils
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
        if(int.TryParse(txtBox.Text, out int tmp))
            return tmp;

        __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
        return defaultVal;
    }

    public static double GetValue(TextBox txtBox, double defaultVal)
    {
        if(double.TryParse(txtBox.Text, out double tmp))
            return tmp;

        __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
        return defaultVal;
    }
}
