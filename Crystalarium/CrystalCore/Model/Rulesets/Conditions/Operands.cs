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
        private CompassPoint cp;
        private int portID;

        public PortValueOperand(AgentType at, CompassPoint cp, int portID) : base(TokenType.integer, at)
        {
            this.cp = cp;
            this.portID = portID;
        }

        internal override void Initialize()
        {
            string errorMessage = "Port Value Operand attempted to target port " + cp + portID + " that agent type '" + AgentType.Name + "' does not have.";
            if (AgentType.Ruleset.DiagonalSignalsAllowed & cp.IsDiagonal())
            {
                throw new InitializationFailedException(errorMessage);
            }

            Direction d = (Direction)cp.ToDirection();

            if (d.IsVertical() & AgentType.Size.X <= portID)
            {
                throw new InitializationFailedException(errorMessage);
            }

            if (d.IsHorizontal() & AgentType.Size.Y <= portID)
            {
                throw new InitializationFailedException(errorMessage);
            }
            base.Initialize();
        }

        internal override Token Resolve(Agent a)
        {
            Signal s = a.Ports[(int)cp][portID].ReceivingSignal;
            int toReturn = 0;
            if(s!=null)
            {
                toReturn = s.Value;
            }

            return new Token(ReturnType, toReturn);
        }
    }

}
