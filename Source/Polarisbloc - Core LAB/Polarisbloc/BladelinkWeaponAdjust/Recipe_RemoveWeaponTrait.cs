using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_RemoveWeaponTrait : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            /*if (ingredient.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
            {
                List<WeaponTraitDef> curTraits = compBladelink.TraitsListForReading;
                List<DebugMenuOption> list = new List<DebugMenuOption>();
                foreach (WeaponTraitDef curTrait in curTraits)
                {
                    list.Add(new DebugMenuOption(curTrait.label, DebugMenuOptionMode.Action, delegate ()
                    {
                        //curTraits.Remove(curTrait);
                        //typeof(CompBladelinkWeapon).GetField("traits", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(compBladelink, curTraits);
                        compBladelink.RemoveWeaponTrait(curTrait);
                    }));
                }
                Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
            }*/
            if (PolarisUtility.ThingRemoveWeaponTrait(ingredient))
            {
                return;
            }
            else
            {
                ingredient.Destroy(DestroyMode.Vanish);
            }
        }
    }
}
