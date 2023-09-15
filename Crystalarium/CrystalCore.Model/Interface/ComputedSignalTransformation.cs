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
    public class ComputedSignalTransformation : Transformation
    {


        private PortID[] ports;
        private Expression value;

        public ComputedSignalTransformation(Expression value, params PortID[] ports) : base()
        {

            ForrbiddenInDefaultState = false;
            MustBeLast = false;

            // give all ports this value.
            this.ports = ports;
            this.value = value;


        }

        internal override void Transform(Agent a)
        {
           var transformation = Resolve(a);
           transformation.Transform(a);

        }


        private ConstantSignalTransformation Resolve(Agent a)
        {
            int val = (int)value.Resolve(a).Value; // we checked earlier, this cast won't fail
            return new ConstantSignalTransformation(val, ports); // note, we aren't running validate on this, but it's fine in this case.
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
