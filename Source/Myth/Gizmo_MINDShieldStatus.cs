using RimWorld;
using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class Gizmo_MINDShieldStatus : Gizmo
{
    private static readonly Texture2D FullShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.8f));

    private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    public Thinkingshield shield;

    public override float GetWidth(float maxWidth)
    {
        return 140f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
        {
            Rect rect;
            var rect2 = rect = overRect.AtZero().ContractedBy(6f);
            rect.height = overRect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect, shield.LabelCap);
            var rect3 = rect2;
            rect3.yMin = overRect.height / 2f;
            Widgets.FillableBar(
                fillPercent: shield.Wearer == null
                    ? shield.Energy / 1f
                    : shield.Energy / Mathf.Max(1f, shield.EnergyMax), rect: rect3, fillTex: FullShieldBarTex,
                bgTex: EmptyShieldBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (shield is { Wearer: { } })
            {
                Widgets.Label(rect3,
                    $"{shield.Energy:F0} / {(shield.Wearer.skills.GetSkill(SkillDefOf.Intellectual).Level * 10f) + 100f:F0}");
            }
            else
            {
                Widgets.Label(rect3, $"{shield.Energy:F0} / 0");
            }

            Text.Anchor = TextAnchor.UpperLeft;
        });
        return new GizmoResult(GizmoState.Clear);
    }
}