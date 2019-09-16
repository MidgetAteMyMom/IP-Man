using System.Collections.Generic;
using IP_Man.Contracts;
using System.IO;
using Newtonsoft.Json;

namespace IP_Man.Model
{
    public class ConfigurationManager
    {
        private string filePath;

        public ConfigurationManager(string newFilePath = @"\IpMan.json")
        {
            filePath = string.Concat(Directory.GetCurrentDirectory(), newFilePath);
            if (!File.Exists(filePath))
            {
                CreateConfigFile();
            }
        }

        // saves all profiles as jsons in a single file
        public void SaveProfiles(List<IpProfile> profiles)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (ISerializable profile in profiles)
                {
                    sw.WriteLine(profile.Serialize());
                }
            }
        }

        // reads all profiles from configuration file
        public List<IpProfile> GetProfiles()
        {
            List<IpProfile> profiles = new List<IpProfile>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (true)
                {
                    string json = sr.ReadLine();
                    if (json != null && json != "")
                    {
                        profiles.Add(JsonConvert.DeserializeObject<IpProfile>(json));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return profiles;
        }

        // create config file if not existent
        private void CreateConfigFile()
        {
            using (File.Create(filePath)) { };
        }
    }
}
