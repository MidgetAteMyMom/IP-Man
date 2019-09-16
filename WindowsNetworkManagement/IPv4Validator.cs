using System;
using System.Collections.Generic;

namespace WindowsNetworkManagement
{
    // Responsibility: Validate that strings representing ipaddresses are valid ipaddresses and check if two addresses are in the same subnet
    public static class IPv4Validator
    {
        // Validate that a given string represents an ipaddress
        public static bool ValidateAddress(string ipAddress)
        {
            bool validAddress = true;
            string[] octets = ipAddress.Split('.');
            // valid ipaddresses have 4 octets
            if (octets.Length != 4)
            {
                validAddress = false;
            }
            foreach (string octet in octets)
            {
                try
                {
                    int octetInt = Int32.Parse(octet);
                    // only numbers between 0 and 255 are allowed
                    if (octetInt < 0 || octetInt > 255)
                    {
                        validAddress = false;
                    }
                } catch (FormatException) // if there are non-number signs inside the octet string, this exception will be raised
                {
                    validAddress = false;
                }
            }
            return validAddress;
        }

        
        // Validate that a given address is a valid subnet mask
        public static bool ValidateSubnetMask(string subnetMask)
        {
            string[] octets = GetStringArrayRepresentation(subnetMask);
            bool lastOctet255 = true;
            List<string> allowedNumbers = new List<string>() { "255", "254", "252", "248", "240", "224", "192", "128", "0" };
            if (!ValidateAddress(subnetMask))
            {
                return false;
            }
            foreach (string octet in octets)
            {
                if (lastOctet255)
                {
                    if (octet == "255")
                    {
                        continue;
                    } else if (allowedNumbers.Contains(octet))
                    {
                        lastOctet255 = false;
                        continue;
                    } else
                    {
                        return false;
                    }
                } else
                {
                    if (octet == "0")
                    {
                        continue;
                    } else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        // Check if given ipaddresses are in the same subnet
        public static bool ValidateSameSubnet(List<string>ipAddresses, string subnetMask)
        {
            string networkAddress = "";
            if (!ValidateSubnetMask(subnetMask))
            {
                throw new FormatException("The provided SubnetMask is not a valid IPv4 subnet mask");
            }
            foreach (string ipAddress in ipAddresses)
            {
                if (!ValidateAddress(ipAddress))
                {
                    throw new FormatException("At least one ipaddress is not a valid IPv4 address");
                }
                string newNetworkAddress = GetNetworkAddressString(ipAddress, subnetMask);
                if (networkAddress != "" && networkAddress != newNetworkAddress)
                {
                    return false;
                }
                networkAddress = newNetworkAddress;
            }
            return true;
        }

        // Return network address as string
        public static string GetNetworkAddressString(string ipAddress, string subnetMask)
        {
            if (!ValidateSubnetMask(subnetMask))
            {
                throw new FormatException("The provided subnetMask is not a valid IPv4 subnet mask");
            }
            if (!ValidateAddress(ipAddress))
            {
                throw new FormatException("The provided ipAddress is not a valid IPv4 address");
            }
            uint ipAddressUint = GetUint32Representation(ipAddress);
            uint subnetMaskUint = GetUint32Representation(subnetMask);
            uint networkAddressUint = ipAddressUint & subnetMaskUint;

            return GetStringRepresentation(networkAddressUint);
        }

        // Return ip address as string array (each octet is a string)
        private static string[] GetStringArrayRepresentation(string addressString)
        {
            return addressString.Split('.');
        }

        // return ip address as byte array
        private static byte[] GetByteArrayRepresentation(string[] octets)
        {
            byte[] byteArray = new byte[4];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(Convert.ToUInt32(octets[i]));
            }
            return byteArray;
        }

        // return ip address as a single unsigned integer to perform binary operation on
        private static uint GetUint32Representation(byte[] byteArrayRepresentation)
        {
            // Make a defensive copy.
            var ipBytes = new byte[byteArrayRepresentation.Length];
            byteArrayRepresentation.CopyTo(ipBytes, 0);

            // Reverse if we are on a little endian architecture.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ipBytes);

            // Convert these bytes to an unsigned 32-bit integer (IPv4 address).
            return BitConverter.ToUInt32(ipBytes, 0);
        }

        private static uint GetUint32Representation(string stringRepresentation)
        {
            return GetUint32Representation(GetByteArrayRepresentation(GetStringArrayRepresentation(stringRepresentation)));
        }

        // return ip address as a single string
        private static string GetStringRepresentation(byte[] byteArrayRepresentation)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArrayRepresentation);
            }
            string[] octets = new string[4];
            string ipAddress;

            for (int i = 0; i < octets.Length; i++)
            {
                octets[i] = Convert.ToString(byteArrayRepresentation[i]);
            }

            ipAddress = String.Join(".", octets);

            return ipAddress;
        }

        private static string GetStringRepresentation(uint uint32Representation)
        {
            return GetStringRepresentation(BitConverter.GetBytes(uint32Representation));
        }
    }
}
