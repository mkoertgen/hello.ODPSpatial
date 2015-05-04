using System;
using System.Windows.Forms;

namespace WinFormsSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BootstrapOracleClient("TODO: Path to Instant client");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void BootstrapOracleClient(string pathToOracleClient)
        {
            var expandedPath = Environment.ExpandEnvironmentVariables(pathToOracleClient);
            new NativeBootsTrapper(expandedPath).Bootstrap();
        }
    }
}
