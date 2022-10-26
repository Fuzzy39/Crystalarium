using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;


namespace CrystalCore.Model.Rules
{
    public class AgentType : InitializableObject
    {
        /*
         * An AgentType is responsible for creating agents, and defines how a particular agent will behave.
         * It also defines  what agentRenderers are used for this agent.
         * 
         */


        private Ruleset _ruleset; // the ruleset this agent type belongs to.

        private string _name; // the human readable name of this agent type.

        private Point _size; // the size of this AgentType, when pointing up.

        // and a list of states. 

        private AgentState _defaultState;

        private List<AgentState> _states;


        // Properties
        public Ruleset Ruleset
        {
            get => _ruleset;
        }


        public string Name
        {
            get => _name;
        }

        public Point Size
        {
            get => _size;
        }

        public AgentState DefaultState
        {
            get => _defaultState;
        }

        public List<AgentState> States
        {
            get
            {
                /*if(Initialized)
                {
                    throw new InvalidOperationException("Cannot modify/access an AgentTypes states after engine is initialized.");
                }*/
                return _states;
            }
        }



        // constructors
        internal AgentType(Ruleset rs, string name, Point size)
        {
            _ruleset = rs;
            _name = name;
            _size = size;
            _defaultState = new AgentState();
            _states = new List<AgentState>();

        }


        // methods

        internal override void Initialize()
        {
            try
            {
                if (Size.X < 1 || Size.Y < 1)
                {
                    throw new InitializationFailedException("Invalid size of " + Size + ". Size must be positive.");
                }

                foreach (Transformation tf in DefaultState.Transformations)
                {
                    if(tf.ChecksRequired)
                    {
                        throw new InitializationFailedException("The default state of an Agent cannot perform a '" + tf.GetType().ToString() + "'.");
                    }
                }

                DefaultState.Initialize();
                DefaultState.Validate(this);
                if (DefaultState.Requirements != null)
                {
                    throw new InitializationFailedException("The default state of an AgentType may not have any requirements.");
                }



                foreach (AgentState state in _states)
                {
                    state.Initialize();
                    state.Validate(this);
                }

            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("AgentType '" + Name + "' Failed to Initialize:" + Util.Util.Indent(e.Message));
            }
            base.Initialize();

        }

        public Agent createAgent(Grid g, Point pos, Direction d)
        {

            if (!Initialized)
            {
                throw new InvalidOperationException("This AgentType cannot be used before it is initialized. Call Engine.Initialize().");
            }

            if (g.Ruleset != Ruleset)
            {
                throw new InvalidOperationException("Cannot add " + Name + " type agent of ruleset " + Ruleset.Name + " to grid of ruleset " + g.Ruleset.Name + ".");
            }

            Rectangle bounds = new Rectangle(pos, GetSize(d));

            // A safe assumption, for now.
            return new PortAgent(g, bounds, this, d);
        }


        // returns the appropriate size for the agent depending on direction.
        internal Point GetSize(Direction d)
        {
            if (d.IsVertical())
            {
                return _size;
            }
            else
            {
                return new Point(_size.Y, _size.X);
            }
        }



        // returns whether an agent of type at can be placed at a location.
        public bool IsValidLocation(Grid g, Point location, Direction facing)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("This AgentType cannot be used before it is initialized. Call Engine.Initialize().");
            }

            Rectangle bounds = new Rectangle(location, GetSize(facing));
            if (g.Bounds.Contains(bounds))
            {
                if (g.EntitiesWithin(bounds).Count == 0)
                {
                    return true;
                }

            }

            return false;
        }



    }
}
