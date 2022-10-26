// namespace
using CrystalCore.Model.Language;

using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rules
{
    /// <summary>
    /// An agent State is a state that an agent of a particular type can be in. it has conditions for the agent to meet to be in the state, as well as transformations
    /// that describe what an agent does while in the state.
    /// </summary>
    public class AgentState : InitializableObject
    {
        // fields

        private Expression _requirements;

        private List<Transformation> _transformations;

        // properties

        public Expression Requirements
        {
            get { return _requirements; }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("An Agent State's condition may not be changed after the engine has been initialized.");
                }
                _requirements = value;
            }
        }

        public List<Transformation> Transformations
        {
            get
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("An Agent State's Actions/Transformations may not be edited or viewed after the engine has been initialized.");
                }

                return _transformations;

            }

        }


        // Constructors
        public AgentState()
        {
            _transformations = new List<Transformation>();
        }

        // Methods

        internal override void Initialize()
        {

            try
            {

                // the condition of an agentstate can be null if it is the default state of the agent.
                if (Requirements != null)
                {
                    Requirements.Initialize();
                    if (Requirements.ReturnType != TokenType.boolean)
                    {
                        throw new InitializationFailedException("AgentState Requirement Expression must return a boolean.");
                    }

                }

                

                // an agentstate can have no transformations, and be inert, if it wishes.
                bool agentDestroyed = false;
                foreach (Transformation tf in Transformations)
                {

                    if (agentDestroyed)
                    {
                        throw new InitializationFailedException("MutateTransformation and DestroyTransformation must be the last transformation that an agent undergoes.");
                    }

             
                   

                    if (tf.MustBeLast)
                    {
                        agentDestroyed = true;
                    }
                }


            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("An AgentState failed to initialize:" + Util.Util.Indent(e.Message));
            }

            base.Initialize();
        }

        internal void Validate(AgentType at)
        {
            try
            {
                foreach (Transformation tf in Transformations)
                {

                    tf.Validate(at);
                }
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("An AgentState failed to initialize:" + Util.Util.Indent(e.Message));
            }
        }

        

    }
}
