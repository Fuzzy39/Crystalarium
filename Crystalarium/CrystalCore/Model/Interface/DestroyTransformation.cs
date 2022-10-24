using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
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
