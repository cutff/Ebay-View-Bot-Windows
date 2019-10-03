using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Principal;

namespace Ebay_View_Bot
{
    class LicensingLibrary
    {
        public static string KDIHAOGOIPOSJGSUOAJGJKSLAIJGAIJ { get; set; }

        public static string hdHWID { get; set; }

        public static string cpuID { get; set; }

        public static string osGUID { get; set; }

        public static void MainGeneration()
        {
            GenerateCPUHWID();
            GenerateHDHWID();
            osGUID = GetMachineGuid();

            KDIHAOGOIPOSJGSUOAJGJKSLAIJGAIJ = cpuID + hdHWID + WindowsIdentity.GetCurrent().Name + osGUID;
        }

        public static void GenerateCPUHWID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();

            foreach (ManagementObject mo in mbsList)
            {
                cpuID = mo["ProcessorID"].ToString();
            }
        }
        
        public static void GenerateHDHWID()
        {
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            dsk.Get();
            hdHWID = dsk["VolumeSerialNumber"].ToString();
        }
        
        public static string GetMachineGuid()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(
                            string.Format("Key Not Found: {0}", location));

                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(
                            string.Format("Index Not Found: {0}", name));

                    return machineGuid.ToString();
                }
            }
        }      
    }
}
