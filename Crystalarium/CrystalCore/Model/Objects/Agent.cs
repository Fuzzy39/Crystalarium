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

        protected List<TransformationRule> _activeRules;
    
        private PortInterface portInterface;

        private bool updatedSignalsThisStep;
        // properties 

       

        public AgentType Type
        {
            get => _type;
        }


        public List<TransformationRule> ActiveRules
        {
            get { return _activeRules; }
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

            updatedSignalsThisStep = false;

           // do the default thing.
           _activeRules = new List<TransformationRule>();
            _activeRules.Add(Type.DefaultState);
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
            _activeRules = DetermineState();

        }

        


        private List<TransformationRule> DetermineState()
        {
            List<TransformationRule> toReturn = new List<TransformationRule>();

            foreach (TransformationRule state in Type.States)
            {
                
                // check if we meet the requirements
                if (state.SatisfiesRequirements(this))
                {

                    toReturn.Add(state);
                }
            }

            if (toReturn.Count == 0)
            {
                toReturn.Add( Type.DefaultState);
            }

            return toReturn;
        }

        private List<Transformation> GetTransformations()
        {
            List<Transformation> toReturn = new List<Transformation>();
            foreach(TransformationRule tr in _activeRules)
            {
                toReturn.AddRange(tr.Transformations);
            }
            return Compact(toReturn);
                
        }

        // add all transformations that can be added.
        private List<Transformation> Compact(List<Transformation> list)
        {

            if (list.Count == 0) { return list; }

            List<Transformation> toReturn = new List<Transformation>();
            Transformation t = list[0];
            bool compacted = false;


            foreach(Transformation lookat in list)
            {
                if (lookat == t) { continue; }

                // compact add together any transformations that can be added to ours.
                if(t.GetType()==lookat.GetType())
                {
                    t = t.Add(lookat);
                    compacted = true;
                    continue;

                }
                toReturn.Add(lookat);

            }
            
            // stick ours back on to the end.
            toReturn.Add(t);

            if(compacted)
            {
                return Compact(toReturn);
            }

            return toReturn;

        }
        /// <summary> 
        /// Runs through transformations of this agent type.
        /// </summary>
        /// <param name="a"></param>
        internal void Execute()
        {
            
            List<Transformation> transformations = GetTransformations();
            foreach(Transformation tf in transformations)
            {
                tf.Transform(this);
            }

            if(!updatedSignalsThisStep)
            {
                OnlyTransmitOn(new PortTransmission[0]);
            }

            updatedSignalsThisStep = false;

        }

        internal void Update()
        {
            if(!portInterface.StatusHadChanged)
            {
                return;
            }
           
            Execute();

        }

        internal void OnlyTransmitOn(PortTransmission[] pts)
        {
            List<Port> ports = new List<Port>();
            foreach(PortTransmission pt in pts)
            {
                ports.Add(GetPort(pt.portID));
            }

            portInterface.OnlyTransmitOn(ports, pts);
            updatedSignalsThisStep = true;
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
         
            _activeRules.Clear();
            _activeRules.Add(Type.DefaultState);

        }

        internal Port GetPort(PortID portID)
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
