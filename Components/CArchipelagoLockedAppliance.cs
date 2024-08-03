using KitchenMods;
using Unity.Entities;

namespace KitchenArchipelago.Components
{
    public class CArchipelagoLockedAppliance: IBufferElementData
    {
        public int ApplianceID;
        public bool IsUnlocked;
    }
}
