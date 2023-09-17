using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Interface
{
    public class SignalTransformation : ITransformation
    {

        public bool ForrbiddenInDefaultState
        {
            get
            {
                return false;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return false;
            }
        }

        private PortID[] ports;
        private Expression value;

        public SignalTransformation(Expression value, params PortID[] ports)
        {

            

            // give all ports this value.
            this.ports = ports;
            this.value = value;


        }

        public SignalTransformation(int value, params PortID[] ports)
        {
            this.value = new IntOperand(value);
            this.ports = ports;
        }

        public Transform CreateTransform(Agent a)
        {
         

            return (a) =>
            {
                int val = (int)value.Resolve(a).Value;
                List<PortTransmission> toTransmit = new(ports.Length);

                // this is a cursed line for no reason at all. just iterating with an index
                { int i = 0; toTransmit.ForEach(trans => { i++; trans = new(val, ports[i]); }); }

                a.TransmitOn(toTransmit.ToArray());
            };


          

        }

        public void Validate(AgentType at)
        {

            foreach (PortID port in ports)
            {
                if (!port.CheckValidity(at))
                {                                   
                    throw new InitializationFailedException(
                        "Signal Transformation: Port ID: " + port.ID + " is not valid for AgentType '" + at.Name + "'.");
                }
            }

            try
            {
                value.Initialize();
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("A Signal Transformation was invalid:" + MiscUtil.Indent(e.Message));
            }

            if (value.ReturnType != TokenType.integer)
            {
                throw new InitializationFailedException("Signal Transformation: Value to transmit must be an integer.");
            }

        }
    }
}
