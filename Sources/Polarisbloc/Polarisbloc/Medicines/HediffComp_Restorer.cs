using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_Restorer : HediffComp
    {
        public HediffCompProperties_Restorer Props
        {
            get
            {
                return (HediffCompProperties_Restorer)base.props;
            }
        }

        private HediffCompProperties_Restorer exactProps = new HediffCompProperties_Restorer();

        private RestorerMode WorkMode
        {
            get
            {
                return this.exactProps.workMode;
            }
        }

        private List<HediffDef> ExtraHediffs
        {
            get
            {
                return this.exactProps.extraHediffs;
            }
        }

        private bool CanRestoreMissingPart
        {
            get
            {
                return this.exactProps.canRestoreMissingPart;
            }
        }

        private int ticks;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            //Scribe_Values.Look<HediffCompProperties_Restorer>(ref this.exactProps, "exactProps");
            Scribe_Values.Look<int>(ref this.ticks, "ticks");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if(this.WorkMode == RestorerMode.antibiotic)
            {
                this.ticks++;
                if (this.ticks >= GenDate.TicksPerHour)
                {
                    this.AntibioticEffect(base.Pawn, this.ExtraHediffs);
                    this.ticks = 0;
                }
            }
            if(this.WorkMode == RestorerMode.restorer)
            {
                if (this.RestorerEffect(base.Pawn, this.CanRestoreMissingPart))
                {
                    this.ExtraEffect(base.Pawn, this.ExtraHediffs);
                }
            }
        }

        private void AntibioticEffect(Pawn pawn, List<HediffDef> extraHediffs)
        {

            foreach (Hediff hediff in from x in pawn.health.hediffSet.hediffs
                                      where x.def.makesSickThought || extraHediffs.Contains(x.def)
                                      select x)
            {
                if (hediff.TendableNow())
                {
                    hediff.Tended(0.8f);
                }
                HediffComp_Immunizable hediffComp_Immunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                if (hediffComp_Immunizable != null)
                {
                    hediff.Severity -= hediffComp_Immunizable.Props.severityPerDayNotImmune / GenDate.TicksPerDay * 1.2f * GenDate.TicksPerHour;
                }
                HediffComp_GrowthMode hediffComp_GrowthMode = hediff.TryGetComp<HediffComp_GrowthMode>();
                if (hediffComp_GrowthMode != null)
                {
                    hediff.Severity -= hediffComp_GrowthMode.Props.severityPerDayGrowing / GenDate.TicksPerDay * 1.5f * GenDate.TicksPerHour;
                }
                if (hediff.Severity <= hediff.def.initialSeverity || !hediff.Visible)
                {
                    pawn.health.RemoveHediff(hediff);
                    return;
                }
            }
        }

        private bool RestorerEffect(Pawn pawn, bool canRestoreMissingPart = false)
        {
            if(canRestoreMissingPart)
            {
                BodyPartRecord bodyPartRecord = this.FindBiggestMissingBodyPart(pawn, 0f);
                if(bodyPartRecord != null)
                {
                    this.RestoreMissingPart(pawn, bodyPartRecord);
                    pawn.health.hediffSet.DirtyCache();
                    Messages.Message("MessageBodyPartCuredByItem".Translate(bodyPartRecord.def.label), pawn, MessageTypeDefOf.PositiveEvent);
                    return false;
                }
            }
            Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => (x.def.isBad && !(x is Hediff_Injury) && !(x is Hediff_MissingPart) || x.IsPermanent()));
            if (hediff != null)
            {
                pawn.health.hediffSet.hediffs.Remove(hediff);
                return false;
            }
            return true;
        }

        private BodyPartRecord FindBiggestMissingBodyPart(Pawn pawn, float minCoverage = 0f)
        {
            BodyPartRecord bodyPartRecord = null;
            foreach (Hediff_MissingPart missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (!(missingPartsCommonAncestor.Part.coverageAbsWithChildren < minCoverage) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missingPartsCommonAncestor.Part) && (bodyPartRecord == null || missingPartsCommonAncestor.Part.coverageAbsWithChildren > bodyPartRecord.coverageAbsWithChildren))
                {
                    bodyPartRecord = missingPartsCommonAncestor.Part;
                }
            }
            return bodyPartRecord;
        }

        private void RestoreMissingPart(Pawn pawn, BodyPartRecord part)
        {

            if (part == null)
            {
                Log.Error("Tried to restore null body part.");
            }
            else
            {
                List<Hediff> mhediffs = (from x in pawn.health.hediffSet.hediffs
                                         where x is Hediff_MissingPart
                                         select x).ToList<Hediff>();
                for(int num = mhediffs.Count - 1; num >= 0; num--)
                {
                    if(mhediffs[num].Part == part)
                    {
                        pawn.health.hediffSet.hediffs.Remove(mhediffs[num]);
                    }
                }
            }
        }

        private void ExtraEffect(Pawn pawn, List<HediffDef> extraHediffs)
        {
            if(extraHediffs == null)
            {
                return;
            }
            foreach(Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if(extraHediffs.Contains(hediff.def))
                {
                    pawn.health.hediffSet.hediffs.Remove(hediff);
                    return;
                }
            }
        }
    }


    public class HediffCompProperties_Restorer : HediffCompProperties
    {
        public RestorerMode workMode = RestorerMode.undifined;

        public List<HediffDef> extraHediffs = new List<HediffDef>();

        public bool canRestoreMissingPart = false;

        public HediffCompProperties_Restorer()
        {
            this.compClass = typeof(HediffComp_Restorer);
        }
    }

    public enum RestorerMode : byte
    {
        undifined,
        antibiotic,
        restorer,
    }
}
