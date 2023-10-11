using CrystalCore.Model.Core;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Simulation
{
    /// <summary>
    /// a pathfinder tries to calculate a connection for a particular port.
    /// </summary>
    internal class Raycaster
    {
        private Port port;
        private Point start;
        private CompassPoint facing;
        private Connection current;
        private int currentLength;
        private DefaultMap Map;

        internal Raycaster(Port port, DefaultMap map)
        {
            this.port = port;
            Map = map;
        }

        /// <summary>
        /// Check to make sure the existing connection is current, and if not, make a new one.
        /// </summary>
        /// <param name="length">the minimum length of the path</param>
        /// <param name="CurrentPath"></param>
        /// <returns>null if no new connection. connection otherwise</returns>
        internal void UpdateConnection(int minLength, int maxLength, Connection currentConnection)
        {

            start = port.Location;
            facing = port.AbsoluteFacing;
            current = currentConnection;


            // first, get the point where we are starting from.

            Point? p = Travel(start, minLength);
            if (p == null)
            {
                // no connection established. May cause issues?
                CreateConnection(null, 1);

                return;
            }
            currentLength = minLength;


            // find a target agent to potentially latch on to, if it exists.
            Point connectTo = (Point)p;

            Agent target = FindTarget(maxLength, ref connectTo);



            if (target == null)
            {
                if (!SameAsBlank())
                {
                    CreateConnection(null, currentLength);

                }
                return;
            }

            Port targetPort = FindPort(target, connectTo, facing.Opposite());

            if (!SameTargetPort(target, targetPort))
            {
                CreateConnection(targetPort, currentLength);
            }



        }

        internal void Destroy()
        {
            port = null;
            current = null;
            Map = null;
        }

        private Agent FindTarget(int maxLength, ref Point end)
        {
            // start looking for targets, one tile at a time.
            Point? nextEnd = end;

            while (nextEnd != null)
            {
                end = (Point)nextEnd;
                Agent target = Map.getAgentAtPos(end);

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

        private void CreateConnection(Port to, int length)
        {
            if (current != null && !current.Destroyed)
            {
                current.Destroy();
            }

            new Connection(Map, port, to, length, facing);

        }

        /// <summary>
        /// Returns whether our current connection is the same as our new one (given that we have found no target)
        /// </summary>
        /// <returns></returns>
        private bool SameAsBlank()
        {
            if (current != null)
            {
                // if the current signal was also targeting null, we can agree that everything is the same, and continue without taking action.
                if (current.Other(port) == null && currentLength == current.Length)
                {
                    return true;
                }
            }


            return false;
        }

        private bool SameTargetPort(Agent targetAgent, Port targetPort)
        {
            if (current == null)
            {
                return false; // whatever we found, it's new.
            }

            Port compareAgainst = current.Other(port);

            // our old connection didn't connect to anything, and we have, so we've got something new.
            if (compareAgainst == null)
            {
                return false;
            }

            Agent agent = compareAgainst.Parent;

            // we aren't connected to the same agent, so we're new.
            if (targetAgent != agent)
            {
                return false;
            }

            // we arent' connected to the same port, so we're new.
            if (compareAgainst != targetPort)
            {
                return false;
            }

            return true;

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
