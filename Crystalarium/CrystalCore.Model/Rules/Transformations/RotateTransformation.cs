using CrystalCore.Model.Simulation;
using CrystalCore.Util;

namespace CrystalCore.Model.Rules.Transformations
{
    internal class RotateTransformation : ITransformation
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
                return false;
            }
        }


        private RotationalDirection direction;

        public RotateTransformation(RotationalDirection direction)
        {

            this.direction = direction;
        }

        public void Validate(AgentType at)
        {
            // nothing can go wrong, that's good.
        }


        public Transform CreateTransform(Agent a)
        {

            return a => a.Rotate(direction);
        }


    }
}
