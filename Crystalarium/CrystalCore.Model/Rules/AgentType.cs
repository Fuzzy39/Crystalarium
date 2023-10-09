using CrystalCore.Model.Rules.Transformations;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


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

        private TransformationRule _defaultState;

        private List<TransformationRule> _rules;


        // Properties
        public Ruleset Ruleset
        {
            get => _ruleset;
        }


        public string Name
        {
            get => _name;
        }

        public Point UpwardsSize
        {
            get => _size;
        }

        public TransformationRule DefaultState
        {
            get => _defaultState;
        }

        public List<TransformationRule> Rules
        {
            get
            {
                /*if(Initialized)
                {
                    throw new InvalidOperationException("Cannot modify/access an AgentTypes states after engine is initialized.");
                }*/
                return _rules;
            }
        }



        // constructors
        internal AgentType(Ruleset rs, string name, Point size)
        {
            _ruleset = rs;
            _name = name;
            _size = size;
            _defaultState = new TransformationRule();
            _rules = new List<TransformationRule>();

        }


        // methods

        public override void Initialize()
        {
            try
            {
                if (UpwardsSize.X < 1 || UpwardsSize.Y < 1)
                {
                    throw new InitializationFailedException("Invalid size of " + UpwardsSize + ". Size must be positive.");
                }

                foreach (ITransformation tf in DefaultState.Transformations)
                {
                    if (tf.ForrbiddenInDefaultState)
                    {
                        throw new InitializationFailedException("The default state of an Agent cannot perform a '" + tf.GetType().ToString() + "'.");
                    }
                }

                DefaultState.Validate(this);


                if (DefaultState.Requirements != null)
                {
                    throw new InitializationFailedException("The default state of an AgentType may not have any requirements.");
                }



                foreach (TransformationRule state in _rules)
                {
                    state.Validate(this);


                }

            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("AgentType '" + Name + "' Failed to Initialize:" + MiscUtil.Indent(e.Message));
            }
            base.Initialize();

        }


        // returns the appropriate size for the agent depending on direction.
        public Point GetSize(Direction d)
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


    }
}
