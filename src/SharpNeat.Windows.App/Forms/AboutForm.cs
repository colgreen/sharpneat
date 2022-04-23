// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Reflection;
using SharpNeat.Evaluation;

#pragma warning disable IDE1006 // Naming Styles

namespace SharpNeat.Windows.App.Forms;

/// <summary>
/// 'About box' form. Displays component version numbers and other info.
/// </summary>
public partial class AboutForm : Form
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public AboutForm()
    {
        InitializeComponent();
        pictureBox1.Image = ResourceUtils.ReadBitmapResource("SharpNeat.Windows.App.Resources.sharpneat_banner.png");
        txtAbout.Text = ResourceUtils.ReadStringResource("SharpNeat.Windows.App.Resources.about.txt");
        txtLicense.Text = ResourceUtils.ReadStringResource("SharpNeat.Windows.App.Resources.license.txt");
    }

    private void AboutForm_Load(object sender, EventArgs e)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(FitnessInfo));
        Version version = assembly.GetName().Version;
        txtVersionInfo.Text = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + "\r\n" +
                              $"SharpNEAT core library {version}";
    }

    private void txtAbout_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
    }
}
