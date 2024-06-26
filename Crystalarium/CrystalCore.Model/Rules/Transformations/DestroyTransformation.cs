﻿using CrystalCore.Model.Simulation;

namespace CrystalCore.Model.Rules.Transformations
{
    internal class DestroyTransformation : ITransformation
    {

        public bool ForrbiddenInDefaultState
        {
            get
            {
                return true;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return true;
            }
        }

        public Transform CreateTransform(Agent a)
        {
            return a => a.Destroy(); // hopefully this is the right way to do things.
        }

        public void Validate(AgentType at)
        {
            // always good to go!
        }

    }
}
