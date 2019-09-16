using System;
using System.Collections.Generic;
using IP_Man.Contracts;
using Newtonsoft.Json;
using WindowsNetworkManagement;

namespace IP_Man.Model
{
    // Responsibility: hold and validate ip profile data
    public class IpProfile : ISerializable
    {
        // Der Public Zugriff auf die Attribute ist notwendig, damit WPF Data Bindings verwenden kann.
        public Guid Guid { get; set; }
        public string ProfileName { get; set; }
        public string InterfaceName { get; set; }
        public bool DynamicIp { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string DefaultGateway { get; set; }
        public bool DynamicDns { get; set; }
        public List<string> Nameservers { get; set; }

        // Konstruktor für komplett dynamische konfiguration
        public IpProfile(string newProfileName, string newInterfaceName)
        {
            Guid = Guid.NewGuid();
            Nameservers = new List<string>();
            SetProfileName(newProfileName);
            SetInterfaceName(newInterfaceName);
            SetDynamicIp(true);
            SetIpAddress("");
            SetSubnetMask("");
            SetDefaultGateway("");
            SetDynamicDns(true);
        }

        // Konstruktor für teilweise statische Konfiguration (IP Dynamisch, DNS Statisch)
        public IpProfile(string newProfileName, string newInterfaceName, List<string> newNameservers)
            : this(newProfileName, newInterfaceName)
        {
            SetDynamicDns(false);
            if (newNameservers != null)
            {
                foreach (string nameserver in newNameservers)
                {
                    if (nameserver != "") AddNameserver(nameserver);
                }
            }
        }

        // Konstruktor für komplett statische Konfiguration ohne parametrisierte booleans
        public IpProfile(string newProfileName, string newInterfaceName, string newIpAddress, string newSubnetMask, string newDefaultGateway, List<string> newNameservers)
            : this(newProfileName, newInterfaceName, newNameservers)
        {
            SetDynamicIp(false);
            SetIpAddress(newIpAddress);
            SetSubnetMask(newSubnetMask);
            SetDefaultGateway(newDefaultGateway);
        }

        // Universeller, komplett parametrisierter Konstruktor für komplett statische Konfiguration und einfache, automatisierte Instanzierung
        public IpProfile(string newProfileName, string newInterfaceName, bool newDynamicIp, string newIpAddress, string newSubnetMask, string newDefaultGateway, bool newDynamicDNS, List<string> newNameservers)
            : this(newProfileName, newInterfaceName, newIpAddress, newSubnetMask, newDefaultGateway, newNameservers)
        {
            SetDynamicIp(newDynamicIp);
            SetDynamicDns(newDynamicDNS);
        }

        // Konstruktor mit parametrisierter Guid
        [JsonConstructor]
        public IpProfile(Guid newGuid, string newProfileName, string newInterfaceName, bool newDynamicIp, string newIpAddress, string newSubnetMask, string newDefaultGateway, bool newDynamicDNS, List<string> newNameservers)
            : this(newProfileName, newInterfaceName, newDynamicIp, newIpAddress, newSubnetMask, newDefaultGateway, newDynamicDNS, newNameservers)
        {
            Guid = newGuid;
        }

        // returns true if the profile would be applyable
        public bool IsValid()
        {
            // ip and dns is set to dynamic
            if (DynamicIp && DynamicDns) return true;
            // only ip is set to dynamic, nameservers are static
            if (DynamicIp && !DynamicDns && Nameservers.Count > 0)
            {
                foreach (string nameserver in Nameservers)
                {
                    // allow nameservers not set
                    if (!(IPv4Validator.ValidateAddress(nameserver) || nameserver == ""))
                    {
                        return false;
                    }
                }
                return true;
            }
            // full static configuration
            if (!DynamicIp && !DynamicDns && IPv4Validator.ValidateAddress(IpAddress) && IPv4Validator.ValidateSubnetMask(SubnetMask) && IPv4Validator.ValidateAddress(DefaultGateway) && Nameservers.Count > 0) {
                List<string> ipAddresses = new List<string>() { IpAddress, DefaultGateway };
                if (IPv4Validator.ValidateSameSubnet(ipAddresses, SubnetMask))
                {
                    foreach (string nameserver in Nameservers)
                    {
                        // allow nameservers not set
                        if (!(IPv4Validator.ValidateAddress(nameserver) || nameserver == ""))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public Guid GetGuid()
        {
            return Guid;
        }

        public void SetProfileName(string newProfileName)
        {
            ProfileName = newProfileName;
        }
        public string GetProfileName()
        {
            return ProfileName;
        }

        public void SetInterfaceName(string newInterfaceName)
        {
            InterfaceName = newInterfaceName;
        }
        public string GetInterfaceName()
        {
            return InterfaceName;
        }

        public void SetDynamicIp(bool newDynamicIp)
        {
            DynamicIp = newDynamicIp;
        }
        public bool GetDynamicIp()
        {
            return DynamicIp;
        }

        public void SetIpAddress(string newIpAddress)
        {
            IpAddress = newIpAddress;
        }
        public string GetIpAddress()
        {
            return IpAddress;
        }

        public void SetSubnetMask(string newSubnetMask)
        {
            SubnetMask = newSubnetMask;
        }
        public string GetSubnetMask()
        {
            return SubnetMask;
        }

        public void SetDefaultGateway(string newDefaultGateway)
        {
            DefaultGateway = newDefaultGateway;
        }
        public string GetDefaultGateway()
        {
            return DefaultGateway;
        }

        public void SetDynamicDns(bool newDynamicDns)
        {
            DynamicDns = newDynamicDns;
        }
        public bool GetDynamicDns()
        {
            return DynamicDns;
        }

        public void AddNameserver(string newNameserver)
        {
            Nameservers.Add(newNameserver);
        }
        public void RemoveNameserver(string oldNameserver)
        {
            Nameservers.Remove(oldNameserver);
        }
        public List<string> GetNameservers()
        {
            return Nameservers;
        }
        public void SetNameservers(List<string> newNameservers)
        {
            Nameservers = newNameservers;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
