using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_AddWeaponTrait : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            /*if (ingredient.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
            {
                List<DebugMenuOption> list = new List<DebugMenuOption>();
                foreach (WeaponTraitDef traitDef in DefDatabase<WeaponTraitDef>.AllDefs)
                {
                    if (compBladelink.CanAddWeaponTrait(traitDef))
                    {
                        list.Add(new DebugMenuOption(traitDef.label, DebugMenuOptionMode.Action, delegate ()
                        {
                            compBladelink.AddWeaponTrait(traitDef);
                        }));
                    }

                }
                Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
            }*/
            if (PolarisUtility.ThingAddWeaponTrait(ingredient))
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
