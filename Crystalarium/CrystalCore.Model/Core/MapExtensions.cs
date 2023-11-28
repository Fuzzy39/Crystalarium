using CrystalCore.Model.OldObjects;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace CrystalCore.Model.Core
{
    public static class MapExtensions
    {

      

        public static Agent getAgentAtPos(this Map g, Point coords)
        {
            Chunk ch = g.getChunkAtCoords(coords);

            if (ch == null)
            {
                return null;
            }

            List<ChunkMember> agents = ch.MembersWithin;
            foreach (ChunkMember cm in agents)
            {
                if (!(cm is Agent))
                {
                    continue;
                }

                Agent a = (Agent)cm;

                if (a.Bounds.Contains(coords))
                {
                    return a;
                }

            }

            return null;
        }




     

        public static List<Agent> AgentsWithin(this Map g, Rectangle bounds)
        {
            List<Entity> possibleList = g.EntitiesWithin(bounds);

            List<Agent> toReturn = new List<Agent>();

            foreach (Entity e in possibleList)
            {
                if (e is Agent)
                {
                    toReturn.Add((Agent)e);
                }
            }

            return toReturn;

        }

    }

}
