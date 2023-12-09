using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Communication
{
    public struct PortDescriptor
    {

        /// <summary>
        /// ID represents the index at which the port is from the given direction that it's facing.
        /// They are assigned from left to right, or top to bottom, depending on Facing. 
        /// If a particular ConnectionNode is 3 tiles wide, then the valid ID range is 0-2 when Facing is North or South.
        /// The only viable ID is 0 if Facing is diagonal.
        /// </summary>
        public readonly int ID;
        public readonly CompassPoint Facing;

        public PortDescriptor(int portID, CompassPoint compassPoint)
        {
            ID = portID;
            Facing = compassPoint;
        }

        //public bool CheckValidity(AgentType at)
        //{
        //    if (!at.Ruleset.DiagonalSignalsAllowed & Facing.IsDiagonal())
        //    {
        //        return false;
        //    }

        //    if (Facing.IsDiagonal())
        //    {
        //        if (ID != 0)
        //        {
        //            return false;
        //        }

        //        return true;
        //    }


        //    Direction d = (Direction)Facing.ToDirection();

        //    if (d.IsVertical() & at.UpwardsSize.X <= ID)
        //    {
        //        return false;
        //    }

        //    if (d.IsHorizontal() & at.UpwardsSize.Y <= ID)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public override bool Equals(object obj)
        {
            if (!(obj is PortDescriptor)) { return false; }
            PortDescriptor other = (PortDescriptor)obj;
            if (other.ID != ID) { return false; }
            if (other.Facing != Facing) { return false; }
            return true;
        }

        public static bool operator ==(PortDescriptor a, PortDescriptor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PortDescriptor a, PortDescriptor b)
        {
            return !a.Equals(b);
        }


        public override string ToString()
        {
            return Facing + "," + ID;
        }
    }
}
