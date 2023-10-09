// namespace
using CrystalCore.Model.Language;
using CrystalCore.Model.Rules.Transformations;
using CrystalCore.Util;
using System.Collections.Generic;

namespace CrystalCore.Model.Rules
{
    /// <summary>
    /// A TransformationRule describes a series of Transformations that happen to an agent if it satisfies certain requirements (a boolean Expression).
    /// </summary>
    public class TransformationRule
    {
        // fields

        private Expression _requirements;
        private List<ITransformation> _transformations;


        public Expression Requirements
        {
            get { return _requirements; }
            set
            {
                _requirements = value;
            }
        }

        public List<ITransformation> Transformations
        {
            get
            {

                return _transformations;

            }

        }

        // Constructors
        public TransformationRule()
        {
            _transformations = new List<ITransformation>();
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
                foreach (ITransformation tf in Transformations)
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

            foreach (ITransformation tf in Transformations)
            {

                tf.Validate(at);
            }


        }



    }
}
