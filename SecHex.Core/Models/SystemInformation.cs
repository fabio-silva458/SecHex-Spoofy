namespace SecHex.Core.Models
{
    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
    }

    public class SystemInformation
    {
        public string InstallationId { get; set; } = string.Empty;
        public string MachineGuid { get; set; } = string.Empty;
        public string MachineId { get; set; } = string.Empty;
        public string ComputerName { get; set; } = string.Empty;
        public string ProcessorName { get; set; } = string.Empty;
        public double TotalMemoryGigabytes { get; set; }
        public List<string> GraphicsCards { get; set; } = new();
        public List<NetworkAdapterInfo> NetworkAdapters { get; set; } = new();
        public string BiosReleaseDate { get; set; } = string.Empty;
        public string BiosVersion { get; set; } = string.Empty;
        public string SmbiosSerialNumber { get; set; } = string.Empty;
    }
}
