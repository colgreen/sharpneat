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
using System;
using System.Reflection;
using System.Windows.Forms;
using SharpNeat.Evaluation;

namespace SharpNeat.Windows.App
{
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
            pictureBox1.Image = AppUtils.ReadBitmapResource("SharpNeat.Windows.App.Resources.sharpneat_banner.png");
            txtAbout.Text = AppUtils.ReadStringResource("SharpNeat.Windows.App.Resources.about.txt");
            txtLicense.Text = AppUtils.ReadStringResource("SharpNeat.Windows.App.Resources.license.txt");
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
}
