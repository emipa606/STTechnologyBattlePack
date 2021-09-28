using UnityEngine;
using Verse;

namespace Myth
{
    [StaticConstructorOnStartup]
    internal class Gizmo_MassShield : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public MassShield shield;

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
                var fillPercent = shield.point / Mathf.Max(1f, shield.pointmax);
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(
                    label: string.Concat(str2: (shield.pointmax * 10f).ToString("F0"),
                        str0: (shield.point * 10f).ToString("F0"), str1: " / "), rect: rect3);
                Text.Anchor = TextAnchor.UpperLeft;
            });
            return new GizmoResult(GizmoState.Clear);
        }
    }
}