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
using QuestFramework.Api;
using ContentPatcher;

namespace UncannyValleyMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        IContentPatcherAPI cpApi;
        IManagedQuestApi qfManagedApi;
        IQuestApi qfApi;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Get C# modded mail
            new ModMail(helper);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            helper.Events.Player.Warped += this.OnWarped;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /// <summary>
        /// Connections between the Content Patcher and SMAPI
        /// </summary>
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Reference to Content Patcher
            cpApi = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            
            // Working with Quest Framework
            {
                qfApi = this.Helper.ModRegistry.GetApi<IQuestApi>("PurrplingCat.QuestFramework");
                //IQuestApi qfApi = this.Helper.ModRegistry.GetApi<IQuestApi>("PurrplingCat.QuestEssentials");
                qfManagedApi = qfApi.GetManagedApi(this.ModManifest);

                qfApi.Events.GettingReady += (_sender, _e) => {
                    //qfManagedApi.RegisterQuest(/* enter quest definition here */);
                };
            }

            

            // Working with Content Patcher
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

        /*********
        ** Private methods
        *********/
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

            /*
            // Load Custom_Mansion
            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/maps/CustomMansion.tmx", ContentSource.ModFolder);
            // add the location
            GameLocation customMap = new GameLocation(mapAssetKey, "Custom_Mansion") { IsOutdoors = true, IsFarm = false };
            Game1.locations.Add(customMap);*/

            // Cutom Mail
            Game1.player.mailbox.Add("MyModMail1");
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

        // adding weapon data
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                return true;
            }
            return false;
        }
        

        public T Load<T>(IAssetInfo asset)
        {
            this.Monitor.Log("Loading Weapon");
            if (asset.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                return ((Mod)this).Helper.Content.Load<T>("assets/weapons/weapons.png", (ContentSource)1);
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
