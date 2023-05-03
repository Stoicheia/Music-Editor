using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class StringUtility
    {
        public static readonly List<char> DIGITS = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        public static string SecondsPrettyString(float s)
        {
            return $"{Mathf.Floor(s/60):F0}:{s%60:00}";
        }
    }
}