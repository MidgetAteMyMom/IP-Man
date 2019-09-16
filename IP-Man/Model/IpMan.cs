using System.Collections.Generic;
using IP_Man.Contracts;

namespace IP_Man.Model
{
    public sealed class IpMan
    {
        private IpProfileManager profileManager;
        private ConfigurationManager configManager;
        private NetworkInterfaceManager interfaceManager;

        private IpMan()
        {
            configManager = new ConfigurationManager();
            profileManager = new IpProfileManager(LoadConfiguration());
            interfaceManager = new NetworkInterfaceManager();
        }

        public static readonly IpMan GetInstance = new IpMan();

        private List<IpProfile> LoadConfiguration()
        {
            return configManager.GetProfiles();
        }

        public List<IpProfile> GetProfiles()
        {
            return profileManager.GetProfiles();
        }

        public List<string> GetInterfaceNames()
        {
            return interfaceManager.GetInterfaceNames();
        }

        public void UpdateProfile(IpProfile newProfile)
        {
            profileManager.UpdateProfile(newProfile);
            configManager.SaveProfiles(profileManager.GetProfiles());
        }

        public void DeleteProfile(IpProfile oldProfile)
        {
            profileManager.RemoveProfile(oldProfile);
            configManager.SaveProfiles(profileManager.GetProfiles());
        }

        public void ApplyProfile(IpProfile profile)
        {
            interfaceManager.ApplyProfile(profile);
        }
    }
}
