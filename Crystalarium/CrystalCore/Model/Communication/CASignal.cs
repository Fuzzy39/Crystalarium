using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{
    internal class CASignal : Signal
    {
        public CASignal(Grid g, Port transmitter, int value) : base(g, transmitter, value)
        {

           // We need to adjust our size to be accurate.
            Point loc = Bounds.Location;

            switch(_start.AbsoluteFacing)
            {
                case CompassPoint.north:
                case CompassPoint.northeast:
                    loc.Y -= 1;
                    break;
                case CompassPoint.northwest:
                    loc.X -= 1;
                    loc.Y -= 1;
                    break;
                case CompassPoint.west:
                case CompassPoint.southwest:
                    loc.X -= 1;
                    break;
            }

            Point size = Bounds.Size;

            if(_start.AbsoluteFacing.IsDiagonal())
            {
                size = new Point(2);
            }
            else if( ((Direction)_start.AbsoluteFacing.ToDirection()).IsVertical())
            {
                size.Y = 2;
            }
            else
            {
                size.X = 2;
            }

            Rectangle bounds = new Rectangle(loc, size);
            if(!Grid.Bounds.Contains(bounds))
            {
                Grid.ExpandToFit(bounds);
            }
            Bounds = bounds;

            
        }

        public override void Update()
        {
            if(_end!=null)
            {
                return;
            }

            Point portLoc = _start.Location + _start.AbsoluteFacing.ToPoint();


            // we now know what kind of port we need.
            Agent a = Grid.getAgentAtPos(portLoc);
            if (a == null)
            {
                return;
            }

            CompassPoint portFacing = _start.AbsoluteFacing.Opposite();

            Port p = FindPort(a, portLoc, portFacing);

            // however we get ports...
           
            _end = p;
            p.Receive(this);

         
         
            return;

        }

       
    }
}
