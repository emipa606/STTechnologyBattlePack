using UnityEngine;
using Verse;

namespace Myth;

[StaticConstructorOnStartup]
internal class Gizmo_GrenadeStatus : Gizmo
{
    private static readonly Texture2D FullShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.8f, 0.24f));

    private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    public GrenadeBelt grenade;

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
            Widgets.Label(rect, grenade.LabelCap);
            var rect3 = rect2;
            rect3.yMin = overRect.height / 2f;
            Widgets.FillableBar(
                fillPercent: grenade.Wearer == null
                    ? grenade.ammo / 1f
                    : grenade.ammo / Mathf.Max(1f, grenade.ammomax), rect: rect3, fillTex: FullShieldBarTex,
                bgTex: EmptyShieldBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect3,
                grenade is { Wearer: not null }
                    ? $"{grenade.ammo:F0} / {grenade.ammomax:F0}"
                    : $"{grenade.ammo:F0} / 0");

            Text.Anchor = TextAnchor.UpperLeft;
        });
        return new GizmoResult(GizmoState.Clear);
    }
}