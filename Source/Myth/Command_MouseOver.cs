using System;
using UnityEngine;
using Verse;

namespace Myth;

public class Command_MouseOver : Command_Action
{
    public Action mouseOverCallback;

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        if (Mouse.IsOver(new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f)) && mouseOverCallback != null)
        {
            mouseOverCallback();
        }

        return base.GizmoOnGUI(topLeft, maxWidth, parms);
    }
}