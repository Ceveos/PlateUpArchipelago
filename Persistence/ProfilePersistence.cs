using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KitchenArchipelago.Persistence
{
    public class ProfilePersistence
    {
        private static ProfilePersistence _instance;
        public static ProfilePersistence Instance
        {
            get
            {
                return _instance ??= new ProfilePersistence();
            }
        }

        private Kitchen.PlayerProfile _profile;
        private Dictionary<string, object> data = new Dictionary<string, object>();

        private ProfilePersistence() { }

        public T Get<T>(string key, T defaultValue = default)
        {
            if (data.ContainsKey(key))
            {
                return (T)data[key];
            }
            return defaultValue;
        }
        public T Get<T>(Setting key)
        {
            var attribute = key.GetSettingAttribute();
            return Get(attribute.Name, (T)attribute.DefaultValue);
        }

        // Generic setter
        public void Set<T>(string key, T value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }
        public void Set<T>(Setting key, T value)
        {
            var attribute = key.GetSettingAttribute();
            Set(attribute.Name, value);
        }

        public void Save()
        {
            KitchenArchipelago.Logger.LogInfo($"Saving settings for user {_profile.Name}.");
            KitchenArchipelago.Logger.LogInfo($"Writing to file path: {GetFilePath()}");
            string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetFilePath(), jsonStr);
            KitchenArchipelago.Logger.LogInfo($"Settings saved successfully for user {_profile.Name}.");
        }

        private void setProfile(Kitchen.PlayerProfile profile)
        {
            Instance.data.Clear();
            Instance._profile = profile;
        }

        public static ProfilePersistence Load(Kitchen.PlayerProfile profile, bool forceReload = false)
        {
            KitchenArchipelago.Logger.LogInfo($"Loading settings for user {profile.Name}.");

            // If user is already loaded, return profile.
            if (!forceReload && Instance._profile == profile)
            {
                KitchenArchipelago.Logger.LogInfo($"Profile already loaded. Returning early.");
                return Instance;
            }

            Instance.setProfile(profile);

            var filePath = Instance.GetFilePath();
            if (!File.Exists(filePath))
            {
                KitchenArchipelago.Logger.LogInfo($"Setings file does not exist for user {profile.Name}. Creating default config.");
                Instance.CreateDefaultConfig();
                Instance.Save();
                return Instance;
            }

            try
            {
                var jsonStr = File.ReadAllText(filePath);
                Instance.data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);

                KitchenArchipelago.Logger.LogInfo($"Loaded settings for user {profile.Name} successfully.");
            }
            catch (Exception e)
            {
                KitchenArchipelago.Logger.LogError($"Error loading settings for user {profile.Name}: {e.Message}");
                KitchenArchipelago.Logger.LogError($"Resetting back to default values.");
                Instance.CreateDefaultConfig();

            }

            return Instance;
        }

        private void CreateDefaultConfig()
        {
            KitchenArchipelago.Logger.LogInfo($"Creating default configuration file.");
            foreach (Setting setting in Enum.GetValues(typeof(Setting))) {
                var attributes = setting.GetSettingAttribute();
                if (attributes != null)
                {
                    KitchenArchipelago.Logger.LogInfo("- Setting " + attributes.Name + " to " + attributes.DefaultValue.ToString());
                    Set(attributes.Name, attributes.DefaultValue);
                }
            }   
        }

        public string GetFilePath()
        {
            return $"{Application.persistentDataPath}/archipelago_{ToFileNameFriendlyString(_profile.Name)}.json";
        }

        public static string ToFileNameFriendlyString(string input)
        {
            // Remove invalid file name characters
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidCharsPattern = string.Format(@"[{0}]+", invalidChars);
            string fileNameFriendly = Regex.Replace(input, invalidCharsPattern, "_");

            fileNameFriendly = fileNameFriendly.Trim();

            return fileNameFriendly;
        }
    }
}
