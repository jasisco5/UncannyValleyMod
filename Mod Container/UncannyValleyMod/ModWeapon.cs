using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    /// <summary>
    /// Class for generating the mod's custom weapon. 
    /// Seperated to have cleaner code
    /// </summary>
    internal class ModWeapon
    {
        public ModSaveData saveModel { get; set; }
        public WeaponToken token { get; set; }

        public ModWeapon(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        /// <summary>
        /// Load the Spectral Sabre into the game
        /// </summary>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // Load Custom Weapon
            if (e.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                e.LoadFromModFile<Texture2D>("assets/weapons/weapons.png", AssetLoadPriority.Medium);
            }
            // Edit in Custom Weapon
            if (e.Name.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;
                    data[65] = "Spectral Sabre/A blade to reap the life and energy from monsters./80/100/1/8/0/0/3/5/5/0/.04/2";

                }, AssetEditPriority.Default);
            }
        }

        public void AddWeaponToInv()
        {
            MeleeWeapon weapon = new MeleeWeapon(65);
            BaseWeaponEnchantment reaping = new ReapingEnchantment();
            weapon.AddEnchantment(reaping);
            weapon.ParentSheetIndex = 65;
            Game1.player.addItemToInventory(weapon);
            saveModel.weaponObtained = true;
            if (token != null) { token.UpdateContext(); }
        }

    }
}
