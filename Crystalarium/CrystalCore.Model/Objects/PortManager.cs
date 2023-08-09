using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    /// <summary>
    ///  A port interface deals with all the dirty work of managing ports that an agent doesn't want to do.
    /// </summary>
    internal class PortManager
    {


        private AgentType type;
        private Agent parent;

        private bool statusChanged; // whether a port started/stopped receiving this step.
        private bool statusHadChanged; // whether a port started/stopped receiging last step.

        private List<List<Port>> _ports; // this agent's ports, stored by direction relative to the agent.
        private List<List<int>> _stalePortValues; // the value each port was receiving at the end of the last simulation step.

        internal List<List<Port>> Ports { get { return _ports; } }
        internal List<Port> PortList
        {
            get
            {
                List<Port> toReturn = new List<Port>();
                foreach (List<Port> ports in Ports)
                {
                    toReturn.AddRange(ports);
                }

                return toReturn;
            }

        }

        internal bool StatusHadChanged {  get { return statusHadChanged; } }


        internal PortManager(AgentType at, Agent parent)
        {
            statusChanged = true; // this is true at initialization so the agent can do things of it's own accord when it is created

            type = at;
            this.parent = parent;

            // create ports (what a helpful comment)
            CreatePorts();

           
        }

        internal void OnCreation()
        {
            // get intersecting connections.
            List<Connection> intersecting = new List<Connection>();
            Rectangle bounds = parent.Bounds;
            bounds.Inflate(1, 1);
            List < Chunk > chunks = parent.Map.ChunksInBounds(Rectangle.Intersect(bounds, parent.Map.Bounds));

            foreach (Chunk ch in chunks)
            {
                foreach (ChunkMember chm in ch.MembersWithin)
                {
                    if (chm.Bounds.Intersects(bounds))
                    {
                        if (chm is Connection && !intersecting.Contains((Connection)chm))
                        {   
                            intersecting.Add((Connection)chm);
                        }
                    }
                }
            }

            // get ports of those connections
            List<Port> toUpdate = new List<Port>();
            foreach (Connection conn in intersecting)
            {
                if (conn.PortA != null && !conn.PortA.Destroyed)
                {
                    toUpdate.Add(conn.PortA);
                }

                if (conn.PortB != null && !conn.PortB.Destroyed)
                {
                    toUpdate.Add(conn.PortB);
                }
            }

            // update all ports that require it
            foreach (Port p in toUpdate)
            {
                p.Update();
            }

            foreach (Port p in PortList)
            {
                p.Update();
            }

        }



        // PORT RELATED
        private void CreatePorts()
        {
            _ports = new List<List<Port>>();


            // should always be 8, but who knows, maybe they'll invent more directions on a compass.
            int length = Enum.GetNames(typeof(CompassPoint)).Length;

            for (int i = 0; i < length; i++)
            {
                List<Port> portList = new List<Port>();
                List<int> staleValues = new List<int>();

                // add relevant ports to this list.
                CompassPoint facing = (CompassPoint)i;

                if (facing.IsDiagonal())
                {
                    _ports.Add(portList);

                    if (!type.Ruleset.DiagonalSignalsAllowed)
                    {
                        continue;
                    }

                    // create a diagonal port.
                    Port p = new Port(facing, 0, parent);
                    portList.Add(p);
                   
                  
                    continue;
                }

                portList = CreateOrthagonalPorts(facing, portList);
                _ports.Add(portList);

            }

            CreateStaleVals();
        }



        // facing is direction of ports to be made relative to upwards face of agent
        private List<Port> CreateOrthagonalPorts(CompassPoint facing, List<Port> ports)
        {
            // create orthagonal ports. Vertical first, diagonal after.

            if (((Direction)facing.ToDirection()).IsVertical())
            {
                for (int j = 0; j < type.UpwardsSize.X; j++)
                {
                    Port p = new Port(facing, j, parent);
                    ports.Add(p);
                  
                   
                }
                return ports;
            }

            for (int j = 0; j < type.UpwardsSize.Y; j++)
            {
                Port p = new Port(facing, j, parent);
                ports.Add(p);


            }

            return ports;

        }

        private void CreateStaleVals()
        {
            _stalePortValues = new List<List<int>>();
            foreach (List<Port> list in _ports)
            {
                List<int> staleList = new List<int>();
                foreach (Port port in list)
                {
                    staleList.Add(0);
                }
                _stalePortValues.Add(staleList);
            }
        }

        internal void Destroy()
        {

            List<Port> toUpdate = new List<Port>();

            foreach (List<Port> ports in _ports)
            {
                foreach (Port port in ports)
                {
                    // we check if the port has a connection because this method could be called while the grid is reseting.
                    // if it is, connections between chunks will get destroyed, and ports might be put in states w
                    if(port.HasConnection && port.ConnectedTo != null)
                    {
                        toUpdate.Add(port.ConnectedTo);
                    }
                    port.Destroy();

                }
            }

            foreach (Port p in toUpdate)
            {
                p.Update();
            }

            _ports.Clear();

        }



        internal void Rotate()
        {
            List<Port> toUpdate = new List<Port>();

            foreach (Port port in PortList)
            {
                
                if (port.ConnectedTo != null)
                {
                    toUpdate.Add(port.ConnectedTo);
                }

                port.DestroyConnection();
 
            }


            foreach (Port p in toUpdate)
            {
                p.Update();
            }

            foreach(Port p in PortList)
            {
                  p.Update();
            }


            statusChanged = true;
        }


        internal void StatusChanged()
        {
            statusChanged = true;
        }

        internal void PreserveState()
        {
            statusHadChanged = statusChanged;
            statusChanged = false;
            if (!statusHadChanged)
            {
                return;
            }

            PreserveValues();
        }


        protected void PreserveValues()
        {
            // loop through all directions
            for (int i = 0; i < _ports.Count; i++)
            {
                List<Port> list = _ports[i];

                // and every port in that direction...
                for (int j = 0; j < list.Count; j++)
                {
                    // and update its stale value.
                    Port p = list[j];
               
                    _stalePortValues[i][j] = p.Value;
                    
                }
            }
        }

        internal void OnlyTransmitOn(List<Port> ports, PortTransmission[] transmits)
        {
            

            List<Port> toTurnOff = new List<Port>(PortList);
            for(int i = 0; i<ports.Count; i++)
            {
                toTurnOff.Remove(ports[i]);
                ports[i].Transmit(transmits[i].value);
            }

            foreach(Port p in toTurnOff)
            {
                p.Transmit(0);
            }

        }
    }
}
