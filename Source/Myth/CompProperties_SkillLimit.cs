using RimWorld;
using Verse;

namespace Myth;

public class CompProperties_SkillLimit : CompProperties
{
    public float level;

    public SkillDef skill;

    public CompProperties_SkillLimit()
    {
        compClass = typeof(CompSkillLimit);
    }
}