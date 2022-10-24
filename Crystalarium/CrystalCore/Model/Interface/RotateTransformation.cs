using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    internal class RotateTransformation : Transformation
    {



        private RotationalDirection direction;

        public RotateTransformation(AgentType at, RotationalDirection direction) : base(at)
        {
            this.direction = direction;
        }

        internal override void Initialize()
        {

            base.Initialize();

        }

        internal override void Transform(Agent a)
        {
            base.Transform(a);
            a.Rotate(direction);
        }
    }
}
