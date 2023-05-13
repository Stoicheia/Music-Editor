using System;
using Rhythm;
using UnityEngine;

namespace UI
{
    public class NodeIntDrawerUI : NodePropertyDrawerUI
    {
        public IntData Data { get; set; }

        protected override void OnRequestValueChange(string s)
        {
            int value;
            int.TryParse(s, out value);
            Data.IntValue = value;
        }
    }
}