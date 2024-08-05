using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KitchenArchipelago.Persistence
{
    public static class Settings
    {
        private static Kitchen.PlayerProfile _profile;
        private static Dictionary<string, object> profileData = new Dictionary<string, object>();

        public static bool Enabled
        {
            get
            {
                return _profile != null && Get<bool>(ProfileConfig.Enabled);
            }
        }

        private static void SetProfile(Kitchen.PlayerProfile profile)
        {
            profileData.Clear();
            _profile = profile;
        }

        private static void CreateDefaultConfig()
        {
            KitchenArchipelago.Logger.LogInfo($"Creating default configuration file.");
            foreach (ProfileConfig setting in Enum.GetValues(typeof(ProfileConfig)))
            {
                var attributes = setting.GetConfigAttribute();
                if (attributes != null)
                {
                    KitchenArchipelago.Logger.LogInfo("- Setting " + attributes.Name + " to " + attributes.DefaultValue.ToString());
                    Set(attributes.Name, attributes.DefaultValue);
                }
            }
        }

        private static string GetFilePath()
        {
            return $"{Application.persistentDataPath}/archipelago_{ToFileNameFriendlyString(_profile.Name)}.json";
        }

        private static string ToFileNameFriendlyString(string input)
        {
            // Remove invalid file name characters
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidCharsPattern = string.Format(@"[{0}]+", invalidChars);
            string fileNameFriendly = Regex.Replace(input, invalidCharsPattern, "_");

            fileNameFriendly = fileNameFriendly.Trim();

            return fileNameFriendly;
        }

        private static T Get<T>(string key, T defaultValue = default)
        {
            if (profileData.ContainsKey(key))
            {
                return (T)profileData[key];
            }
            return defaultValue;
        }
        private static void Set<T>(string key, T value)
        {
            if (profileData.ContainsKey(key))
            {
                profileData[key] = value;
            }
            else
            {
                profileData.Add(key, value);
            }
        }

        public static T Get<T>(ProfileConfig key)
        {
            var attribute = key.GetConfigAttribute();
            return Get(attribute.Name, (T)attribute.DefaultValue);
        }

        public static void Set<T>(ProfileConfig key, T value)
        {
            var attribute = key.GetConfigAttribute();
            Set(attribute.Name, value);
        }

        public static void Save()
        {
            KitchenArchipelago.Logger.LogInfo($"Saving settings for user {_profile.Name}.");
            KitchenArchipelago.Logger.LogInfo($"Writing to file path: {GetFilePath()}");
            string jsonStr = JsonConvert.SerializeObject(profileData, Formatting.Indented);
            File.WriteAllText(GetFilePath(), jsonStr);
            KitchenArchipelago.Logger.LogInfo($"Settings saved successfully for user {_profile.Name}.");
        }

        public static void Load(Kitchen.PlayerProfile profile, bool forceReload = false)
        {
            KitchenArchipelago.Logger.LogInfo($"Loading settings for user {profile.Name}.");

            // If user is already loaded, return profile.
            if (!forceReload && _profile == profile)
            {
                KitchenArchipelago.Logger.LogInfo($"Profile already loaded. Returning early.");
                return;
            }

            _profile = profile;
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                KitchenArchipelago.Logger.LogInfo($"Setings file does not exist for user {profile.Name}. Creating default config.");
                CreateDefaultConfig();
                Save();
            }

            try
            {
                var jsonStr = File.ReadAllText(filePath);
                profileData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);

                KitchenArchipelago.Logger.LogInfo($"Loaded settings for user {profile.Name} successfully.");
            }
            catch (Exception e)
            {
                KitchenArchipelago.Logger.LogError($"Error loading settings for user {profile.Name}: {e.Message}");
                KitchenArchipelago.Logger.LogError($"Resetting back to default values.");
                CreateDefaultConfig();
            }
        }
    }
}
