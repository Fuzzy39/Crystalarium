using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Transformations
{
    /// <summary>
    /// A way an agent can change itself or it's environment
    /// </summary>
    public abstract class Transformation : InitializableObject
    {
        // Transformations should set this property. default states cannot have transformations that change the agent.
        public bool ChangesAgent { get; protected set; }
        protected AgentType AgentType;

        public Transformation(AgentType at)
        {
            AgentType = at;
        }

        internal virtual void Transform(Agent a)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Agent Cannot perform transformation until it has been initialized.");
            }

            if (a.Type != AgentType)
            {
                throw new InvalidOperationException("AgentType of this transformation does not match this agent.");
            }
        }

    }

    
}
