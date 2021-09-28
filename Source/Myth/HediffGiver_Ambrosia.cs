using Verse;

namespace Myth
{
    [StaticConstructorOnStartup]
    internal class HediffGiver_Ambrosia : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff hediffDef)
        {
            if (hediffDef != null && pawn.ageTracker.AgeBiologicalYears > 20)
            {
                pawn.ageTracker.AgeBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks - 3600000;
            }
        }
    }
}