using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Management;

namespace WindowsNetworkManagement
{
    // This class will do anything only if ran under administrator privileges. If the application has unsufficent privileges, these mehods will not do anything without any notice
    public static class NetworkSettingsControler
    {
        // Sets a static ipaddress for the given networkinterface
        static public void SetStaticIp(NetworkInterface nic, string newIpAddress, string newSubnetMask)
        {
            ManagementObject MO = GetNicManagementObject(nic);
            ManagementBaseObject setIp;
            ManagementBaseObject newIp = MO.GetMethodParameters("EnableStatic");

            newIp["IPAddress"] = new string[] { newIpAddress };
            newIp["SubnetMask"] = new string[] { newSubnetMask };

            setIp = MO.InvokeMethod("EnableStatic", newIp, null);
        }

        // Sets the ipaddress for the given interface to dynamic
        static public void SetIpToDynamic(NetworkInterface nic)
        {
            ManagementObject MO = GetNicManagementObject(nic);
            ManagementBaseObject setIp;
            ManagementBaseObject newIp = MO.GetMethodParameters("EnableDHCP");

            setIp = MO.InvokeMethod("EnableDHCP", newIp, null);
        }

        // Sets a default gateway for the given network interface statically
        static public void SetStaticGateway(NetworkInterface nic, string newGatewayAddress)
        {
            ManagementObject MO = GetNicManagementObject(nic);
            ManagementBaseObject setGateway;
            ManagementBaseObject newGateway = MO.GetMethodParameters("SetGateways");

            newGateway["DefaultIPGateway"] = new string[] { newGatewayAddress };
            newGateway["GatewayCostMetric"] = new int[] { 1 };

            setGateway = MO.InvokeMethod("SetGateways", newGateway, null);
        }

        // Sets static nameservers for the given network interface
        static public void SetStaticNameservers(NetworkInterface nic, List<string> newNameservers)
        {
            // Set nameservers to dynamic first, because windows will not delete a old secondary dns if the current profile only contains a primary one
            SetNameserversToDynamic(nic);
            ManagementObject MO = GetNicManagementObject(nic);
            ManagementBaseObject newDNS = MO.GetMethodParameters("SetDNSServerSearchOrder");
            string[] nameservers = new string[newNameservers.Count];
            for (int i = 0; i < nameservers.Length; i++)
            {
                nameservers[i] = newNameservers[i];
            }
            newDNS["DNSServerSearchOrder"] = nameservers;
            ManagementBaseObject setDNS = MO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
        }

        // Sets the nameservers for the given network interface to dynamic
        static public void SetNameserversToDynamic(NetworkInterface nic)
        {
            ManagementObject MO = GetNicManagementObject(nic);
            ManagementBaseObject newDNS = MO.GetMethodParameters("SetDNSServerSearchOrder");
            newDNS["DNSServerSearchOrder"] = null;
            ManagementBaseObject setDNS = MO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
        }

        // Returns a configurable ManagementObject("Win32_NetworkAdapterConfiguration") for the given System.Net.Network.Information NetworkInterface Object
        static private ManagementObject GetNicManagementObject(NetworkInterface nic)
        {
            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();

            foreach (ManagementObject MO in MOC)
            {
                if (MO["Description"].ToString().Contains(nic.Description))
                {
                    if ((bool)MO["IPEnabled"])
                    {
                        return MO;
                    }
                }
            }
            throw new KeyNotFoundException("There is No ip-enabled, connected interface that matches the given NetworkInterface object");
        }
    }
}
