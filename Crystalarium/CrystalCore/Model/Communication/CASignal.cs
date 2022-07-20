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

            //Console.WriteLine("New Signal made, comming from Agent " + transmitter.Parent);
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

            CompassPoint portFacing = _start.Facing;
            for (int i = 0; i < 4; i++)
            {
                portFacing = portFacing.Rotate(RotationalDirection.clockwise);
            }

            // however we get ports...
            List<Port> potentialMatches = a.Ports[(int)portFacing];

            foreach(Port p in potentialMatches)
            {
                if(p.Location.Equals(portLoc))
                {
                    // hooray! We've succeeded!
                   
                    _end = p;
                    Console.WriteLine("Signal Connected from Agent " + _start.Parent + " to " + _end.Parent);
                    return;
                }
            }

            throw new InvalidOperationException("No port to do anything! It's Borked, fool!");

        }
    }
}
