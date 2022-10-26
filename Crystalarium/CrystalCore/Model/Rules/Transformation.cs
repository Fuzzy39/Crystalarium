using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rules
{
    /// <summary>
    /// A way an agent can change itself or it's environment
    /// </summary>
    public abstract class Transformation
    {
        // Transformations should set this property. default states cannot have transformations that change the agent.
        internal protected bool ChecksRequired { get; protected set; }
        internal protected bool MustBeLast { get; protected set; }


        public Transformation()
        {

        }

        internal abstract void Validate(AgentType at);

        internal abstract void Transform(object o);

    }


}
