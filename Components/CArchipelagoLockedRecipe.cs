using KitchenMods;
using Unity.Entities;

namespace KitchenArchipelago.Components
{
    public class CArchipelagoLockedRecipe: IBufferElementData
    {
        public int ApplianceID;
        public bool IsUnlocked;
    }
}
