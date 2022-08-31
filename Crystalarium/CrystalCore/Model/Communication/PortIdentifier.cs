﻿using CrystalCore.Model.Rulesets;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{

    internal enum PortStatus
    {
        inactive,
        receiving,
        transmitting,
        transceiving
    }

    public enum PortChannelMode
    {
        halfDuplex,
        fullDuplex
    }

    // relative portID.
    public struct PortIdentifier
    {

        public int ID;  
        public CompassPoint Facing;

        public PortIdentifier(int portID, CompassPoint compassPoint)
        {
            ID = portID;
            Facing = compassPoint;
        }
        public bool CheckValidity(AgentType at)
        {
            if (!at.Ruleset.DiagonalSignalsAllowed & Facing.IsDiagonal())
            {
                return false;
            }

            if (Facing.IsDiagonal())
            {
                if(ID!=0)
                {
                    return false;
                }

                return true;
            }


            Direction d = (Direction)Facing.ToDirection();

            if (d.IsVertical() & at.Size.X <= ID)
            {
                return false;
            }

            if (d.IsHorizontal() & at.Size.Y <= ID)
            {
                return false;
            }

            return true;
        }


        public override string ToString()
        {
            return Facing + "," + ID;
        }
    }
}
