using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Toaster
{
    public enum DiskSmartSupport
    {
        Unknown = 0,
        Unavailable,
        AvailableButDisabled,
        AvailableAndEnabled
    }

    public class DetectedDisk
    {
        // Read from call to smartctl.
        public DiskSmartSupport SmartSupport { get; set; }
        public Boolean? OverallFailurePredicted { get; set; }

        public Dictionary<int, SmartAttribute> SmartAttributes = new Dictionary<int, SmartAttribute>();
        public String SmartRotationRate { get; set; }  // Ideally, we wouldn't lean on SMARTCTL to retrieve this, but it's not available in the Win32_DiskDrive query.
        public String SmartctlOutput { get; set; }  // Just the whole big command output
        public String SmartGrade { get; set; }
        public Boolean? SmartPassed { get; set; }  // True, False, null (Undetermined)	

        //Read from Win32_DiskDrive
        public UInt32 DiskNumber { get; set; }
        public String Vendor { get; set; }
        public String ProductId { get; set; }
        public ushort? Availability { get; set; }
        public uint? BytesPerSector { get; set; }
        public ushort[] Capabilities { get; set; }
        public string[] CapabilityDescriptions { get; set; }
        public string Caption { get; set; }
        public string CompressionMethod { get; set; }
        public uint? ConfigManagerErrorCode { get; set; }
        public bool? ConfigManagerUserConfig { get; set; }
        public string CreationClassName { get; set; }
        public ulong? DefaultBlockSize { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }
        public bool? ErrorCleared { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorMethodology { get; set; }
        public string FirmwareRevision { get; set; }
        public uint? Index { get; set; }
        public DateTime? InstallDate { get; set; }
        public string InterfaceType { get; set; }
        public uint? LastErrorCode { get; set; }
        public string Manufacturer { get; set; }
        public ulong? MaxBlockSize { get; set; }
        public ulong? MaxMediaSize { get; set; }
        public bool? MediaLoaded { get; set; }
        public string MediaType { get; set; }
        public ulong? MinBlockSize { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public bool? NeedsCleaning { get; set; }
        public uint? NumberOfMediaSupported { get; set; }
        public uint? Partitions { get; set; }
        public string PNPDeviceID { get; set; }
        public ushort[] PowerManagementCapabilities { get; set; }
        public bool? PowerManagementSupported { get; set; }
        public uint? SCSIBus { get; set; }
        public ushort? SCSILogicalUnit { get; set; }
        public ushort? SCSIPort { get; set; }
        public ushort? SCSITargetId { get; set; }
        public uint? SectorsPerTrack { get; set; }
        public string SerialNumber { get; set; }
        public uint? Signature { get; set; }
        public ulong? Size { get; set; }
        public string Status { get; set; }
        public ushort? StatusInfo { get; set; }
        public string SystemCreationClassName { get; set; }
        public string SystemName { get; set; }
        public ulong? TotalCylinders { get; set; }
        public uint? TotalHeads { get; set; }
        public ulong? TotalSectors { get; set; }
        public ulong? TotalTracks { get; set; }
        public uint? TracksPerCylinder { get; set; }


        public const string VIA_SAS_ADDRESS_DESCRIPTOR = "Via SAS Address: ";

        public class SmartAttribute
        {
            public string AttributeDescription { get; set; }
            public string AttributeType { get; set; }
            public int Current { get; set; }
            public int Worst { get; set; }
            public int Threshold { get; set; }
            public string RawValue { get; set; }
            public bool IsOK { get; set; }

            public SmartAttribute(string attributeName)
            {
                this.AttributeDescription = attributeName;
            }
        }
    }
}
