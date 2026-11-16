using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class HediffGiver_Ambrosia : HediffGiver
{
    public override void OnIntervalPassed(Pawn pawn, Hediff cause)
    {
        if (cause != null && pawn.ageTracker.AgeBiologicalYears > 20)
        {
            pawn.ageTracker.AgeBiologicalTicks -= 3600000;
        }
    }
}