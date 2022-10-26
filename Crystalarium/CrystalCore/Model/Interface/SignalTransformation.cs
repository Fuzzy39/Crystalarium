using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    /// <summary>
    /// A transformation in which an agent begins transmitting.
    /// </summary>
    public class SignalTransformation: Transformation
    {
        private PortIdentifier[] ports;
        private int value;
        private bool transmit; // whether this transformation results in the agent transmitting or stopping transmission.


        public SignalTransformation( int value, bool transmit, params PortIdentifier[] ports) : base()
        {
            ChecksRequired = false; 
            MustBeLast = false;

            this.value = value;
            this.ports = ports;
            this.transmit = transmit;
        }

        internal override void Validate(AgentType at)
        {
            foreach (PortIdentifier portID in ports)
            {
                if (!portID.CheckValidity(at))
                {
                    throw new InitializationFailedException(
                        "Transmit Transformation: Port ID: " + portID + " is not valid for AgentType '" + at.Name + "'.");
                }
            }

        }

        internal override void Transform(object o)
        {

            if(!(o is Agent))
            {
                throw new ArgumentException("A signaltransformation acts on an Agent object.");
            }

            Agent a = (Agent)o;

            foreach (PortIdentifier portID in ports)
            {
                if (transmit)
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
