using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets
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
            if(!Initialized)
            {
                throw new InvalidOperationException("Agent Cannot perform transformation until it has been initialized.");
            }

            if(a.Type != AgentType)
            {
                throw new InvalidOperationException("AgentType of this transformation does not match this agent.");
            }
        }

    }

    public class TransmitTransformation : Transformation
    {
        private PortIdentifier[] ports;
        private int value;
        public TransmitTransformation(AgentType at, int value, params PortIdentifier[] ports):base(at)
        {
            this.value = value;
            this.ports = ports;
            ChangesAgent = false;
        }

        internal override void Initialize()
        {
            foreach(PortIdentifier portID in ports)
            {
                if(!portID.CheckValidity(AgentType))
                {
                    throw new InitializationFailedException(
                        "Transmit Transformation: Port ID: " + portID + " is not valid for AgentType '" + AgentType.Name + "'.");
                }
            }  
            
            base.Initialize();

        }

        internal override void Transform(Agent a)
        {
            base.Transform(a);
            foreach(PortIdentifier portID in ports)
            {
                a.GetPort(portID).Transmit(value);
            }
        }

    }
}
