using CrystalCore.Model.Communication;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System.Drawing;

namespace CrystalCore.Model.Simulation
{
    // oh boy, do we have a lot in store for you!


    /// <summary>
    /// Agents are the 'building blocks' of Crystalarium's simulation. They are user code's main interface with the simulation as it's running.
    /// Agents are objects on the map. Each simulation step, an agent will look at its inputs and use its 'agency' to decide what to do.
    /// (according to the CARL code stored in the agent type.)
    /// </summary>
    public interface Agent
    {

        public AgentType Type { get; }


        // this could maybe be private (i.e., not in the interface)
        public Node Node { get; }

        public void Rotate(RotationalDirection rd);

        internal void Mutate(AgentType type);

        internal void TransmitOn(PortTransmission[] transmissions);
     

        public void Destroy();

        internal void PrepareSimulationStep();

        internal void DoSimulationStep();



        public string ToString()
        {
            if(Type == null)
            {
                return "Agent { Destroyed }";
            }

            return "Agent { Type:\"" + Type.Name + "\", Location:" + Node.Physical.Bounds.Location + ", Facing:" + Node.Facing + " }";
        }


    }
}
