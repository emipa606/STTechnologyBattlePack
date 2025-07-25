using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class GrenadeBelt : Apparel
{
    private static readonly Texture2D lunch = ContentFinder<Texture2D>.Get("UI/Lunch");

    private readonly TargetingParameters tgp = new();

    public float ammo;

    private float ammoomax;

    private int cooldown;

    private int CooldownTicks = 150;

    private ThingDef project;

    private float range;

    private float restperticks;

    private float tick;

    private bool Visiblerange;

    public float ammomax
    {
        get
        {
            if (ammoomax != 0f || Wearer == null)
            {
                return ammoomax;
            }

            var comp = GetComp<Comp_GrenadeBelt>();
            if (comp.props is CompProperties_GrenadeBelt belt)
            {
                CooldownTicks = belt.CooldownTicks;
                project = belt.project;
                restperticks = belt.restperticks;
                range = belt.range;
                ammoomax = belt.armo;
                tgp.canTargetBuildings = true;
                tgp.canTargetLocations = true;
                tgp.canTargetPawns = true;
            }
            else
            {
                Log.Error("Belt definition not of type \"BeltThingDef\"");
            }

            return ammoomax;
        }
    }

    public override void SpawnSetup(Map map, bool RE)
    {
        base.SpawnSetup(map, RE);
        var comp = GetComp<Comp_GrenadeBelt>();
        if (comp.props is CompProperties_GrenadeBelt belt)
        {
            CooldownTicks = belt.CooldownTicks;
            project = belt.project;
            restperticks = belt.restperticks;
            range = belt.range;
            ammoomax = belt.armo;
            ammo = ammomax;
            tgp.canTargetBuildings = true;
            tgp.canTargetLocations = true;
            tgp.canTargetPawns = true;
        }
        else
        {
            Log.Error("Belt definition not of type \"BeltThingDef\"");
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer == null)
        {
            ammo = 0f;
            cooldown = 0;
        }
        else
        {
            if (Wearer.Downed || Wearer.Dead)
            {
                return;
            }

            tick += 1f;
            if (cooldown > 0)
            {
                cooldown--;
            }

            if (!(tick >= restperticks))
            {
                return;
            }

            tick = 0f;
            if (ammo < ammomax)
            {
                ammo += 1f;
            }
        }
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        yield return new Command_Lunchgrenade
        {
            targetingParams = tgp,
            action = delegate(LocalTargetInfo target) { launch(Wearer, target)?.Invoke(); },
            mouseOverCallback = OnMouseOverGizmo,
            icon = lunch,
            defaultLabel = "使用挂件".Translate(),
            defaultDesc = "使用腰带挂件".Translate()
        };
        yield return new Gizmo_GrenadeStatus
        {
            grenade = this
        };
    }

    public override void DrawWornExtras()
    {
        base.DrawWornExtras();
        drawRangeOverlay();
    }

    private void drawRangeOverlay()
    {
        if (!Visiblerange)
        {
            return;
        }

        Visiblerange = false;
        if (range <= GenRadial.MaxRadialPatternRadius)
        {
            GenDraw.DrawRadiusRing(Wearer.Position, range);
        }
    }

    private Action launch(Pawn pawn, LocalTargetInfo target)
    {
        Action result = null;
        if (pawn.skills.GetSkill(SkillDefOf.Shooting) == null)
        {
            return null;
        }

        var max = GenRadial.NumCellsInRadius(20.3f / (pawn.skills.GetSkill(SkillDefOf.Shooting).Level + 2) *
                                             1.5f);
        var num = Rand.Range(0, max);
        if (Wearer == null || Wearer.Downed || Wearer.Dead)
        {
            return null;
        }

        if (target.Cell.InHorDistOf(Wearer.Position, range))
        {
            if (ammo > 0f && cooldown == 0)
            {
                var verbShoot = new Verb_Shoot
                {
                    caster = Wearer,
                    verbProps = new VerbProperties
                    {
                        targetParams = tgp,
                        minRange = 0f,
                        range = range,
                        requireLineOfSight = false
                    }
                };
                verbShoot.TryFindShootLineFromTo(Wearer.Position, target, out var shootLine);
                var aim = target.Cell + GenRadial.RadialPattern[num];
                result = delegate
                {
                    var projectile = (Projectile)GenSpawn.Spawn(project, shootLine.Source, pawn.Map);
                    projectile.Launch(pawn, target, aim, ProjectileHitFlags.All);
                };
                cooldown = CooldownTicks;
                ammo -= 1f;
            }
            else
            {
                "OutOfAmmo".Translate();
            }
        }
        else
        {
            "OutOfRange".Translate();
        }

        return result;
    }

    private void OnMouseOverGizmo()
    {
        Visiblerange = true;
    }
}