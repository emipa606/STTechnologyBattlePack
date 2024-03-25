using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
public class Building_Humidifier : Building
{
    private static readonly Material ShieldSparksMat =
        MaterialPool.MatFrom("Things/Projectile/ShieldSparks", MatBases.LightOverlay);

    public static readonly Texture2D UION = ContentFinder<Texture2D>.Get("UI/ON");

    public static readonly Texture2D UIOFF = ContentFinder<Texture2D>.Get("UI/OFF");

    private float areasize;

    private float areasize1;

    private float cblue;

    private float cgreen;

    private float cred;

    private float currentAngle;

    public Material currentMatrialColour;

    public Material currentMoteColour;

    private float currentSize = 0.5f;

    private bool currentSizeflag = true;

    private HediffDef hediffDef;

    private bool isshow;

    private CompPowerTrader PowerValue;

    private float radius;

    public override void SpawnSetup(Map map, bool RE)
    {
        PowerValue = GetComp<CompPowerTrader>();
        base.SpawnSetup(map, RE);
        var comp = GetComp<Comp_HumidifierBuilding>();
        if (comp.props is CompProperties_HumidifierBuilding building)
        {
            hediffDef = building.hediffDef;
            radius = building.radius;
            cred = building.cred;
            cgreen = building.cgreen;
            cblue = building.cblue;
        }
        else
        {
            Log.Error("HumidifierBuilding definition not of type \"HumidifierBuildingThingDef\"");
        }
    }

    public void showa()
    {
        isshow = false;
    }

    public void showb()
    {
        isshow = true;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        var list = new List<Gizmo>();
        var command_Toggle = new Command_Toggle();
        if (isshow)
        {
            command_Toggle.icon = UIOFF;
            command_Toggle.isActive = () => isshow;
            command_Toggle.defaultDesc = "关闭";
            command_Toggle.defaultLabel = "视觉效果";
            command_Toggle.toggleAction = showa;
        }
        else
        {
            command_Toggle.icon = UION;
            command_Toggle.isActive = () => isshow;
            command_Toggle.defaultDesc = "打开";
            command_Toggle.defaultLabel = "视觉效果";
            command_Toggle.toggleAction = showb;
        }

        command_Toggle.activateSound = SoundDefOf.Click;
        command_Toggle.groupKey = 88234441;
        list.Add(command_Toggle);
        var gizmos = base.GetGizmos();
        return gizmos != null ? list.AsEnumerable().Concat(gizmos) : list.AsEnumerable();
    }

    public bool IsPowerOn()
    {
        return PowerValue is { PowerOn: true };
    }

    public override void Tick()
    {
        base.Tick();
        if (!IsPowerOn())
        {
            return;
        }

        if (areasize1 == 90f)
        {
            areasize1 = 0f;
            areasize += 0.08f;
        }

        areasize1 += 1f;
        if (isshow)
        {
            DrawSubField(Vectors.IntVecToVec(Position), areasize, 0.02f);
            DrawSubField(Vectors.IntVecToVec(Position), radius, 0.07f);
            if (areasize > radius)
            {
                areasize = 0f;
            }
        }

        if (areasize1 % 30f == 0f)
        {
            DrawCenter(Vectors.IntVecToVec(Position));
        }

        if (areasize1 % 80f == 0f)
        {
            CheckAndGive();
        }
    }

    public bool CheckAndGive()
    {
        var allPawnsSpawned = Map.mapPawns.AllPawnsSpawned;
        if (allPawnsSpawned == null)
        {
            return false;
        }

        var list = new List<Pawn>();
        foreach (var pawn in allPawnsSpawned)
        {
            if (pawn.Position.InHorDistOf(Position, radius))
            {
                list.Add(pawn);
            }
        }

        foreach (var pawn in list)
        {
            if (pawn is { Dead: false })
            {
                HealthUtility.AdjustSeverity(pawn, hediffDef, 0.1f);
            }
        }

        return false;
    }

    public void DrawCenter(Vector3 position)
    {
        if (currentSize > 0.5)
        {
            currentSizeflag = true;
        }

        if (currentSize <= 0.3)
        {
            currentSizeflag = false;
        }

        if (areasize1 % 3f == 0f)
        {
            if (currentSizeflag)
            {
                currentSize -= 0.02f;
            }
            else
            {
                currentSize += 0.02f;
            }
        }

        position += new Vector3(0.5f, 11f, 0.5f);
        var s = new Vector3(currentSize, 0f, currentSize);
        var matrix = default(Matrix4x4);
        currentAngle += 1f;
        matrix.SetTRS(position, Quaternion.Euler(0.5f, currentAngle, 0f), s);
        UnityEngine.Graphics.DrawMesh(MeshPool.plane20, matrix,
            FadedMaterialPool.FadedVersionOf(ShieldSparksMat, 1f), 0);
    }

    public void DrawSubField(Vector3 position, float Radius, float dreeg)
    {
        position += new Vector3(0.5f, 0f, 0.5f);
        var s = new Vector3(Radius, 1f, Radius);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(position, Quaternion.identity, s);
        if (currentMatrialColour == null)
        {
            currentMatrialColour = SolidColorMaterials.NewSolidColorMaterial(new Color(cred, cgreen, cblue, dreeg),
                ShaderDatabase.MetaOverlay);
        }

        UnityEngine.Graphics.DrawMesh(Graphics.CircleMesh, matrix, currentMatrialColour, 0);
    }
}