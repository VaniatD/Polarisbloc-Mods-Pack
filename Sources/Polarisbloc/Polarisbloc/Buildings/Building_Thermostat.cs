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
        public override void TickRare()
        {
            if (this.compPowerTrader.PowerOn)
            {
                float ambientTemperature = base.AmbientTemperature;
                float num;
                if (ambientTemperature > this.compTempControl.targetTemperature - 1 && ambientTemperature < this.compTempControl.targetTemperature + 1)
                {
                    num = 0f;
                }
                else if (ambientTemperature < this.compTempControl.targetTemperature - 1)
                {
                    if (ambientTemperature < 20f)
                    {
                        num = 1f;
                    }
                    else if (ambientTemperature > 150f)
                    {
                        num = 0f;
                    }
                    else
                    {
                        num = Mathf.InverseLerp(150f, 100f, ambientTemperature);
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
                float energyLimit = this.compTempControl.Props.energyPerSecond * num * 4.16666651f;
                float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, this.compTempControl.targetTemperature);
                bool flag = !Mathf.Approximately(num2, 0f);
                CompProperties_Power props = this.compPowerTrader.Props;
                if (flag)
                {
                    this.GetRoomGroup().Temperature += num2;
                    this.compPowerTrader.PowerOutput = -props.basePowerConsumption;
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
