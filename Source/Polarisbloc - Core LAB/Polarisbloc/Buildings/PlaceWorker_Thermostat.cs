using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class PlaceWorker_Thermostat : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            Room room = center.GetRoom(currentMap);
            if (room != null && !room.UsesOutdoorTemperature)
            {
                GenDraw.DrawFieldEdges(room.Cells.ToList<IntVec3>(), Color.magenta);
            }
        }
    }
}
