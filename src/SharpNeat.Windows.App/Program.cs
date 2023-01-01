// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace SharpNeat.Windows.App;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Initialise logging.
        ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.properties"));

        // Add top level exception handler.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // Launch main app form/window.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var mainForm = new MainForm();
        Application.Run(mainForm);
    }
}
