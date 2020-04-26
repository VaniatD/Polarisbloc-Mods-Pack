using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace Polarisbloc_SecurityForce
{
    public class CaniculaBullet : Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null)
                {
                    if (pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
                    {
                        dinfo.SetAmount(dinfo.Amount * 4);
                    }
                    else
                    {
                        dinfo.SetAmount(dinfo.Amount * 2);
                    }
                    if (this.def.projectile.speed >= 150 && Rand.Chance(0.25f))
                    {
                        BodyPartRecord bodyPart = pawn.health.hediffSet.GetBrain();
                        if (bodyPart != null)
                        {
                            dinfo.SetHitPart(bodyPart);
                        }
                    }
                }
                DamageInfo dinfoEX = new DamageInfo(dinfo)
                {
                    Def = DamageDefOf.EMP
                };
                hitThing.TakeDamage(dinfoEX);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
        }
    }
}
