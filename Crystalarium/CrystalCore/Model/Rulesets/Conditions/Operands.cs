using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{
    /// <summary>
    /// A constant int operand.
    /// </summary>
    public class IntOperand : Expression
    {
        private int value;
        public IntOperand(AgentType at, int value) : base(TokenType.integer, at)
        {

            this.value = value;
        }
        internal override Token Resolve(Agent a)
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
        public BoolOperand(AgentType at, bool value) : base(TokenType.boolean, at)
        {

            this.value = value;
        }
        internal override Token Resolve(Agent a)
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

        public ThresholdOperand(AgentType at, int threshold) : base(TokenType.integer, at)
        {
            this.threshold = threshold;
        }

        internal override Token Resolve(Agent a)
        {
            int toReturn = 0;
            foreach (Port p in a.PortList)
            {
                if (p.IsReceiving)
                {
                    if (p.ReceivingSignal.Value >= threshold)
                    {
                        toReturn++;
                    }
                }
            }
            return new Token(ReturnType, toReturn);
        }
    }

    internal class PortValueOperand : Expression
    {
        private PortIdentifier portID;

        public PortValueOperand(AgentType at, PortIdentifier portID) : base(TokenType.integer, at)
        {
            
            this.portID = portID;
        }

        internal override void Initialize()
        {
            if (!portID.CheckValidity(AgentType))
            {
                throw new InitializationFailedException("Port Value Operand attempted to target port " + portID +
                    " that agent type '" + AgentType.Name + "' does not have.");
            }
           
            base.Initialize();
        }

        internal override Token Resolve(Agent a)
        {
            Signal s = a.Ports[(int)portID.Facing][portID.ID].ReceivingSignal;
            int toReturn = 0;
            if(s!=null)
            {
                toReturn = s.Value;
            }

            return new Token(ReturnType, toReturn);
        }
    }

}
