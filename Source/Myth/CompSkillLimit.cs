using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

public class CompSkillLimit : ThingComp
{
    private int tick;

    private CompProperties_SkillLimit Properties => (CompProperties_SkillLimit)props;

    public override void CompTick()
    {
        base.CompTick();
        tick++;
        if (tick < 150)
        {
            return;
        }

        if (parent is Apparel { Wearer: not null } apparel &&
            apparel.Wearer.skills.GetSkill(Properties.skill) != null &&
            apparel.Wearer.skills.GetSkill(Properties.skill).Level < Properties.level)
        {
            if (apparel.Wearer is { Map: not null })
            {
                MoteMaker.ThrowText(
                    new Vector3(apparel.Wearer.Position.x + 1f, apparel.Wearer.Position.y,
                        apparel.Wearer.Position.z + 1f), apparel.Wearer.Map, "技能等级不足".Translate(), Color.red);
            }

            apparel.Wearer.apparel.TryDrop(apparel, out _);
        }

        tick = 0;
    }
}