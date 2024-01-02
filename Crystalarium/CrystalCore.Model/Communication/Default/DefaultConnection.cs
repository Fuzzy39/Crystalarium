using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System.Data.Common;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultConnection : Connection
    {
        

        private Port _portA;
        private Port _portB;
            


        private bool _destroyed;

        private MapObject _physical;
        private Point _size;

        private ComponentFactory _factory;

        public DefaultConnection(ComponentFactory factory, Port initial)
        {

            _destroyed = false;
            _factory = factory;
            _size = new Point(1, 1);
            

            _portA = null;
            _portB = null;

            ConnectToPort(initial);
            _physical = factory.CreateObject(_portA.Location, this);
            Update();

        }


        private void ConnectToPort(Port p)
        {

            if (p.Connection != null)
            {
                p.Connection.Disconnect(p);

            }

            p.Connection = this;

            if (_portA == null)
            {
                _portA = p;
            }
            else
            {

                if (_portB != null)
                {
                    throw new InvalidOperationException("Can only have two things connected to a connection...");
                }

                _portB = p;

            }

            // this could be seen as less than effecient, but...
            OnValuesUpdated?.Invoke(this, new(true));
            OnValuesUpdated?.Invoke(this, new(false));

            // too bad.

        }


        public Port PortA => _portA;

        public Port PortB => _portB;

        public int FromA => _portA == null ? 0 : _portA.Output;

        public int FromB => _portB == null ? 0 : _portB.Output;

        public MapObject Physical => _physical;

        public bool HasCollision => false;

        // uhhhhh
        public Point Size => _size;

        public bool Destroyed => _destroyed;

        public event ConnectionEventHandler OnValuesUpdated;


        public void Destroy()
        {
            _destroyed = true;
            if (_portA != null)
            {
                Disconnect(_portA);
            }
            if (_portB != null)
            {
                Disconnect(_portB);
            }


            OnValuesUpdated = null;
            _physical.Destroy();
            _physical = null;
            _factory = null;
            _size = new Point(0, 0);

        }

        public void Disconnect(Port toDisconnect)
        {
            bool isPortA = IsPortA(toDisconnect);

            // does not leave the connection in a stable state.
            if (isPortA)
            {
                _portA.Connection = null;
                _portA = null;
            }
            else
            {
                _portB.Connection = null;
                _portB = null;
            }

            //Somebody else is responsible for this port now, we don't care.
            //OnValuesUpdated?.Invoke(this, new(!isPortA));

        }

        public bool IsPortA(Port p)
        {
            if (_portA == p)
            {
                return true;
            }

            if (p != _portB)
            {
                throw new ArgumentException("Port: " + p + " is not connected to Connection: " + this);
            }

            if(p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            return false;
        }

        public Port OtherPort(Port port)
        {
            if (IsPortA(port))
            {
                return _portB;
            }

            return _portA;

        }

        public void Transmit(Port from, int value)
        {
            OnValuesUpdated.Invoke(this, new(!IsPortA(from)));
        }

        public void Update()
        {
            // Okay, this method will be...
            // complex.

            // should never have a destroyed port, the call disconnect on destroy.

            if ((_portA == null) && (_portB == null))
            {
                // we have no reason to exist anymore. nothing to connect to.
                Destroy();
                return;
            }


            if (_portA == null)
            {
                // swap A and B to make finding a new next port simpler.
                _portA = _portB;

                _portB = null;

            }

            //Now, either B is gone or present, and A is present.

            Point bLoc = _portA.Location;
            MapObject obj = _physical.Grid.FindClosestObjectInDirection(ref bLoc, _portA.AbsoluteFacing);


            if (obj is null)
            {
                // well. We have some work to do.
                // TODO determine size, etc.
                DetermineSize(_portA.Location, bLoc);
                return;
            }



            if (obj.Entity is not Node)
            {
                throw new NotImplementedException("An edge case that I was too lazy to write code for came up.\nTo be fair, it wasn't possible for it to happen when I wrote the code.");
            }


            Node node = (Node)(obj.Entity);
            Port toConnect = node.GetPort(_portA.AbsoluteFacing.Opposite(), bLoc);

            if (_portB == toConnect)
            {
                // nothing to update.
                return;
            }

            if (_portB != null)
            {
                Disconnect(_portB);
            }

            ConnectToPort(toConnect);
            // Now, determine size.
            DetermineSize(_portA.Location, bLoc);
        }


        private void DetermineSize(Point start, Point end)
        {
            Rectangle bounds = MiscUtil.RectFromPoints(start, end);
            bounds.Size += new Point(1); // fix off by one errors due to having to include the end point in the rectangle.

            if (bounds.Size.Equals( _size) && _physical != null)
            {
                return;
            }

            _size = bounds.Size;

            if (_physical != null)
            {
                _physical.Destroy();

            }

            _physical = _factory.CreateObject(bounds.Location, this);
        }




        public override string ToString()
        {
            return "Connection: { A:" + (PortA == null ? "null" : PortA.Location.ToString()) + " B: " + (PortB == null ? "null" : PortB.Location.ToString())
                + " Bounds:" + (Physical == null ? "null": Physical.Bounds) + "}";

        }
    }
}
