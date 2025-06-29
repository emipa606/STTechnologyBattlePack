using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class StField : Apparel
{
    private static readonly Material ShieldSparksMat =
        MaterialPool.MatFrom("Things/Projectile/field", MatBases.LightOverlay);

    private static readonly Texture2D UIOFF = ContentFinder<Texture2D>.Get("UI/FieldOFF");

    private static readonly Texture2D UION = ContentFinder<Texture2D>.Get("UI/FieldON");

    private HediffDef buff;
    private float currentAngle = Random.Range(0f, 360f);

    private bool forEnemy;

    private bool isactive;

    public float point;

    private float pointmaxo;

    private float range;

    private float recoverypointpercheck;

    private float tick;

    private float transparency = Random.Range(10f, 90f);

    private bool Visiblerange;

    public float Pointmax
    {
        get
        {
            if (pointmaxo != 0f || Wearer == null)
            {
                return pointmaxo;
            }

            var comp = GetComp<Comp_StField>();
            if (comp.props is CompProperties_StField field)
            {
                buff = field.buff;
                forEnemy = field.forEnemy;
                range = field.range;
                pointmaxo = field.pointmax;
                recoverypointpercheck = field.recoverypointpercheck;
            }
            else
            {
                Log.Error("Field definition not of type \"FieldThingDef\"");
            }

            return pointmaxo;
        }
    }

    public override void SpawnSetup(Map map, bool RE)
    {
        base.SpawnSetup(map, RE);
        var comp = GetComp<Comp_StField>();
        if (comp.props is CompProperties_StField field)
        {
            buff = field.buff;
            forEnemy = field.forEnemy;
            range = field.range;
            pointmaxo = field.pointmax;
            recoverypointpercheck = field.recoverypointpercheck;
        }
        else
        {
            Log.Error("Field definition not of type \"FieldThingDef\"");
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer == null)
        {
            point = 0f;
            return;
        }

        tick += 1f;
        if (!(tick >= 100f))
        {
            return;
        }

        tick = 0f;
        switch (isactive)
        {
            case true when Wearer != null:
            {
                if (Wearer.Dead || Wearer.Downed)
                {
                    isactive = false;
                    return;
                }

                checkAndGive();
                point -= 1f;
                if (!(point <= 0f))
                {
                    return;
                }

                point = 0f;
                powerDown();
                break;
            }
            case false when Wearer != null:
            {
                point += recoverypointpercheck;
                if (point > Pointmax)
                {
                    point = Pointmax;
                }

                break;
            }
        }
    }

    private void powerDown()
    {
        MoteMaker.ThrowText(new Vector3(Wearer.Position.x + 1f, Wearer.Position.y, Wearer.Position.z + 1f),
            Wearer.Map, "能量耗尽".Translate(), Color.blue);
        isactive = false;
    }

    private void checkAndGive()
    {
        var allPawnsSpawned = Wearer.Map.mapPawns.AllPawnsSpawned;
        if (allPawnsSpawned == null)
        {
            return;
        }

        var list = new List<Pawn>();
        if (forEnemy)
        {
            foreach (var pawn in allPawnsSpawned)
            {
                if (pawn.Position.InHorDistOf(Wearer.Position, range) &&
                    pawn.Faction is { IsPlayer: false })
                {
                    list.Add(pawn);
                }
            }
        }
        else
        {
            foreach (var pawn in allPawnsSpawned)
            {
                if (pawn.Position.InHorDistOf(Wearer.Position, range) &&
                    pawn.Faction is { IsPlayer: true })
                {
                    list.Add(pawn);
                }
            }
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var k = 0; k < list.Count; k++)
        {
            var pawn = list[k];
            if (pawn is { Dead: false })
            {
                HealthUtility.AdjustSeverity(pawn, buff, 0.1f);
            }
        }
    }

    private void tswitch()
    {
        if (isactive)
        {
            isactive = false;
        }
        else if (point <= 1f)
        {
            MoteMaker.ThrowText(new Vector3(Wearer.Position.x + 1f, Wearer.Position.y, Wearer.Position.z + 1f),
                Wearer.Map, "立场能量过低无法启动".Translate(), Color.blue);
        }
        else
        {
            isactive = true;
        }
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        if (isactive)
        {
            yield return new Command_MouseOver
            {
                action = tswitch,
                mouseOverCallback = OnMouseOverGizmo,
                icon = UIOFF,
                defaultLabel = "OFF".Translate(),
                defaultDesc = "OFF Gizmo".Translate()
            };
        }
        else
        {
            yield return new Command_MouseOver
            {
                action = tswitch,
                mouseOverCallback = OnMouseOverGizmo,
                icon = UION,
                defaultLabel = "ON".Translate(),
                defaultDesc = "ON Gizmo".Translate()
            };
        }

        yield return new Gizmo_StField
        {
            field = this
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
            transparency = Random.Range(10f, 80f);
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