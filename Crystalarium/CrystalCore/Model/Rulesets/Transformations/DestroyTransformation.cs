using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Transformations
{
    internal class DestroyTransformation : Transformation
    {

        public DestroyTransformation(AgentType at) : base(at)
        {
            ChangesAgent = true;   
        }

        internal override void Transform(Agent a)
        {
            base.Transform(a);
            a.Destroy(); // hopefull this is the right way to do things.
        }
    }
}
