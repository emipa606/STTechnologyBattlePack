using Verse;

namespace Myth
{
    internal class MentalState_Remove : HediffGiver
    {
        public int maxtick;
        public int mintick;

        public float recoveryMtbDays;

        public override void OnIntervalPassed(Pawn pawn, Hediff hediffDef)
        {
            if (pawn.MentalStateDef == null)
            {
                return;
            }

            pawn.MentalStateDef.minTicksBeforeRecovery = 20;
            pawn.MentalStateDef.maxTicksBeforeRecovery = 30;
        }
    }
}