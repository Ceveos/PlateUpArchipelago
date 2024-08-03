using Archipelago;
using Archipelago.MultiClient.Net;
using HarmonyLib;
using Kitchen;
using KitchenArchipelago.Archipelago;
using KitchenLib;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

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
        public const string MOD_GAMEVERSION = ">=1.1.4";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif

        private static readonly Harmony m_harmony = new(MOD_GUID);
        private ProfilePersistence m_settings;
        private Connection m_session;
        private EntityQuery RecipeEntities;
        private EntityQuery ApplianceEntities;


        public KitchenArchipelago() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            m_harmony.PatchAll(Assembly.GetExecutingAssembly());
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            Players.Main.OnPlayerInfoChanged += OnPlayerInfoChanged;
            ApplianceEntities = GetEntityQuery(new QueryHelper()
                .All(typeof(CAppliance))
                .None(typeof(CUnsellableAppliance))
            );
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
        }

        protected override void OnUpdate()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
