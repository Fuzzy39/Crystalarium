using CrystalCore.Model.Language;
using System;

namespace CrystalCore.Model.Rules
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
    /// The amount of ports exceeding the threshold value. If the value is positive, values must be equal to or greater than the threshold.
    /// If 0, the value must be non-zero
    /// If negative, the value must be less than or equal to the value.
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

                if (MeetsThreshold(p.Value))
                {
                    toReturn++;
                }

            }
            return new Token(ReturnType, toReturn);
        }

        private bool MeetsThreshold(int value)
        {
            if (threshold > 0)
            {
                return value >= threshold;
            }
            if (threshold == 0)
            {
                return value != 0;
            }
            return value <= 0;
        }
    }

    public class PortValueOperand : Expression
    {
        private PortID portID;

        public PortValueOperand(PortID portID) : base(TokenType.integer)
        {

            this.portID = portID;
        }

        public override void Initialize()
        {


            base.Initialize();
        }

        internal override Token Resolve(object agent)
        {
            if (!(agent is object))
            {
                throw new ArgumentException("agent parameter must of of type Agent");
            }
            Agent a = (Agent)agent;
            Port p = a.GetPort(portID);

            return new Token(ReturnType, p.Value);

        }
    }

}
