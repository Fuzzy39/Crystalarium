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
    public abstract class Agent : Entity
    {

        private AgentType _type;

        protected AgentState _state;
    
        private bool statusChanged; // whether a port started/stopped receiving this step.
        private bool statusHadChanged; // whether a port started/stopped receiging last step.
        
       

        public AgentType Type
        {
            get => _type;
        }


        public AgentState State
        {
            get { return _state; }
        }


        // Constructors
        internal Agent(Grid g, Rectangle bounds, AgentType t, Direction facing) : base(g, bounds, (t.Ruleset.RotateLock ? Direction.up:facing))
        {

            if (g.Ruleset != t.Ruleset)
            {
                throw new InvalidOperationException("Cannot add " + t.Name + " type agent of ruleset " + t.Ruleset.Name + " to grid of ruleset " + g.Ruleset.Name + ".");
            }

            _type = t;
            statusChanged = true; // this is true at initialization so the agent can do things of it's own accord when it is created

            Init();

            // do the default thing.
            _state = Type.DefaultState;
            Execute();

        }

        protected abstract void Init();
        

        protected void StatusChanged()
        {
            statusChanged = true;
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

            StatusChanged();
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
            statusHadChanged = statusChanged;
            statusChanged = false;
            if(!statusHadChanged)
            {
                return;
            }

            PreserveValues();
            _state = DetermineState();

        }

        protected abstract void PreserveValues();


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
                Token t = state.Requirements.Resolve(this);
                if ((bool)t.Value)
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
            if(!statusHadChanged)
            {
                return;
            }
           
            Execute();

        }
    }
}
