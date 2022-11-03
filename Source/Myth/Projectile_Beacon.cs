using RimWorld;
using Verse;
using Verse.Sound;

namespace Myth;

public class Projectile_Beacon : Projectile_Explosive
{
    public float ticktodrop;

    protected override void Explode()
    {
        var map = Map;
        GenSpawn.Spawn(
            MakeSkyfaller(def.projectile.preExplosionSpawnThingDef, def.projectile.postExplosionSpawnThingDef),
            Position, map);
        SoundDefOf.Standard_Drop.PlayOneShot(new TargetInfo(Position, map));
        Destroy();
    }

    public static Skyfaller MakeSkyfaller(ThingDef skyfaller, ThingDef innerThing)
    {
        var thing = ThingMaker.MakeThing(innerThing);
        thing.SetFaction(Faction.OfPlayer);
        var skyfaller2 = (Skyfaller)ThingMaker.MakeThing(skyfaller);
        if (skyfaller2.innerContainer.TryAdd(thing))
        {
            return skyfaller2;
        }

        Log.Error($"Could not add {thing.ToStringSafe()} to a skyfaller.");
        thing.Destroy();

        return skyfaller2;
    }
}