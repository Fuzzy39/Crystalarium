using CrystalCore.Model.Core;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private bool _inputUpdated;

        private int _outputting;

        // no clue how we're going to give port a grid, but not my problem in this exact moment.
        public DefaultPort(PortDescriptor descriptor, Direction parentRotation, Rectangle parentBounds)
        {
            
            _descriptor = descriptor;
            _absFacing = CalculateAbsoluteFacing(parentRotation);
            _location = CalculateLocation(parentRotation, parentBounds);
            _inputUpdated = true;
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

            Point anchor = parentBounds.Location;

            // get actual facing direction
            Direction? d = AbsoluteFacing.ToDirection();
            if (d == null)
            {
                //return anchor; // diagonal ports means that the agent is 1x1.


                // I don't remember why this switch exists, but I guess it's important for diagonal signals?
                switch (AbsoluteFacing) 
                {
                    case CompassPoint.northwest:
                        return anchor;
                    case CompassPoint.southwest:
                        return anchor + new Point(0, parentBounds.Height - 1);
                    case CompassPoint.northeast:
                        return anchor + new Point(parentBounds.Width - 1, 0);
                    case CompassPoint.southeast:
                        return anchor - new Point(1) + parentBounds.Size;


                }
                throw new InvalidOperationException("Failed to interpret compass direction to location.");

            }

            Direction facing = (Direction)d;



            int x = anchor.X + DetermineRelX(facing, parentRotation, parentBounds);
            int y = anchor.Y + DetermineRelY(facing, parentRotation, parentBounds);
            return new Point(x, y);

        }

        private int DetermineRelX(Direction facing, Direction parentRotation, Rectangle parentBounds)
        {
            // facing is absolute here.
            int x = 0;
            if (facing.IsVertical())
            {
                if (parentRotation.IsPositive())
                {
                    x += parentBounds.Width - 1 - _descriptor.ID;
                }
                else
                {
                    x += _descriptor.ID;
                 
                }
            }

            if (facing == Direction.right)
            {

                x += parentBounds.Width - 1;
            }

            return x;
        }


        private int DetermineRelY(Direction facing, Direction parentRotation, Rectangle parentBounds)
        {
            int y = 0;
            if (facing.IsHorizontal())
            {
                if (parentRotation == Direction.up || parentRotation == Direction.right)
                {
                    y += _descriptor.ID;
                }
                else
                {
                    y += parentBounds.Height - 1 - _descriptor.ID;
                }
            }

            if (facing == Direction.down)
            {
                y += parentBounds.Height - 1;
            }

            return y;
        }



        public bool Destroyed => _destroyed;

        public event EventHandler? OnDestroy;

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
                if (_connection != null)
                {
                    _connection.Disconnect(this);
                    _connection.OnValuesUpdated -= OnPortValueChanged;
                }
                _connection = value;
                _connection.OnValuesUpdated += OnPortValueChanged;
                _inputUpdated = true;
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

        public bool InputUpdated
        {
            get
            {
                bool temp = _inputUpdated;
                _inputUpdated = false;
                return temp;
            }
        }

        private void OnPortValueChanged(object sender, EventArgs e )
        {
            _inputUpdated = true;
        }


        public void Destroy()
        {
            _destroyed = true;
            // TODO: tell connection to disconnect from us
            _connection.Disconnect(this);
            _connection.OnValuesUpdated -= OnPortValueChanged;
            _connection = null;
            _inputUpdated = false;

            OnDestroy?.Invoke(this, new());
        }
    }
}   
