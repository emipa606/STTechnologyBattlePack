using Verse;

namespace Myth;

public class CompProperties_GrenadeBelt : CompProperties
{
    public int CooldownTicks;
    public float armo;
    public ThingDef project;

    public float range;

    public float restperticks;

    public CompProperties_GrenadeBelt()
    {
        compClass = typeof(Comp_GrenadeBelt);
    }
}