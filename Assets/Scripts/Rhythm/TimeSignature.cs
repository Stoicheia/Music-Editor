using System;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public class TimeSignature
    {
        public int Numerator => _numerator;
        public int Denominator => _denominator;
        [SerializeField] private int _numerator;
        [SerializeField] private int _denominator;

        public TimeSignature(int a, int b)
        {
            _numerator = a;
            _denominator = b;
        }

        public int BeatsInABar()
        {
            return _numerator;
        }
    }
}