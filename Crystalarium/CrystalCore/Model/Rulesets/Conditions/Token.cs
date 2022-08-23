using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{
    internal enum TokenType
    {
        boolean,
        integer
    }

    internal class Token
    {
        public TokenType Type;
        private int value;

        public object Value
        {
            get 
            {
                if(Type == TokenType.boolean)
                {
                    return value > 1;
                }

                return value;
                    
            }

            set
            {
                if(value is bool & Type==TokenType.boolean)
                {
                    this.value = (bool)value ? 1 : 0;
                }

                if (value is int & Type == TokenType.integer)
                {
                    this.value = (int)value;
                }

                throw new ArgumentException("value must be of boolean or integer type.");
            }
        }

        internal Token(TokenType type, object value)
        {
            Type = type;
            value = 0;
            Value = value;
        }


    }
}
