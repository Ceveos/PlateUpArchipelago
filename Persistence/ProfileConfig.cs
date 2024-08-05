using System;
using System.Reflection;

namespace KitchenArchipelago.Persistence
{
    public enum ProfileConfig
    {
        [Config("bEnabled", false, "Whether or not this extension is enabled (for this profile).")]
        Enabled,

        [Config("sHost", "archipelago.gg:", "Archipelago host name")]
        Host,

        [Config("sUser", "", "Archipelago room user name")]
        User,

        [Config("sPassword", "", "Archipelago room password")]
        Password,

        [Config("fCookingSpeedModifier", 0.5f, "Affects cooking/chopping.")]
        CookingSpeedModifier,

        [Config("fBurningSpeedModifier", 2f, "Affects how fast items can burn.")]
        BurningSpeedModifier,

        [Config("fMovementSpeedModifier", 0.75f, "Affects how fast you move.")]
        MovementSpeedModifier,

        [Config("fBlueprintCostModifier", 1.5f, "Affects how expensive blueprints are.")]
        BlueprintCostModifier,

        [Config("fCustomerPatienceModifier", 0.75f, "Affects how expensive blueprints are.")]
        CustomerPatienceModifier,

        [Config("saUnlockedRecipes", new string[] { }, "All unlocked recipes.")]
        UnlockedRecipes,

        [Config("saUnlockedSettingCards", new string[] { }, "All unlocked setting cards.")]
        UnlockedSettingCards,

        [Config("saAPItemLog", new string[] { }, "All items unlocked from Archipelago.")]
        ArchipelagoItemLog,
    }
}
