using CrystalCore.Model.Elements;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{


    internal class Connection : ChunkMember
    {

        private Port portA;
        private Port portB;

        private int fromA; // the value of this signal from a to b
        private int fromB; // the value of this signal from b to a.

        private int _maxLength = 0;
        private int _minLength = 1;
        private int _length = 0;

        public int Length { get => _length; }

        internal Port Start
        {
            get
            {
                if (PortA == null)
                {
                    return PortB;
                }

                return PortA;
            }
        }

        internal Port End
        {
            get
            {
                if (PortA == Start)
                {
                    return PortB;
                }

                return PortA;
            }
        }

        // how long can this beam get? 0 or lower means limitless.
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }

            set
            {
                if (value <= 0)
                {
                    _maxLength = 0;
                    return;
                }

                if (value < _minLength)
                {
                    throw new InvalidOperationException("Maximum beam length cannot be less than minimum.");
                }

                _maxLength = value;

            }

        }

        public int MinLength
        {
            get { return _minLength; }

            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException("Maximum beam length cannot be less than one.");
                }


                if (_maxLength != 0 & value > _maxLength)
                {
                    throw new InvalidOperationException("Minimum beam length cannot be greater than minimum.");
                }

                _minLength = value;

            }

        } // how short can it be?


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




        public Connection(Map m, Port transmitter) : base(m, new Rectangle(transmitter.Location, new Point(1)))
        {

           
            fromA = 0;
            fromB = 0;

            Connect(transmitter);


            MaxLength = m.Ruleset.SignalMaxLength;
            MinLength = m.Ruleset.SignalMinLength;
            _length = 0;

            Update(); 

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
            Connection s = (Connection)o;
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




        /*#########################
         *  BEAM METHODS
         * ############################
         */

        public void Update()
        {
            // we start looking for targets by getting the point where our min

            if (Destroyed)
            {
                throw new InvalidOperationException("This signal should be destroyed. Is it? " + Destroyed + "! It should not be updated, but it was.");
            }

            // we found all connections. no need to update.
            if (Start != null
               && End != null
               && !Start.Destroyed
               && !End.Destroyed)
            {

                return;
            }

            Point start = (PortA == null) ? PortB.Location : PortA.Location;
            Point? p = Travel(start, MinLength);
            if (p == null)
            {
                _length = 1;
                return;
            }
            int length = MinLength;
            Point end = (Point)p;

            Agent target = FindTarget(length, ref end);
            TransmitTo(target, end);

        }

        private Agent FindTarget(int length, ref Point end)
        {
            // start looking for targets, one tile at a time.
            Point? nextEnd = end;
            while (nextEnd != null)
            {
                end = (Point)nextEnd;
                Agent target = Map.AgentAt(end);

                // We found a target!
                if (target != null)
                {


                    // this cast is safe, if we exist, port agents must.
                    _length = length;
                    return target;

                }

                // If we have a max length, have we reached it?
                if (MaxLength != 0 & length == MaxLength)
                {
                    break;
                }

                // otherwise, get a bit longer.
                length++;
                nextEnd = Travel(end, 1);
            }

            // at this point, we have either reached our max length, or hit the end of the grid without finding a target.
            // we should update our length and bounds to reflect that.
            _length = length;
            return null;
        }

        private void SetBounds(Point start, Point end)
        {
            Rectangle r = Util.Util.RectFromPoints(start, end);
            r.Width++;
            r.Height++;
            Bounds = r;
        }

        private void TransmitTo(Agent target, Point loc)
        {
            if (target != null)
            {
                TransmitToTarget(target, loc);
            }



            // we need to adjust our bounds now.
            SetBounds(Start.Location, loc);

        }


        private void TransmitToTarget(Agent target, Point loc)
        {


            CompassPoint portFacing = Start.AbsoluteFacing.Opposite();

            Port p = FindPort(target, loc, portFacing);

            if (p == End)
            {
                // oh, nothing changed. Alright, neat.
                return;
            }

            p.Connect(this);





        }


        private Point? Travel(Point start, int distance)
        {

            Point toReturn = start;
            for (int i = 0; i < distance; i++)
            {
                Point p = toReturn + Start.AbsoluteFacing.ToPoint();
                if (!Map.Bounds.Contains(p))
                {
                    // this is the end of the road for us.
                    return null;
                }
                toReturn = p;
            }

            return toReturn;

        }


    }
}
