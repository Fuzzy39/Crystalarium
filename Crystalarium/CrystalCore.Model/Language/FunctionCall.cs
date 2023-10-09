using CrystalCore.Util;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Language
{

    /// <summary>
    /// A Condition describes the situation around an agent for it to qualify for a certain state.
    /// </summary>
    public class FunctionCall : Expression
    {
        // wowee, lookit me!

        // Fields

        private List<Expression> parameters;


        private Operator _operation;

        // Properties
        public List<Expression> Parameters
        {
            get { return new List<Expression>(parameters); }
        }


        public Operator Operation
        {
            get { return _operation; }
        }

        // Constructors
        public FunctionCall(Operator operation, params Expression[] parameters) : base(operation.ReturnType())
        {
            this.parameters = new List<Expression>(parameters);
            _operation = operation;
        }


        // Methods
        public override void Initialize()
        {
            try
            {
                if (parameters.Count < 2)
                {
                    throw new InitializationFailedException("Too few operands. Expected at least two.");
                }

                TokenType? last = null;
                foreach (Expression expression in parameters)
                {
                    if (expression == null)
                    {
                        throw new InitializationFailedException("An Operand was null.");
                    }

                    expression.Initialize();
                    if (last == null)
                    {
                        last = expression.ReturnType;
                        continue;
                    }

                    if (!Operation.IsValid((TokenType)last, expression.ReturnType))
                    {
                        throw new InitializationFailedException("Type Mismatch. Types " + last + " and " + expression.ReturnType + " are not compatable" +
                       "with " + _operation);
                    }

                    last = Operation.ReturnType();

                }


            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("A FunctionCall failed to initialize:" + MiscUtil.Indent(e.Message));
            }

            base.Initialize();
        }


        internal override Token Resolve(object context)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("This Condition must be initalized before it may be resolved.");
            }

            List<Token> tokens = new List<Token>();
            foreach (Expression expr in parameters)
            {
                tokens.Add(expr.Resolve(context));
            }

            Token prev = tokens[0];
            for (int i = 1; i < tokens.Count; i++)
            {
                prev = _operation.Operate(prev, tokens[i]);
            }

            return prev;
        }
    }
}
