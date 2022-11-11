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
            : base(m, GetBounds(from, length, direction))
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

        private static Rectangle GetBounds(Port A,  int length, CompassPoint from)
        {
            if (A == null)
            {
                throw new ArgumentException("first port may not be null!");
            }

            Point a = A.Location;
           
             

            Point b = from.ToPoint();
            b.X *= (length-1);
            b.Y *= (length-1);

            b += a;
            if (!from.IsDiagonal())
            {
                Direction d = (Direction)from.ToDirection();
                if (d.IsVertical())
                {
                    b.X++;
                }
                else
                {
                    b.Y++;
                }
            }   
            return Util.Util.RectFromPoints(a, b);
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




        public void Update()
        {

            if (portA!= null && portA.Destroyed)
            {
                portA = null;
            }

            if (portB != null && portB.Destroyed)
            {
                portB = null;
            }

            if(PortA == null && PortB == null)
            {
                Destroy();
            }

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
