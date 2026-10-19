using Microsoft.Win32;
using System.Net.NetworkInformation;
using SecHex.Core.Models;

namespace SecHex.Core.Services
{
    public static class SystemInfoService
    {
        public static SystemInformation GetSystemInformation()
        {
            var info = new SystemInformation();

            // 1. Installation ID
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                info.InstallationId = key?.GetValue("InstallationID") as string ?? "Não disponível";
            }
            catch { }

            // 2. Machine GUID
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                info.MachineGuid = key?.GetValue("MachineGuid") as string ?? "Não disponível";
            }
            catch { }

            // 3. Machine ID
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient");
                info.MachineId = key?.GetValue("MachineId") as string ?? "Não disponível";
            }
            catch { }

            // 4. Computer Name
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName");
                info.ComputerName = key?.GetValue("ComputerName") as string ?? Environment.MachineName;
            }
            catch { }

            // 5. Processor Name
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                info.ProcessorName = key?.GetValue("ProcessorNameString") as string 
                                     ?? Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") 
                                     ?? "Desconhecido";
            }
            catch { }

            // 6. Memory
            try
            {
                var memInfo = GC.GetGCMemoryInfo();
                info.TotalMemoryGigabytes = memInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);
            }
            catch { }

            // 7. Graphics Cards
            try
            {
                string displayClass = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
                using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(displayClass);
                if (classKey != null)
                {
                    foreach (string subKey in classKey.GetSubKeyNames())
                    {
                        if (subKey.Length == 4 && int.TryParse(subKey, out _))
                        {
                            using RegistryKey? adapterKey = classKey.OpenSubKey(subKey);
                            string? desc = adapterKey?.GetValue("DriverDesc") as string;
                            if (!string.IsNullOrEmpty(desc))
                            {
                                info.GraphicsCards.Add(desc);
                            }
                        }
                    }
                }
            }
            catch { }

            // 8. Network Adapters
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        info.NetworkAdapters.Add(new NetworkAdapterInfo
                        {
                            Name = nic.Name,
                            MacAddress = nic.GetPhysicalAddress().ToString(),
                            Id = nic.Id
                        });
                    }
                }
            }
            catch { }

            // 9. BIOS & SMBIOS
            try
            {
                using RegistryKey? sysInfo = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation");
                info.BiosReleaseDate = sysInfo?.GetValue("BIOSReleaseDate") as string ?? "Não disponível";
                info.BiosVersion = sysInfo?.GetValue("BIOSVersion") as string ?? "Não disponível";

                using RegistryKey? smbios = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
                info.SmbiosSerialNumber = smbios?.GetValue("SystemSerialNumber") as string ?? "Não disponível";
            }
            catch { }

            return info;
        }
    }
}
