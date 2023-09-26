// Namespace

using CrystalCore.Model.Language;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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


        private bool _rotateLock; // whether agents can be rotated or face a direction other than up.
        private bool _diagonalSignalsAllowed; // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        private bool _runDefaultStateOnRotation; // whether agents will run their default transformations after they are rotated.

        private AgentType _defaultAgent; // The agent type that fills all empty spaces.

        // signals
        private int _beamMinLength;
        private int _beamMaxLength;

        

        public bool RotateLock // whether agents can be rotated or face a direction other than up.
        {
            get => _rotateLock;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _rotateLock = value;
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

        // mildly hacky.
        public int SignalMinLength
        {
            get => _beamMinLength;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }


                if (value < 1)
                {
                    throw new InvalidOperationException("Minimum beam length cannot be less than one.");
                }


                if (_beamMaxLength != 0 & value > _beamMaxLength)
                {
                    throw new InvalidOperationException("Minimum beam length cannot be greater than minimum.");
                }

                _beamMinLength = value;
            }
        }

        public int SignalMaxLength
        {
            get => _beamMaxLength;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                if (value <= 0)
                {
                    _beamMaxLength = 0;
                    return;
                }

                if (value < _beamMinLength)
                {
                    throw new InvalidOperationException("Maximum beam length cannot be less than minimum.");
                }

                _beamMaxLength = value;
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

        public AgentType DefaultAgent
        {
            get => _defaultAgent;
            set
            {
                if(Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized.");  }
                _defaultAgent = value;
            }
        }



        public Ruleset(string name)
        {
            Name = name;
            _agentTypes = new List<AgentType>();


            RotateLock = false;
            DiagonalSignalsAllowed = false;
            RunDefaultStateOnRotation = true;

            SignalMinLength = 1; // defaults, infinite, unbounded beams.
            SignalMaxLength = 0;

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

                initializeDefaultAgent();
               
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("Ruleset '" + Name + "' could not be initialized: " + MiscUtil.Indent(e.Message));
            }

            base.Initialize();
        }



        private void initializeDefaultAgent()
        {
            if (_defaultAgent == null)
            {
                return;
            }

            if (!_agentTypes.Contains(_defaultAgent))
            {
                throw new InitializationFailedException("Agent Type '" + _defaultAgent.Name + "' cannot be used as the default agent because it is not a member of this ruleset.");
            }

            int area = _defaultAgent.UpwardsSize.X*_defaultAgent.UpwardsSize.Y;
            if (area != 1)
            {
                throw new InitializationFailedException("Agent Type '" + _defaultAgent.Name +
                    "' cannot be used as a default agent because it is not 1 by 1. Its size is " + _defaultAgent.UpwardsSize);

            }


        }

    }
}
