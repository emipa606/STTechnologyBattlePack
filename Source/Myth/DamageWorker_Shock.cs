using Verse;

namespace Myth
{
    public class DamageWorker_Shock : DamageWorker
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            var result = new DamageResult();
            (thing as Pawn)?.health.AddHediff(dinfo.Def.hediff);
            return result;
        }
    }
}