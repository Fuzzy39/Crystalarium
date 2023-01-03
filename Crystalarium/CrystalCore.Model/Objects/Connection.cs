using CrystalCore.Model.Elements;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{


    public class Connection : ChunkMember
    {

        private Port portA;
        private Port portB;

        private int _length = 0;

        public int Length { get => _length; }

        public Port Start
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

        public Port End
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

        public Port PortA
        {
            get { return portA; }
        }

        public Port PortB
        {
            get { return portB; }
        }

        public int FromA
        {
            get 
            {
                if (portA == null)
                {
                    return 0;
                }

                return portA.TransmittingValue;
            }
        }

        public int FromB
        {
            get 
            {
                if (portB == null)
                {
                    return 0;
                }

                return portB.TransmittingValue;
            }
        }




        public Connection(Map m, Port from, Port to, int length, CompassPoint direction ) 
            : base(m, GetBounds(from, to, length, direction))
        {

           

            portA = from;
            portB = to;

            from.Connect(this);
            if(portB != null)
            {
                portB.Connect(this);
            }

            _length = length;

           

          
        }

        private static Rectangle GetBounds(Port from, Port to, int length, CompassPoint dirfrom)
        {
            if (from == null)
            {
                throw new ArgumentException("first port may not be null!");
            }
            
           

            // hideous.

            Point size;

            if (dirfrom.IsDiagonal())
            {
                Point p = dirfrom.ToPoint();

                size = p * new Point(length);
                size.X = Math.Abs(size.X);
                size.Y = Math.Abs(size.Y);

                Point loc = from.Location;
  
                if(p.X == -1)
                {
                    loc.X -= size.X - 1; 
                }
                if (p.Y == -1)
                {
                    loc.Y -= size.Y - 1;
                }

                return new Rectangle(loc, size);
            }


            size = new Point(length, 1);
            Direction d = (Direction)dirfrom.ToDirection();

            if ( d.IsVertical() )
            {
                size = new Point(size.Y, size.X);
            }

            if(d.IsPositive())
            {
                return new Rectangle(from.Location, size);
            }

            Point start = from.Location;
            
            if(d.IsVertical())
            {
                start.Y += -size.Y +(to==null?1:0);

            }
            else
            {
                start.X += -size.X + (to == null ? 1 : 0);
            }
      
            Rectangle toReturn = new Rectangle(start, size);
            return toReturn;

        }

        public override void Destroy()
        {

            if(PortA != null)
            {
                PortA.Disconnect(); // this will only be called when there is another signal ready to displace us. therefore, it is impossible for a port to have no connection this way.
                
            }
            if(PortB != null)
            {
                portB.Disconnect();
            }

            base.Destroy();
        }



        public int Receive(Port p)
        {
            if(p == portA)
            {
                return FromB;

            }
            
            if(p == portB)
            {
                return FromA;
            }

            throw new ArgumentException(p + " is not connected to signal " + this+" it cannot receive a value");
        }


        public override string ToString()
        {
            return "Connection: { A:" + (portA==null?"null":portA.Location.ToString()) + " B: " + (portB == null ? "null" : portB.Location.ToString()) + " Bounds:" + Bounds + " Length: "+Length+"}";

        }

        public Port Other(Port p)
        {
            if(p == PortA)
            {
                return portB;
            }
            if(p == PortB)
            {
                return portA;
            }

            throw new Exception("invalid port: not of this connection");

        }


       


    }
}
