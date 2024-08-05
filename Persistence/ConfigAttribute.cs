using System;
using System.Reflection;

namespace KitchenArchipelago.Persistence
{

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigAttribute : Attribute
    {
        public string Name { get; }
        public object DefaultValue { get; }
        public string Description { get; }

        public ConfigAttribute(string name, object defaultValue, string description = "", bool global = false)
        {
            Name = name;
            DefaultValue = defaultValue;
            Description = description;
        }
    }

    public static class ConfigExtension
    {
        public static ConfigAttribute GetConfigAttribute(this ProfileConfig config)
        {
            var memberInfo = config.GetType().GetMember(config.ToString())[0];

            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttribute<ConfigAttribute>();
            }

            return null;
        }
    }
}
