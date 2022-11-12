using CrystalCore.Model.Elements;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    /// <summary>
    /// a pathfinder tries to calculate a connection for a particular port.
    /// </summary>
    internal class Pathfinder
    {
        private Port port;
        private Point start;
        private CompassPoint facing;
        private Connection current;
        private int currentLength;
        private Map Map;

        internal Pathfinder(Port port, Map map)
        {
            this.port = port;
            this.Map = map;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">the minimum length of the path</param>
        /// <param name="CurrentPath"></param>
        /// <returns>null if no new connection. connection otherwise</returns>
        internal void FindPath(int minLength, int maxLength, Connection currentConnection)
        {
            
            start = port.Location;
            facing = port.AbsoluteFacing;
            current = currentConnection;


            // first, get the point where we are starting from.

            Point? p = Travel(start, minLength);
            if (p == null)
            {
                // no connection established. May cause issues?
                if (current != null) { current.Destroy(); }
                new Connection
                (
                   Map,
                   port,
                   null,
                   1,
                   facing
                );

                return;
            }
            currentLength = minLength;

            Point connectTo = (Point)p;

            Agent target = FindTarget(maxLength, ref connectTo);

            // check that this target is novel.
            Connection same = CheckTarget(target);
            if(same != null)
            {
                return;
            }


            // it is! grab the appropriate port.
            if (current != null) { current.Destroy(); }
            Console.WriteLine("Should be false: "+ port.HasConnection);
            Connection c = new Connection
            (
                Map, 
                port, 
                FindPort(target, connectTo, facing.Opposite()), 
                currentLength, 
                facing
            );
            Console.WriteLine("FULLY OPERATIONAL? : " + c);

        

        }


        private Agent FindTarget(int maxLength, ref Point end)
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
                   
                    return target;

                }

                // If we have a max length, have we reached it?
                if (maxLength != 0 & currentLength == maxLength)
                {
                    break;
                }

                // otherwise, get a bit longer.
                currentLength++;
                nextEnd = Travel(end, 1);
            }

            // at this point, we have either reached our max length, or hit the end of the grid without finding a target.
            // we should update our length and bounds to reflect that.
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Null if a new connection must be made, otherwise a conenction already exists.</returns>
        private Connection CheckTarget(Agent target)
        {
            // if we found no target...
            if(current == null)
            {
                if (target == null)
                {
                    //current.Destroy();
                    return new Connection
                        (
                        Map,
                        port,
                        null,
                        currentLength,
                        facing
                        );
                }
                else
                {
                    return null;
                }
            }


            if ( current.Other(port) == null && target == null)
            {
                if (currentLength == current.Length)
                {
                    return current;
                }

                // into the void!
                current.Destroy();
                return new Connection
                    (
                    Map,
                    port,
                    null,
                    currentLength,
                    facing
                    ) ;

            }

            // if we are connected to the same target that we are calculating now, there's no need to do anything.
            if (current.Other(port) != null && target == current.Other(port).Parent)
            {
                return current;
            }

            return null;
        }

        


        private Point? Travel(Point start, int distance)
        {

            Point toReturn = start;
            for (int i = 0; i < distance; i++)
            {
                Point p = toReturn + facing.ToPoint();
                if (!Map.Bounds.Contains(p))
                {
                    // this is the end of the road for us.
                    return null;
                }
                toReturn = p;
            }

            return toReturn;

        }



        private Port FindPort(Agent a, Point loc, CompassPoint AbsFacing)
        {
            // we need to find a port with the absolute facing matching ours.
            List<Port> potentialMatches = null;
            foreach (List<Port> ports in a.Ports)
            {
                if (ports.Count > 0)
                {
                    if (ports[0].AbsoluteFacing == AbsFacing)
                    {
                        potentialMatches = ports;
                        break;
                    }
                }
            }

            foreach (Port p in potentialMatches)
            {
                if (p.Location.Equals(loc))
                {
                    // hooray! We've succeeded!

                    return p;
                }
            }

            throw new InvalidOperationException("Could not find port facing (Absolute): " + AbsFacing + " on tile " + loc + " in agent " + a + "\nSomething went wrong...");
        }

    }
}
