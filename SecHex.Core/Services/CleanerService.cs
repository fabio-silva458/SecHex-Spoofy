using SecHex.Core.Helpers;
using SecHex.Core.Models;

namespace SecHex.Core.Services
{
    public static class CleanerService
    {
        public static OperationResult FlushDns()
        {
            bool success = ProcessHelper.RunHiddenProcess("ipconfig", "/flushdns");
            return success ? OperationResult.Ok("Cache DNS limpo com sucesso.") : OperationResult.Fail("Falha ao limpar cache DNS.");
        }

        public static OperationResult ClearWindowsLogs()
        {
            string[] logs = new string[] { "Application", "Security", "System", "Setup" };
            int cleared = 0;
            foreach (string log in logs)
            {
                if (ProcessHelper.RunHiddenProcess("wevtutil", $"cl \"{log}\""))
                {
                    cleared++;
                }
            }

            return OperationResult.Ok($"Logs de eventos limpos ({cleared}/{logs.Length} canais).");
        }

        public static OperationResult ClearTemporaryFiles()
        {
            int deletedFiles = 0;
            string userTemp = Path.GetTempPath();
            string winTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

            deletedFiles += CleanDirectory(userTemp);
            deletedFiles += CleanDirectory(winTemp);

            return OperationResult.Ok($"Arquivos temporários removidos ({deletedFiles} itens limpos).");
        }

        public static OperationResult CleanUbisoftCache()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            CleanDirectory(Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "cache"));
            CleanDirectory(Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "logs"));
            CleanDirectory(Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "savegames"));
            CleanDirectory(Path.Combine(appData, "Ubisoft Game Launcher", "spool"));

            return OperationResult.Ok("Cache da Ubisoft limpo com sucesso.");
        }

        public static OperationResult CleanValorantCache()
        {
            string valorantSaved = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VALORANT", "saved");
            CleanDirectory(valorantSaved);

            return OperationResult.Ok("Cache do Valorant limpo com sucesso.");
        }

        public static OperationResult ResetNetworkStack()
        {
            try
            {
                ProcessHelper.RunHiddenProcess("netsh", "winsock reset");
                ProcessHelper.RunHiddenProcess("netsh", "int ip reset");
                ProcessHelper.RunHiddenProcess("netsh", "int tcp reset all");
                ProcessHelper.RunHiddenProcess("ipconfig", "/release");
                ProcessHelper.RunHiddenProcess("ipconfig", "/flushdns");
                ProcessHelper.RunHiddenProcess("ipconfig", "/renew");

                // Reaplica bloqueio de portas e acesso remoto
                SecurityHardeningService.DisableRemoteAccess();

                return OperationResult.Ok("Pilha de rede redefinida e regras de bloqueio remoto reaplicadas.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao resetar rede: " + ex.Message);
            }
        }

        private static int CleanDirectory(string path)
        {
            int count = 0;
            try
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                            count++;
                        }
                        catch { }
                    }

                    foreach (DirectoryInfo subDir in di.GetDirectories())
                    {
                        try
                        {
                            subDir.Delete(true);
                            count++;
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return count;
        }
    }
}
