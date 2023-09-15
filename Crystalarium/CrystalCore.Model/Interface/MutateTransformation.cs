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
            ForrbiddenInDefaultState = true;
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

            if (!mutateTo.UpwardsSize.Equals(at.UpwardsSize))
            {
                throw new InitializationFailedException("Mutation Transformation: Agents that are mutated cannot change size.");
            }


        }

        internal override void Transform(Agent a)
        {
            a.Mutate(mutateTo);

        }

    
    }
}
