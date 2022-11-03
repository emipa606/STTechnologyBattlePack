using Verse;

namespace Myth;

public class CompProperties_GrenadeBelt : CompProperties
{
    public float armo;

    public int CooldownTicks;
    public ThingDef project;

    public float range;

    public float restperticks;

    public CompProperties_GrenadeBelt()
    {
        compClass = typeof(Comp_GrenadeBelt);
    }
}