using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Myth;

[StaticConstructorOnStartup]
internal class MassShield : Apparel
{
    private static readonly Material ShieldSparksMat =
        MaterialPool.MatFrom("Things/Projectile/massshield", MatBases.LightOverlay);

    private static readonly Texture2D SWITCH = ContentFinder<Texture2D>.Get("UI/MassShieldSwitch");

    private List<IntVec3> cellstoprotect;
    private float currentAngle = Random.Range(0f, 360f);

    private bool isactive;

    public float point;

    private float pointmaxo;

    private float pointperdamage;

    private float range;

    private float restpertick;

    private int restticks;

    private int shieldstate;

    private long TickCount;

    private float ticktoblock = 2f;

    private float ticktorest = -1f;

    private float transparency = Random.Range(10f, 90f);

    private bool Visiblerange;

    public float pointmax
    {
        get
        {
            if (pointmaxo != 0f || Wearer == null)
            {
                return pointmaxo;
            }

            var comp = GetComp<Comp_MassShield>();
            if (comp.props is CompProperties_MassShield shield)
            {
                ticktoblock = shield.ticktoblock;
                pointperdamage = shield.pointperdamage;
                restticks = shield.restticks;
                pointmaxo = shield.pointmax;
                restpertick = shield.restpertick;
                range = shield.range;
            }
            else
            {
                Log.Error("shield definition not of MassShield");
            }

            return pointmaxo;
        }
    }

    public override void SpawnSetup(Map map, bool RE)
    {
        base.SpawnSetup(map, RE);
        var comp = GetComp<Comp_MassShield>();
        if (comp.props is CompProperties_MassShield shield)
        {
            ticktoblock = shield.ticktoblock;
            pointperdamage = shield.pointperdamage;
            restticks = shield.restticks;
            pointmaxo = shield.pointmax;
            restpertick = shield.restpertick;
            range = shield.range;
        }
        else
        {
            Log.Error("shield definition not of MassShield");
        }
    }

    protected override void Tick()
    {
        base.Tick();
        TickCount++;
        if (Wearer == null)
        {
            point = 0f;
            TickCount = 0L;
        }
        else if (shieldstate == 1)
        {
            ticktorest -= 1f;
            if (ticktorest <= 0f)
            {
                reset();
            }
        }
        else
        {
            if (shieldstate != 0)
            {
                return;
            }

            point = restpertick + point;
            if (point > pointmax)
            {
                point = pointmax;
            }

            if (!isactive)
            {
                return;
            }

            if (TickCount is >= 100 or 0L)
            {
                TickCount = 1L;
                reCalibrateCells();
            }

            if (TickCount % ticktoblock != 0f)
            {
                return;
            }

            if (Wearer.Dead || Wearer.Downed)
            {
                isactive = false;
            }
            else
            {
                tickProtection();
            }
        }
    }

    private void tickProtection()
    {
        if (cellstoprotect == null)
        {
            return;
        }

        foreach (var item in cellstoprotect)
        {
            ProtectSquare(item);
        }

        if (point <= 0f)
        {
            breakShield();
        }
    }

    private void reCalibrateCells()
    {
        cellstoprotect = [];
        foreach (var item in GenRadial.RadialCellsAround(Wearer.Position, range, false))
        {
            if (Vectors.EuclDist(item, Wearer.Position) >= range - 1.5f)
            {
                cellstoprotect.Add(item);
            }
        }
    }

    protected virtual void ProtectSquare(IntVec3 square)
    {
        if (!square.InBounds(Wearer.Map))
        {
            return;
        }

        var list = Wearer.Map.thingGrid.ThingsListAt(square);
        var list2 = new List<Thing>();
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

            var shouldReturn = true;
            var thing = ReflectionHelper.GetInstanceField(typeof(Projectile), projectile, "launcher") as Thing;
            if (thing is { Faction.IsPlayer: true })
            {
                shouldReturn = false;
            }

            if (projectile.def.projectile.flyOverhead && !willTargetLandInRange(projectile))
            {
                shouldReturn = false;
            }

            if (!shouldReturn)
            {
                continue;
            }

            var exactRotation = projectile.ExactRotation;
            var exactPosition = projectile.ExactPosition;
            exactPosition.y = 0f;
            var vector = Vectors.IntVecToVec(Wearer.Position);
            vector.y = 0f;
            var b = Quaternion.LookRotation(exactPosition - vector);
            if (!(Quaternion.Angle(exactRotation, b) > 90f))
            {
                continue;
            }

            FleckMaker.ThrowLightningGlow(projectile.ExactPosition, Wearer.Map, 0.5f);
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            processDamage(projectile.def.projectile.GetDamageAmount(thing));
            list2.Add(projectile);
        }

        foreach (var item in list2)
        {
            item.Destroy();
        }
    }

    private void breakShield()
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
        FleckMaker.ThrowDustPuff(
            Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) *
                                   Rand.Range(0.3f, 0.6f)), Wearer.Map, Rand.Range(0.8f, 1.2f));
        ticktorest = restticks;
        shieldstate = 1;
        isactive = false;
    }

    private void processDamage(int damage)
    {
        if (point <= 0f)
        {
            return;
        }

        if ((int)(damage * pointperdamage) <= point)
        {
            point -= (int)(damage * pointperdamage);
        }
        else
        {
            point = 0f;
        }
    }

    private bool willTargetLandInRange(Projectile projectile)
    {
        var targetLocationFromProjectile = getTargetLocationFromProjectile(projectile);
        return !(Vector3.Distance(Wearer.Position.ToVector3(), targetLocationFromProjectile) > range);
    }

    private static Vector3 getTargetLocationFromProjectile(Projectile projectile)
    {
        return (Vector3)projectile.GetType()
            .GetField("destination", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(projectile)!;
    }

    private void reset()
    {
        if (Wearer.Spawned)
        {
            SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            FleckMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
        }

        ticktorest = -1f;
        shieldstate = 0;
        point = pointmax * 0.1f;
    }

    private void tswitch()
    {
        if (isactive)
        {
            isactive = false;
        }
        else if (shieldstate == 1)
        {
            MoteMaker.ThrowText(new Vector3(Wearer.Position.x + 1f, Wearer.Position.y, Wearer.Position.z + 1f),
                Wearer.Map, "护盾未充能就绪".Translate(), Color.blue);
        }
        else
        {
            isactive = true;
        }
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        yield return new Command_MouseOver
        {
            action = tswitch,
            mouseOverCallback = OnMouseOverGizmo,
            icon = SWITCH,
            defaultLabel = "ON/OFF".Translate(),
            defaultDesc = "开启/关闭护盾立场".Translate()
        };
        yield return new Gizmo_MassShield
        {
            shield = this
        };
    }

    public override void DrawWornExtras()
    {
        base.DrawWornExtras();
        drawRangeOverlay();
    }

    private void drawRangeOverlay()
    {
        if (isactive)
        {
            var matrix = default(Matrix4x4);
            matrix.SetTRS(s: new Vector3(range, 1f, range), pos: Wearer.DrawPos + Altitudes.AltIncVect,
                q: Quaternion.Euler(0f, currentAngle, 0f));
            UnityEngine.Graphics.DrawMesh(MeshPool.plane20, matrix,
                FadedMaterialPool.FadedVersionOf(ShieldSparksMat, 0.9f), 0);
            currentAngle = Random.Range(0f, 360f);
            transparency = Random.Range(10f, 90f);
        }

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

    private void OnMouseOverGizmo()
    {
        Visiblerange = true;
    }
}