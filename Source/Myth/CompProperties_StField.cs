using Verse;

namespace Myth;

public class CompProperties_StField : CompProperties
{
    public HediffDef buff;

    public bool forEnemy;

    public float pointmax;

    public float range;

    public float recoverypointpercheck;

    public CompProperties_StField()
    {
        compClass = typeof(Comp_StField);
    }
}