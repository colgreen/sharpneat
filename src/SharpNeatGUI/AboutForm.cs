/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Windows.Forms;
using System.Reflection;
using SharpNeatGUI.Properties;

namespace SharpNeatGUI
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
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
			txtAbout.Text = Resources.About;
            txtLicensing.Text = Resources.Licensing;

			Version oVersion = Assembly.GetExecutingAssembly().GetName().Version;
			txtVersionInfo.Text =	"SharpNEAT:          " + oVersion.Major.ToString() + "." + oVersion.Minor.ToString()+ "." + oVersion.Revision.ToString() + "." + oVersion.Build.ToString() + "\r\n" + 
									".NET Framework:  " + Environment.Version.ToString();
        }

        private void txtAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }
    }
}
