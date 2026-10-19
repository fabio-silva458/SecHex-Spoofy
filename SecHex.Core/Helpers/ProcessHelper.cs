using System.Diagnostics;

namespace SecHex.Core.Helpers
{
    public static class ProcessHelper
    {
        public static bool RunHiddenProcess(string fileName, string arguments, int timeoutMs = 10000)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using Process? process = Process.Start(psi);
                if (process == null) return false;

                if (timeoutMs > 0)
                {
                    return process.WaitForExit(timeoutMs);
                }
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
