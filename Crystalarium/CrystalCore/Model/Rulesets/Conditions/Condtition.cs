using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{

    /// <summary>
    /// A Condition describes the situation around an agent for it to qualify for a certain state.
    /// </summary>
    public class Condition : Expression
    {
        // wowee, lookit me!

        // Fields
        private Expression _first;
        private Expression _second;

        private Operator _operation;

        // Properties
        public Expression First
        {
            get { return _first; }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Condition may not be modified after engine is initallized.");
                }
                _first = value;
            }

        }

        public Expression Second
        {
            get { return _second; }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Condition may not be modified after engine is initallized.");
                }
                _second = value;
            }
        }

        public Operator Operation
        {
            get { return _operation; }
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Condition may not be modified after engine is initallized.");
                }
                _operation = value;
            }
        }

        // Constructors
        public Condition(AgentType at) : base(TokenType.boolean, at) { }


        // Methods
        internal override void Initialize()
        {
            try
            {
                if (_first == null || _second == null)
                {
                    throw new InitializationFailedException("An Operand was null.");
                }

                _first.Initialize();
                _second.Initialize();
               
                if(_operation== null)
                {
                    throw new InitializationFailedException("The Operator was null.");
                }

                if (!_operation.IsValid(_first.ReturnType, _second.ReturnType) )
                {
                    throw new InitializationFailedException("Type Mismatch. Types " + _first.ReturnType + " and " + _second.ReturnType + " are not compatable" +
                        "with " + _operation.Type);
                }
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("A Condition failed to initialize:" + Util.Util.Indent(e.Message));
            }

            base.Initialize();
        }


        internal override Token Resolve(Agent agent)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("This Condition must be initalized before it may be resolved.");
            }
            Token a = _first.Resolve(agent);
            Token b = _second.Resolve(agent);
            return _operation.Operate(a, b);
        }
    }
}
