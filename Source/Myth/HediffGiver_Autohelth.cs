using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Myth;

internal class HediffGiver_Autohelth : HediffGiver
{
    public new bool canAffectAnyLivePart;

    public bool cure;

    public float healthcount;

    public new HediffDef hediff = HediffDefOf.Anesthetic;

    public new List<BodyPartDef> partsToAffect;

    public float quality;

    private int tick;
    public int tickmtb;

    public override void OnIntervalPassed(Pawn pawn, Hediff hediffDef)
    {
        Log.Message(hediff.LabelCap);
        if (pawn == null || pawn.Dead || pawn.Map == null)
        {
            return;
        }

        tick++;
        switch (tickmtb)
        {
            case -1:
            {
                for (var i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
                {
                    if (!pawn.health.hediffSet.hediffs[i].IsTended() &&
                        !pawn.health.hediffSet.hediffs[i].IsPermanent() &&
                        pawn.health.hediffSet.hediffs[i].def.tendable)
                    {
                        pawn.health.hediffSet.hediffs[i].Tended(3f, 0);
                        if (cure)
                        {
                            (pawn.health.hediffSet.pawn.health.hediffSet.hediffs[i] as Hediff_Injury)?.Heal(2f);
                        }
                    }
                    else if (cure)
                    {
                        if (pawn.health.hediffSet.hediffs[i] is HediffWithComps hediffWithComps &&
                            string.Compare(pawn.health.hediffSet.hediffs[i].def.defName, hediffDef.def.defName,
                                StringComparison.Ordinal) != 0 &&
                            pawn.health.hediffSet.hediffs[i].def.isBad)
                        {
                            pawn.health.RemoveHediff(hediffWithComps);
                        }
                    }
                }

                break;
            }
            case >= 0 when tick >= tickmtb:
            {
                for (var j = 0; j < pawn.health.hediffSet.hediffs.Count; j++)
                {
                    if (!pawn.health.hediffSet.hediffs[j].IsTended() &&
                        !pawn.health.hediffSet.hediffs[j].IsPermanent() &&
                        pawn.health.hediffSet.hediffs[j].def.tendable)
                    {
                        pawn.health.hediffSet.hediffs[j].Tended(quality, 0);
                    }

                    if (cure)
                    {
                        (pawn.health.hediffSet.hediffs[j] as Hediff_Injury)?.Heal(healthcount);
                    }
                }

                break;
            }
        }

        if (tick > tickmtb)
        {
            tick = 0;
        }
    }
}