﻿using CrystalCore.Util;

namespace CrystalCore.Model.Rules
{


    // relative portID.
    public struct PortID
    {

        public int ID;
        public CompassPoint Facing;

        public PortID(int portID, CompassPoint compassPoint)
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
                if (ID != 0)
                {
                    return false;
                }

                return true;
            }


            Direction d = (Direction)Facing.ToDirection();

            if (d.IsVertical() & at.UpwardsSize.X <= ID)
            {
                return false;
            }

            if (d.IsHorizontal() & at.UpwardsSize.Y <= ID)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PortID)) { return false; }
            PortID other = (PortID)obj;
            if (other.ID != ID) { return false; }
            if (other.Facing != Facing) { return false; }
            return true;

        }
        public override string ToString()
        {
            return Facing + "," + ID;
        }
    }


    public struct PortTransmission
    {
        public PortID portID;
        public int value;

        public PortTransmission(int value, PortID pid)
        {
            portID = pid;
            this.value = value;
        }

        public PortTransmission(int value, int portID, CompassPoint compassPoint) : this(value, new PortID(portID, compassPoint))
        {


        }
    }
}