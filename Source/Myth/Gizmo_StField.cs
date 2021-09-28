using UnityEngine;
using Verse;

namespace Myth
{
    [StaticConstructorOnStartup]
    internal class Gizmo_StField : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public StField field;

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
                Widgets.Label(rect, field.LabelCap);
                var rect3 = rect2;
                rect3.yMin = overRect.height / 2f;
                var fillPercent = field.point / Mathf.Max(1f, field.pointmax);
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3,
                    (field.point * 10f).ToString("F0") + " / " + (field.pointmax * 10f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            });
            return new GizmoResult(GizmoState.Clear);
        }
    }
}