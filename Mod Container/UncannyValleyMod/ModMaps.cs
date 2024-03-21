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
using xTile;

namespace UncannyValleyMod
{
    internal class ModMaps
    {
        private const bool USEPATCHER = true;

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
            if (!USEPATCHER)
            {
                //helper.Events.Content.AssetRequested += this.OnAssetRequested;
                helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // Load Custom Town
            //if (e.Name.IsEquivalentTo("Maps/Town"))
            //{
            //    e.Edit(asset =>
            //    {
            //        IAssetDataForMap editor = asset.AsMap();
            //        Map sourceMap = this.helper.ModContent.Load<Map>("assets/maps/CustomTown.tmx");
            //        Rectangle replaceRect = new Rectangle(109, 74, 11, 5);
            //        editor.PatchMap(sourceMap, replaceRect, replaceRect, PatchMapMode.ReplaceByLayer);
            //    });
            //}
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load Custom_Mansion_Exterior
            // get the internal asset key for the map file
            string mapAssetKey = this.helper.ModContent.GetInternalAssetName("assets/maps/CustomMansionExterior.tmx").ToString();
            // add the location
            GameLocation mansionExterior = new GameLocation(mapAssetKey, "Custom_Mansion_Exterior") { IsOutdoors = true, IsFarm = false };
            if(saveModel.weaponObtained) { mansionExterior.setTileProperty(25, 26, "Buildings", "Action", "Warp 44 47 Custom_Mansion_Interior"); }
            Game1.locations.Add(mansionExterior);

            // Custom_Mansion
            //mapAssetKey = helper.Content.GetActualAssetKey("assets/maps/CustomMansion.tmx", ContentSource.ModFolder);
            mapAssetKey = this.helper.ModContent.GetInternalAssetName("assets/maps/CustomMansion.tmx").ToString();
            GameLocation mansionInterior = new GameLocation(mapAssetKey, "Custom_Mansion_Interior") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(mansionInterior);
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Player is in the FarmHouse
            if (Game1.getLocationFromName("FarmHouse") == e.NewLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is in FarmHouse", LogLevel.Debug);
                // Spawn a Journal Scrap
                //if (saveModel.canSpawnNote)
                //{
                //    Game1.getLocationFromName("FarmHouse")
                //     .dropObject(new StardewValley.Object(new Vector2(6 * 64, 8 * 64),
                //     842, "Journal Scrap", true, true, false, true));
                //    saveModel.canSpawnNote = false;
                //}
                if (!saveModel.weaponObtained) { modWeapon.AddWeaponToInv(); }
                return;
            }
            // Player is in the Town
            if (!USEPATCHER && Game1.getLocationFromName("Town") == e.NewLocation)
            {
                Warp mansionWarp = new Warp(112, 76, "Custom_Mansion_Exterior", 1, 49, false);
                e.NewLocation.warps.Add(mansionWarp);
                return;
            }
            // Player is Outside the Mansion
            if (Game1.getLocationFromName("Custom_Mansion_Exterior") == e.NewLocation)
            {
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
                // Stop Raining
                //Game1.isRaining = false;

                return;
            }
        }
    }
}
