﻿using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using System;

namespace CrystalCore.Model.Interface
{
    /// <summary>
    /// A constant int operand.
    /// </summary>
    public class IntOperand : Expression
    {
        private int value;
        public IntOperand(int value) : base(TokenType.integer)
        {

            this.value = value;
        }
        internal override Token Resolve(object none)
        {
            return new Token(ReturnType, value);
        }

    }

    /// <summary>
    /// A constant bool operand.
    /// </summary>
    public class BoolOperand : Expression
    {
        private bool value;
        public BoolOperand(bool value) : base(TokenType.boolean)
        {

            this.value = value;
        }
        internal override Token Resolve(object none)
        {
            return new Token(ReturnType, value);
        }

    }

    /// <summary>
    /// whether the given number of ports on the agent are receiving a signal of a particular value.
    /// </summary>
    public class ThresholdOperand : Expression
    {
        private int threshold;

        public ThresholdOperand(int threshold) : base(TokenType.integer)
        {
            this.threshold = threshold;
        }

        internal override Token Resolve(object agent)
        {
            if (!(agent is object))
            {
                throw new ArgumentException("agent parameter must of of type Agent");
            }
            Agent a = (Agent)agent;
            
            int toReturn = 0;
     
            foreach (Port p in a.PortList)
            {
               
                if (p.Value >= threshold)
                {
                    toReturn++;
                }
                
            }
            return new Token(ReturnType, toReturn);
        }
    }

    public class PortValueOperand : Expression
    {
        private PortID portID;

        public PortValueOperand(PortID portID) : base(TokenType.integer)
        {

            this.portID = portID;
        }

        internal override void Initialize()
        {


            base.Initialize();
        }

        internal override Token Resolve(object agent)
        {
            if(!(agent is object))
            {
                throw new ArgumentException("agent parameter must of of type Agent");
            }
            Agent a = (Agent)agent;
            Port p = a.GetPort(portID);

            return new Token(ReturnType, p.Value);
            
        }
    }

}
