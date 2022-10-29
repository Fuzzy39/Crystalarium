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
    /// Agents are entities that actively participate in the simulation. It deterimines its state every stepa and acts accordingly.
    /// </summary>
    public class Agent : Entity
    {

        private AgentType _type;

        protected AgentState _state;
    
       private PortInterface portInterface;

        // properties 

       

        public AgentType Type
        {
            get => _type;
        }


        public AgentState State
        {
            get { return _state; }
        }

        internal List<List<Port>> Ports
        {
            get { return portInterface.Ports; }
        }

        internal List<Port> PortList
        {
            get { return portInterface.PortList; }
        }

        // Constructors
        public Agent(Map g, Point location, AgentType t, Direction facing) : base(g, new Rectangle(location, t.Size), (t.Ruleset.RotateLock ? Direction.up:facing))
        {

            if (g.Ruleset != t.Ruleset)
            {
                throw new InvalidOperationException("Cannot add " + t.Name + " type agent of ruleset " + t.Ruleset.Name + " to grid of ruleset " + g.Ruleset.Name + ".");
            }

            _type = t;
            portInterface = new PortInterface(t, this);
            g.UpdateSignals(new List<Chunk>(ChunksWithin));

            // if diagonal signals are allowed, then agents should not be bigger than 1 by 1
            if (Type.Ruleset.DiagonalSignalsAllowed && Bounds.Size.X * Bounds.Size.Y > 1)
            {
                throw new InvalidOperationException("The Ruleset '" + Type.Ruleset.Name + "' has specified that diagonal signals are allowed, which requires that no agents are greater than 1 by 1 in size.");
            }


            // do the default thing.
            _state = Type.DefaultState;
            Execute();

        }



        public override void Destroy()
        {

            List<Chunk> toUpdate = new List<Chunk>(ChunksWithin);
            Map g = Map;
            base.Destroy();


            portInterface.Destroy();

            g.UpdateSignals(new List<Chunk>(toUpdate));

        }


        



        public override void Rotate(RotationalDirection d)
        {

            if (Type.Ruleset.RotateLock)
            {
                Console.WriteLine("Warning: Ruleset '" + Type.Ruleset.Name + "' has Rotation Lock enabled, and Agents cannot be set facing any other direction than up. " +
               "\n    Agent '" + this.ToString() + "' has attempted to rotate " + d + ".");
                return;
            }

            base.Rotate(d);

            portInterface.StatusChanged();
            RecombobulateSignals();
        }




        public override string ToString()
        {
            return "Agent { Type:\"" + Type.Name + "\", Location:" + Bounds.Location + ", Facing:" + Facing + " }";
        }

        /// <summary>
        /// This method tranistions the current simulation step's state into the last step. This allows us to freely change the state of the grid without 
        /// causing any changes to what we are making decisions about.
        /// </summary>
        internal void PreserveState()
        {
            portInterface.PreserveState();
            _state = DetermineState();

        }

        


        private AgentState DetermineState()
        {
            
            foreach (AgentState state in Type.States)
            {
                // if a state has no requirements, it fits the bill!
                if (state.Requirements == null)
                {
                    return state;
                }

                // otherwise, check if we meet the requirements.
                
                if (state.SatisfiesRequirements(this))
                {

                    return state;
                }
            }

            return Type.DefaultState;
        }

        /// <summary> 
        /// Runs through transformations of this agent type.
        /// </summary>
        /// <param name="a"></param>
        internal void Execute()
        {

            foreach (Transformation tf in _state.Transformations)
            {
                tf.Transform(this);
            }

        }

        internal void Update()
        {
            if(!portInterface.StatusHadChanged)
            {
                return;
            }
           
            Execute();

        }


        // how often do you get to type recombobulate? not often!
        private void RecombobulateSignals()
        {
            // stop transmitting and receiving signals, then 'reboot'
            List<Chunk> toUpdate = new List<Chunk>();
            toUpdate.AddRange(ChunksWithin);


            foreach (Port p in portInterface.PortList)
            {
                if (p.Status == PortStatus.transmitting || p.Status == PortStatus.transceiving)
                {
                    int v = p.TransmittingValue;

                    p.StopTransmitting();

                    //p.Transmit(v);
                }

                if (p.Status == PortStatus.receiving)
                {
                    toUpdate.AddRange(p.ReceivingSignal.ChunksWithin);
                }
                p.StopReceiving();


            }

            Map.UpdateSignals(toUpdate);
            _state = Type.DefaultState;


        }

        internal Port GetPort(PortIdentifier portID)
        {
            if (!portID.CheckValidity(Type))
            {
                throw new InvalidOperationException("Bad PortID.");
            }
            return portInterface.Ports[(int)portID.Facing][portID.ID];
        }


       /* ///<summary>
        /// 
        /// </summary>
        /// <returns> the value this port was receiving at the end of the last simulation step.</returns>
        internal int GetPortValue(PortIdentifier portID)
        {
            if (!portID.CheckValidity(Type))
            {
                throw new InvalidOperationException("Bad PortID.");
            }

            return _stalePortValues[(int)portID.Facing][portID.ID];
        }*/
    }
}
