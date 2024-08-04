﻿using Kitchen;
using KitchenArchipelago.Archipelago;
using KitchenArchipelago.Persistence;
using KitchenLib;
using KitchenLib.Logging;
using KitchenMods;
using System.Linq;
using System.Reflection;

// Namespace should have "Kitchen" in the beginning
namespace KitchenArchipelago
{
    public class KitchenArchipelago : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "dev.casasola.plateup-archipelago";
        public const string MOD_NAME = "Archipelago";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "Alex";
        public const string MOD_GAMEVERSION = ">=1.1.9";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        internal static KitchenLogger Logger;

        private ProfilePersistence m_settings;
        private Connection m_session;


        public KitchenArchipelago() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            Players.Main.OnPlayerInfoChanged += OnPlayerInfoChanged;
            
        }
        protected override void OnPostActivate(Mod mod)
        {
            Logger = InitLogger();
        }

        private void OnPlayerInfoChanged()
        {
            PlayerInfo? player = Players.Main.All().FirstOrDefault(player => player.IsLocalUser && player.HasProfile);

            if (player.HasValue)
            {
                m_settings = ProfilePersistence.Load(player.Value.Profile);
            }
            else
            {
                m_settings = null;
            }
            OnSettingsChanged();
        }

        /// <summary>
        /// When archipelago settings change
        /// </summary>
        private void OnSettingsChanged()
        {
            bool enabled = m_settings.Get<bool>(Setting.Enabled);
            if (enabled)
            {
                Connection.Instance.Connect(m_settings);
            }
            else
            {
                Connection.Instance.Disconnect();
            }
        }
        protected override void OnUpdate()
        {
        }
    }
}
