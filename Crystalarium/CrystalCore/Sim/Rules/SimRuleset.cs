using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim.Rules
{
    public class SimRuleset
    {
        
        private List<AgentType> _agentTypes; // the types of agents that make up this ruleset.


        public string Name // the human readable name of this ruleset.
        {
            get;
            private set;
        }

        public List<AgentType> AgentTypes 
        {
            get => _agentTypes;
            
        }

        public SimRuleset(string name)
        {
            Name = name;
            _agentTypes = new List<AgentType>();
        }


    }
}
