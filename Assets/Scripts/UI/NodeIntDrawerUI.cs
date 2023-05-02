using System;
using Rhythm;
using UnityEngine;

namespace UI
{
    public class NodeIntDrawerUI : NodePropertyDrawerUI
    {
        public IntData Data { get; set; }
        private void OnValidate()
        {
            throw new NotImplementedException();
        }

        public override void OnRequestValueChange(string s)
        {
            int value;
            int.TryParse(s, out value);
            Data.IntValue = value;
        }
    }
}