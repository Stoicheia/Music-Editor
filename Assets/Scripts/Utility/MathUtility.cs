using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utility
{
    public static class MathUtility
    {
        public static float BeatsToSeconds(float beats, float bpm)
        {
            return beats * 60 / bpm;
        }
        
        public static float SecondsToBeats(float seconds, float bpm)
        {
            return seconds * bpm / 60;
        }

        public static float InverseLerpUnclamped(float left, float right, float value)
        {
            return (value - left) / (right - left);
        }

        public static (float, bool) PositionToTime(Vector2 pos, Transform rel, Rect panel, 
            float leftTime, float rightTime, [CanBeNull] Func<float, float> snapFunc, bool clamp, bool forceValid)
        {
            Func<float, float, float, float> floatLerpFunc = clamp ? Mathf.Lerp : Mathf.LerpUnclamped;
            Func<float, float, float, float> floatInvLerpFunc = clamp ? Mathf.InverseLerp : InverseLerpUnclamped;
            
            float time;
            bool valid = true;

            Vector2 panelLeft = new Vector2(panel.xMin, panel.y);
            Vector2 panelRight = new Vector2(panel.xMax, panel.y);

            Vector2 anchoredMousePos = rel.transform.InverseTransformPoint(pos);

            if (!panel.Contains(anchoredMousePos))
            {
                valid = forceValid;
            }
            float relPos = floatInvLerpFunc(panelLeft.x, panelRight.x, anchoredMousePos.x);
            time = floatLerpFunc(leftTime, rightTime, relPos);

            bool snapToGrid = snapFunc != null;
            if (snapToGrid) time = snapFunc(time);

            return (time, valid);
        }
    }
}