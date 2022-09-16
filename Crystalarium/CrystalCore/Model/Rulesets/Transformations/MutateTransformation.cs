using CrystalCore.Model.Grids;
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

        private String mutateTo;

        public MutateTransformation(AgentType at, string mutateTo) : base(at)
        {
            this.mutateTo = mutateTo;
        }

        internal override void Initialize()
        {
            if (AgentType.Ruleset.GetAgentType(mutateTo) == null)
            {
                throw new InitializationFailedException("Mutation Transformation: unkown mutate type.");
            }

            if(!AgentType.Ruleset.GetAgentType(mutateTo).Size.Equals(AgentType.Size))
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
            Agent b = AgentType.Ruleset.GetAgentType(mutateTo).createAgent(g, loc, d);
            if (b== null)
            {
                throw new InvalidOperationException("Failed to mutate agent.");
            }
            g.UpdateSignals(b.ChunksWithin);
        }

    }
}
