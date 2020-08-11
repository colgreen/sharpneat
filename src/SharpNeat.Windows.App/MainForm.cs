using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using log4net.Repository;
using SharpNeat.Experiments;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// SharpNEAT main GUI window.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly ILog __log = LogManager.GetLogger(typeof(MainForm));
        private List<INeatExperimentFactory> _experimentFactoryList;

        #region Form Constructor / Initialisation

        /// <summary>
        /// Construct and initialize the form.
        /// </summary>
        public MainForm()
        {
            // Set the default culture for all threads in the application to the Invariant culture.
            // This is a cheap way of ensuring that all form fields and data IO routines
            // read and write textual data in the same format, in particular the use of a dot as the
            // decimal separator (some culture use a comma).
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            InitializeComponent();
            Logger.SetListBox(lbxLog);

            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.properties"));




            List<INeatExperimentFactory> experimentFactoryList = AppUtils.ScanAssembliesForNeatExperiments();



        }

        #endregion




        ///// <summary>
        ///// Initialise the problem domain combobox. The list of problem domains is read from an XML file; this 
        ///// allows changes to be made and new domains to be plugged-in without recompiling binaries.
        ///// </summary>
        //private void InitProblemDomainList()
        //{
        //    // Find all experiment config data files in the current directory (*.experiments.xml)
        //    foreach(string filename in Directory.EnumerateFiles(".", "*.experiments.xml"))
        //    {
        //        List<ExperimentInfo> expInfoList = ExperimentInfo.ReadExperimentXml(filename);
        //        foreach(ExperimentInfo expInfo in expInfoList) {
        //            cmbExperiments.Items.Add(new ListItem(string.Empty, expInfo.Name, expInfo));
        //        }
        //    }
        //    // Pre-select first item.
        //    cmbExperiments.SelectedIndex = 0;
        //}




    }
}
