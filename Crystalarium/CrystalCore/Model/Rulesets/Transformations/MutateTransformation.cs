using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Transformations
{
    public class MutateTransformation : Transformation
    {

        private AgentType mutateTo;

        public MutateTransformation(AgentType at, AgentType mutateTo) : base(at)
        {
            this.mutateTo = mutateTo;
        }

        internal override void Initialize()
        {
            if (mutateTo == null)
            {
                throw new InitializationFailedException("Mutation Transformation: unkown mutate type.");
            }

            if(!mutateTo.Size.Equals(AgentType.Size))
            {
                throw new InitializationFailedException("Mutation Transformation: Agents that are mutated cannot change size.");
            }

            base.Initialize();

        }

        internal override void Transform(Agent a)
        {
            base.Transform(a);
            Grid g = a.Grid;
            Direction d =a.Facing;
            Point loc = a.Bounds.Location;
            a.Destroy();
            if (mutateTo.createAgent(g, loc, d) == null)
            {
                throw new InvalidOperationException("Failed to mutate agent.");
            }
        }

    }
}
