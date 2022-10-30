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
        private PortTransmission[] ports;
    
      


        public SignalTransformation( int value, params PortID[] ports) : base()
        {

            ChecksRequired = false;
            MustBeLast = false;

            // give all ports this value.
            this.ports = new PortTransmission[ports.Length];
            for (int i =0; i<ports.Length; i++)
            {
                this.ports[i] = new PortTransmission(value, ports[i]);
            }

          
        }

        public SignalTransformation(params PortTransmission[] ports )
        {
            ChecksRequired = false;
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

            if(!(o is Agent))
            {
                throw new ArgumentException("A signaltransformation acts on an Agent object.");
            }

            Agent a = (Agent)o;

            a.OnlyTransmitOn(ports);
        }

        internal override Transformation Add(Transformation toAdd)
        {
            CheckType(toAdd);
            SignalTransformation other = (SignalTransformation)toAdd;


            // This is the list of our transmissions that are entirely unique to us compared to ToAdd.
            List<PortTransmission> unalike = new List<PortTransmission>(ports);


            // get the portIDs we are transmiting on.
            List<PortID> toTransmitOn = new List<PortID>();
            foreach(PortTransmission pt in ports)
            {
                toTransmitOn.Add(pt.portID);
            }

            // if we share any portIDs with toAdd, we remove them from the unique list.
            foreach(PortTransmission portTrans in other.ports)
            {

                if(toTransmitOn.Contains(portTrans.portID))
                {
                    PortTransmission? toRemove = null;
                    foreach(PortTransmission pt in unalike)
                    {
                        if(pt.portID.Equals(portTrans.portID))
                        {
                            toRemove = pt;
                            break;
                        }
                    }
                    unalike.Remove((PortTransmission)toRemove);
                }
                
            }

            List<PortTransmission> toTransmit = new List<PortTransmission>(other.ports);
            toTransmit.AddRange(unalike);

            return new SignalTransformation(toTransmit.ToArray());

        }
    }
}
