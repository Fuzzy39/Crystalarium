using CrystalCore.Model;
using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View.Configs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets
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
        private PortChannelMode _portChannelMode; // the channel mode of every port created by this ruleset.

        // signals
        private SignalType _signalType;
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

        public PortChannelMode PortChannelMode // the channel mode of every port created by this ruleset.
        {
            get => _portChannelMode;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _portChannelMode = value;
            }
        } 

        public SignalType SignalType 
        {
            get => _signalType; 
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _signalType = value;
            } 
        }

        // mildly hacky.
        public int BeamMinLength
        {
            get => _beamMinLength;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
                _beamMinLength = value;
            }
        }

        public int BeamMaxLength
        {
            get => _beamMaxLength;
            set
            {
                if (Initialized) { throw new InvalidOperationException("Cannot Modify Ruleset after it has been initialized."); }
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
            PortChannelMode = PortChannelMode.fullDuplex;
            SignalType = SignalType.Beam;


            BeamMinLength = 1; // defaults, infinite, unbounded beams.
            BeamMaxLength = 0;

        }

        // create a new agentType.
        public AgentType CreateType(String name, Point size)
        {
            if(Initialized)
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

        internal Port CreatePort(CompassPoint facing, int ID, Agent parent)
        {

            if (!Initialized)
            {
                throw new InvalidOperationException("Ruleset cannot be used before it is initialized. Call Engine.Initialize().");
            }

            switch (PortChannelMode)
            {
                case PortChannelMode.fullDuplex:
                    return new FullPort(facing, ID, parent);
                case PortChannelMode.halfDuplex:
                    return new HalfPort(facing, ID, parent);

            }

            throw new InvalidOperationException("Invalid Port Channel: No case to create Port of type: " + PortChannelMode.ToString());

        }

        internal Signal CreateSignal(Grid g, Port transmitter, int value)
        {

            if (!Initialized)
            {
                throw new InvalidOperationException("Ruleset cannot be used before it is initialized. Call Engine.Initialize().");
            }

            switch (SignalType)
            {

                case SignalType.Beam:
                    return new Beam(g, transmitter, value, BeamMinLength, BeamMaxLength);

            }

            throw new InvalidOperationException("Invalid Signal Type: No case to create Signal of type: " + SignalType.ToString());
        }

        internal override void Initialize()
        {
            try
            {
                foreach(AgentType at in _agentTypes)
                {
                    at.Initialize();
                }
            }
            catch(InitializationFailedException e)
            {
                throw new InitializationFailedException("Ruleset '" + Name + "' could not be initialized: " + Util.Util.Indent(e.Message));
            }

            base.Initialize();
        }

    }
}
