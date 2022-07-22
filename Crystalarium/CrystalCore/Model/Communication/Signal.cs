using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{

    public enum SignalType
    {
        CASignal
    }

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
            Update();
        }
        
        // this should only be called by Port.StopTransmitting()
        public override void Destroy()
        {
            if (_end != null)
            {
                Console.WriteLine("Signal disconnected by transmitter.");
                _end.StopReceiving();
            }
            
            base.Destroy();

        }

        public abstract void Update();

        protected Port FindPort(Agent a, Point loc, CompassPoint AbsFacing)
        {
            // we need to find a port with the absolute facing matching ours.
            List<Port> potentialMatches = null;
            foreach (List<Port> ports in a.Ports)
            {
                if (ports.Count > 0)
                {
                    if (ports[0].AbsoluteFacing == AbsFacing)
                    {
                        potentialMatches = ports;
                        break;
                    }
                }
            }

            foreach (Port p in potentialMatches)
            {
                if (p.Location.Equals(loc))
                {
                    // hooray! We've succeeded!

                    return p;
                }
            }

            throw new InvalidOperationException("Could not find port facing (Absolute): " + AbsFacing + " on tile " + loc + " in agent " + a + "\nSomething went wrong...");
        }


        public void Reset()
        {
            Console.WriteLine("Signal disconnected by receiver.");

            _end = null;
        }

    }
}
