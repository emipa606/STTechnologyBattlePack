using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Myth;

[StaticConstructorOnStartup]
public class Thinkingshield : Apparel
{
    private const float MinDrawSize = 1.2f;

    private const float MaxDrawSize = 1.55f;

    private const float MaxDamagedJitterDist = 0.05f;

    private const int JitterDurationTicks = 8;

    private static readonly Material BubbleMat =
        MaterialPool.MatFrom("Things/Projectile/tKshield", ShaderDatabase.Transparent);

    private readonly float ApparelScorePerEnergyMax = 0.25f;

    private readonly float EnergyLossPerDamage = 2f;

    private readonly int KeepDisplayingTicks = 1000;

    private readonly int lastKeepDisplayTick = -9999;

    private readonly int StartingTicksToReset = 3200;

    private float EnergyOnReset;

    private Vector3 impactAngleVect;

    private int lastAbsorbDamageTick = -9999;

    private int ticksToReset = -1;

    public float EnergyMax
    {
        get
        {
            if (Wearer != null)
            {
                return (Wearer.skills.GetSkill(SkillDefOf.Intellectual).Level * 10) +
                       (this.GetStatValue(StatDefOf.EnergyShieldEnergyMax) * 100f);
            }

            return 0f;
        }
    }

    private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate) / 3f;

    public float Energy { get; private set; }

    public ShieldState ShieldState => ticksToReset > 0 ? ShieldState.Resetting : ShieldState.Active;

    private bool ShouldDisplay
    {
        get
        {
            if (Wearer.Dead || Wearer.Downed || Wearer.IsPrisonerOfColony &&
                Wearer.MentalStateDef is not { IsAggro: true })
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
        yield return new Gizmo_MINDShieldStatus
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
        if ((dinfo.Instigator == null || dinfo.Instigator.Position.AdjacentTo8WayOrInside(Wearer.Position)) &&
            !dinfo.Def.isExplosive)
        {
            return false;
        }

        if (dinfo.Instigator is AttachableThing attachableThing && attachableThing.parent == Wearer)
        {
            return false;
        }

        if (dinfo.Def.isExplosive)
        {
            Energy -= dinfo.Amount * 5f * EnergyLossPerDamage;
        }
        else
        {
            Energy -= dinfo.Amount * EnergyLossPerDamage;
        }

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
        if (Wearer.skills.GetSkill(SkillDefOf.Intellectual) == null)
        {
            return;
        }

        var max = GenRadial.NumCellsInRadius((20.3f - Wearer.skills.GetSkill(SkillDefOf.Intellectual).Level) *
                                             0.5f);
        var num = Rand.Range(0, max);
        if (dinfo.Instigator != null)
        {
            switch (dinfo.Instigator)
            {
                case Pawn:
                {
                    if (dinfo.Weapon is { Verbs: not null } && dinfo.Weapon.Verbs.Count != 0)
                    {
                        var intVec = dinfo.Instigator.Position + GenRadial.RadialPattern[num];
                        var projectile = (Projectile)GenSpawn.Spawn(dinfo.Weapon.Verbs[0].defaultProjectile, intVec,
                            dinfo.Instigator.Map);
                        projectile.Launch(Wearer, Wearer.Position.ToVector3(), new LocalTargetInfo(intVec),
                            dinfo.Instigator, ProjectileHitFlags.All, false, this);
                    }

                    break;
                }
                case Building building:
                {
                    var intVec2 = dinfo.Instigator.Position + GenRadial.RadialPattern[num];
                    if (building.def.building.turretGunDef != null &&
                        building.def.building.turretGunDef.Verbs.Count != 0)
                    {
                        var projectile2 = (Projectile)GenSpawn.Spawn(
                            building.def.building.turretGunDef.Verbs[0].defaultProjectile, intVec2,
                            dinfo.Instigator.Map);
                        projectile2.Launch(Wearer, Wearer.Position.ToVector3(), new LocalTargetInfo(intVec2),
                            dinfo.Instigator, ProjectileHitFlags.All, false, this);
                    }

                    break;
                }
            }
        }

        SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
        var vector = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
        var num2 = (int)Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
        for (var i = 0; i < num2; i++)
        {
            FleckMaker.ThrowDustPuff(vector, Wearer.Map, Rand.Range(0.8f, 1.2f));
        }

        lastAbsorbDamageTick = Find.TickManager.TicksGame;
    }

    private void Break()
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
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