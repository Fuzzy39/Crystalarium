using CrystalCore.Model.Communication;
using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Rules.Transformations;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Simulation.Default
{
    internal class DefaultAgent : Agent
    {

        private AgentType _type;
        private Node _node;
        private List<Transform> _nextTransforms;


        public AgentType Type => _type;

        public Node Node => _node;



        public DefaultAgent(Node node, AgentType t)
        {
          

            _type = t;
         
            _node = node;

            _nextTransforms = new List<Transform>();

            

        }
 
        public void Rotate(RotationalDirection rd)
        {
            _node.Rotate(rd);

            if(_type.Ruleset.RunDefaultStateOnRotation)
            {
                RunDefaultTransforms();
            }
        }

        public void Mutate(AgentType type)
        {

            _type = type;
            RunDefaultTransforms();
        }

        public void TransmitOn(PortTransmission[] pts)
        {
            List<Port> ports = new List<Port>();
            foreach (PortTransmission pt in pts)
            {
                Node.GetPort(pt.descriptor).Output = pt.value;
            }

        }


        public void Destroy()
        {
            Node.Destroy();
            _type = null; // not that this really matters.
        }


        public void PrepareSimulationStep()
        {
            _node.DoSimulationStep();
            _nextTransforms = GetTransformations(GetActiveRules());
        }

        public void DoSimulationStep()
        {
            if ((!Node.ChangedLastStep)&&(!_type.Ruleset.PerformTransformationsEveryStep))
            {
                return;
            }

            RunTransformations(_nextTransforms);
            

        }

        private void RunTransformations(List<Transform> transforms)
        {
            // reset all active transmissions
            Node.PortList.ForEach(port => port.Output = 0);

            // perform rules
            transforms.ForEach(tf => tf.Invoke(this));
            transforms.Clear();
        }

        private void RunDefaultTransforms()
        {
            List<Transform> defaultTranforms = new List<Transform>();
            _type.DefaultState.Transformations.ForEach(t => defaultTranforms.Add(t.CreateTransform(this)));
            RunTransformations(defaultTranforms);
        }



        // old code to interface with old parts of the system

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


    }
}
