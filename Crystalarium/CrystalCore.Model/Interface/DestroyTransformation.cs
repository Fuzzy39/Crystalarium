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
            ForrbiddenInDefaultState = true;
            MustBeLast = true;
        }


        internal override void Transform(Agent a)
        { 
            a.Destroy(); // hopefull this is the right way to do things.
        }

        internal override void Validate(AgentType at)
        {
            
        }

    }
}
