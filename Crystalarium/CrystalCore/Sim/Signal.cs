using CrystalCore.Sim.Base;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim
{
    internal abstract class Signal : ChunkMember
    {

        private Port _start;
        private Port _end;

        private int _value;

        public int Value { get { return _value; } }
        public Port Start { get { return _start; } }
        public Port End { get { return _end; } }


        public Signal(Grid g, Point location, Port transmitter, int value) : base(g, new Rectangle(location, new Point(1)))
        {

            _start = transmitter;
            _value = value; 
        }

        public override void Destroy()
        {
            _start.Stop();
            _end.Stop();
            base.Destroy();

        }

        public abstract void Update();

       


    }
}
