using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenArchipelago.Recipes
{
    internal class RecipeSystem : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            //throw new NotImplementedException();
        }

        private void addDishOption(int settingId)
        {
            RestaurantSetting setting = getGdo(settingId);
            string settingName = setting != null ? setting.Name : settingId.ToString();
            //Main.availableSettingOptions.Add(settingId, settingName);
        }
        private RestaurantSetting getGdo(int id)
        {
            return GameData.Main.Get<GameDataObject>()
                .Where(gdo => gdo.ID == id)
                .Select(gdo => (RestaurantSetting)gdo)
                .First();
        }
    }
}
