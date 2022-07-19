using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace Polarisbloc
{
    public class Building_Thermostat : Building_TempControl
    {
        private bool highPower = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.highPower, "highPower");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            yield return new Command_Toggle
            {
                defaultLabel = "PolarisThermostatEfficientModeLabel".Translate(),
                defaultDesc = "PolarisThermostatEfficientModeDesc".Translate(),
                icon = this.def.uiIcon,
                isActive = () => this.highPower,
                toggleAction = delegate
                {
                    this.highPower = !this.highPower;
                }
            };
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Set room temp to target",
                    action = delegate
                    {
                        this.GetRoom(RegionType.Set_All).Temperature = this.compTempControl.targetTemperature;
                    }
                };
            }
        }

        public override void TickRare()
        {
            if (this.compPowerTrader.PowerOn)
            {
                float ambientTemperature = base.AmbientTemperature;
                float num;
                /*if (ambientTemperature > this.compTempControl.targetTemperature - 1 && ambientTemperature < this.compTempControl.targetTemperature + 1)
                {
                    num = 0f;
                }*/
                if (Mathf.Abs(ambientTemperature - this.compTempControl.targetTemperature) < 1f)
                {
                    num = 0f;
                }
                else if (ambientTemperature < this.compTempControl.targetTemperature - 1)
                {
                    if (ambientTemperature < 20f)
                    {
                        num = 1f;
                    }
                    else if (ambientTemperature > 200f)
                    {
                        num = 0f;
                    }
                    else
                    {
                        num = Mathf.InverseLerp(200f, 20f, ambientTemperature);
                    }
                }
                else if (ambientTemperature > this.compTempControl.targetTemperature + 1)
                {
                    if (ambientTemperature < -50f)
                    {
                        num = -Mathf.InverseLerp(-273f, -50f, ambientTemperature);
                    }
                    else
                    {
                        num = -1f;
                    }
                }
                else
                {
                    num = 0f;
                }
                if (this.highPower)
                {
                    num *= 4;
                }
                float energyLimit = this.compTempControl.Props.energyPerSecond * num * 4.16666651f;
                float tempOffset = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, this.compTempControl.targetTemperature);
                bool flag = !Mathf.Approximately(tempOffset, 0f);
                CompProperties_Power props = this.compPowerTrader.Props;
                if (flag)
                {
                    this.GetRoom(RegionType.Set_All).Temperature += tempOffset;
                    float powerConsumption = -props.basePowerConsumption;
                    if (this.highPower)
                    {
                        powerConsumption *= 4;
                    }
                    this.compPowerTrader.PowerOutput = powerConsumption;
                }
                else
                {
                    this.compPowerTrader.PowerOutput = -props.basePowerConsumption * this.compTempControl.Props.lowPowerConsumptionFactor;
                }
                this.compTempControl.operatingAtHighPower = flag;
            }
        }

    }
}
