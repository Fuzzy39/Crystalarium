using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Transformations
{
    /// <summary>
    /// A transformation in which an agent begins transmitting.
    /// </summary>
    public class SignalTransformation : Transformation
    {
        private PortIdentifier[] ports;
        private int value;
        private bool transmit; // whether this transformation results in the agent transmitting or stopping transmission.


        public SignalTransformation(AgentType at, int value, bool transmit, params PortIdentifier[] ports) : base(at)
        {
            this.value = value;
            this.ports = ports;
            ChangesAgent = false;
            this.transmit = transmit;
        }

        internal override void Initialize()
        {
            foreach (PortIdentifier portID in ports)
            {
                if (!portID.CheckValidity(AgentType))
                {
                    throw new InitializationFailedException(
                        "Transmit Transformation: Port ID: " + portID + " is not valid for AgentType '" + AgentType.Name + "'.");
                }
            }

            base.Initialize();

        }

        internal override void Transform(Agent a)
        {
            base.Transform(a);
          
            foreach (PortIdentifier portID in ports)
            {
                if(transmit)
                {
                    Port p = ((PortAgent)a).GetPort(portID);
                    //if (a.GetPortValue(portID) == 0  || p is FullPort)
                    {
                        p.Transmit(value);
                        continue;
                    }

                }
                else
                {
                    ((PortAgent)a).GetPort(portID).StopTransmitting();
                }
            }
        }
    }
}
