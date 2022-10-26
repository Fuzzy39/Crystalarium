using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Interface
{
    public class MutateTransformation : Transformation
    {

        private AgentType mutateTo;

        public MutateTransformation(AgentType mutateTo) : base()
        {
            ChecksRequired = true;
            MustBeLast = true;

            if (mutateTo == null)
            {
                throw new ArgumentNullException();

            }
                
            this.mutateTo = mutateTo;
        }

        internal override void Validate(AgentType at)
        {
            if (at.Ruleset!=mutateTo.Ruleset)
            {
                throw new InitializationFailedException("Mutation Transformation: unkown mutate type.");
            }

            if (!mutateTo.Size.Equals(at.Size))
            {
                throw new InitializationFailedException("Mutation Transformation: Agents that are mutated cannot change size.");
            }


        }

        internal override void Transform(object o)
        {
            if (!(o is Agent))
            {
                throw new ArgumentException("o must be a ");
            }
            Agent a = (Agent)o;

            Grid g = a.Grid;
            Direction d = a.Facing;
            Point loc = a.Bounds.Location;
            a.Destroy();
            Agent b = mutateTo.createAgent(g, loc, d);
            if (b == null)
            {
                throw new InvalidOperationException("Failed to mutate agent.");
            }
            g.UpdateSignals(b.ChunksWithin);
        }

    }
}
