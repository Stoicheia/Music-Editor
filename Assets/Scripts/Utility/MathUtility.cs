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
    }
}