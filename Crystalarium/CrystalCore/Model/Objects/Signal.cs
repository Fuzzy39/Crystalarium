using CrystalCore.Model.Elements;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{


    internal abstract class Signal : ChunkMember
    {

        protected Port portA;
        protected Port portB;

        private int fromA; // the value of this signal from a to b
        private int fromB; // the value of this signal from b to a.






        public Signal(Map g, Port transmitter) : base(g, new Rectangle(transmitter.Location, new Point(1)))
        {

            portA = transmitter;
            fromA = 0;
            fromB = 0;

            portA.OnDestroy += OnPortDestroyed;
            portA.SetupConnection(this);
      
            //Update(); // subclasses need to call this method after their constructor has ran.
        }

        private void OnPortDestroyed(object o, EventArgs e)
        {
            if (o == portA)
            {
                portA = null;
            }
            else
            {
                portB = null;
            }

            if(portA==null & portB == null)
            {
                this.Destroy();
                return;
            }

            Update();
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


        public int Receive(Port p)
        {
            if(p == portA)
            {
                return fromB;

            }
            
            if(p == portB)
            {
                return fromA;
            }

            throw new ArgumentException(p + " is not connected to signal " + this+" it cannot receive a value");
        }


        public void Transmit(Port p, int value)
        {
            if(p==portA)
            {
                fromA = value;
                portB.ValueChange();
            }

            if(p == portB)
            {
                fromB = value;
                portA.ValueChange();
            }

            throw new ArgumentException(p + " is not connected to signal " + this + " it cannot transmit a value");

        }

        public override string ToString()
        {
            return "Signal: { A:" + portA + " B: " + portB + " Bounds:" + Bounds + "}";

        }

    }
}
