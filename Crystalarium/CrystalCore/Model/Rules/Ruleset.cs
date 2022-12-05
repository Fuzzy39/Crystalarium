﻿// Namespace

using CrystalCore.Model.Language;
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


        private bool _rotateLock; // whether agents can be rotated or face a direction other than up.
        private bool _diagonalSignalsAllowed; // whether signals are only allowed to face the 4 orthagonal directions. If set to false, no AgentType can be of size greater than 1 x 1.
        private bool _runDefaultStateOnRotation; // whether agents will run their default transformations after they are rotated.

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
                    throw new InvalidOperationException("Mainimum beam length cannot be less than one.");
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





        internal Ruleset(string name)
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

            foreach (AgentType at in _agentTypes)
            {
                if (name == at.Name)
                {
                    throw new ArgumentException("Agent Type name already used in this ruleset");
                }
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


        internal override void Initialize()
        {
            try
            {
                foreach (AgentType at in _agentTypes)
                {
                    at.Initialize();
                }
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("Ruleset '" + Name + "' could not be initialized: " + Util.Util.Indent(e.Message));
            }

            base.Initialize();
        }

    }
}
