﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Archipelago
{
    public enum Setting
    {
        [Setting("bEnabled", false, "Whether or not this extension is enabled (for this profile).")]
        Enabled,
        
        [Setting("sHost", "archipelago.gg:", "Archipelago host name")]
        Host,

        [Setting("sName", "", "Archipelago user name")]
        Name,

        [Setting("lUnlockedAppliances", new string[] { }, "Unlocked appliances")]
        UnlockedAppliances,

        [Setting("lChecksCompleted", new string[] { }, "Checks completed")]
        ChecksCompleted,
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SettingAttribute : Attribute
    {
        public string Name { get; }
        public object DefaultValue { get; }
        public string Description { get; }

        public SettingAttribute(string name, object defaultValue, string description = "")
        {
            Name = name;
            DefaultValue = defaultValue;
            Description = description;
        }
    }

    public static class SettingsExtension
    {
        public static SettingAttribute GetSettingAttribute(this Setting setting)
        {
            var memberInfo = setting.GetType().GetMember(setting.ToString())[0];

            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttribute<SettingAttribute>();
            }

            return null;
        }
    }

}
