using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Toaster
{
    public class DiskDetector
    {
        private class Win32_DiskDrive
        {
            public ushort? Availability;
            public uint? BytesPerSector;
            public ushort[] Capabilities;
            public string[] CapabilityDescriptions;
            public string Caption;
            public string CompressionMethod;
            public uint? ConfigManagerErrorCode;
            public bool? ConfigManagerUserConfig;
            public string CreationClassName;
            public ulong? DefaultBlockSize;
            public string Description;
            public string DeviceID;
            public bool? ErrorCleared;
            public string ErrorDescription;
            public string ErrorMethodology;
            public string FirmwareRevision;
            public uint? Index;
            public DateTime? InstallDate;
            public string InterfaceType;
            public uint? LastErrorCode;
            public string Manufacturer;
            public ulong? MaxBlockSize;
            public ulong? MaxMediaSize;
            public bool? MediaLoaded;
            public string MediaType;
            public ulong? MinBlockSize;
            public string Model;
            public string Name;
            public bool? NeedsCleaning;
            public uint? NumberOfMediaSupported;
            public uint? Partitions;
            public string PNPDeviceID;
            public ushort[] PowerManagementCapabilities;
            public bool? PowerManagementSupported;
            public uint? SCSIBus;
            public ushort? SCSILogicalUnit;
            public ushort? SCSIPort;
            public ushort? SCSITargetId;
            public uint? SectorsPerTrack;
            public string SerialNumber;
            public uint? Signature;
            public ulong? Size;
            public string Status;
            public ushort? StatusInfo;
            public string SystemCreationClassName;
            public string SystemName;
            public ulong? TotalCylinders;
            public uint? TotalHeads;
            public ulong? TotalSectors;
            public ulong? TotalTracks;
            public uint? TracksPerCylinder;
        }
        private readonly string SMARTCTL_FULL_FILE_PATH = @"C:\Program Files\smartmontools\bin\smartctl.exe";
        /// <summary>
        /// Detects all normal (not primary, not boot) disks, and gathers attributes about each.
        /// </summary>
        /// <param name="hadError"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public List<DetectedDisk> Detect(bool doGatherSmartData, out Boolean hadError, out String errorMessage)
        {
            // Inititialize out parameters.
            hadError = false;
            errorMessage = null;

            // Initialize return object.
            List<DetectedDisk> detectedDisks = new List<DetectedDisk>();

            // Assume primary and boot partitions are both Physical Disk 0.
            UInt32 primaryPartitionDiskNumber = 0;
            UInt32 bootPartitionDiskNumber = 0;

            // Keep track of errors encountered while iterating disks.
            List<string> diskIterationErrors = new List<string>();
            // Get the list of Win32_DiskDrive items.
            List<Win32_DiskDrive> w32diskDrives;
            try
            {
                w32diskDrives = GetDiskDrives();
            }
            catch (Exception ex)
            {
                hadError = true;
                errorMessage = "Error while retrieving disk drives: " + ex.Message;

                w32diskDrives = new List<Win32_DiskDrive>();
            }
            foreach (Win32_DiskDrive w32diskDrive in w32diskDrives)
            {
                try
                {
                    if (w32diskDrive.Index.HasValue)
                    {
                        // Skip this if it is the hosting the boot partition or primary partition.
                        if ((w32diskDrive.Index.Value != primaryPartitionDiskNumber) && (w32diskDrive.Index.Value != bootPartitionDiskNumber))
                        {
                            // Make sure it has the other attributes we need.
                            if ((w32diskDrive.Size.HasValue) &&
                                (w32diskDrive.BytesPerSector.HasValue) &&
                                (w32diskDrive.SectorsPerTrack.HasValue) &&
                                (w32diskDrive.TotalTracks.HasValue))
                            {
                                // This is one we want, as a detected disk.
                                DetectedDisk disk = new DetectedDisk();

                                // Parse the Vendor and ProductId out of the PNPDeviceId.
                                string vendor = "UNK";
                                string productId = "UNK";
                                string pnpDeviceId = w32diskDrive.PNPDeviceID;
                                if (false == String.IsNullOrEmpty(pnpDeviceId))
                                {
                                    // EXAMPLE: SCSI\DISK&VEN_WDC&PROD_WD3200BEKX-75B7W\4&2C8732C3&0&000000
                                    string[] pnpThreePartsArray = pnpDeviceId.Split('\\');
                                    if (pnpThreePartsArray.Length >= 2)
                                    {
                                        string[] typeVenProdRevArray = pnpThreePartsArray[1].Split('&');
                                        foreach (string part in typeVenProdRevArray)
                                        {
                                            if (part.StartsWith("VEN_"))
                                            {
                                                vendor = part.Substring(("VEN_").Length);
                                            }
                                            if (part.StartsWith("PROD_"))
                                            {
                                                productId = part.Substring(("PROD_").Length);
                                            }
                                        }
                                    }
                                }

                                disk.DiskNumber = w32diskDrive.Index.Value;
                                disk.Vendor = vendor;
                                disk.ProductId = productId;
                                //Data From Win32_DiskDrive Below
                                disk.Availability = w32diskDrive.Availability;
                                disk.BytesPerSector = w32diskDrive.BytesPerSector;
                                disk.Capabilities = w32diskDrive.Capabilities;
                                disk.CapabilityDescriptions = w32diskDrive.CapabilityDescriptions;
                                disk.Caption = w32diskDrive.Caption;
                                disk.CompressionMethod = w32diskDrive.CompressionMethod;
                                disk.ConfigManagerErrorCode = w32diskDrive.ConfigManagerErrorCode;
                                disk.ConfigManagerUserConfig = w32diskDrive.ConfigManagerUserConfig;
                                disk.CreationClassName = w32diskDrive.CreationClassName;
                                disk.DefaultBlockSize = w32diskDrive.DefaultBlockSize;
                                disk.Description = w32diskDrive.Description;
                                disk.DeviceID = w32diskDrive.DeviceID;
                                disk.ErrorCleared = w32diskDrive.ErrorCleared;
                                disk.ErrorDescription = w32diskDrive.ErrorDescription;
                                disk.ErrorMethodology = w32diskDrive.ErrorMethodology;
                                disk.FirmwareRevision = w32diskDrive.FirmwareRevision;
                                disk.Index = w32diskDrive.Index;
                                disk.InstallDate = w32diskDrive.InstallDate;
                                disk.InterfaceType = w32diskDrive.InterfaceType;
                                disk.Manufacturer = w32diskDrive.Manufacturer;
                                disk.MaxBlockSize = w32diskDrive.MaxBlockSize;
                                disk.MaxMediaSize = w32diskDrive.MaxMediaSize;
                                disk.MediaLoaded = w32diskDrive.MediaLoaded;
                                disk.MediaType = w32diskDrive.MediaType;
                                disk.MinBlockSize = w32diskDrive.MinBlockSize;
                                disk.Model = w32diskDrive.Model;
                                disk.Name = w32diskDrive.Name;
                                disk.NeedsCleaning = w32diskDrive.NeedsCleaning;
                                disk.NumberOfMediaSupported = w32diskDrive.NumberOfMediaSupported;
                                disk.Partitions = w32diskDrive.Partitions;
                                disk.PNPDeviceID = w32diskDrive.PNPDeviceID;
                                disk.PowerManagementCapabilities = w32diskDrive.PowerManagementCapabilities;
                                disk.PowerManagementSupported = w32diskDrive.PowerManagementSupported;
                                disk.SCSIBus = w32diskDrive.SCSIBus;
                                disk.SCSILogicalUnit = w32diskDrive.SCSILogicalUnit;
                                disk.SCSIPort = w32diskDrive.SCSIPort;
                                disk.SCSITargetId = w32diskDrive.SCSITargetId;
                                disk.SectorsPerTrack = w32diskDrive.SectorsPerTrack;
                                disk.SerialNumber = String.IsNullOrEmpty(w32diskDrive.SerialNumber) ? "" : w32diskDrive.SerialNumber.Trim();
                                disk.Signature = w32diskDrive.Signature;
                                disk.Size = w32diskDrive.Size;
                                disk.Status = w32diskDrive.Status;
                                disk.StatusInfo = w32diskDrive.StatusInfo;
                                disk.SystemCreationClassName = w32diskDrive.SystemCreationClassName;
                                disk.SystemName = w32diskDrive.SystemName;
                                disk.TotalCylinders = w32diskDrive.TotalCylinders;
                                disk.TotalHeads = w32diskDrive.TotalHeads;
                                disk.TotalSectors = w32diskDrive.TotalSectors;
                                disk.TotalTracks = w32diskDrive.TotalTracks;
                                disk.TracksPerCylinder = w32diskDrive.TracksPerCylinder;
                                detectedDisks.Add(disk);
                            }
                            else
                            {
                                diskIterationErrors.Add("Physical Disk " + w32diskDrive.Index.Value.ToString() + " is missing fundamental attributes.");
                                continue;  // Jump out without adding it to the collection to return.
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if ((null != w32diskDrive) && (w32diskDrive.Index.HasValue))
                    {
                        diskIterationErrors.Add("Unexpected error while processing Physical Disk " + w32diskDrive.Index.Value.ToString() + ": " + ex.Message);
                        continue;  // Jump out without adding it to the collection to return.
                    }
                    else
                    {
                        diskIterationErrors.Add("Unexpected error while processing Physical Disk: " + ex.Message);
                        continue;  // Jump out without adding it to the collection to return.
                    }
                }
            }

            // See if we had any errors while iterating the disks.
            if (false == hadError)
            {
                if (diskIterationErrors.Count > 0)
                {
                    hadError = true;
                    errorMessage = "Encountered errors while iterating disks: " + String.Join("; ", diskIterationErrors);
                }
            }

            if (false == hadError)
            {
                // Since gathering SMART data can take a while, we don't do it unless caller wants us to.
                if (doGatherSmartData)
                {
                    if (false == File.Exists(SMARTCTL_FULL_FILE_PATH))
                    {
                        hadError = true;
                        errorMessage = "Unable to find third-party SMARTCTL software at: " + SMARTCTL_FULL_FILE_PATH;
                    }
                    else
                    {
                        try
                        {
                            // Grab the SMART-related data for the detected disks.
                            populateSmartAttributes_againstSmartctl(SMARTCTL_FULL_FILE_PATH, detectedDisks);

                            // Consider the SMART-related data, and apply the SMART grades.
                            foreach (DetectedDisk disk in detectedDisks)
                            {
                                // Interpret the SMART output only if it is available and enabled.
                                if (disk.SmartSupport == DiskSmartSupport.AvailableAndEnabled)
                                {
                                    var smartAttributeFailures = disk.SmartAttributes.Values.Where(sav => false == sav.IsOK);
                                    if (true == smartAttributeFailures.Any())
                                    {
                                        // Okay, we know there is at least one failure reported by the SMART attributes.
                                        // We will want to list out the problems.
                                        // So see if there was also an overall failure.
                                        string smartGrade = "FAILED";
                                        if (disk.OverallFailurePredicted.HasValue && disk.OverallFailurePredicted.Value)
                                        {
                                            // Also an overall failure.
                                            smartGrade += " [overall failure predicted; also failures with SMART attributes: ";
                                        }
                                        else
                                        {
                                            // No overall failure predicted, but at least one SMART attribute failure reported.
                                            smartGrade += " [failures with SMART attributes: ";
                                        }
                                        bool isFirstFailure = true;
                                        foreach (var failure in smartAttributeFailures)
                                        {
                                            if (false == isFirstFailure)
                                            {
                                                smartGrade += ", ";
                                            }
                                            smartGrade += failure.AttributeDescription;
                                        }
                                        smartGrade += "]";

                                        disk.SmartGrade = smartGrade;
                                        disk.SmartPassed = false;
                                    }
                                    else
                                    {
                                        if (disk.OverallFailurePredicted.HasValue)
                                        {
                                            if (disk.OverallFailurePredicted.Value)
                                            {
                                                disk.SmartGrade = "FAILED [overall failure predicted]";
                                                disk.SmartPassed = false;
                                            }
                                            else
                                            {
                                                // We want to let the user know if we are passing it based on only the crude check (without detailed SMART data).
                                                if (disk.SmartAttributes.Values.Any())
                                                {
                                                    // We had some detailed SMART data, yet nothing to cause failure.
                                                    disk.SmartGrade = "PASSED [found SMART attributes]";
                                                    disk.SmartPassed = true;
                                                }
                                                else
                                                {
                                                    // We are passing it on the crude check, without any detailed SMART data.
                                                    disk.SmartGrade = "PASSED [no SMART attributes found]";
                                                    disk.SmartPassed = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // We really don't know.
                                            disk.SmartGrade = "UNK [unable to find overall failure indicator]";
                                            disk.SmartPassed = null; // To indicate unknown.
                                        }
                                    }
                                }
                                else
                                {
                                    // SMART is not available and enabled on the disk.
                                    if (disk.SmartSupport == DiskSmartSupport.Unavailable)
                                    {
                                        disk.SmartGrade = "UNK [SMART unavailable]";
                                        disk.SmartPassed = null; // To indicate unknown.
                                    }
                                    else if (disk.SmartSupport == DiskSmartSupport.AvailableButDisabled)
                                    {
                                        disk.SmartGrade = "UNK [SMART disabled]";
                                        disk.SmartPassed = null; // To indicate unknown.
                                    }
                                    else
                                    {
                                        disk.SmartGrade = "UNK [unable to determine SMART support]";
                                        disk.SmartPassed = null; // To indicate unknown.
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            hadError = true;
                            errorMessage = "Encountered unexpected error while gathering SMART data: " + ex.Message;
                        }
                    }
                }
            }

            return detectedDisks;
        }

        private List<Win32_DiskDrive> GetDiskDrives()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            ManagementObjectCollection collection = searcher.Get();

            var items = new List<Win32_DiskDrive>();
            foreach (ManagementObject obj in collection)
            {
                var item = new Win32_DiskDrive
                {
                    Availability = (ushort?)obj["Availability"],
                    BytesPerSector = (uint?)obj["BytesPerSector"],
                    Capabilities = (ushort[])obj["Capabilities"],
                    CapabilityDescriptions = (string[])obj["CapabilityDescriptions"],
                    Caption = (string)obj["Caption"],
                    CompressionMethod = (string)obj["CompressionMethod"],
                    ConfigManagerErrorCode = (uint?)obj["ConfigManagerErrorCode"],
                    ConfigManagerUserConfig = (bool?)obj["ConfigManagerUserConfig"],
                    CreationClassName = (string)obj["CreationClassName"],
                    DefaultBlockSize = (ulong?)obj["DefaultBlockSize"],
                    Description = (string)obj["Description"],
                    DeviceID = (string)obj["DeviceID"],
                    ErrorCleared = (bool?)obj["ErrorCleared"],
                    ErrorDescription = (string)obj["ErrorDescription"],
                    ErrorMethodology = (string)obj["ErrorMethodology"],
                    FirmwareRevision = (string)obj["FirmwareRevision"],
                    Index = (uint?)obj["Index"],
                    InstallDate = (DateTime?)obj["InstallDate"],
                    InterfaceType = (string)obj["InterfaceType"],
                    LastErrorCode = (uint?)obj["LastErrorCode"],
                    Manufacturer = (string)obj["Manufacturer"],
                    MaxBlockSize = (ulong?)obj["MaxBlockSize"],
                    MaxMediaSize = (ulong?)obj["MaxMediaSize"],
                    MediaLoaded = (bool?)obj["MediaLoaded"],
                    MediaType = (string)obj["MediaType"],
                    MinBlockSize = (ulong?)obj["MinBlockSize"],
                    Model = (string)obj["Model"],
                    Name = (string)obj["Name"],
                    NeedsCleaning = (bool?)obj["NeedsCleaning"],
                    NumberOfMediaSupported = (uint?)obj["NumberOfMediaSupported"],
                    Partitions = (uint?)obj["Partitions"],
                    PNPDeviceID = (string)obj["PNPDeviceID"],
                    PowerManagementCapabilities = (ushort[])obj["PowerManagementCapabilities"],
                    PowerManagementSupported = (bool?)obj["PowerManagementSupported"],
                    SCSIBus = (uint?)obj["SCSIBus"],
                    SCSILogicalUnit = (ushort?)obj["SCSILogicalUnit"],
                    SCSIPort = (ushort?)obj["SCSIPort"],
                    SCSITargetId = (ushort?)obj["SCSITargetId"],
                    SectorsPerTrack = (uint?)obj["SectorsPerTrack"],
                    SerialNumber = (string)obj["SerialNumber"],
                    Signature = (uint?)obj["Signature"],
                    Size = (ulong?)obj["Size"],
                    Status = (string)obj["Status"],
                    StatusInfo = (ushort?)obj["StatusInfo"],
                    SystemCreationClassName = (string)obj["SystemCreationClassName"],
                    SystemName = (string)obj["SystemName"],
                    TotalCylinders = (ulong?)obj["TotalCylinders"],
                    TotalHeads = (uint?)obj["TotalHeads"],
                    TotalSectors = (ulong?)obj["TotalSectors"],
                    TotalTracks = (ulong?)obj["TotalTracks"],
                    TracksPerCylinder = (uint?)obj["TracksPerCylinder"]
                };

                items.Add(item);
            }

            return items;
        }
        private void populateSmartAttributes_againstSmartctl(string smartCtlExeFullPath, List<DetectedDisk> detectedDisks)
        {
            foreach (DetectedDisk disk in detectedDisks)
            {
                // First, just call the normal SMARTCTL routine to gather all SMART data, letting it determine the device type by itself.
                populateSmartAttributesForDisk_againstSmartctl(smartCtlExeFullPath, disk);

                // As long as SMARTCTL is saying that support is unavailable (or unknown), we are going to make an additional call to SMARTCTL, with command
                // flags that force it to interpret the disk as a certain device type.  We learned that it doesn't already choose the right device type,
                // and when it is wrong, it will say that SMART support is unavailable (or unknown), when that is not necessarily true.

                if ((disk.SmartSupport == DiskSmartSupport.Unknown) || (disk.SmartSupport == DiskSmartSupport.Unavailable))
                {
                    // We will make another call to SMARTCTL to gather all SMART data, but this time we will force it to treate it as a SAT device.
                    populateSmartAttributesForDisk_againstSmartctl(smartCtlExeFullPath, disk, "sat");
                }

                if ((disk.SmartSupport == DiskSmartSupport.Unknown) || (disk.SmartSupport == DiskSmartSupport.Unavailable))
                {
                    // We will make another call to SMARTCTL to gather all SMART data, but this time we will force it to treate it as a ATA device.
                    populateSmartAttributesForDisk_againstSmartctl(smartCtlExeFullPath, disk, "ata");
                }

                if ((disk.SmartSupport == DiskSmartSupport.Unknown) || (disk.SmartSupport == DiskSmartSupport.Unavailable))
                {
                    // We will make another call to SMARTCTL to gather all SMART data, but this time we will force it to treate it as a SCSI device.
                    populateSmartAttributesForDisk_againstSmartctl(smartCtlExeFullPath, disk, "scsi");
                }
            }
        }

        private void populateSmartAttributesForDisk_againstSmartctl(string smartCtlExeFullPath, DetectedDisk disk, string forceDeviceType = null)
        {
            // Build up the command that will gather all SMART information for a given physical disk.
            string command = null;
            if (String.IsNullOrEmpty(forceDeviceType))
            {
                // Call the normal routine to gather all SMART data, letting it determine the device type by itself.
                command = @"""" + smartCtlExeFullPath + @"""" + " -a /dev/pd" + disk.DiskNumber.ToString();
            }
            else
            {
                // Rather than rely on SMARTCTL to determine the device type, force it to treat it as a certain device type, because we have found
                // that it doesn't always choose the correct device type, by itself.  And when it has the wrong device type, it will incorrectly
                // report that SMART support is unavailable.
                command = @"""" + smartCtlExeFullPath + @"""" + " -a -d " + forceDeviceType + " /dev/pd" + disk.DiskNumber.ToString();
            }

            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
            procStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = false;

            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string standardOutput = proc.StandardOutput.ReadToEnd();  // Seems odd to read to end before waiting for exit, but that's what ol' MSDN tells us to do.
            proc.WaitForExit();

            if (false == String.IsNullOrEmpty(standardOutput))
            {
                // Grab a copy of the big old output.
                disk.SmartctlOutput = standardOutput;

                string[] outputLines = standardOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                // Read SMART support and rotation rate, which is in the information section.
                // Seek past the header line that denotes the information section, and parse out the lines that speak to SMART support and rotation rate.
                bool isPastInfoSectionHeader = false;
                string supportLabel = "SMART support is: ";
                string rotationRateLabel = "Rotation Rate: ";
                bool foundSupportLabels = false;
                bool supportIsAvailable = false;
                bool supportIsEnabled = false;
                for (int lineIdx = 0; lineIdx < outputLines.Length; lineIdx++)
                {
                    string line = outputLines[lineIdx];
                    if ((false == isPastInfoSectionHeader) && (line.StartsWith("=== START OF INFORMATION SECTION ===")))
                    {
                        isPastInfoSectionHeader = true;
                    }
                    else if (isPastInfoSectionHeader)
                    {
                        if (line.StartsWith("=== START"))
                        {
                            // Start of the next section, so we are past the area that reveals SMART support and rotation rate.
                            break;
                        }
                        else
                        {
                            // This is in the first chunk of lines after the information section header, where we find the SMART support indicators and rotation rate.
                            if (line.StartsWith(supportLabel))
                            {
                                foundSupportLabels = true;
                                string supportValue = line.Substring(supportLabel.Length).Trim();
                                if (supportValue.StartsWith("Available"))
                                {
                                    supportIsAvailable = true;
                                }
                                else if (supportValue.StartsWith("Enabled"))
                                {
                                    supportIsEnabled = true;
                                }
                            }
                            else if (line.StartsWith(rotationRateLabel))
                            {
                                // Found the rotation rate.
                                disk.SmartRotationRate = line.Substring(rotationRateLabel.Length).Trim();
                            }
                        }
                    }
                }
                if (foundSupportLabels)
                {
                    if (supportIsAvailable)
                    {
                        if (supportIsEnabled)
                        {
                            disk.SmartSupport = DiskSmartSupport.AvailableAndEnabled;
                        }
                        else
                        {
                            disk.SmartSupport = DiskSmartSupport.AvailableButDisabled;
                        }
                    }
                    else
                    {
                        disk.SmartSupport = DiskSmartSupport.Unavailable;
                    }
                }

                // Look for additional SMART information only if SMART support is available and enabled.
                if (disk.SmartSupport == DiskSmartSupport.AvailableAndEnabled)
                {
                    // Read SMART overall health test result (or health status, as not every drive supports self test logging).
                    // Seek past the header line that denotes the data section, and parse out the line that speaks to the SMART overall health test result (or health status).
                    bool isPastDataSectionHeader = false;
                    string overallTestResultLabel = "SMART overall-health self-assessment test result: ";
                    string healthStatusLabel = "SMART Health Status: ";
                    for (int lineIdx = 0; lineIdx < outputLines.Length; lineIdx++)
                    {
                        string line = outputLines[lineIdx];
                        if ((false == isPastDataSectionHeader) && (line.StartsWith("=== START OF READ SMART DATA SECTION ===")))
                        {
                            isPastDataSectionHeader = true;
                        }
                        else if (isPastDataSectionHeader)
                        {
                            if (line.StartsWith("=== START"))
                            {
                                // Start of the next section, so we are past the area that reveals SMART overall health test result (or health status).
                                break;
                            }
                            else
                            {
                                // This is in the first chunk of lines after the data section header, where we find the SMART overall health test result (or health status).
                                if (line.StartsWith(overallTestResultLabel))
                                {
                                    string overallTestResultValue = line.Substring(overallTestResultLabel.Length).Trim();
                                    if (overallTestResultValue.StartsWith("PASSED"))
                                    {
                                        disk.OverallFailurePredicted = false;
                                    }
                                    else
                                    {
                                        disk.OverallFailurePredicted = true;
                                    }
                                }
                                else if (line.StartsWith(healthStatusLabel))
                                {
                                    // Remember, no every drive supports self test logging, so on these disks we read the health status instead.
                                    string healthStatusResultValue = line.Substring(healthStatusLabel.Length).Trim();
                                    if (healthStatusResultValue == "OK")
                                    {
                                        disk.OverallFailurePredicted = false;
                                    }
                                    else
                                    {
                                        disk.OverallFailurePredicted = true;
                                    }
                                }
                            }
                        }
                    }

                    // Read SMART attributes.
                    // Seek past the header line that appears above the attribute lines, then parse the attribute lines. 
                    bool isPastAttrHeader = false;
                    bool hadErrorParsingAttributes = false;
                    for (int lineIdx = 0; lineIdx < outputLines.Length; lineIdx++)
                    {
                        string line = outputLines[lineIdx];
                        if ((false == isPastAttrHeader) && (line.StartsWith("ID#")))
                        {
                            isPastAttrHeader = true;
                        }
                        else if (isPastAttrHeader)
                        {
                            if (line.Trim().Length == 0)
                            {
                                // First blank line after listing the attributes, which means there are no more attributes listed.
                                break;
                            }
                            else
                            {
                                // This must be an attribute line.

                                bool hadParseError = false;
                                int smartAttributeId = -1;
                                string attributeName = null;
                                int value = 0;
                                int worst = 0;
                                int thresh = 0;
                                string attrType = null;
                                string rawValue = null;
                                parseSmartctlAttributeOutputLine(line, out hadParseError, out smartAttributeId, out attributeName, out value, out worst, out thresh, out attrType, out rawValue);

                                if (hadParseError)
                                {
                                    // Keep track of the error.
                                    hadErrorParsingAttributes = true;
                                }
                                else if (false == disk.SmartAttributes.ContainsKey(smartAttributeId))
                                {
                                    DetectedDisk.SmartAttribute attr = new DetectedDisk.SmartAttribute(attributeName);

                                    attr.AttributeType = attrType;
                                    attr.Current = value;
                                    attr.Worst = worst;
                                    attr.RawValue = rawValue;
                                    attr.Threshold = thresh;

                                    if (attrType.ToUpper() == "PRE-FAIL")
                                    {
                                        // This type of attribute has a meaningful threshold.
                                        if (value <= thresh)
                                        {
                                            // Failure is imminent.
                                            attr.IsOK = false;
                                        }
                                        else
                                        {
                                            attr.IsOK = true;
                                        }
                                    }
                                    else
                                    {
                                        attr.IsOK = true;  // Must be an OLD-AGE attribute, so no real threshold ... let's just say its okay.
                                    }

                                    disk.SmartAttributes.Add(smartAttributeId, attr);
                                }
                            }
                        }
                    }

                    if (hadErrorParsingAttributes)
                    {
                        // There was at least one attribute line that we had trouble parsing.

                        // So, if the disk hasn't had any sort of SMART failure indicated up to this point, we are going to flip the SMART support indicator to "Unknown", since
                        // we don't really want to give it a passing grade if we had problems parsing attributes.
                        if ((disk.OverallFailurePredicted.HasValue) && (disk.OverallFailurePredicted.Value))
                        {
                            // Already going to be reporting a failure, so the parse error doesn't really affect anything.
                        }
                        else
                        {
                            var smartAttributeFailures = disk.SmartAttributes.Values.Where(sav => false == sav.IsOK);
                            if (true == smartAttributeFailures.Any())
                            {
                                // Already going to be reporting a failure, so the parse error doesn't really affect anything.
                            }
                            else
                            {
                                // No SMART failure of any kind to report -- so we need to flip the SMART support indicator to "Unknown", so that this disk doesn't slip through with a passing grade.
                                disk.SmartSupport = DiskSmartSupport.Unknown;
                            }
                        }
                    }
                }
            }
        }

        private void parseSmartctlAttributeOutputLine(string line, out bool hadParseError, out int id, out string attributeName, out int value, out int worst, out int thresh, out string attrType, out string rawValue)
        {
            // Initialize the output parameters.
            hadParseError = false;
            id = -1;
            attributeName = null;
            value = 0;
            worst = 0;
            thresh = 0;
            attrType = null;
            rawValue = null;

            // Parse the line.
            try
            {
                id = Int32.Parse(line.Substring(0, 3).Trim());
                attributeName = line.Substring(4, 23).Trim();
                value = Int32.Parse(line.Substring(37, 5).Trim());
                worst = Int32.Parse(line.Substring(43, 5).Trim());
                thresh = Int32.Parse(line.Substring(49, 6).Trim());
                attrType = line.Substring(56, 9).Trim();
            }
            catch
            {
                hadParseError = true;
            }

            try
            {
                rawValue = line.Substring(87, line.Length - 87).Trim();
            }
            catch
            {
                // Not really a meaningful value, so eat the error.
                rawValue = "";
            }
        }
    }
}
