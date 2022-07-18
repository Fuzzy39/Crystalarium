using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Rulesets
{
    public class Ruleset
    {
        
        /*
         * A Ruleset describes a usable system that Crystalarium can use.
         * It describes both how it looks and behaves.
         * 
         */

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

        public Ruleset(string name)
        {
            Name = name;
            _agentTypes = new List<AgentType>();
        }

        // create a new agentType.
        public AgentType CreateType(String name, Point size)
        {
            foreach(AgentType at in _agentTypes)
            {
                if(name == at.Name)
                {
                    throw new ArgumentException("Agent Type name already used in this ruleset");
                }
            }    

            _agentTypes.Add(new AgentType(this, name, size));
            
            return _agentTypes[_agentTypes.Count - 1];
        }

       public AgentType GetAgentType(string name)
       {
            foreach(AgentType at in _agentTypes)
            {
                if (name == at.Name)
                {
                    return at;
                }
            }
            return null;
       }
  
    }
}
