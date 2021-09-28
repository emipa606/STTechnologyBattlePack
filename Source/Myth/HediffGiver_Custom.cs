using System;
using System.Collections.Generic;
using Verse;

namespace Myth
{
    internal class HediffGiver_Custom : HediffGiver
    {
        public List<string> bodyPartsToAffect;
        public DamageDef def;

        public HediffDef hediffDef;

        public float mtbDays;

        public float severity;

        private List<HediffDef> test;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            var num = mtbDays;
            var num2 = 60000f;
            var num3 = 60f;
            if (num == float.PositiveInfinity)
            {
                return;
            }

            if (num <= 0f)
            {
                Log.Error("MTBEventOccurs with mtb=" + num);
                return;
            }

            if (num2 <= 0f)
            {
                Log.Error("MTBEventOccurs with mtbUnit=" + num2);
                return;
            }

            if (num3 <= 0f)
            {
                Log.Error("MTBEventOccurs with checkDuration=" + num3);
                return;
            }

            var num4 = num3 / (num * (double)num2);
            if (num4 <= 0.0)
            {
                Log.Error(
                    "chancePerCheck is " + num4 + ". mtb=" + num + ", mtbUnit=" + num2 + ", checkDuration=" + num3);
                return;
            }

            var num5 = 1.0;
            if (num4 < 0.0001)
            {
                while (num4 < 0.0001)
                {
                    num4 *= 8.0;
                    num5 /= 8.0;
                }

                if (Rand.Value > num5)
                {
                    return;
                }
            }

            if (!(Rand.Value < num4))
            {
                return;
            }

            if (def != null)
            {
                var num6 = Convert.ToInt32(severity);
                foreach (var allPart in pawn.RaceProps.body.AllParts)
                {
                    foreach (var item in bodyPartsToAffect)
                    {
                        if (allPart.def.LabelCap.RawText.Equals(item))
                        {
                            pawn.TakeDamage(new DamageInfo(def, num6, 0, -1f, null, allPart));
                        }
                    }
                }
            }
            else if (hediffDef != null)
            {
                HealthUtility.AdjustSeverity(pawn, hediffDef, severity);
            }
        }
    }
}