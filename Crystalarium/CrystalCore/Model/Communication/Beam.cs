using CrystalCore.Model.Grids;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Communication
{
    internal class Beam : Signal
    {

        private int _maxLength=0;
        private int _minLength=1;
        private int _length=0;

       

        // how long can this beam get? 0 or lower means limitless.
        public int MaxLength 
        {
            get
            {
                return _maxLength;
            }

            set
            {
                if(value <= 0)
                {
                    _maxLength = 0;
                    return;
                }

                if(value<_minLength)
                {
                    throw new InvalidOperationException("Maximum beam length cannot be less than minimum.");
                }

                _maxLength = value;
              
            }
        
        }

        public int MinLength 
        {
            get {  return _minLength; }
            
            set
            {
                if(value < 1)
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



        public int Length { get => _length; }


        public Beam(Grid g, Port transmitter, int value, int min, int max) : base(g, transmitter, value)
        {
             MaxLength =  max;
             MinLength = min;
             _length = 0;
          
             Update(); // update, to be safe.
          
        }

        public override void Update()
        {
            // we start looking for targets by getting the point where our min

            if(Start==null)
            {
                throw new InvalidOperationException("This signal should be destroyed. Is it? "+Destroyed+"! It should not be updated, but it was.");
            }
            Point start = _start.Location;
            Point? p = Travel(start, MinLength);
            if (p==null)
            {
                _length = 1;
                return;
            }
            int length = MinLength;
            Point end = (Point)p;

            PortAgent target = FindTarget(ref length, ref end);
            TransmitTo(target, end, length);

        }

        private PortAgent FindTarget(ref int length, ref Point end)
        {
            // start looking for targets, one tile at a time.
            Point? nextEnd = end;
            while (nextEnd != null)
            {
                end = (Point)nextEnd;
                List<Agent> targets = Grid.AgentsWithin(new Rectangle(end, new Point(1)));

                // We found a target!
                if (targets.Count != 0)
                {


                    // this cast is safe, if we exist, port agents must.
                    PortAgent target = (PortAgent)targets[0];
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
            return null;
        }

        private void SetBounds(Point start, Point end)
        {
            Rectangle r = Util.Util.RectFromPoints(start, end);
            r.Width++; 
            r.Height++; 
            Bounds = r;
        }

        private void TransmitTo(PortAgent target, Point loc, int length)
        {
            if(target!=null)
            {
                TransmitToTarget(target, loc);
            }

            _length = length;

            // we need to adjust our bounds now.
            SetBounds(_start.Location, loc);

        }


        private void TransmitToTarget(PortAgent target, Point loc)
        {

            
            CompassPoint portFacing = _start.AbsoluteFacing.Opposite();

            Port p = FindPort(target, loc, portFacing);

            if(p == _end)
            {
                // oh, nothing changed. Alright, neat.
                return;
            }

            if(_end!=null)
            {
                _end.StopReceiving();
            }

            _end = p;
            p.Receive(this);
            


        }

        
        private Point? Travel(Point start, int distance)
        {

            Point toReturn = start;
            for(int i = 0; i<distance; i++)
            {
                Point p = toReturn + _start.AbsoluteFacing.ToPoint();
                if (!Grid.Bounds.Contains(p))
                {
                    // this is the end of the road for us.
                    return null;
                }
                toReturn= p;
            }

            return toReturn;

        }
    }
}
