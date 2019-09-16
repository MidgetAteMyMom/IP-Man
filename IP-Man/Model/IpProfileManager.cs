using System.Collections.Generic;
using IP_Man.Contracts;

namespace IP_Man.Model
{
    // Responsebility: Manage and hold IpProfiles
    public class IpProfileManager
    {
        private List<IpProfile> profiles;

        public IpProfileManager(List<IpProfile> newProfiles)
        {
            profiles = newProfiles;
        }

        // return all profiles
        public List<IpProfile> GetProfiles()
        {
            return profiles;
        }

        // add a profile
        public void AddProfile(IpProfile profile)
        {
            profiles.Add(profile);
        }

        // remove a profile
        public void RemoveProfile(IpProfile profile)
        {
            for (int i = 0; i < profiles.Count; i++)
            {
                if (profiles[i].GetGuid() == profile.GetGuid())
                {
                    profiles.RemoveAt(i);
                    break;
                }
            }
        }

        // update a profile
        public void UpdateProfile(IpProfile newProfile)
        {
            bool addProfile = true;
            foreach (IpProfile profile in profiles)
            {
                if (profile.GetGuid() == newProfile.GetGuid())
                {
                    addProfile = false;
                    profile.SetProfileName(newProfile.GetProfileName());
                    profile.SetInterfaceName(newProfile.GetInterfaceName());
                    profile.SetDynamicIp(newProfile.GetDynamicIp());
                    profile.SetIpAddress(newProfile.GetIpAddress());
                    profile.SetSubnetMask(newProfile.GetSubnetMask());
                    profile.SetDefaultGateway(newProfile.GetDefaultGateway());
                    profile.SetDynamicDns(newProfile.GetDynamicDns());
                    profile.SetNameservers(newProfile.GetNameservers());
                    break;
                }
            }
            if (addProfile)
            {
                AddProfile(newProfile);
            }
        }
    }
}
