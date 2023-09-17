using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    internal class RotateTransformation : ITransformation
    {

        public bool ForrbiddenInDefaultState
        {
            get
            {
                return true;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return false;
            }
        }


        private RotationalDirection direction;

        public RotateTransformation(RotationalDirection direction) 
        {
            
            this.direction = direction;
        }

        public void Validate(AgentType at)
        {
          // nothing can go wrong, that's good.
        }
       

        public Transform CreateTransform(Agent a)
        {
            
            return a => a.Rotate(direction);
        }

      
    }
}
