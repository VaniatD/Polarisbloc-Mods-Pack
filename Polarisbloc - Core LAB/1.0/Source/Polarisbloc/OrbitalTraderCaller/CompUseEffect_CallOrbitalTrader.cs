using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompUseEffect_CallOrbitalTrader : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            Map map = user.MapHeld;
            if (map.passingShipManager.passingShips.Count >= 5)
            {
                Messages.Message("PolarisOrbitalTraderCouldNotArrival".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            TraderKindDef traderKindDef = base.parent.TryGetComp<CompOrbitalTraderCaller>().traderKindDef;
            TradeShip tradeShip = new TradeShip(traderKindDef);
            if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && b.GetComp<CompPowerTrader>().PowerOn))
            {
                Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label), LetterDefOf.PositiveEvent, null);
            }
            map.passingShipManager.AddShip(tradeShip);
            tradeShip.GenerateThings();
            return;
        }
    }
}
