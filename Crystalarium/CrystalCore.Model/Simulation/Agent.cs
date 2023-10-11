using CrystalCore.Model.Core;
using CrystalCore.Model.DefaultObjects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Simulation
{
    /// <summary>
    /// Agents are entities that actively participate in the simulation. It deterimines its state every stepa and acts accordingly.
    /// </summary>
    public class Agent : OldEntity
    {

        private AgentType _type;

        protected List<Transform> _nextTransforms;

        private PortManager portInterface;


        // properties 

        // internal event EventHandler OnPortsDestroyed;

        public AgentType Type
        {
            get => _type;
        }

        internal List<List<Port>> Ports
        {
            get { return portInterface.Ports; }
        }

        public List<Port> PortList
        {
            get { return portInterface.PortList; }
        }

        // Constructors
        public Agent(DefaultMap g, Point location, AgentType t, Direction facing) : base(g, location, t.UpwardsSize, t.Ruleset.RotateLock ? Direction.up : facing)
        {

            if (g.Ruleset != t.Ruleset)
            {
                throw new InvalidOperationException("Cannot add " + t.Name + " type agent of ruleset " + t.Ruleset.Name + " to grid of ruleset " + g.Ruleset.Name + ".");
            }

            _type = t;

            // create the portinterface
            portInterface = new PortManager(t, this);
            portInterface.OnCreation();


            // if diagonal signals are allowed, then agents should not be bigger than 1 by 1
            if (Type.Ruleset.DiagonalSignalsAllowed && Bounds.Size.X * Bounds.Size.Y > 1)
            {
                throw new InvalidOperationException("The Ruleset '" + Type.Ruleset.Name + "' has specified that diagonal signals are allowed, which requires that no agents are greater than 1 by 1 in size.");
            }



            // do the default thing.
            _nextTransforms = new();
            SetDefaultTransformsNext();
            Execute();

            Ready();

        }



        public override void Destroy()
        {


            base.Destroy();
            portInterface.Destroy();



        }


        internal void StatusChanged()
        {
            portInterface.StatusChanged();
        }



        public override void Rotate(RotationalDirection d)
        {

            if (Type.Ruleset.RotateLock)
            {
                Console.WriteLine("Warning: Ruleset '" + Type.Ruleset.Name + "' has Rotation Lock enabled, and Agents cannot be set facing any other direction than up. " +
               "\n    Agent '" + ToString() + "' has attempted to rotate " + d + ".");
                return;
            }

            base.Rotate(d);
            portInterface.Rotate();



            if (Type.Ruleset.RunDefaultStateOnRotation)
            {
                SetDefaultTransformsNext();
                Execute();
            }
        }


        internal void Mutate(AgentType at)
        {
            if (at.UpwardsSize != Type.UpwardsSize)
            {
                throw new ArgumentException("An agent of type " + Type.Name + " cannot be mutated to type " + at.Name);
            }

            _type = at;

            // do the default thing.

            SetDefaultTransformsNext();
            Transform();
            portInterface.StatusChanged();

        }

        internal void SetDefaultTransformsNext()
        {
            _nextTransforms.Clear();
            Type.DefaultState.Transformations.ForEach(t => _nextTransforms.Add(t.CreateTransform(this)));
        }

        public override string ToString()
        {
            return "Agent { Type:\"" + Type.Name + "\", Location:" + Bounds.Location + ", Facing:" + Facing + " }";
        }

        /// <summary>
        /// This method tranistions the current simulation step's state into the last step. This allows us to freely change the state of the grid without 
        /// causing any changes to what we are making decisions about.
        /// </summary>
        internal void CalculateNextStep()
        {
            portInterface.PreserveState();
            _nextTransforms = GetTransformations(GetActiveRules());

        }




        private List<TransformationRule> GetActiveRules() //Indiana
        {

            List<TransformationRule> toReturn = new List<TransformationRule>();

            foreach (TransformationRule state in Type.Rules)
            {

                // check if we meet the requirements
                if (state.SatisfiesRequirements(this))
                {

                    toReturn.Add(state);
                }
            }

            if (toReturn.Count == 0)
            {
                toReturn.Add(Type.DefaultState);
            }

            return toReturn;
        }

        private List<Transform> GetTransformations(List<TransformationRule> activeRules)
        {
            List<ITransformation> transformations = new List<ITransformation>();
            foreach (TransformationRule tr in activeRules)
            {
                transformations.AddRange(tr.Transformations);
            }
            transformations = SortTransformations(transformations);
            List<Transform> toReturn = new();

            transformations.ForEach(trans => toReturn.Add(trans.CreateTransform(this)));


            return toReturn;
        }

        // add all transformations that can be added.
        // This method assumes that the list passed into it is in the same order that the transformation rules are in.
        private List<ITransformation> SortTransformations(List<ITransformation> list)
        {

            if (list.Count == 0) { return list; }

            List<ITransformation> toReturn = new List<ITransformation>();
            ITransformation last = null;

            foreach (ITransformation tr in list)
            {
                if (tr.MustBeLast && last == null)
                {
                    last = tr;
                    continue;
                }

                // any subsequent must be last transformations are disregarded.
                if (tr.MustBeLast)
                {
                    continue;
                }

                toReturn.Add(tr);

            }

            if (last != null)
            {
                toReturn.Add(last);
            }

            return toReturn;


        }


        internal void Update()
        {
            if (!portInterface.StatusHadChanged)
            {
                return;
            }

            Execute();

        }

        /// <summary> 
        /// Runs through transformations of this agent type.
        /// </summary>
        /// <param name="a"></param>
        internal void Execute()
        {

            // reset transmissions
            portInterface.ResetTransmissions();

            Transform();


        }

        private void Transform()
        {
            for (int i = 0; i < _nextTransforms.Count; i++)
            {
                _nextTransforms[i].Invoke(this);

            }
            _nextTransforms.Clear();
        }


        internal void TransmitOn(PortTransmission[] pts)
        {
            List<Port> ports = new List<Port>();
            foreach (PortTransmission pt in pts)
            {
                ports.Add(GetPort(pt.portID));
            }

            portInterface.TransmitOn(ports, pts);

        }



        public Port GetPort(PortID portID)
        {
            if (!portID.CheckValidity(Type))
            {
                throw new InvalidOperationException("Bad PortID.");
            }
            return portInterface.Ports[(int)portID.Facing][portID.ID];
        }


        /* ///<summary>
         /// 
         /// </summary>
         /// <returns> the value this port was receiving at the end of the last simulation step.</returns>
         internal int GetPortValue(PortIdentifier portID)
         {
             if (!portID.CheckValidity(Type))
             {
                 throw new InvalidOperationException("Bad PortID.");
             }

             return _stalePortValues[(int)portID.Facing][portID.ID];
         }*/
    }
}
