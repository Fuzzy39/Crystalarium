using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Objects
{
    internal class Beam : Signal
    {

        private int _maxLength = 0;
        private int _minLength = 1;
        private int _length = 0;

        internal Port Start
        {
            get
            {
                if (portA == null)
                {
                    return portB;
                }

                return portA;
            }
        }

        internal Port End
        {
            get
            {
                if(portA == Start)
                {
                    return portB;
                }

                return portA;
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



        public int Length { get => _length; }


        public Beam(Map m, Port transmitter) : base(m, transmitter)
        {
            MaxLength = m.Ruleset.BeamMaxLength;
            MinLength = m.Ruleset.BeamMinLength;
            _length = 0;

            Update(); // update, to be safe.

        }

        public override void Update()
        {
            // we start looking for targets by getting the point where our min

            if (Destroyed)
            {
                throw new InvalidOperationException("This signal should be destroyed. Is it? " + Destroyed + "! It should not be updated, but it was.");
            }

            Point start = (portA==null)? portB.Location : portA.Location;
            Point? p = Travel(start, MinLength);
            if (p == null)
            {
                _length = 1;
                return;
            }
            int length = MinLength;
            Point end = (Point)p;

            Agent target = FindTarget(length, ref end);
            TransmitTo(target, end);

        }

        private Agent FindTarget(int length, ref Point end)
        {
            // start looking for targets, one tile at a time.
            Point? nextEnd = end;
            while (nextEnd != null)
            {
                end = (Point)nextEnd;
                Agent target = Map.AgentAt(end);

                // We found a target!
                if (target != null)
                {


                    // this cast is safe, if we exist, port agents must.
                    _length = length;
                    return (Agent)target;

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
            _length = length;
            return null;
        }

        private void SetBounds(Point start, Point end)
        {
            Rectangle r = Util.Util.RectFromPoints(start, end);
            r.Width++;
            r.Height++;
            Bounds = r;
        }

        private void TransmitTo(Agent target, Point loc)
        {
            if (target != null)
            {
                TransmitToTarget(target, loc);
            }



            // we need to adjust our bounds now.
            SetBounds(Start.Location, loc);

        }


        private void TransmitToTarget(Agent target, Point loc)
        {


            CompassPoint portFacing = Start.AbsoluteFacing.Opposite();

            Port p = FindPort(target, loc, portFacing);

            if (p == End)
            {
                // oh, nothing changed. Alright, neat.
                return;
            }

            if (End != null)
            {
                throw new InvalidOperationException("well, a beam shouldn't have two ends...");
            }

            if (portA == null)
            {
                p = portA;
               

            }
            else
            {
                p = portB;
                
            }
            p.SetupConnection(this);

           



        }


        private Point? Travel(Point start, int distance)
        {

            Point toReturn = start;
            for (int i = 0; i < distance; i++)
            {
                Point p = toReturn + Start.AbsoluteFacing.ToPoint();
                if (!Map.Bounds.Contains(p))
                {
                    // this is the end of the road for us.
                    return null;
                }
                toReturn = p;
            }

            return toReturn;

        }
    }
}
