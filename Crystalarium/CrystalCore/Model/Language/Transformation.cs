﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Language
{
    /// <summary>
    /// A way an agent can change itself or it's environment
    /// </summary>
    public abstract class Transformation
    {
        // Transformations should set this property. default states cannot have transformations that change the agent.
      
 

        public Transformation()
        {
     
        }

        internal abstract void Transform(object o);
        

    }


}
