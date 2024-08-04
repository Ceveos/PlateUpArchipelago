using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KitchenArchipelago.Persistence
{
    public abstract class SettingEntry
    {
        [JsonProperty]
        public string Name;

        [JsonProperty]
        public JObject Value;
    }

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
        private Dictionary<string, SettingEntry> _persistentSettings = new Dictionary<string, SettingEntry>();

        private ProfilePersistence() { }

        public T Get<T>(string name, T defaultValue = default)
        {
            if (_persistentSettings.TryGetValue(name, out SettingEntry entry))
            {
                return entry.Value.ToObject<T>;
            }
            else
            {
                var newEntry = new SettingEntry<T>(name, defaultValue);
                _persistentSettings[name] = newEntry;
                return newEntry.Value;
            }
        }
        public T Get<T>(Setting setting)
        {
            var attribute = setting.GetSettingAttribute();

            if (attribute == null)
                throw new InvalidOperationException("Setting not found");

            return Get<T>(attribute.Name);
        }

        public void Set<T>(string name, T value)
        {
            if (_persistentSettings.TryGetValue(name, out SettingEntry entry))
            {
                ((SettingEntry<T>)entry).Value = value;
            }
            else
            {
                var newEntry = new SettingEntry<T>(name, value);
                _persistentSettings.Add(name, newEntry);
            }
        }

        public void Set<T>(Setting setting, T value)
        {
            var attribute = setting.GetSettingAttribute();

            if (attribute == null)
                throw new InvalidOperationException("Setting not found");

            Set(attribute.Name, value);
        }

        public void Save()
        {
            KitchenArchipelago.LogInfo($"Saving settings for user {_profile.Name}.");
            KitchenArchipelago.LogInfo($"Writing to file path: {GetFilePath()}");
            string jsonStr = JsonConvert.SerializeObject(_persistentSettings.Values, Formatting.Indented);
            File.WriteAllText(GetFilePath(), jsonStr);
            KitchenArchipelago.LogInfo($"Settings saved successfully for user {_profile.Name}.");
        }

        private void setProfile(Kitchen.PlayerProfile profile)
        {
            Instance._persistentSettings.Clear();
            Instance._profile = profile;
        }

        public static ProfilePersistence Load(Kitchen.PlayerProfile profile, bool forceReload = false)
        {
            KitchenArchipelago.LogInfo($"Loading settings for user {profile.Name}.");

            // If user is already loaded, return profile.
            if (!forceReload && Instance._profile == profile)
            {
                KitchenArchipelago.LogInfo($"Profile already loaded. Returning early.");
                return Instance;
            }

            Instance.setProfile(profile);

            var filePath = Instance.GetFilePath();
            if (!File.Exists(filePath))
            {
                KitchenArchipelago.LogInfo($"Setings file does not exist for user {profile.Name}. Creating default config.");
                Instance.CreateDefaultConfig();
                Instance.Save();
                return Instance;
            }

            try
            {
                var jsonStr = File.ReadAllText(filePath);
                Instance._persistentSettings = JsonConvert.DeserializeObject<Dictionary<string, SettingEntry>>(jsonStr);

                KitchenArchipelago.LogInfo($"Loaded settings for user {profile.Name} successfully.");
            }
            catch (Exception e)
            {
                KitchenArchipelago.LogError($"Error loading settings for user {profile.Name}: {e.Message}");
                KitchenArchipelago.LogError($"Resetting back to default values.");
                Instance.CreateDefaultConfig();

            }

            return Instance;
        }

        private void CreateDefaultConfig()
        {
            Set(Setting.Enabled, false);
            Set(Setting.Host, "archipelago.gg:");
            Set(Setting.User, "");
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
