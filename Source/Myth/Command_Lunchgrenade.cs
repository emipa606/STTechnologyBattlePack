using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Myth
{
    public class Command_Lunchgrenade : Command
    {
        public Action<LocalTargetInfo> action;
        public Action mouseOverCallback;

        public TargetingParameters targetingParams;

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            Find.Targeter.BeginTargeting(targetingParams, delegate(LocalTargetInfo target) { action(target); });
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (Mouse.IsOver(new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f)) && mouseOverCallback != null)
            {
                mouseOverCallback();
            }

            return base.GizmoOnGUI(topLeft, maxWidth, parms);
        }
    }
}