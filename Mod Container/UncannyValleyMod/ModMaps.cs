using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.Xna.Framework;

namespace UncannyValleyMod
{
    internal class ModMaps
    {
        public ModSaveData oldSaveModel { get; private set; }
        public ModSaveData saveModel { get; private set; }
        public Dictionary<string, Token> tokens { get; set; }
        private IModHelper helper;
        private IMonitor Monitor;
        private ModWeapon modWeapon;

        public void SetSaveModel(ModSaveData _saveModel)
        {
            this.oldSaveModel = _saveModel.DeepClone();
            this.saveModel = _saveModel;
        }

        public ModMaps(IModHelper helper, IMonitor monitor, ModWeapon modWeapon)
        {
            this.helper = helper;
            this.Monitor = monitor;
            this.modWeapon = modWeapon;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            /*
            // Load Custom_Mansion
            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/maps/CustomMansion.tmx", ContentSource.ModFolder);
            // add the location
            GameLocation customMap = new GameLocation(mapAssetKey, "Custom_Mansion") { IsOutdoors = true, IsFarm = false };
            Game1.locations.Add(customMap);
            */
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Player is in the FarmHouse
            if (Game1.getLocationFromName("FarmHouse") == e.NewLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is in FarmHouse", LogLevel.Debug);
                // Spawn a Journal Scrap
                if (saveModel.canSpawnNote)
                {
                    Game1.getLocationFromName("FarmHouse")
                     .dropObject(new StardewValley.Object(new Vector2(6 * 64, 8 * 64),
                     842, "Journal Scrap", true, true, false, true));
                    saveModel.canSpawnNote = false;
                }
                if (!saveModel.weaponObtained) { modWeapon.AddWeaponToInv(); }
                return;
            }
            // Player is Outside the Mansion
            if (Game1.getLocationFromName("Custom_Mansion_Exterior") == e.NewLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is outside the mansion", LogLevel.Debug);
                // Start Raining
                //Game1.isRaining = true;
                //Game1.isDebrisWeather = true;

                // Door warp
                if (!oldSaveModel.weaponObtained && saveModel.weaponObtained)
                {
                    // Bit of a dirty check, could probably be done OnAssetRequested
                    e.NewLocation.setTileProperty(25, 26, "Buildings", "Action", "Warp 44 47 Custom_Mansion_Interior");
                    //Warp mansionDoor = new Warp(25, 26, "Custom_Mansion_Interior", 44, 47, false);
                    //e.NewLocation.warps.Add(mansionDoor);
                }

                return;
            }
            // Player is leaving the mansino exterior
            if (Game1.getLocationFromName("Custom_Mansion_Exterior") == e.OldLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is leaving the mansion", LogLevel.Debug);
                // Stop Raining
                //Game1.isRaining = false;

                return;
            }
        }
    }
}
