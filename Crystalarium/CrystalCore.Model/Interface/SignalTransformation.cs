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
    public class SignalTransformation: Transformation
    {
        private PortID[] ports;
        private Expression value;

        private PortTransmission[] constantPorts;


        public SignalTransformation( Expression value, params PortID[] ports) : base()
        {

            ChecksRequired = false;
            MustBeLast = false;

            // give all ports this value.
            this.ports = ports;
            this.value = value;
            constantPorts = null;
                
          
          
        }

        
        public SignalTransformation(params PortTransmission[] ports )
        {
            ChecksRequired = false;
            MustBeLast = false; 

            this.ports = null;
            this.value = null;
            constantPorts = ports;  
        }


        public SignalTransformation(int value, params PortID[] ports)
        {
            ChecksRequired = false;
            MustBeLast = false;

            this.ports = null;
            this.value = null;

            constantPorts = new PortTransmission[ports.Length]; 
            for(int i = 0; i < ports.Length; i++)
            {
                constantPorts[i] = new PortTransmission(value, ports[i]);
            }
        }


        private PortTransmission[] CalculateTransmissions(Agent a)
        {

            if(constantPorts!=null)
            {
                return constantPorts;
            }

            int value = (int)this.value.Resolve(a).Value;
            var toReturn = new PortTransmission[ports.Length];
            for (int i = 0; i < ports.Length; i++)
            {
                toReturn[i] = new PortTransmission(value, ports[i]);
            }
            return toReturn;

        }


        internal override void Validate(AgentType at)
        {
            foreach (PortID port in ports)
            {
                if (!port.CheckValidity(at))
                {
                    throw new InitializationFailedException(
                        "Signal Transformation: Port ID: " + port.ID + " is not valid for AgentType '" + at.Name + "'.");
                }
            }

            if(constantPorts!=null)
            {
                return;
            }

            try
            {
                value.Initialize();
            }
            catch(InitializationFailedException e)
            {
                throw new InitializationFailedException("A Signal Transformation was invalid:" + MiscUtil.Indent(e.Message));
            }

            if(value.ReturnType!=TokenType.integer)
            {
                throw new InitializationFailedException("Signal Transformation: Value to transmit must be an integer.");
            }

        }

        internal override void Transform(object o)
        {

            if(!(o is Agent))
            {
                throw new ArgumentException("A signaltransformation acts on an Agent object.");
            }

            Agent a = (Agent)o;
           

            a.OnlyTransmitOn(CalculateTransmissions(a));
        }

        /// <summary>
        /// This method is scary
        /// It combines the portTransmissions by port of this transform and toAdd, favoring toAdd if there is a conflict.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        internal override Transformation Add(Transformation toAdd)
        {
            CheckType(toAdd);
            SignalTransformation other = (SignalTransformation)toAdd;


            // This is the list of our transmissions that are entirely unique to us compared to ToAdd.
            List<PortTransmission> unalike = CalculateTransmissions();


            // get the portIDs we are transmiting on.
            List<PortID> toTransmitOn = new List<PortID>();
            foreach(PortTransmission pt in constantPorts)
            {
                toTransmitOn.Add(pt.portID);
            }

            // if we share any portIDs with toAdd, we remove them from the unique list.
            foreach(PortTransmission portTrans in other.constantPorts)
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

            List<PortTransmission> toTransmit = new List<PortTransmission>(other.constantPorts);
            toTransmit.AddRange(unalike);

            return new SignalTransformation(toTransmit.ToArray());

        }
    }
}
