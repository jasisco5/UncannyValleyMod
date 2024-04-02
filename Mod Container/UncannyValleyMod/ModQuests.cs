using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace UncannyValleyMod
{
    internal class ModQuests
    {
        IModHelper helper;
        IMonitor monitor;
        Dictionary<int, bool> questObtained = new Dictionary<int, bool>();
        private void initQuestObtained()
        {
            questObtained.Clear();
            questObtained[2051901] = false;
            questObtained[2051902] = false;
            questObtained[2051903] = false;
        }
        public ModQuests(IModHelper _helper, IMonitor _monitor)
        {
            monitor = _monitor;
            helper = _helper;
            //SpaceCore.Api scApi;
            initQuestObtained();
            helper.Events.GameLoop.OneSecondUpdateTicking += this.OnQuestActivity;

            // Start quest code
            SpaceCore.Events.SpaceEvents.OnEventFinished += this.OnEventFinished;
            // OnItemEaten - Check player.itemToEat for what they just ate.
            // BombExploded - When a bomb explodes in a location. Useful for zelda-like puzzle walls
        }

        private void OnEventFinished(object sender, EventArgs e)
        {
            this.monitor.Log($"Event id = {Game1.CurrentEvent.id}", LogLevel.Debug);
            // Jodi Event
            if (Game1.CurrentEvent.id == "20020615")
            {
                this.monitor.Log($"Starting Act1", LogLevel.Debug);
                Game1.player.addQuest("2051901");
            }
            // Meet Apprentice
            if (Game1.CurrentEvent.id == "20020614")
            {
                this.monitor.Log($"Starting Act1_2", LogLevel.Debug);
                Game1.player.addQuest("2051902");
                
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
                // Obtained Act2_1
                if (!questObtained[2051902] && Game1.player.hasQuest("2051902"))
                {
                    questObtained[2051902] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act2_1", LogLevel.Debug);
                }
                // Obtained Act2_2
                if (!questObtained[2051903] && Game1.player.hasQuest("2051903"))
                {
                    questObtained[2051903] = true;
                    this.monitor.Log($"{Game1.player.Name} obtained quest Act2_2", LogLevel.Debug);

                    helper.Events.GameLoop.OneSecondUpdateTicking += this.Act2_2;
                }
            }
        }

        // Speaking to 'Brennen' completes the quest
        private void Act2_2(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if(Game1.currentSpeaker != null)
            {
                this.monitor.Log($"Current Speaker = {Game1.currentSpeaker.Name}.", LogLevel.Debug);
                if (Game1.currentSpeaker.Name == "Brennan")
                {
                    Game1.player.completeQuest("2051903");
                    helper.Events.GameLoop.OneSecondUpdateTicking -= this.Act2_2;
                }
            }
            
        }
    }
}
