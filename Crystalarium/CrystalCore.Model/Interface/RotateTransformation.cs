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

        public RotateTransformation(RotationalDirection direction) 
        {
            ForrbiddenInDefaultState = true;
            MustBeLast = false;
            this.direction = direction;
        }

        internal override void Validate(AgentType at)
        {
          // nothing can go wrong, that's good.
        }
       

        internal override void Transform(object o)
        {
            if(!(o is Agent))
            {
                throw new ArgumentException("o must be a ");
            }
            Agent a = (Agent)o;
            a.Rotate(direction);
        }

      
    }
}
