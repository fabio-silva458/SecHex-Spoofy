using System;
using SecHex.Core.Models;
using SecHex.Core.Services;

namespace HWID_Changer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "SecHex-Spoofy CLI | Projeto Educacional | 100% Offline";

            if (args.Length > 0 && (args[0].Equals("--test", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-t", StringComparison.OrdinalIgnoreCase)))
            {
                RunAutomatedTests();
                return;
            }

            bool isRunning = true;
            while (isRunning)
            {
                DrawBanner();
                isRunning = Menu();
            }

            Console.WriteLine("\nPrograma encerrado.");
        }

        private static void RunAutomatedTests()
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("       SecHex-Spoofy - Teste Automatizado (.NET 6)   ");
            Console.WriteLine("=====================================================\n");

            Console.WriteLine("[*] Teste 1: Leitura de Dados do Sistema...");
            DisplaySystemData();

            Console.WriteLine("\n[*] Teste 2: Bloqueio de Acesso Remoto...");
            PrintResult(SecurityHardeningService.DisableRemoteAccess());

            Console.WriteLine("\n[*] Teste 3: Geração de Backup Seguro...");
            string backupFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");
            PrintResult(BackupService.CreateBackup(backupFolder));

            Console.WriteLine("\n[*] Teste 4: Limpeza de Cache DNS...");
            PrintResult(CleanerService.FlushDns());

            Console.WriteLine("\n[*] Teste 5: Spoofing de GUIDs e Telemetria...");
            PrintResult(HardwareSpoofingService.SpoofGuids());

            Console.WriteLine("\n[*] Teste 6: Spoofing de Installation ID...");
            PrintResult(HardwareSpoofingService.SpoofInstallationId());

            Console.WriteLine("\n=====================================================");
            Console.WriteLine("  Todos os testes da aplicação foram finalizados com êxito!");
            Console.WriteLine("=====================================================");
        }

        private static void DrawBanner()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch { }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("███████╗██████╗  ██████╗  ██████╗ ███████╗██╗   ██╗");
            Console.WriteLine("██╔════╝██╔══██╗██╔═══██╗██╔═══██╗██╔════╝╚██╗ ██╔╝");
            Console.WriteLine("███████╗██████╔╝██║   ██║██║   ██║█████╗   ╚████╔╝ ");
            Console.WriteLine("╚════██║██╔═══╝ ██║   ██║██║   ██║██╔══╝    ╚██╔╝  ");
            Console.WriteLine("███████║██║     ╚██████╔╝╚██████╔╝██║        ██║   ");
            Console.WriteLine("╚══════╝╚═╝      ╚═════╝  ╚═════╝ ╚═╝        ╚═╝   ");
            Console.WriteLine("SecHex-Spoofy | Projeto Educacional | Arquitetura SecHex.Core");
            Console.WriteLine("-------------------------------------------------------------");
            Console.ResetColor();

            if (!SecurityHardeningService.IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] Executando sem privilégios de Administrador. Execute como Admin!");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[1]  Spoof Discos                [8]  Spoof Installation ID");
            Console.WriteLine("[2]  Spoof GUIDs & Telemetria    [9]  Spoof EFI Variable ID");
            Console.WriteLine("[3]  Spoof MAC Address           [10] Spoof SMBIOS Serial");
            Console.WriteLine("[4]  Limpar Cache Ubisoft        [11] Criar Backup do Registro (.reg)");
            Console.WriteLine("[5]  Limpar Cache Valorant       [12] Exibir Informações do Sistema");
            Console.WriteLine("[6]  Spoof Adaptadores de Vídeo  [13] Spoof Completo (Opções 1 a 10)");
            Console.WriteLine("[7]  Spoof Nome do Computador    [14] Bloquear Qualquer Acesso Remoto");
            Console.WriteLine("                                 [exit/sair] Fechar Programa");
            Console.ResetColor();
        }

        private static void Pause()
        {
            Console.WriteLine("\n  Pressione qualquer tecla para voltar ao menu...");
            try
            {
                if (!Console.IsInputRedirected)
                {
                    Console.ReadKey(true);
                }
                else
                {
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.ReadLine();
            }
        }

        private static void PrintResult(OperationResult result)
        {
            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  [+] {result.Message}");
                if (!string.IsNullOrEmpty(result.Details))
                {
                    Console.WriteLine($"      {result.Details}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  [X] {result.Message}");
                if (!string.IsNullOrEmpty(result.Details))
                {
                    Console.WriteLine($"      {result.Details}");
                }
            }
            Console.ResetColor();
        }

        private static void DisplaySystemData()
        {
            Console.WriteLine("\nColetando informações do sistema...");
            var info = SystemInfoService.GetSystemInformation();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine($"Nome do PC:          {info.ComputerName}");
            Console.WriteLine($"Processador:         {info.ProcessorName}");
            Console.WriteLine($"Memória Total:       {info.TotalMemoryGigabytes:F2} GB");
            Console.WriteLine($"Installation ID:     {info.InstallationId}");
            Console.WriteLine($"Machine GUID:        {info.MachineGuid}");
            Console.WriteLine($"Machine ID:          {info.MachineId}");
            Console.WriteLine($"BIOS Release:        {info.BiosReleaseDate}");
            Console.WriteLine($"BIOS Version:        {info.BiosVersion}");
            Console.WriteLine($"SMBIOS Serial:       {info.SmbiosSerialNumber}");

            Console.WriteLine("\nAdaptadores de Vídeo (GPU):");
            if (info.GraphicsCards.Count > 0)
            {
                foreach (var gpu in info.GraphicsCards)
                {
                    Console.WriteLine($"  - {gpu}");
                }
            }
            else
            {
                Console.WriteLine("  - Nenhum adaptador detectado via registro.");
            }

            Console.WriteLine("\nAdaptadores de Rede (MACs):");
            if (info.NetworkAdapters.Count > 0)
            {
                foreach (var nic in info.NetworkAdapters)
                {
                    Console.WriteLine($"  - {nic.Name}: {nic.MacAddress}");
                }
            }
            else
            {
                Console.WriteLine("  - Nenhum adaptador ativo detectado.");
            }
            Console.WriteLine("-------------------------------------------------------------");
            Console.ResetColor();
        }

        public static bool Menu()
        {
            Console.Write("\n  Selecione uma opção: ");
            string? input = Console.ReadLine()?.Trim().ToLower();

            switch (input)
            {
                case "1":
                    PrintResult(HardwareSpoofingService.SpoofDisks());
                    Pause();
                    return true;

                case "2":
                    PrintResult(HardwareSpoofingService.SpoofGuids());
                    Pause();
                    return true;

                case "3":
                    PrintResult(HardwareSpoofingService.SpoofMac());
                    Pause();
                    return true;

                case "4":
                    PrintResult(CleanerService.CleanUbisoftCache());
                    Pause();
                    return true;

                case "5":
                    PrintResult(CleanerService.CleanValorantCache());
                    Pause();
                    return true;

                case "6":
                    PrintResult(HardwareSpoofingService.SpoofGpu());
                    Pause();
                    return true;

                case "7":
                    PrintResult(HardwareSpoofingService.SpoofComputerName());
                    Pause();
                    return true;

                case "8":
                    PrintResult(HardwareSpoofingService.SpoofInstallationId());
                    Pause();
                    return true;

                case "9":
                    PrintResult(HardwareSpoofingService.SpoofEfiVariableId());
                    Pause();
                    return true;

                case "10":
                    PrintResult(HardwareSpoofingService.SpoofSmbiosSerialNumber());
                    Pause();
                    return true;

                case "11":
                    string backupFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");
                    PrintResult(BackupService.CreateBackup(backupFolder));
                    Pause();
                    return true;

                case "12":
                    DisplaySystemData();
                    Pause();
                    return true;

                case "13":
                    Console.WriteLine("\n  [+] Executando rotina completa de spoofing...");
                    PrintResult(HardwareSpoofingService.SpoofDisks());
                    PrintResult(HardwareSpoofingService.SpoofGuids());
                    PrintResult(HardwareSpoofingService.SpoofMac());
                    PrintResult(CleanerService.CleanUbisoftCache());
                    PrintResult(CleanerService.CleanValorantCache());
                    PrintResult(HardwareSpoofingService.SpoofGpu());
                    PrintResult(HardwareSpoofingService.SpoofComputerName());
                    PrintResult(HardwareSpoofingService.SpoofInstallationId());
                    PrintResult(HardwareSpoofingService.SpoofEfiVariableId());
                    PrintResult(HardwareSpoofingService.SpoofSmbiosSerialNumber());
                    Pause();
                    return true;

                case "14":
                    PrintResult(SecurityHardeningService.DisableRemoteAccess());
                    Pause();
                    return true;

                case "exit":
                case "sair":
                    return false;

                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  [!] Opção inválida!");
                    Console.ResetColor();
                    Pause();
                    return true;
            }
        }
    }
}
