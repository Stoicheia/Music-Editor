using UnityEngine;

namespace Utility
{
    public struct BarBeatSubdiv
    {
        public int Bar;
        public int Beat;
        public float Subdiv;

        public int BeatsPerBar;

        public BarBeatSubdiv(int bar, int beat, float subdiv, int beatsPerBar)
        {
            Bar = bar;
            Beat = beat;
            Subdiv = subdiv;
            BeatsPerBar = beatsPerBar;
            Recount();
        }

        public BarBeatSubdiv Add(int bar, int beat, float subdiv)
        {
            Bar += bar;
            Beat += beat;
            Subdiv += subdiv;
            
            Recount();
            return this;
        }

        public BarBeatSubdiv NextBar()
        {
            Bar++;
            Beat = 1;
            Subdiv = 0;
            
            Recount();
            return this;
        }
        
        public BarBeatSubdiv NewBar()
        {
            Bar = 1;
            Beat = 1;
            Subdiv = 0;
            
            Recount();
            return this;
        }

        public BarBeatSubdiv ChangeBeatsPerBar(int bpb)
        {
            BeatsPerBar = bpb;
            Recount();
            return this;
        }

        public static BarBeatSubdiv Beginning(int beatsPerBar)
        {
            return new BarBeatSubdiv(1, 1, 0, beatsPerBar);
        }

        private void Recount()
        {
            Beat += Mathf.FloorToInt(Subdiv);
            Subdiv -= Mathf.FloorToInt(Subdiv);

            while (Beat > BeatsPerBar)
            {
                Beat -= BeatsPerBar;
                Bar++;
            }
        }
    }
}