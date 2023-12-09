using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultConnection : Connection, Entity
    {
        // TODO connections should not store values. when connections are mutated, they could be lost.

        private Port _portA;
        private Port _portB;

        private int _fromA;
        private int _fromB;

        private bool _destroyed;

        private MapObject _physical;
        private Point _size;

        private ComponentFactory _factory;

        public Connection(ComponentFactory factory, Port initial)
        {

            _destroyed = false;
            _factory = factory;
            _size = new Point(0,0);

            _portA = initial;
            _portA.Connection = this;

            _fromA = 0;
            _fromB = 0;
            _portB = null;

            Update();
            OnReady?.Invoke(this, EventArgs.Empty);
        }


        public Port PortA => _portA;

        public Port PortB => _portB;

        public int FromA => _fromA;

        public int FromB => _fromB;

        public MapObject Physical => _physical;

        public bool HasCollision => false;

        // uhhhhh
        public Point Size => _size;

        public bool Destroyed => _destroyed;

        public event ConnectionEventHandler OnValuesUpdated;
        public event EventHandler OnReady;

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Disconnect(Port toDisconnect)
        {
            throw new NotImplementedException();
        }

        public bool IsPortA(Port p)
        {
            if(_portA == p)
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
            if(IsPortA(port))
            {
                return _portB;
            }

            return _portA;
            
        }

        public void Transmit(Port from, int value)
        {
            throw new NotImplementedException();
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


            _fromB = 0;

            if (aGone)
            {
                // swap A and B to make finding a new next port simpler.
                _portA = _portB;
                _fromA = _fromB;
                _portB = null;
                
            }

            //Now, either B is gone or present, and A is present.

            Point bLoc = _portA.Location;
            MapObject obj = _physical.Grid.FindClosestObjectInDirection(ref bLoc, _portA.AbsoluteFacing, out int length);

            if (obj is null)
            {
                // well. We have some work to do.
                // TODO determine size, etc.
                DetermineSize(length);
                _fromB = 0;
                return;
            }

            if (obj is not Node)
            {
                throw new NotImplementedException("An edge case that I was too lazy to write code for came up.\nTo be fair, it wasn't possible for it to happen when I wrote the code.");
            }

            if(_portB == obj)
            {
                // nothing to update.
                return;
            }

            Node n = (Node)obj;
            _portB = n.SomeMagicFunctionThatFindsThePortWeWant(_portA.AbsoluteFacing.Opposite, bLoc);
            _portB.Connection = this;
            _fromB = 0;

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

            if(_physical!= null) 
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
