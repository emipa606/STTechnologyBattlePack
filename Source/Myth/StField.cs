using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Myth
{
    [StaticConstructorOnStartup]
    internal class StField : Apparel
    {
        private static readonly Material ShieldSparksMat =
            MaterialPool.MatFrom("Things/Projectile/field", MatBases.LightOverlay);

        public static Texture2D UIOFF = ContentFinder<Texture2D>.Get("UI/FieldOFF");

        public static Texture2D UION = ContentFinder<Texture2D>.Get("UI/FieldON");

        private HediffDef buff;
        private float currentAngle = Random.Range(0f, 360f);

        public bool forEnemy;

        private bool isactive;

        public float point;

        public float pointmaxo;

        private float range;

        private float recoverypointpercheck;

        private float tick;

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

        public override void Tick()
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
            if (isactive && Wearer != null)
            {
                if (Wearer.Dead || Wearer.Downed)
                {
                    isactive = false;
                    return;
                }

                CheckAndGive();
                point -= 1f;
                if (!(point <= 0f))
                {
                    return;
                }

                point = 0f;
                PowerDown();
            }
            else if (!isactive && Wearer != null)
            {
                point += recoverypointpercheck;
                if (point > pointmax)
                {
                    point = pointmax;
                }
            }
        }

        private void PowerDown()
        {
            MoteMaker.ThrowText(new Vector3(Wearer.Position.x + 1f, Wearer.Position.y, Wearer.Position.z + 1f),
                Wearer.Map, "能量耗尽".Translate(), Color.blue);
            isactive = false;
        }

        public bool CheckAndGive()
        {
            var allPawnsSpawned = Wearer.Map.mapPawns.AllPawnsSpawned;
            if (allPawnsSpawned == null)
            {
                return false;
            }

            var list = new List<Pawn>();
            if (forEnemy)
            {
                for (var i = 0; i < allPawnsSpawned.Count; i++)
                {
                    if (allPawnsSpawned[i].Position.InHorDistOf(Wearer.Position, range) &&
                        allPawnsSpawned[i].Faction is { IsPlayer: false })
                    {
                        list.Add(allPawnsSpawned[i]);
                    }
                }
            }
            else
            {
                for (var j = 0; j < allPawnsSpawned.Count; j++)
                {
                    if (allPawnsSpawned[j].Position.InHorDistOf(Wearer.Position, range) &&
                        allPawnsSpawned[j].Faction is { IsPlayer: true })
                    {
                        list.Add(allPawnsSpawned[j]);
                    }
                }
            }

            for (var k = 0; k < list.Count; k++)
            {
                var pawn = list[k];
                if (pawn is { Dead: false })
                {
                    HealthUtility.AdjustSeverity(pawn, buff, 0.1f);
                }
            }

            return false;
        }

        private void Tswitch()
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
                    action = Tswitch,
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
                    action = Tswitch,
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
            DrawRangeOverlay();
        }

        private void DrawRangeOverlay()
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
}