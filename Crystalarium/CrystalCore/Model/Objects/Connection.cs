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
            get 
            {
                if (portA == null)
                {
                    return 0;
                }

                return portA.TransmittingValue;
            }
        }

        internal int FromB
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
      
            return new Rectangle(start, size);


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
            return "Signal: { A:" + (portA==null?"null":PortA.ToString()) + " B: " + (portB == null ? "null" : portB.ToString()) + " Bounds:" + Bounds + "}";

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
