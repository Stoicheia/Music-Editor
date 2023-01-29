using UnityEngine;

namespace Utility
{
    public static class StringUtility
    {
        public static string SecondsPrettyString(float s)
        {
            return $"{Mathf.Floor(s/60):F0}:{s%60:00}";
        }
    }
}