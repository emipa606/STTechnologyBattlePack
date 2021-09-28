using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Myth
{
    [StaticConstructorOnStartup]
    public class EnergyShield : ShieldBelt
    {
        private const float MinDrawSize = 1.2f;

        private const float MaxDrawSize = 1.55f;

        private const float MaxDamagedJitterDist = 0.05f;

        private const int JitterDurationTicks = 8;

        private static readonly Material BubbleMat =
            MaterialPool.MatFrom("Things/Projectile/shield", ShaderDatabase.Transparent);

        private readonly float ApparelScorePerEnergyMax = 0.25f;

        private readonly float EnergyLossPerDamage = 0.027f;

        private readonly float EnergyOnReset = 0.2f;

        private readonly int KeepDisplayingTicks = 1000;

        private readonly int lastKeepDisplayTick = -9999;

        private readonly int StartingTicksToReset = 3200;

        private Vector3 impactAngleVect;

        private int lastAbsorbDamageTick = -9999;

        private int ticksToReset = -1;

        public float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax);

        private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate) / 60f;

        public new float Energy { get; private set; }

        public new ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }

                return ShieldState.Active;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                if (Wearer.Dead || Wearer.Downed || Wearer.IsPrisonerOfColony &&
                    (Wearer.MentalStateDef == null || !Wearer.MentalStateDef.IsAggro))
                {
                    return false;
                }

                if (!Wearer.Drafted && !Wearer.Faction.HostileTo(Faction.OfPlayer))
                {
                    return Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks;
                }

                return true;
            }
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            yield return new Gizmo_EnergyShieldStatus
            {
                shield = this
            };
        }

        public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }

        public override void Tick()
        {
            base.Tick();
            if (Wearer == null)
            {
                Energy = 0f;
            }
            else if (ShieldState == ShieldState.Resetting)
            {
                ticksToReset--;
                if (ticksToReset <= 0)
                {
                    Reset();
                }
            }
            else if (ShieldState == ShieldState.Active)
            {
                Energy += EnergyGainPerTick;
                if (Energy > EnergyMax)
                {
                    Energy = EnergyMax;
                }
            }
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (ShieldState != ShieldState.Active || dinfo.Instigator == null && !dinfo.Def.isExplosive)
            {
                return false;
            }

            if (dinfo.Instigator != null)
            {
                if (dinfo.Instigator is AttachableThing attachableThing && attachableThing.parent == Wearer)
                {
                    return false;
                }
            }

            Energy -= dinfo.Amount * EnergyLossPerDamage;
            if (Energy < 0f)
            {
                Break();
                return false;
            }

            AbsorbedDamage(dinfo);
            return true;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            if (Energy > EnergyMax * 0.9 && dinfo.Instigator != null)
            {
                if (dinfo.Instigator is Pawn pawn)
                {
                    if (pawn.equipment.Primary != null)
                    {
                        var position = dinfo.Instigator.Position;
                        var projectile = (Projectile)GenSpawn.Spawn(
                            pawn.equipment.Primary.def.Verbs[0].defaultProjectile, position, dinfo.Instigator.Map);
                        //projectile..set_FreeIntercept(!projectile.def.projectile.flyOverhead);
                        //if (!projectile.def.projectile.flyOverhead)
                        //{
                        //	projectile.set_InterceptWalls(true);
                        //}
                        projectile.Launch(Wearer, (LocalTargetInfo)Wearer, position, projectile.HitFlags, false, this);
                    }
                }
                else
                {
                    var position2 = dinfo.Instigator.Position;
                    if (dinfo.Instigator is Building building && building.def.building.turretGunDef != null)
                    {
                        var projectile2 = (Projectile)GenSpawn.Spawn(
                            building.def.building.turretGunDef.Verbs[0].defaultProjectile, position2,
                            dinfo.Instigator.Map);
                        //projectile2.set_FreeIntercept(!projectile2.def.projectile.flyOverhead);
                        //if (!projectile2.def.projectile.flyOverhead)
                        //{
                        //	projectile2.set_InterceptWalls(true);
                        //}
                        projectile2.Launch(Wearer, Wearer, position2, ProjectileHitFlags.All, false, this);
                    }
                }
            }

            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            var vector = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
            var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
            FleckMaker.Static(vector, Wearer.Map, FleckDefOf.ExplosionFlash, num);
            var num2 = (int)num;
            for (var i = 0; i < num2; i++)
            {
                FleckMaker.ThrowDustPuff(vector, Wearer.Map, Rand.Range(0.8f, 1.2f));
            }

            lastAbsorbDamageTick = Find.TickManager.TicksGame;
            KeepDisplaying();
        }

        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
            for (var i = 0; i < 6; i++)
            {
                FleckMaker.ThrowDustPuff(
                    Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) *
                                           Rand.Range(0.3f, 0.6f)), Wearer.Map, Rand.Range(0.8f, 1.2f));
            }

            Energy = 0f;
            ticksToReset = StartingTicksToReset;
        }

        public override bool AllowVerbCast(Verb verb)
        {
            return true;
        }

        private void Reset()
        {
            if (Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
                FleckMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
            }

            ticksToReset = -1;
            Energy = EnergyOnReset;
        }

        public override void DrawWornExtras()
        {
            if (ShieldState != ShieldState.Active || !ShouldDisplay)
            {
                return;
            }

            var num = Mathf.Lerp(1.2f, 1.55f, Energy);
            var drawPos = Wearer.Drawer.DrawPos;
            drawPos.y = AltitudeLayer.Pawn.AltitudeFor();
            var num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
            if (num2 < 8)
            {
                var num3 = (8 - num2) / 8f * 0.05f;
                drawPos += impactAngleVect * num3;
                num -= num3;
            }

            float angle = Rand.Range(0, 360);
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            UnityEngine.Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
        }
    }
}