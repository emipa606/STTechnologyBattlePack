using Verse;

namespace Myth;

public class CompProperties_HumidifierBuilding : CompProperties
{
    public float cblue;

    public float cgreen;

    public float cred;
    public HediffDef hediffDef;

    public float radius;

    public CompProperties_HumidifierBuilding()
    {
        compClass = typeof(Comp_HumidifierBuilding);
    }
}