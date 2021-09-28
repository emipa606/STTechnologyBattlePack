using RimWorld;
using Verse;

namespace Myth
{
    public class Verb_MeleeAttackBlink : Verb_MeleeAttack
    {
        protected override bool TryCastShot()
        {
            var pawn = caster as Pawn;
            var thing = currentTarget.Thing;
            if (pawn == null || thing == null)
            {
                return false;
            }

            if (!pawn.Position.InHorDistOf(thing.Position, 3f))
            {
                if (!base.TryCastShot())
                {
                    return false;
                }

                var intVec = thing.Position - pawn.Position;
                intVec.x = (int)(intVec.x * 0.9);
                intVec.z = (int)(intVec.z * 0.9);
                pawn.Position += intVec;
                if (thing is Pawn pawn2)
                {
                    HealthUtility.AdjustSeverity(pawn2, HediffDef.Named("ST2MY"), 0.1f);
                }

                return true;
            }

            if (base.TryCastShot())
            {
                return true;
            }

            return false;
        }

        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            return null;
        }
    }
}