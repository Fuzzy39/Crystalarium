using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{

    /// <summary>
    /// A Condition describes the situation around an agent for it to qualify for a certain state.
    /// </summary>
    internal class Condtition : Resolvable
    {
        // wowee, lookit me!

        // Fields
        private Operand _first;
        private Operand _second;

        private Operation _operation;

        // Properties
        public Operand First
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

        public Operand Second
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

        public Operation Operation
        {
            get { return _operation; }
        }

        // Constructors
        public Condition()
        {
            _operation = new Operation();
        }

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
                _operation.Initialize();

                if (!_operation.IsValidType(_first.TokenType) ||
                    !_operation.IsValidType(_second.TokenType))
                {
                    throw new InitializationFailedException("Type Mismatch. Types " + _first.TokenType + " and " + _second.TokenType + " are not compatable" +
                        "with " + _operation);
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
                throw new InvalidOperationException("This Condition must be initalized before it may be resolved.")
            }
            Token a = _first.Resolve(agent);
            Token b = _second.Resolve(agent);
            return _operation.operate(a, b);
        }
    }
}
