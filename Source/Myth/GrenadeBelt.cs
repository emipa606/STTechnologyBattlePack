using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth
{
    [StaticConstructorOnStartup]
    internal class GrenadeBelt : Apparel
    {
        public static Texture2D lunch = ContentFinder<Texture2D>.Get("UI/Lunch");

        private readonly TargetingParameters tgp = new TargetingParameters();

        public float ammo;

        public float ammoomax;

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

        public override void Tick()
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
                action = delegate(LocalTargetInfo target) { Lunch(Wearer, target, out var _)?.Invoke(); },
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
            DrawRangeOverlay();
        }

        private void DrawRangeOverlay()
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

        private Action Lunch(Pawn pawn, LocalTargetInfo target, out string failStr)
        {
            failStr = "";
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
                    var verb_Shoot = new Verb_Shoot
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
                    verb_Shoot.TryFindShootLineFromTo(Wearer.Position, target, out var shootLine);
                    var aim = target.Cell + GenRadial.RadialPattern[num];
                    result = delegate
                    {
                        var projectile = (Projectile)GenSpawn.Spawn(project, shootLine.Source, pawn.Map);
                        //projectile.set_FreeIntercept(!projectile.def.projectile.flyOverhead);
                        //if (!projectile.def.projectile.flyOverhead)
                        //{
                        //	projectile.set_InterceptWalls(true);
                        //}
                        projectile.Launch(pawn, target, aim, ProjectileHitFlags.All);
                    };
                    //ShotReport.HitReportFor(Wearer, verb_Shoot, target);
                    cooldown = CooldownTicks;
                    ammo -= 1f;
                }
                else
                {
                    failStr = "OutOfAmmo".Translate();
                }
            }
            else
            {
                failStr = "OutOfRange".Translate();
            }

            return result;
        }

        private void OnMouseOverGizmo()
        {
            Visiblerange = true;
        }
    }
}