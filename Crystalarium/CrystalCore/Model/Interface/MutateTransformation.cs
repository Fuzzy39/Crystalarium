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

            Map g = a.Map;
            Direction d = a.Facing;
            Rectangle bounds = a.Bounds;
            a.Destroy();
            Agent b = new Agent(g, bounds.Location, mutateTo, d);
            if (b == null)
            {
                throw new InvalidOperationException("Failed to mutate agent.");
            }
            
        }

        internal override Transformation Add(Transformation toAdd)
        {
            CheckType(toAdd);
            return toAdd;
        }
    }
}
