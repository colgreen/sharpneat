// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Serilog;

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
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink<LogWindowSink>()
            .CreateLogger();

        // Add top level exception handler.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // Launch main app form/window.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var mainForm = new MainForm();
        Application.Run(mainForm);
    }
}
