using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System.Data.Common;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultConnection : Connection
    {
        // TODO connections should not store values. when connections are mutated, they could be lost.

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
            _size = new Point(0, 0);
            

            _portA = null;
            _portB = null;

            ConnectToPort(initial);

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
            OnValuesUpdated.Invoke(this, new(true));
            OnValuesUpdated.Invoke(this, new(false));

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

            // does not leave the connection in a stable state.
            if (IsPortA(toDisconnect))
            {
                _portA = null;
            }
            else
            {
                _portB = null;
            }


            OnValuesUpdated.Invoke(this, new(!IsPortA(toDisconnect)));

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
            bool aGone = _portA == null;
            bool bGone = _portB == null;


            if (aGone && bGone)
            {
                // we have no reason to exist anymore. nothing to connect to.
                Destroy();
                return;
            }


            if (aGone)
            {
                // swap A and B to make finding a new next port simpler.
                _portA = _portB;

                _portB = null;

            }

            //Now, either B is gone or present, and A is present.

            Point bLoc = _portA.Location;
            MapObject obj = _physical.Grid.FindClosestObjectInDirection(ref bLoc, _portA.AbsoluteFacing, out int length);


            if (_portB == obj)
            {
                // nothing to update.
                return;
            }

            // B is different. how so?


            if (obj is null)
            {
                // well. We have some work to do.
                // TODO determine size, etc.
                DetermineSize(length);
                return;
            }

            if (obj is not Node)
            {
                throw new NotImplementedException("An edge case that I was too lazy to write code for came up.\nTo be fair, it wasn't possible for it to happen when I wrote the code.");
            }

        

            Node n = (Node)obj;

            ConnectToPort(n.GetPort(_portA.AbsoluteFacing.Opposite(), bLoc));
            // Now, determine size.
            DetermineSize(length);
        }


        private void DetermineSize(int length)
        {
            Rectangle bounds = GetBounds(_portA, _portB, length);
            if (bounds.Size != _size)
            {
                _size = bounds.Size;
            }

            if (_physical != null)
            {
                _physical.Destroy();

            }

            _physical = _factory.CreateObject(bounds.Location, this);
        }


        // I guess we can trust this...
        private static Rectangle GetBounds(Port from, Port to, int length)
        {
            if (from == null)
            {
                throw new ArgumentException("first port may not be null!");
            }

            // hideous
            // if this breaks, I'm probably just gonna rewrite it from scratch.
            // not a clue what it does.

            Point size;
            CompassPoint dirfrom = from.AbsoluteFacing;
            if (dirfrom.IsDiagonal())
            {
                Point p = dirfrom.ToPoint();

                size = p * new Point(length);
                size.X = Math.Abs(size.X);
                size.Y = Math.Abs(size.Y);

                Point loc = from.Location;

                if (p.X == -1)
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

            if (d.IsVertical())
            {
                size = new Point(size.Y, size.X);
            }

            if (d.IsPositive())
            {
                return new Rectangle(from.Location, size);
            }

            Point start = from.Location;

            if (d.IsVertical())
            {
                start.Y += -size.Y + (to == null ? 1 : 0);

            }
            else
            {
                start.X += -size.X + (to == null ? 1 : 0);
            }

            Rectangle toReturn = new Rectangle(start, size);
            return toReturn;

        }
    }
}
