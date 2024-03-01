using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    internal class ModMail
    {
        public ModMail(IModHelper helper) 
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data\\mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    // "MyModMail1" is referred to as the mail Id.  It is how you will uniquely identify and reference your mail.
                    // The @ will be replaced with the player's name.  Other items do not seem to work (i.e. %pet or %farm)
                    // %item object 388 50 %%   - this adds 50 pieces of wood when added to the end of a letter.
                    // %item money 250 601  %%  - this sends a random amount of gold from 250 to 601 inclusive.
                    // %item cookingRecipe %%   - this is for recipes (did not try myself)  Not sure how it know which recipe. 
                    data["MyModMail1"] = "Hello @... ^A single carat is a new line ^^Two carats will double space.";
                    data["MyModMail2"] = "This is how you send an existing item via email! %item object 388 50 %%";
                    data["MyModMail3"] = "Coin $   Star =   Heart <   Dude +  Right Arrow >   Up Arrow `";
                    data["MyWizardMail"] = "Include Wizard in the mail Id to use the special background on a letter";
                    data["spawnNote"] = "";
                }, AssetEditPriority.Early);
           }
        }
    }
}
