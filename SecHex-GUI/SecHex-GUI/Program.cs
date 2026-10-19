namespace SecHex_GUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"), e.ExceptionObject?.ToString() ?? "Unknown crash");
                };
                Application.ThreadException += (s, e) =>
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"), e.Exception?.ToString() ?? "Unknown thread exception");
                };

                ApplicationConfiguration.Initialize();
                Application.Run(new Spoofy());
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"), ex.ToString());
                MessageBox.Show("Erro ao inicializar: " + ex.Message, "SecHex Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}