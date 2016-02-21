/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
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
			txtVersionInfo.Text =	"SharpNEAT:          " + oVersion.Major.ToString() + "." + oVersion.Minor.ToString()+ "." + oVersion.Revision.ToString() + " Build " + oVersion.Build.ToString() + "\r\n" + 
									".NET Framework:  " + Environment.Version.ToString();
        }
    }
}
