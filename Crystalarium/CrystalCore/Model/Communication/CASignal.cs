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
        public CASignal(Grid g, Point location, Port transmitter, int value) : base(g, location, transmitter, value)
        {
        }

        public override void Update()
        {

            Point portLoc = _start.Location + _start.AbsoluteFacing.ToPoint();


            // we now know what kind of port we need.
            Agent a = Grid.getAgentAtPos(portLoc);
            if (a == null)
            {
                return;
            }

            CompassPoint portFacing = _start.Facing;
            for (int i = 0; i < 4; i++)
            {
                portFacing = portFacing.Rotate(RotationalDirection.clockwise);
            }

            // however we get ports...

        }
    }
}
