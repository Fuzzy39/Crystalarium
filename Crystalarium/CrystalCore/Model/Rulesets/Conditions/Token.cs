﻿using System;
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
        private int _value;

        public object Value
        {
            get 
            {
                if(Type == TokenType.boolean)
                {
                    return _value > 0;
                }

                return _value;
                    
            }

            set
            {
                if(value is bool & Type==TokenType.boolean)
                {
                    this._value = ((bool)value) ? 1 : 0;
                    return;
                }

                if (value is int & Type == TokenType.integer)
                {
                    this._value = (int)value;
                    return;
                }

                throw new ArgumentException("value must be of boolean or integer type.");
            }
        }

        internal Token(TokenType type, object value)
        {
            Type = type;
            _value = 0;
            Value = value;
        }


    }
}
