using Microsoft.Win32;
using System.Security.Principal;
using SecHex.Core.Helpers;
using SecHex.Core.Models;

namespace SecHex.Core.Services
{
    public static class SecurityHardeningService
    {
        public static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Desativa qualquer possibilidade de acesso remoto externo à máquina (RDP, WinRM, Registro Remoto, Firewall).
        /// </summary>
        public static OperationResult DisableRemoteAccess()
        {
            List<string> actionsApplied = new();
            try
            {
                // 1. Desativar RDP (Área de Trabalho Remota)
                using (RegistryKey? rdpKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", true))
                {
                    if (rdpKey != null)
                    {
                        rdpKey.SetValue("fDenyTSConnections", 1, RegistryValueKind.DWord);
                        actionsApplied.Add("RDP Terminal Server desativado (fDenyTSConnections = 1)");
                    }
                }

                // 2. Desativar Assistência Remota
                using (RegistryKey? raKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Remote Assistance", true))
                {
                    if (raKey != null)
                    {
                        raKey.SetValue("fAllowToGetHelp", 0, RegistryValueKind.DWord);
                        actionsApplied.Add("Assistência Remota desativada (fAllowToGetHelp = 0)");
                    }
                }

                // 3. Parar e desabilitar serviço de Registro Remoto
                ProcessHelper.RunHiddenProcess("sc", "stop RemoteRegistry");
                ProcessHelper.RunHiddenProcess("sc", "config RemoteRegistry start= disabled");
                actionsApplied.Add("Serviço de Registro Remoto desativado");

                // 4. Parar e desabilitar WinRM (PowerShell Remoting)
                ProcessHelper.RunHiddenProcess("sc", "stop WinRM");
                ProcessHelper.RunHiddenProcess("sc", "config WinRM start= disabled");
                actionsApplied.Add("Gerenciamento Remoto WinRM desativado");

                // 5. Parar e desabilitar serviço de Terminal Server
                ProcessHelper.RunHiddenProcess("sc", "stop TermService");
                ProcessHelper.RunHiddenProcess("sc", "config TermService start= disabled");
                actionsApplied.Add("Serviço TermService desativado");

                // 6. Criar regras de bloqueio no Firewall do Windows para portas 3389, 5985, 5986
                ProcessHelper.RunHiddenProcess("netsh", "advfirewall firewall add rule name=\"SecHex-Block-RDP\" dir=in action=block protocol=TCP localport=3389");
                ProcessHelper.RunHiddenProcess("netsh", "advfirewall firewall add rule name=\"SecHex-Block-WinRM\" dir=in action=block protocol=TCP localport=5985,5986");
                actionsApplied.Add("Portas 3389, 5985 e 5986 bloqueadas no Firewall");

                return OperationResult.Ok("Bloqueio de acesso remoto aplicado com sucesso.", string.Join("; ", actionsApplied));
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Falha ao aplicar bloqueio de acesso remoto: " + ex.Message);
            }
        }
    }
}
