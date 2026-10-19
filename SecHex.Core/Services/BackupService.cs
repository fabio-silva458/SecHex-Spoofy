using SecHex.Core.Helpers;
using SecHex.Core.Models;

namespace SecHex.Core.Services
{
    public static class BackupService
    {
        private static readonly string[] CriticalKeysToBackup = new string[]
        {
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\ComputerName",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\SQMClient",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SystemInformation"
        };

        public static OperationResult CreateBackup(string baseBackupFolder)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string sessionFolder = Path.Combine(baseBackupFolder, timestamp);
                Directory.CreateDirectory(sessionFolder);

                int exportedCount = 0;
                int index = 1;
                foreach (string key in CriticalKeysToBackup)
                {
                    string safeFileName = $"key_{index++}.reg";
                    string destination = Path.Combine(sessionFolder, safeFileName);

                    if (ProcessHelper.RunHiddenProcess("reg", $"export \"{key}\" \"{destination}\" /y"))
                    {
                        exportedCount++;
                    }
                }

                return OperationResult.Ok($"Backup concluído com sucesso ({exportedCount} chaves salvas).", sessionFolder);
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao gerar backup de registro: " + ex.Message);
            }
        }
    }
}
