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
            TradeShip tradeShip = new TradeShip(traderKindDef, this.GetFaction(traderKindDef));
            if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && (b.GetComp<CompPowerTrader>() == null || b.GetComp<CompPowerTrader>().PowerOn)))
            {
                //Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label), LetterDefOf.PositiveEvent, null);
                //Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label, "TraderArrivalNoFaction".Translate()), LetterDefOf.PositiveEvent, null);
                Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label, (tradeShip.Faction == null) ? "TraderArrivalNoFaction".Translate() : "TraderArrivalFromFaction".Translate(tradeShip.Faction.Named("FACTION"))), LetterDefOf.PositiveEvent, null);
            }
            map.passingShipManager.AddShip(tradeShip);
            tradeShip.GenerateThings();
            return;
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (base.CanBeUsedBy(p, out failReason))
            {
                if (p.Map.passingShipManager.passingShips.Count >= 5)
                {
                    failReason = "PolarisOrbitalTraderCouldNotArrival".Translate();
                    return false;
                }
                return true;
            }

            return false;
        }

        private Faction GetFaction(TraderKindDef trader)
        {
            if (trader.faction == null)
            {
                return null;
            }
            Faction result;
            if (!(from f in Find.FactionManager.AllFactions
                  where f.def == trader.faction
                  select f).TryRandomElement(out result))
            {
                return null;
            }
            return result;
        }
    }
}
