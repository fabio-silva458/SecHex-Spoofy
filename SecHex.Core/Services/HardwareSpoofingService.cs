using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Text;
using SecHex.Core.Helpers;
using SecHex.Core.Models;

namespace SecHex.Core.Services
{
    public static class HardwareSpoofingService
    {
        public static OperationResult SpoofDisks()
        {
            try
            {
                int modified = 0;
                using RegistryKey? scsiPorts = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\Scsi");
                if (scsiPorts != null)
                {
                    foreach (string port in scsiPorts.GetSubKeyNames())
                    {
                        using RegistryKey? scsiBuses = Registry.LocalMachine.OpenSubKey($@"HARDWARE\DEVICEMAP\Scsi\{port}");
                        if (scsiBuses == null) continue;

                        foreach (string bus in scsiBuses.GetSubKeyNames())
                        {
                            using RegistryKey? targetKey = Registry.LocalMachine.OpenSubKey($@"HARDWARE\DEVICEMAP\Scsi\{port}\{bus}\Target Id 0\Logical Unit Id 0", true);
                            if (targetKey != null && targetKey.GetValue("DeviceType")?.ToString() == "DiskPeripheral")
                            {
                                string identifier = RandomHelper.GenerateAlphanumeric(14);
                                string serialNumber = RandomHelper.GenerateAlphanumeric(14);

                                targetKey.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(serialNumber));
                                targetKey.SetValue("Identifier", identifier);
                                targetKey.SetValue("InquiryData", Encoding.UTF8.GetBytes(identifier));
                                targetKey.SetValue("SerialNumber", serialNumber);
                                modified++;
                            }
                        }
                    }
                }

                using RegistryKey? diskPeripherals = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\MultifunctionAdapter\0\DiskController\0\DiskPeripheral");
                if (diskPeripherals != null)
                {
                    foreach (string disk in diskPeripherals.GetSubKeyNames())
                    {
                        using RegistryKey? diskPeripheral = Registry.LocalMachine.OpenSubKey($@"HARDWARE\DESCRIPTION\System\MultifunctionAdapter\0\DiskController\0\DiskPeripheral\{disk}", true);
                        diskPeripheral?.SetValue("Identifier", $"{RandomHelper.GenerateAlphanumeric(8)}-{RandomHelper.GenerateAlphanumeric(8)}-A");
                        modified++;
                    }
                }

                return OperationResult.Ok("Discos spoofados com sucesso.", $"{modified} entradas atualizadas.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao spoofar discos: " + ex.Message);
            }
        }

