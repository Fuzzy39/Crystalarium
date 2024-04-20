using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication
{
    public  interface Node : Entity
    {

        public Agent Agent { get; internal set; }

        public MapObject Physical { get; }


        /// <summary>
        /// A list 8 elements long (one for each possible port facing) 
        ///  each of which being a list containing as many ports as the node has in that direction. 
        /// The second north facing port would be [0][1].
        /// </summary>
        public List<List<Port>> Ports { get; }
        
        public List<Port> PortList { get; }


        public Direction Facing { get; }

        /// <summary>
        /// whether the node's inputs changed on the last simulation step.
        /// </summary>
        public bool ChangedLastStep { get; }


        public void Rotate(RotationalDirection direction);


        /// <summary>
        /// Note: Simulation code should not get the value of ports this way, use <see cref="Node.GetStablePortValue(PortDescriptor)"/>
        /// </summary>
        /// <param name="facing">The direction the port is facing, where a port on the top of the node faces North.</param>
        /// <param name="Location">The location, in tiles, of the port.</param>
        /// <returns></returns>
        public Port GetPort(CompassPoint facing, Point Location);


        /// <summary>
        ///  Note: Simulation code should not get the value of ports this way, use <see cref="Node.GetStablePortValue(PortDescriptor)"/>    
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Port GetPort(PortDescriptor desc);


        /// <summary>
        /// Gets the value of the port at the end of the last simulation step.
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public int GetStablePortValue(PortDescriptor desc);


        /// <summary>
        ///  Should update Stable Port Values.
        /// </summary>
        public void DoSimulationStep();


        public void OnGridResize(object sender, EventArgs e);

        public void OnPortUpdated(object port, EventArgs e);



    }
}
