using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Polarisbloc_SecurityForce
{
	[StaticConstructorOnStartup]
	public class PolarisShieldBelt_IV : Apparel
	{
		private float energy;

		private int ticksToReset = -1;

		private int lastKeepDisplayTick = -9999;

		private Vector3 impactAngleVect;

		private int lastAbsorbDamageTick = -9999;

		private const float MinDrawSize = 1.2f;

		private const float MaxDrawSize = 1.45f;

		private const float MaxDamagedJitterDist = 0.05f;

		private const int JitterDurationTicks = 8;

		private int StartingTicksToReset = 1200;

		private float EnergyOnReset = 1f;

		private float EnergyLossPerDamage = 0.02f;

		private int KeepDisplayingTicks = 600;

		private float ApparelScorePerEnergyMax = 0.25f;

		private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/PolarisShield_IVBubble", ShaderDatabase.Transparent);

        private int LightningColdDownTicks = 120;

        private int ColdDownTick;

        private bool canLightning = true;

		private float EnergyMax
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
			}
		}

		private float EnergyGainPerTick
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
			}
		}

		public float Energy
		{
			get
			{
				return this.energy;
			}
		}

		public ShieldState ShieldState
		{
			get
			{
				if (this.ticksToReset > 0)
				{
					return ShieldState.Resetting;
				}
				return ShieldState.Active;
			}
		}

		private bool ShouldDisplay
		{
			get
			{
				Pawn wearer = base.Wearer;
				return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
			Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
			Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
            Scribe_Values.Look<bool>(ref this.canLightning, "canLightning", true, false);

        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != base.Wearer)
                yield break;
            yield return new Command_Toggle
            {
                hotKey = KeyBindingDefOf.Command_TogglePower,
                icon = TexCommand.Forbidden,
                defaultLabel = "PlrsLightningActiveLabel".Translate(),
                defaultDesc = "PlrsLightningActiveDESC".Translate(),
                isActive = () => this.canLightning,
                toggleAction = () => { this.canLightning = !this.canLightning; },
            };
            if (this.Wearer.Drafted)
            {
                if (this.ShieldState == ShieldState.Active)
                {
                    yield return new Command_Action
                    {
                        action = delegate
                        {
                            if (this.energy <= 0.5f)
                            {
                                Messages.Message("PlrsNoEnoughEnergyToDoExplosion".Translate(), this.Wearer, MessageTypeDefOf.NegativeEvent);
                            }
                            else
                            {
                                this.energy -= 0.5f;
                                GenExplosion.DoExplosion(this.Wearer.PositionHeld, this.Wearer.MapHeld, 2.2f, DamageDefOf.Bomb, this.Wearer, 20, -1f, DamageDefOf.Bomb.soundExplosion, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                            }
                        },
                        defaultLabel = "PlrsDoExplosionLabel".Translate(),
                        defaultDesc = "PlrsDoExplosionDESC".Translate(),
                        icon = base.def.uiIcon,
                        hotKey = KeyBindingDefOf.Misc6,
                    };
                }
                if (this.ShieldState != ShieldState.Active)
                {
                    yield return new Command_Action
                    {
                        action = delegate
                        {
                            if (this.HitPoints <= 20)
                            {
                                Messages.Message("PlrsNoEnoughHitPointsToReset".Translate(), this.Wearer, MessageTypeDefOf.NegativeEvent);
                            }
                            else
                            {
                                this.HitPoints -= 20;
                                this.Reset();
                            }
                        },
                        defaultLabel = "PlrsForceResetLabel".Translate(),
                        defaultDesc = "PlrsForceResetDESC".Translate(),
                        icon = TexCommand.DesirePower,
                        hotKey = KeyBindingDefOf.Misc7,
                    };
                }
            }
            yield return (Gizmo)new Gizmo_PolarisShield_IVStatus
            {
                shield = this
            };
            
        }

        public override float GetSpecialApparelScoreOffset()
		{
			return this.EnergyMax * this.ApparelScorePerEnergyMax;
		}

		public override void Tick()
		{
			base.Tick();
			if (base.Wearer == null)
			{
				this.energy = 0f;
				return;
			}
            if (!CheckLightningAvaliable())
            {
                this.ColdDownTick++;
            }
			if (this.ShieldState == ShieldState.Resetting)
			{
				this.ticksToReset--;
				if (this.ticksToReset <= 0)
				{
					this.Reset();
				}
			}
			else if (this.ShieldState == ShieldState.Active)
			{
				this.energy += this.EnergyGainPerTick;
				if (this.energy > this.EnergyMax)
				{
					this.energy = this.EnergyMax;
				}
			}
		}

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Instigator == base.Wearer) return true;
            if (dinfo.Def == DamageDefOf.SurgicalCut) return false;
            if (this.ShieldState == ShieldState.Active)
            {
                if(CheckLightningAvaliable() && dinfo.Instigator != null)
                {
                    if (dinfo.Instigator.def != ThingDefOf.Fire && dinfo.Instigator.Faction != base.Wearer.Faction)
                    {
                        Map map = dinfo.Instigator.MapHeld;
                        map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(map, dinfo.Instigator.PositionHeld));
                        this.ColdDownTick = 0;
                    }
                }
                if (dinfo.Instigator != null && dinfo.Instigator.Position.AdjacentTo8WayOrInside(base.Wearer.Position))
                {
                    this.energy -= (float)dinfo.Amount * this.EnergyLossPerDamage;
                }
                this.energy -= (float)dinfo.Amount * this.EnergyLossPerDamage;
                if (dinfo.Instigator is Building building && dinfo.Amount <= 10)
                {
                    this.energy += (float)dinfo.Amount * this.EnergyLossPerDamage;
                    Map map = dinfo.Instigator.MapHeld;
                    map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(map, dinfo.Instigator.PositionHeld));
                }
                if (dinfo.Amount > 10) this.energy += (float)dinfo.Amount * this.EnergyLossPerDamage - 0.15f;
                if (this.energy < 0f )
                {
                    this.Break();
                }
                else
                {
                    this.AbsorbedDamage(dinfo);
                }
                return true;
            }
            return false;
        }

        public void KeepDisplaying()
		{
			this.lastKeepDisplayTick = Find.TickManager.TicksGame;
		}

		private void AbsorbedDamage(DamageInfo dinfo)
		{
			SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
			Vector3 loc = base.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
			float num = Mathf.Min(10f, 2f + (float)dinfo.Amount / 10f);
			MoteMaker.MakeStaticMote(loc, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
			int num2 = (int)num;
			for (int i = 0; i < num2; i++)
			{
				MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
			this.KeepDisplaying();
		}

		private void Break()
		{
			SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			MoteMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
				MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.energy = 0f;
			this.ticksToReset = this.StartingTicksToReset;
		}

		private void Reset()
		{
			if (base.Wearer.Spawned)
			{
				SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
				MoteMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
			}
			this.ticksToReset = -1;
			this.energy = this.EnergyOnReset;
		}

		public override void DrawWornExtras()
		{
			if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
			{
				float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
				Vector3 vector = base.Wearer.Drawer.DrawPos;
				vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
				int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
				if (num2 < 8)
				{
					float num3 = (float)(8 - num2) / 8f * 0.05f;
					vector += this.impactAngleVect * num3;
					num -= num3;
				}
				float angle = (float)Rand.Range(0, 360);
				Vector3 s = new Vector3(num, 1f, num);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, PolarisShieldBelt_IV.BubbleMat, 0);
			}
		}

        private bool CheckLightningAvaliable()
        {
            if (this.ColdDownTick >= this.LightningColdDownTicks && this.canLightning) return true;
            else return false;
        }
        
        /*private void SelfDestory(Pawn pawn)
        {
            this.Destroy(DestroyMode.Vanish);
            pawn.apparel.Remove(this);
            GenExplosion.DoExplosion(pawn.PositionHeld, pawn.MapHeld, 5, DamageDefOf.Flame, this, 50, SoundDefOf.BulletImpactGround, null, null, null, 0, 1, false, null, 0, 1, 0.5f, false);
            string text = "MessageSuccessfullySelfDestoryActive".Translate(new object[]
                            {
                            pawn.LabelShort,
                            this.LabelShort
                            });
            Messages.Message(text, pawn, MessageTypeDefOf.NegativeEvent);
        }*/
	}
}
