using Verse;

namespace Myth;

public class CompProperties_MassShield : CompProperties
{
    public float pointmax;

    public float pointperdamage;

    public float range;

    public float restpertick;
    public int restticks;

    public float ticktoblock;

    public CompProperties_MassShield()
    {
        compClass = typeof(Comp_MassShield);
    }
}