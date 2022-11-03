using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class Gizmo_EnergyShieldStatus : Gizmo
{
    private static readonly Texture2D FullShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.4f, 0.6f));

    private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    public EnergyShield shield;

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
            var fillPercent = shield.Energy / Mathf.Max(1f, shield.EnergyMax);
            Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(
                label: $"{shield.Energy * 100f:F0} / {shield.EnergyMax * 100f:F0}",
                rect: rect3);
            Text.Anchor = TextAnchor.UpperLeft;
        });
        return new GizmoResult(GizmoState.Clear);
    }
}