        public static OperationResult SpoofGuids()
        {
            try
            {
                using (RegistryKey? hardwareGuid = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001", true))
                {
                    hardwareGuid?.SetValue("HwProfileGuid", $"{{{Guid.NewGuid()}}}");
                }

                using (RegistryKey? machineGuid = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", true))
                {
                    machineGuid?.SetValue("MachineGuid", Guid.NewGuid().ToString());
                }

                using (RegistryKey? machineId = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient", true))
                {
                    machineId?.SetValue("MachineId", $"{{{Guid.NewGuid()}}}");
                }

                using (RegistryKey? systemInfo = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation", true))
                {
                    if (systemInfo != null)
                    {
                        systemInfo.SetValue("BIOSReleaseDate", RandomHelper.GenerateRandomDateString());
                        systemInfo.SetValue("BIOSVersion", RandomHelper.GenerateAlphanumeric(10));
                        systemInfo.SetValue("ComputerHardwareId", $"{{{Guid.NewGuid()}}}");
                        systemInfo.SetValue("ComputerHardwareIds", $"{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n");
                        systemInfo.SetValue("SystemManufacturer", "SystemManufacturer-" + RandomHelper.GenerateAlphanumeric(6));
                        systemInfo.SetValue("SystemProductName", "Product-" + RandomHelper.GenerateAlphanumeric(6));
                    }
                }

                using (RegistryKey? update = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate", true))
                {
                    update?.SetValue("SusClientId", Guid.NewGuid().ToString());
                    update?.SetValue("SusClientIdValidation", Encoding.UTF8.GetBytes(RandomHelper.GenerateAlphanumeric(25)));
                }

                return OperationResult.Ok("GUIDs e telemetria atualizados com sucesso.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao atualizar GUIDs: " + ex.Message);
            }
        }

        public static OperationResult SpoofMac()
        {
            try
            {
                int modified = 0;
                using RegistryKey? networkAdapters = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}");
                if (networkAdapters == null)
                    return OperationResult.Fail("Chave de adaptadores de rede não encontrada.");

                foreach (string adapter in networkAdapters.GetSubKeyNames())
                {
                    if (adapter == "Properties") continue;

                    using RegistryKey? networkAdapter = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}\{adapter}", true);
                    if (networkAdapter != null && networkAdapter.GetValue("BusType") != null)
                    {
                        string newMac = RandomHelper.GenerateMacAddress();
                        networkAdapter.SetValue("NetworkAddress", newMac);

                        object? instanceId = networkAdapter.GetValue("NetCfgInstanceId");
                        if (instanceId != null)
                        {
                            RestartNetworkInterface(instanceId.ToString()!);
                        }
                        modified++;
                    }
                }

                return OperationResult.Ok("MAC Address alterado com sucesso.", $"{modified} adaptadores modificados.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao alterar MAC Address: " + ex.Message);
            }
        }

        public static OperationResult SpoofComputerName()
        {
            try
            {
                string randomName = "PC-" + RandomHelper.GenerateAlphanumeric(8);
                using (RegistryKey? computerName = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName", true))
                {
                    computerName?.SetValue("ComputerName", randomName);
                    computerName?.SetValue("ActiveComputerName", randomName);
                    computerName?.SetValue("ComputerNamePhysicalDnsDomain", "");
                }

                using (RegistryKey? activeComputerName = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\ComputerName\ActiveComputerName", true))
                {
                    activeComputerName?.SetValue("ComputerName", randomName);
                    activeComputerName?.SetValue("ActiveComputerName", randomName);
                    activeComputerName?.SetValue("ComputerNamePhysicalDnsDomain", "");
                }

                using (RegistryKey? tcpipParams = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", true))
                {
                    tcpipParams?.SetValue("Hostname", randomName);
                    tcpipParams?.SetValue("NV Hostname", randomName);
                }

                using (RegistryKey? tcpipInterfaces = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true))
                {
                    if (tcpipInterfaces != null)
                    {
                        foreach (string interfaceKey in tcpipInterfaces.GetSubKeyNames())
                        {
                            using RegistryKey? interfaceSubKey = tcpipInterfaces.OpenSubKey(interfaceKey, true);
                            interfaceSubKey?.SetValue("Hostname", randomName);
                            interfaceSubKey?.SetValue("NV Hostname", randomName);
                        }
                    }
                }

                return OperationResult.Ok("Nome do computador alterado com sucesso.", "Novo nome: " + randomName);
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao alterar nome do PC: " + ex.Message);
            }
        }

        public static OperationResult SpoofInstallationId()
        {
            try
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true);
                if (key != null)
                {
                    string newId = Guid.NewGuid().ToString();
                    key.SetValue("InstallationID", newId);
                    return OperationResult.Ok("Installation ID alterado.", newId);
                }
                return OperationResult.Fail("Chave de versão do Windows não encontrada.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao alterar Installation ID: " + ex.Message);
            }
        }

        public static OperationResult SpoofEfiVariableId()
        {
            try
            {
                using RegistryKey? efiVariables = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Nsi\{eb004a03-9b1a-11d4-9123-0050047759bc}\26", true);
                if (efiVariables != null)
                {
                    string efiVariableId = Guid.NewGuid().ToString();
                    efiVariables.SetValue("VariableId", efiVariableId);
                    return OperationResult.Ok("Variável EFI alterada.", efiVariableId);
                }
                return OperationResult.Fail("Chave de variáveis EFI não encontrada.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao alterar EFI VariableId: " + ex.Message);
            }
        }

        public static OperationResult SpoofSmbiosSerialNumber()
        {
            try
            {
                using RegistryKey? smbiosData = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS", true);
                if (smbiosData != null)
                {
                    string serialNumber = RandomHelper.GenerateAlphanumeric(12);
                    smbiosData.SetValue("SystemSerialNumber", serialNumber);
                    return OperationResult.Ok("Serial SMBIOS alterado.", serialNumber);
                }
                return OperationResult.Fail("Chave SMBIOS não encontrada.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao alterar SMBIOS: " + ex.Message);
            }
        }

        public static OperationResult SpoofGpu()
        {
            try
            {
                string displayClass = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
                using RegistryKey? classKey = Registry.LocalMachine.OpenSubKey(displayClass, true);
                if (classKey != null)
                {
                    int spoofed = 0;
                    foreach (string subKeyName in classKey.GetSubKeyNames())
                    {
                        if (subKeyName.Length == 4 && int.TryParse(subKeyName, out _))
                        {
                            using RegistryKey? adapterKey = classKey.OpenSubKey(subKeyName, true);
                            if (adapterKey != null && adapterKey.GetValue("DriverDesc") != null)
                            {
                                string fakeName = "Display Adapter " + RandomHelper.GenerateAlphanumeric(4);
                                adapterKey.SetValue("DriverDesc", fakeName);
                                adapterKey.SetValue("HardwareInformation.AdapterString", fakeName);
                                spoofed++;
                            }
                        }
                    }
                    return OperationResult.Ok("Adaptadores de vídeo spoofados com sucesso.", $"{spoofed} dispositivos alterados.");
                }
                return OperationResult.Fail("Classe de vídeo não encontrada.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Erro ao spoofar GPU: " + ex.Message);
            }
        }

        private static void RestartNetworkInterface(string adapterId)
        {
            try
            {
                string interfaceName = "Ethernet";
                foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (i.Id == adapterId)
                    {
                        interfaceName = i.Name;
                        break;
                    }
                }

                ProcessHelper.RunHiddenProcess("netsh", $"interface set interface \"{interfaceName}\" disable");
                ProcessHelper.RunHiddenProcess("netsh", $"interface set interface \"{interfaceName}\" enable");
            }
            catch { }
        }
    }
}
