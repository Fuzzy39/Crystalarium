// namespace
using CrystalCore.Model.Language;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rules
{
    /// <summary>
    /// A TransformationRule describes a series of Transformations that happen to an agent if it satisfies certain requirements (a boolean Expression).
    /// </summary>
    public class TransformationRule
    {
        // fields

        private Expression _requirements;
        private List<Transformation> _transformations;


        public Expression Requirements
        {
            get { return _requirements; }
            set
            {
                _requirements = value;
            }
        }

        public List<Transformation> Transformations
        {
            get
            {

                return _transformations;

            }

        }

        // Constructors
        public TransformationRule()
        {
            _transformations = new List<Transformation>();
        }

        internal bool SatisfiesRequirements(object context)
        {
            if (_requirements == null)
            {
                return true;
            }

            return (bool)_requirements.Resolve(context).Value;
        }

        internal void Validate(AgentType at)
        {
            try
            {
                ValidateChildren(at);

                // the condition of an agentstate can be null if it is the default state of the agent.
                if (Requirements != null)
                {
                    Requirements.Initialize();
                    if (Requirements.ReturnType != TokenType.boolean)
                    {
                        throw new InitializationFailedException("Transformation requirements must evaluate to a boolean.");
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
                throw new InitializationFailedException("A Transformation Rule was invalid:" + MiscUtil.Indent(e.Message));
            }
        }


        private void ValidateChildren(AgentType at)
        {

            foreach (Transformation tf in Transformations)
            {

                tf.Validate(at);
            }


        }



    }
}
