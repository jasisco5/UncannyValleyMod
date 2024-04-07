﻿using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Monsters;
using SpaceCore.UI;

namespace UncannyValleyMod
{
    internal class ModQuests
    {
        IModHelper helper;
        IMonitor monitor;
        Dictionary<int, bool> questObtained = new Dictionary<int, bool>();

        bool mansionMonstersSpawned = false;

        private void initQuestObtained()
        {
            questObtained.Clear();
            questObtained[2051901] = false;
            questObtained[2051902] = false;
            questObtained[2051903] = false;
            questObtained[2051904] = false;
            questObtained[2051905] = false;
        }
        public ModQuests(IModHelper _helper, IMonitor _monitor)
        {
            monitor = _monitor;
            helper = _helper;
            //SpaceCore.Api scApi;
            initQuestObtained();
            helper.Events.GameLoop.OneSecondUpdateTicking += this.OnQuestActivity;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.DayStarted += (object sender, DayStartedEventArgs e) => { mansionMonstersSpawned = false; };

            // Start quest code
            SpaceCore.Events.SpaceEvents.OnEventFinished += this.OnEventFinished;
            // OnItemEaten - Check player.itemToEat for what they just ate.
            // BombExploded - When a bomb explodes in a location. Useful for zelda-like puzzle walls
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (Game1.getLocationFromName("Custom_Mansion_Interior") == e.NewLocation)
            {
                // On Slay Monster Quest
                if (questObtained[2051904] == true && questObtained[2051905] == false)
                {
                    SpawnMansionMonsters();
                }
            }
        }

        private void OnEventFinished(object sender, EventArgs e)
        {
            this.monitor.Log($"Event id = {Game1.CurrentEvent.id}", LogLevel.Debug);
            // Jodi Event
            if (Game1.CurrentEvent.id == "2051901")
            {
                this.monitor.Log($"Starting Act1", LogLevel.Debug);
                Game1.player.addQuest("2051901");
            }
            // Meet Apprentice
            if (Game1.CurrentEvent.id == "2051902")
            {
                this.monitor.Log($"Starting Act1_2", LogLevel.Debug);
                Game1.player.addQuest("2051902");
            }
            // Meet Butler
            if (Game1.CurrentEvent.id == "2051903")
            {
                this.monitor.Log($"Starting Act2_3", LogLevel.Debug);
                Game1.player.addQuest("2051903");
            }
        }

        private void OnQuestActivity(object sender, OneSecondUpdateTickingEventArgs e)
        {
            // True when the quest icon is glowing
            if (Game1.player.hasNewQuestActivity())
            {
                // First Obtained the inital quest
                if (!questObtained[2051901] && Game1.player.hasQuest("2051901"))
                {
                    questObtained[2051901] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act1", LogLevel.Debug);
                }
                // Obtained Act2_1 (Go to Mansion)
                if (!questObtained[2051902] && Game1.player.hasQuest("2051902"))
                {
                    questObtained[2051902] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act2_1", LogLevel.Debug);
                }
                // Obtained Act2_2 (Speak to the Butler)
                if (!questObtained[2051903] && Game1.player.hasQuest("2051903"))
                {
                    questObtained[2051903] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act2_2", LogLevel.Debug);

                    helper.Events.GameLoop.OneSecondUpdateTicking += this.Act2_2;
                }
                // Obtained Act2_3 (Slay Slimes)
                if (!questObtained[2051904] && Game1.player.hasQuest("2051904"))
                {
                    questObtained[2051904] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act2_3", LogLevel.Debug);

                    SpawnMansionMonsters();
                }
            }
        }

        // Speaking to 'Butler' completes the quest
        private void Act2_2(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if(Game1.currentSpeaker != null)
            {
                this.monitor.Log($"Current Speaker = {Game1.currentSpeaker.Name}.", LogLevel.Debug);
                if (Game1.currentSpeaker.Name == "Butler")
                {
                    Game1.player.completeQuest("2051903");
                    helper.Events.GameLoop.OneSecondUpdateTicking -= this.Act2_2;
                    Game1.player.addQuest("2051904");
                }
            }
        }
        // Spawn Monsters
        private void SpawnMansionMonsters()
        {
            if (mansionMonstersSpawned) { return; }
            this.monitor.Log($"Spawning Slimes", LogLevel.Debug);
            mansionMonstersSpawned = true;
            Monster[] slimes = new Monster[20];
            slimes[0] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(70 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[1] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(8 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[2] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(4 * 64, 42 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[3] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(4 * 64, 10 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[4] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(23 * 64, 8 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[5] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(37 * 64, 9 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[6] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(49 * 64, 11 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[7] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(65 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[8] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(89 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[9] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(69 * 64, 43 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[10] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(71 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[11] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(9 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[12] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(5 * 64, 42 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[13] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(5 * 64, 10 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[14] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(24 * 64, 8 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[15] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(38 * 64, 9 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[16] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(50 * 64, 11 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[17] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(66 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[18] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(90 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[19] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(70 * 64, 43 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            foreach (var slime in slimes)
            {
                Game1.currentLocation.characters.Add(slime);
            }
            Monster[] skeletons = new Monster[10];
            skeletons[0] = new Skeleton(new Microsoft.Xna.Framework.Vector2(70 * 64, 27 * 64), false);
            skeletons[1] = new Skeleton(new Microsoft.Xna.Framework.Vector2(8 * 64, 27 * 64),  false);
            skeletons[2] = new Skeleton(new Microsoft.Xna.Framework.Vector2(4 * 64, 42 * 64),  false);
            skeletons[3] = new Skeleton(new Microsoft.Xna.Framework.Vector2(4 * 64, 10 * 64),  false);
            skeletons[4] = new Skeleton(new Microsoft.Xna.Framework.Vector2(23 * 64, 8 * 64),  false);
            skeletons[5] = new Skeleton(new Microsoft.Xna.Framework.Vector2(37 * 64, 9 * 64),  true);
            skeletons[6] = new Skeleton(new Microsoft.Xna.Framework.Vector2(49 * 64, 11 * 64), false);
            skeletons[7] = new Skeleton(new Microsoft.Xna.Framework.Vector2(65 * 64, 10 * 64), false);
            skeletons[8] = new Skeleton(new Microsoft.Xna.Framework.Vector2(89 * 64, 10 * 64), false);
            skeletons[9] = new Skeleton(new Microsoft.Xna.Framework.Vector2(69 * 64, 43 * 64), true);
            foreach (var skeleton in skeletons)
            {
                Game1.currentLocation.characters.Add(skeleton);
                Game1.currentLocation.characters.Add(skeleton);
            }
        }
    }
}
