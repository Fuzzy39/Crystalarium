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

        private Port portA;
        private Port portB;

        private int fromA; // the value of this signal from a to b
        private int fromB; // the value of this signal from b to a.

        internal Port PortA
        {
            get { return portA; }
        }

        internal Port PortB
        {
            get { return portB; }
        }

        internal int FromA
        {
            get { return fromA; }
        }

        internal int FromB
        {
            get { return fromB; }
        }




        public Signal(Map g, Port transmitter) : base(g, new Rectangle(transmitter.Location, new Point(1)))
        {

           
            fromA = 0;
            fromB = 0;

            Connect(transmitter);
            
      
            //Update(); // subclasses need to call this method after their constructor has ran.
        }


        public override void Destroy()
        {

            if(PortA != null)
            {
                PortA.Disconnect(); // this will only be called when there is another signal ready to displace us. therefore, it is impossible for a port to have no connection this way.
                PortA.OnDestroy -= OnPortDestroyed;
                PortA.OnConnect -= OnSignalConnectedToOwnPort;
            }
            if(PortB != null)
            {
                portB.Disconnect();

                PortB.OnDestroy -= OnPortDestroyed;
                PortB.OnConnect -= OnSignalConnectedToOwnPort;

            }




            base.Destroy();
        }

        private void OnPortDestroyed(object o, EventArgs e)
        {
            if(Destroyed)
            {
                return;
            }

            if (portA != null && portA.Destroyed)
            {
                portA = null;
            }
            else
            {
                portB = null;
            }

            if(portA==null && portB == null)
            {
                this.Destroy();
                return;
            }

            Console.WriteLine("I'm still alive! " + this);
            //Update();
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
        
        internal virtual void Connect(Port p)
        {
            if (portA == null)
            {
                portA = p;


            }
            else
            {
                portB = p;


            }

           
            p.OnDestroy += OnPortDestroyed;
            p.OnConnect += OnSignalConnectedToOwnPort;

            if(portA == portB)
            {
                throw new InvalidOperationException("yeah, no, a port should not be on both ends of a beam");
            }
        }

        public void OnSignalConnectedToOwnPort(object o, EventArgs e)
        {
            Signal s = (Signal)o;
            if(s!=this)
            {
                // we have been surplanted.
                this.Destroy();
            }
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
            if(p == null)
            {
                throw new ArgumentNullException("Can't transmit from a null port.");
            }

            if(p==portA)
            {
                fromA = value;
                if (portB != null)
                {
                    portB.ValueChange();
                }
                return;
            }

            if(p == portB)
            {
                fromB = value;
                if (portA != null)
                {
                    portA.ValueChange();
                }
                return;
            }

            throw new ArgumentException(p + " is not connected to signal " + this + " it cannot transmit a value");

        }

        public override string ToString()
        {
            return "Signal: { A:" + (portA==null?"null":PortA.ToString()) + " B: " + (portB == null ? "null" : portB.ToString()) + " Bounds:" + Bounds + "}";

        }

    }
}
