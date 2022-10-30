using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    internal class DestroyTransformation : Transformation
    {

        public DestroyTransformation() : base()
        {
            ChecksRequired = true;
            MustBeLast = true;
        }

        internal override Transformation Add(Transformation toAdd)
        {
            CheckType(toAdd);
            return this; // not like it matters.
        }

        internal override void Transform(object o)
        {
            
            if (!(o is Agent))
            {
                throw new ArgumentException("o must be a ");
            }
            Agent a = (Agent)o;
            
            a.Destroy(); // hopefull this is the right way to do things.
        }

        internal override void Validate(AgentType at)
        {
            
        }

    }
}
