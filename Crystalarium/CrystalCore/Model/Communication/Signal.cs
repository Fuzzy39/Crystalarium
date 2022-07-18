using CrystalCore.Model.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{
    internal abstract class Signal : ChunkMember
    {

        protected Port _start;
        protected Port _end;

        private int _value;

        public int Value { get { return _value; } }
        public Port Start { get { return _start; } }
        public Port End { get { return _end; } }


        public Signal(Grid g, Port transmitter, int value) : base(g, new Rectangle(transmitter.Location, new Point(1)))
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
