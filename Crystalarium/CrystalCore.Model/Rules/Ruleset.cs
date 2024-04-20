// Namespace

using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Rules
{
    public class Ruleset : InitializableObject
    {

        /*
         * A Ruleset describes a usable system that Crystalarium can use.
         * It describes both how it looks and behaves.
         * 
         */

        private List<AgentType> _agentTypes; // the types of agents that make up this ruleset.


        private bool _performTransformationsEveryStep; // whether agents should perform transformations on every tick, instead of only when their inputs change.
        private bool _diagonalSignalsAllowed; // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        private bool _runDefaultStateOnRotation; // whether agents will run their default transformations after they are rotated.


        public bool PerformTransformationsEveryStep // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        {
            get => _performTransformationsEveryStep;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _performTransformationsEveryStep = value;
            }
        }


        public bool DiagonalSignalsAllowed // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        {
            get => _diagonalSignalsAllowed;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _diagonalSignalsAllowed = value;
            }
        }

        public bool RunDefaultStateOnRotation // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        {
            get => _runDefaultStateOnRotation;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _runDefaultStateOnRotation = value;
            }
        }

      
        public string Name // the human readable name of this ruleset.
        {
            get;
            private set;
        }

        public List<AgentType> AgentTypes
        {
            get => _agentTypes;

        }





        public Ruleset(string name)
        {
            Name = name;
            _agentTypes = new List<AgentType>();

            PerformTransformationsEveryStep = false;
            DiagonalSignalsAllowed = false;
            RunDefaultStateOnRotation = true;
                
        
        }

        // create a new agentType.
        public AgentType CreateType(string name, Point size)
        {
            if (Initialized)
            {
                throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized.");
            }



            _agentTypes.Add(new AgentType(this, name, size));

            return _agentTypes[_agentTypes.Count - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Agent Type, or null if it does not exist</returns>
        public AgentType GetAgentType(string name)
        {
            foreach (AgentType at in _agentTypes)
            {
                if (name == at.Name)
                {
                    return at;
                }
            }
            return null;
        }


        public override void Initialize()
        {
            try
            {
                foreach (AgentType at in _agentTypes)
                {
                    List<AgentType> types = new(_agentTypes);
                    types.Remove(at);

                    types.ForEach(ty =>
                    {
                        if (at.Name == ty.Name)
                        {
                            throw new InitializationFailedException("The agent type name '" + at.Name + "' has been used multiple times in this ruleset. The names of agent types should be unique human-readable identifiers.");
                        }
                    });

                }

                foreach (AgentType at in _agentTypes)
                {
                    at.Initialize();
                }
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("Ruleset '" + Name + "' could not be initialized: " + MiscUtil.Indent(e.Message));
            }

            base.Initialize();
        }

    }
}
