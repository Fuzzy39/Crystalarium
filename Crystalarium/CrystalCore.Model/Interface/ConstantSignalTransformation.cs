using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    /// <summary>
    /// A transformation in which an agent begins transmitting.
    /// </summary>
    public class ConstantSignalTransformation : Transformation
    {
        private PortTransmission[] ports;




        public ConstantSignalTransformation(int value, params PortID[] ports) : base()
        {

            ForrbiddenInDefaultState = false;
            MustBeLast = false;

            // give all ports this value.
            this.ports = new PortTransmission[ports.Length];
            for (int i = 0; i < ports.Length; i++)
            {
                this.ports[i] = new PortTransmission(value, ports[i]);
            }


        }

        public ConstantSignalTransformation(params PortTransmission[] ports)
        {
            ForrbiddenInDefaultState = false;
            MustBeLast = false;

            this.ports = ports;
        }

        internal override void Validate(AgentType at)
        {
            foreach (PortTransmission portTrans in ports)
            {
                if (!portTrans.portID.CheckValidity(at))
                {
                    throw new InitializationFailedException(
                        "Transmit Transformation: Port ID: " + portTrans.portID + " is not valid for AgentType '" + at.Name + "'.");
                }
            }

        }

        internal override void Transform(object o)
        {

            if (!(o is Agent))
            {
                throw new ArgumentException("A signaltransformation acts on an Agent object.");
            }

            Agent a = (Agent)o;

            a.OnlyTransmitOn(ports);
        }

      
    }
}