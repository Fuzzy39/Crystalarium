using CrystalCore.Model.Physical;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultConnection : Connection, Entity
    {

        private Port _portA;
        private Port _portB;

        private int _fromA;
        private int _fromB;

        private bool _destroyed;

        private MapObject _physical;

        public Connection(ComponentFactory factory, Port initial)
        {

            factory.CreateObject(x, this);
            _portA = initial;

            // as an entity, we need to create our physicalrep.
            // via the factory, of course.
        }


        public Port PortA => throw new NotImplementedException();

        public Port PortB => throw new NotImplementedException();

        public int FromA => throw new NotImplementedException();

        public int FromB => throw new NotImplementedException();

        public MapObject Physical => throw new NotImplementedException();

        public bool HasCollision => throw new NotImplementedException();

        public Point Size => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

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
            throw new NotImplementedException();
        }

        public Port OtherPort(Port port)
        {
            throw new NotImplementedException();
        }

        public void Transmit(Port from, int value)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
