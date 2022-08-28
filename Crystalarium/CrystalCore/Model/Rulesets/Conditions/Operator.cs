using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{
    public enum OperatorType
    {
        EqualTo,
        GreaterThan,
        LessThan,
        NotEqualTo,
        Or,
        And,
        Xor
    }
    public class Operator
    {
        private OperatorType _type;

        internal OperatorType Type
        {
            get { return _type; }
        }
        public Operator(OperatorType ot)
        {
            _type = ot;
        }

        internal Token Operate(Token a, Token b)
        {
          
            if (!IsValid(a.Type,b.Type))
            {
                throw new InvalidOperationException("Invalid operation. How did this happen?");
            }
        
            return _type switch
            {
                OperatorType.EqualTo => new Token(TokenType.boolean, a.Value == b.Value),
                OperatorType.NotEqualTo => new Token(TokenType.boolean, a.Value != b.Value),

                OperatorType.GreaterThan => new Token(TokenType.boolean, ((int)a.Value) > ((int)b.Value)),
                OperatorType.LessThan => new Token(TokenType.boolean, ((int)a.Value) < ((int)b.Value)),
              
                OperatorType.Or => new Token(TokenType.boolean, (bool)a.Value || (bool)b.Value),
                OperatorType.And => new Token(TokenType.boolean, (bool)a.Value & (bool)b.Value),
                OperatorType.Xor => new Token(TokenType.boolean, (bool)a.Value ^ (bool)b.Value),

                _ => throw new InvalidOperationException("Missing a case here!"),
            };
        }

        internal bool IsValid(TokenType a, TokenType b)
        { 
            if(a!=b)
            {
                if(_type==OperatorType.NotEqualTo || _type == OperatorType.EqualTo)
                {
                    return true;
                }

                return false;
            }

            if(a == TokenType.boolean)
            {
                if(_type == OperatorType.Or || _type == OperatorType.And || _type == OperatorType.Xor)
                {
                    return true;
                }

                return false;
            }

            if(_type == OperatorType.GreaterThan || _type == OperatorType.LessThan)
            {
                return true;
            }

            return false;
        }

        internal TokenType ReturnType()
        {

            return TokenType.boolean;
        }
    }
}
