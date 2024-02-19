using System;
using System.Threading;
using Netcode;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;

namespace UncannyValleyMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.Events.Player.Warped += this.OnWarped;

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load Custom_Mansion
            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/maps/CustomMansion.tmx", ContentSource.ModFolder);
            // add the location
            GameLocation customMap = new GameLocation(mapAssetKey, "Custom_Mansion") { IsOutdoors = true, IsFarm = false };
            Game1.locations.Add(customMap);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }


        private void OnWarped(object sender, WarpedEventArgs e)
        {
            this.Monitor.Log($"{e.Player.Name} warped from {e.OldLocation} to {e.NewLocation}", LogLevel.Debug);
            // Player is in the FarmHouse
            if (Game1.getLocationFromName("FarmHouse") == e.NewLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is in FarmHouse", LogLevel.Debug);
                // Spawn a Journal Scrap
                if (Game1.getLocationFromName("FarmHouse")
                    .dropObject(new StardewValley.Object(new Vector2(6 * 64, 8 * 64), 842, "Journal Scrap", true, true, false, true))
                    ) { }
                MeleeWeapon weapon = new MeleeWeapon(65);
                weapon.ParentSheetIndex = 65;
                Game1.player.addItemToInventory(weapon);
                return;
            }
            // Player is Outside the Mansion
            if (Game1.getLocationFromName("Custom_Mansion") == e.NewLocation)
            {
                this.Monitor.Log($"{e.Player.Name} is outside the mansion", LogLevel.Debug);
                // Spawn a Journal Scrap
                if (Game1.getLocationFromName("Custom_Mansion")
                    .dropObject(new StardewValley.Object(new Vector2(12 * 64, 48 * 64), 842, "Journal Scrap", true, true, false, true))
                    ) { }

                return;
            }

        }


        //Loading the custom town
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/Town");
        }

        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>("assets/maps/CustomTown.tmx");
        }

        // adding weapon data
        public bool CanLoadWeapon<T>(IAssetInfo asset)
        {
            if (asset.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                return true;
            }
            return false;
        }

        public T LoadWeapon<T>(IAssetInfo asset)
        {
            if (asset.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                return ((Mod)this).Helper.Content.Load<T>("assets/Images/weapons.png", (ContentSource)1);
            }
            throw new InvalidOperationException("Unexpected asset '" + asset.Name + "'.");
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.Name.IsEquivalentTo("Data/Weapons"))
            {
                return true;
            }
            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
            if (((IAssetInfo)asset).Name.IsEquivalentTo("Data/Weapons"))
            {
                IDictionary<int, string> data = ((IAssetData<IDictionary<int, string>>)(object)asset.AsDictionary<int, string>()).Data;
                data[65] = "Spectral Sabre/A blade to reap the life and energy from monsters./80/100/1/8/0/0/3/5/5/0/.04/2";
            }
        }
    }
}
