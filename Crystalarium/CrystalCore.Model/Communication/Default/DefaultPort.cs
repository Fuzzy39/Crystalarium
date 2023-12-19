using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultPort : Port
    {
        
        private bool _destroyed;

        // Port
        private readonly PortDescriptor _descriptor;
        private readonly CompassPoint _absFacing;

        private readonly Point _location;

        private Connection _connection;

        private int _outputting;

      
        public DefaultPort(PortDescriptor descriptor, Direction parentRotation, Rectangle parentBounds)
        {
            _descriptor = descriptor;
            _absFacing = CalculateAbsoluteFacing(parentRotation);
            _location = CalculateLocation(parentRotation, parentBounds);
            _outputting = 0;

        }

        private CompassPoint CalculateAbsoluteFacing(Direction parentRotation)
        {
            CompassPoint toReturn = _descriptor.Facing;
            for (Direction i = parentRotation; i != Direction.up; i = i.Rotate(RotationalDirection.cw))
            {
                toReturn = toReturn.Rotate(RotationalDirection.ccw);
                toReturn = toReturn.Rotate(RotationalDirection.ccw);
            }
            return toReturn;
        }


        private Point CalculateLocation(Direction parentRotation, Rectangle parentBounds)
        {

         

            // get actual facing direction.

            Direction? d = AbsoluteFacing.ToDirection();
            if (d == null)
            {
                // diagonal ports can only exist if an agent is 1x1, so we don't need to care.
                if(!parentBounds.Size.Equals(new(1)))
                {
                    throw new ArgumentException("Diagonal ports can only exist on 1x1 nodes. If this is a problem, make this code better.");
                }
                return parentBounds.Location;
            }

            Direction absfacing = (Direction)d;


            // get abs parent size
            Point absParentSize = parentBounds.Size;
            if(parentRotation.IsHorizontal())
            {
               
                absParentSize.Deconstruct(out int sizeY,  out int sizeX);
                absParentSize = new(sizeX, sizeY);
            }

            // calculate it
            int x = parentBounds.Location.X + DetermineRel(absfacing, absParentSize.X, true);
            int y = parentBounds.Location.Y + DetermineRel(absfacing, absParentSize.Y, false);
            return new Point(x, y);

        }

        private int DetermineRel(Direction absFacing, int MaxSize, bool isX)
        {
           // basically, if this is X, we want to check if it's vertical, if y we want to check horizontal.
           if(absFacing.IsHorizontal() ^ isX) 
           {
                return _descriptor.ID;
           }

           if(absFacing.IsPositive())
           {
                return MaxSize-1;
           }

           return 0;
        }


        public bool Destroyed => _destroyed;

        public event EventHandler? OnDestroy;
        public event EventHandler? OnInputUpdated;

        public PortDescriptor Descriptor => _descriptor;

        public CompassPoint AbsoluteFacing => _absFacing;

        public Point Location => _location;

        /// <summary>
        /// On setting a new connection, the old connection must be updated.
        /// </summary>
        public Connection Connection
        {
            get => _connection;
            set
            {
                // given that this is being called from connection, it will set things up appropriately, we just need to worry about the old
                // connection and ourselves.
                if(_connection != null)
                {
                    _connection.OnValuesUpdated -= OnPortValueChanged;
                }

                _connection = value;
                _connection.OnValuesUpdated += OnPortValueChanged;
            }
        }

        
        public Port ConnectedTo => _connection.OtherPort(this);

        // TODO: when output is set, the other port has to change inputUpdated
        public int Output
        {
            get
            {
                return _outputting;
            }

            set
            {
                _outputting = value;
                _connection.Transmit(this, value);
            }
        }

        public int Input
        {
            get
            {
                if (_connection.IsPortA(this))
                {
                    return _connection.FromB;
                }

                return _connection.FromA;
            }
        }


        private void OnPortValueChanged(Connection sender, Connection.EventArgs e)
        {
            if (sender.IsPortA(this) == e.PortAUpdated)
            {
                OnInputUpdated?.Invoke(this, e);
            }

        }


        public void Destroy()
        {
            _destroyed = true;
            // TODO: tell connection to disconnect from us
            _connection.Disconnect(this);
            _connection.OnValuesUpdated -= OnPortValueChanged;
            _connection = null;
            OnInputUpdated = null;


            OnDestroy?.Invoke(this, EventArgs.Empty);
        }
    }
}
