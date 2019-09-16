using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using WindowsNetworkManagement;
using IP_Man.Contracts;

namespace IP_Man.Model
{
    sealed public class NetworkInterfaceManager
    {
        private List<NetworkInterface> interfaces;

        public NetworkInterfaceManager()
        {
            interfaces = GetNetworkAdapters();
        }

        // Gibt alle NetzwerkInterface zurück
        private List<NetworkInterface> GetNetworkAdapters()
        {
            List<NetworkInterface> nics = new List<NetworkInterface>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                nics.Add(nic);
            }
            return nics;
        }

        // Gibt die Namen aller NetzwerkInterface als Liste zurück
        public List<string> GetInterfaceNames()
        {
            List<string> names = new List<string>();
            foreach (NetworkInterface nic in interfaces)
            {
                names.Add(nic.Name);
            }
            return names;
        }

        // Wendet einstellungen aus dem Profil an
        public void ApplyProfile(IpProfile profile)
        {
            if (profile.IsValid())
            {
                NetworkInterface currentInterface = GetInterface(profile.GetInterfaceName());
                if (profile.GetDynamicIp())
                {
                    NetworkSettingsControler.SetIpToDynamic(currentInterface);
                    if (profile.GetDynamicDns())
                    {
                        NetworkSettingsControler.SetNameserversToDynamic(currentInterface);
                    }
                    else
                    {
                        NetworkSettingsControler.SetStaticNameservers(currentInterface, profile.GetNameservers());
                    }
                }
                else
                {
                    NetworkSettingsControler.SetStaticIp(currentInterface, profile.GetIpAddress(), profile.GetSubnetMask());
                    NetworkSettingsControler.SetStaticGateway(currentInterface, profile.GetDefaultGateway());
                    NetworkSettingsControler.SetStaticNameservers(currentInterface, profile.GetNameservers());
                }
            }
            else
            {
                throw new Exception(string.Concat("IPProfil ", profile.GetProfileName(), " ist ungültig."));
            }
        }

        // Gibt NetzwerkInterface mit parameter name zurück
        private NetworkInterface GetInterface(string name)
        {
            foreach (NetworkInterface nic in interfaces)
            {
                if (nic.Name == name) return nic;
            }
            throw new KeyNotFoundException("Kein Interface hat den gesuchten Namen!");
        }
    }
}
