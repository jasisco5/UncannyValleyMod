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
using ContentPatcher;
using SpaceShared.APIs;
using Microsoft.Xna.Framework.Audio;

namespace UncannyValleyMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        // Variables
        IContentPatcherAPI cpApi;
        SpaceCore.Api scApi;
        IModHelper helper;
        ModSaveData saveModel;
        Dictionary<string, Token> tokens = new Dictionary<string, Token>();

        // Other File References
        ModWeapon modWeapon;
        ModQuests modQuests;
        ModMaps modMaps;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            // Set Up Events
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.Events.Player.Warped += this.OnWarped;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            helper.Events.GameLoop.Saving += this.OnSaving;

            // helper.Events.GameLoop.UpdateTicking += this.SoundSystem;

            // Get C# modded content
            modWeapon = new ModWeapon(helper);
            modMaps = new ModMaps(helper, this.Monitor, modWeapon);
            modQuests = new ModQuests(helper, this.Monitor);
        }





        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Connections between the Content Patcher and SMAPI
        /// </summary>
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Reference to Space Core
            scApi = new SpaceCore.Api();
            scApi.RegisterSerializerType(typeof(ReapingEnchantment));
            // Reference to Content Patcher
            cpApi = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            // Working with Content Patcher
            AddTokens();
            {

                ///
                /// Loading owned Content Packs
                ///
                foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
                {
                    this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");

                    // Loading JSON Data
                    {
                        //YourDataModel data = contentPack.ReadJsonFile<YourDataFile>("content.json");
                    }
                    // Read Content Data
                    {
                        //Texture2D image = contentPack.LoadAsset<Texture2D>("image.png");

                        // Passing an asset name to game code
                        //tilesheet.ImageSource = contentPack.GetActualAssetKey("image.png");
                    }

                }
            }


        }

        

        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.helper.Data.WriteSaveData("savedata", saveModel);
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Content Patcher Conditionals
            {
                ///
                /// Checking Conditions
                ///
                // Create a model of the conditions you want to check
                var rawConditions = new Dictionary<string, string>
                {
                    ["PlayerGender"] = "male",             // player is male
                    ["Relationship: Abigail"] = "Married", // player is married to Abigail
                    ["HavingChild"] = "{{spouse}}",        // Abigail is having a child
                    ["Season"] = "Winter"                  // current season is winter
                };

                // Call the API to parse the conditions into an IManagedConditions wrapper
                // This is an expensive operation, so stash this wrapper if you want to reuse it
                var conditions = cpApi.ParseConditions(
                   manifest: this.ModManifest,
                   rawConditions: rawConditions,
                   formatVersion: new SemanticVersion("1.30.0")
                );

                // Conditions don't update automatically
                conditions.UpdateContext();
            }

            // Custom Save Data
            saveModel = this.Helper.Data.ReadSaveData<ModSaveData>("savedata");
            if(saveModel == null)
            {
                // create default entry
                this.Helper.Data.WriteSaveData<ModSaveData>("savedata", new ModSaveData());
                saveModel = this.Helper.Data.ReadSaveData<ModSaveData>("savedata");
            }
            modMaps.SetSaveModel(saveModel);
            modWeapon.saveModel = saveModel;

            foreach( KeyValuePair<string, Token> entry in tokens ) 
            {
                entry.Value.saveModel = saveModel;
                entry.Value.UpdateContext();
            }

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
        }


        // Helper Methods
        // Content Patcher Tokens
        private void AddTokens()
        {

            ///
            /// Adding a token to Content Patcher api
            /// 
            // To use it in a Content Pack, list this mod as a dependency 
            cpApi.RegisterToken(this.ModManifest, "PlayerName", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { Game1.player.Name };
            
                // or save is currently loading
                if (SaveGame.loaded?.player != null)
                    return new[] { SaveGame.loaded.player.Name };
            
                // no save loaded (e.g. on the title screen)
                return null;
            });
            if (!tokens.ContainsKey("Weapon")) { tokens.Add("Weapon", new WeaponToken());  }
            modWeapon.token = (WeaponToken)tokens["Weapon"];
            cpApi.RegisterToken(this.ModManifest, "WeaponObtained", tokens["Weapon"]);


            modMaps.tokens = tokens;

        }

        // helper method for sounds
        // checks the in game location, then checks the player coordinates before playing the sounds at a specified location
        /*private void SoundSystem(object sender, EventArgs e)
        {
            SoundPlayer();
        }*/

        /*private void SoundPlayer()
        {
            // check game location
            // ---- MANSION EXT ----
            if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Exterior"))
            {
                // check coordinates
                if (Game1.player.getTileX() > 20 && Game1.player.getTileX() < 30 && Game1.player.getTileY() > 34 && Game1.player.getTileY() < 44)
                {
                    // play sound
                    Game1.currentLocation.playSoundAt("thunder", new Vector2(25, 39));
                }
            }
            else if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Interior"))
            {

            }
        }*/
    }
}
