using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using System;

namespace CrystalCore.Model.Rules.Transformations
{
    public class MutateTransformation : ITransformation
    {


        public bool ForrbiddenInDefaultState
        {
            get
            {
                return false;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return true;
            }
        }


        private AgentType mutateTo;

        public MutateTransformation(AgentType mutateTo) : base()
        {

            if (mutateTo == null)
            {
                throw new ArgumentNullException();

            }

            this.mutateTo = mutateTo;
        }

        public void Validate(AgentType at)
        {
            if (at.Ruleset != mutateTo.Ruleset)
            {
                throw new InitializationFailedException("Mutation Transformation: unkown mutate type.");
            }

            if (!mutateTo.UpwardsSize.Equals(at.UpwardsSize))
            {
                throw new InitializationFailedException("Mutation Transformation: Agents that are mutated cannot change size.");
            }


        }

        public Transform CreateTransform(Agent a)
        {

            return a => a.Mutate(mutateTo);

        }


    }
}
