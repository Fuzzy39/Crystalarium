﻿using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rulesets.Conditions;
///
namespace CrystalCore.Model.Rulesets.Operands
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
        public BoolOperand(bool value) : base(TokenType.boolean)
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

        public ThresholdOperand(int threshold) : base(TokenType.integer)
        {
            this.threshold = threshold;
        }

        internal override Token Resolve(Agent a)
        {
            int toReturn = 0;
            PortAgent pa = (PortAgent)a;
            foreach (Port p in pa.PortList)
            {
                if (p.Status == PortStatus.receiving || p.Status == PortStatus.transceiving)
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

    public class PortValueOperand : Expression
    {
        private PortIdentifier portID;

        public PortValueOperand(PortIdentifier portID) : base(TokenType.integer)
        {

            this.portID = portID;
        }

        internal override void Initialize()
        {


            base.Initialize();
        }

        internal override Token Resolve(Agent a)
        {
            Port p = ((PortAgent)a).GetPort(portID);
            if (p.Status == PortStatus.receiving || p.Status == PortStatus.transceiving)
            {
                return new Token(ReturnType, p.ReceivingSignal.Value);
            }

            return new Token(ReturnType, 0);
        }
    }

}
