using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class Assist : Apparel
{
    private float cooldowntick;

    private float coolticks;

    private bool isactive;

    public override void SpawnSetup(Map map, bool RE)
    {
        base.SpawnSetup(map, RE);
        var comp = GetComp<Comp_Assist>();
        if (comp.props is CompProperties_Assist assist)
        {
            cooldowntick = assist.cooldowntick;
        }
        else
        {
            Log.Error("Assist definition not of type \"AssistDef\"");
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer == null)
        {
            return;
        }

        coolticks += 1f;
        if (!(coolticks > cooldowntick))
        {
            return;
        }

        if (!Wearer.Downed && !Wearer.Dead && Wearer.Map != null)
        {
            var position = Wearer.Position;
            ProtectSquare(position);
        }

        coolticks = 0f;
    }

    protected virtual void ProtectSquare(IntVec3 square)
    {
        if (!square.InBounds(Wearer.Map))
        {
            return;
        }

        var list = Wearer.Map.thingGrid.ThingsListAt(square);
        _ = new List<Thing>();
        var i = 0;
        for (var num = list.Count; i < num; i++)
        {
            if (list[i] == null || list[i] is not Projectile)
            {
                continue;
            }

            var projectile = (Projectile)list[i];
            if (projectile.Destroyed)
            {
                continue;
            }

            if (ReflectionHelper.GetInstanceField(typeof(Projectile), projectile, "launcher") is Pawn pawn &&
                pawn == Wearer && def != getTargetEquipmentFromProjectile(projectile))
            {
                doShot(pawn, projectile);
            }
        }
    }

    private void doShot(Pawn pawn, Projectile projectile)
    {
        if (pawn.equipment.Primary == null || pawn.equipment.Primary.def.Verbs.Count == 0 ||
            pawn.equipment.Primary.def.Verbs[0].defaultProjectile == null)
        {
            return;
        }

        var targetEquipmentFromProjectile = getTargetEquipmentFromProjectile(projectile);
        if (targetEquipmentFromProjectile == null)
        {
            return;
        }

        var intVec = getTargetLocationFromProjectile(projectile).ToIntVec3();
        var projectile2 = (Projectile)GenSpawn.Spawn(
            targetEquipmentFromProjectile.Verbs[0].defaultProjectile, pawn.DrawPos.ToIntVec3(), pawn.Map);
        var intVec2 = new IntVec3((int)Wearer.DrawPos.x + 1, (int)Wearer.DrawPos.y,
            (int)Wearer.DrawPos.z + 1);
        projectile2.Launch(Wearer, intVec2, intVec, projectile.HitFlags, false, this);
    }

    private static Vector3 getTargetLocationFromProjectile(Projectile projectile)
    {
        return (Vector3)projectile.GetType()
            .GetField("destination", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(projectile)!;
    }

    private static ThingDef getTargetEquipmentFromProjectile(Projectile projectile)
    {
        return (ThingDef)projectile.GetType()
            .GetField("equipmentDef", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(projectile);
    }
}