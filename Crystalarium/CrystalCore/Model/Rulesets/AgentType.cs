using CrystalCore.Model;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;


namespace CrystalCore.Model.Rulesets
{
    public class AgentType:InitializableObject
    {
        /*
         * An AgentType is responsible for creating agents, and defines how a particular agent will behave.
         * It also defines what agentRenderers are used for this agent.
         * 
         */


        private Ruleset _ruleset; // the ruleset this agent type belongs to.

        private string _name; // the human readable name of this agent type.

        private Point _size; // the size of this AgentType, when pointing up.

        // and a list of states. (when we have those)

        // Properties
        public Ruleset Ruleset
        {
            get => _ruleset;
        }

    
        public string Name
        {
            get => _name;
        }

        public Point Size
        {
            get => _size;
        }
         


        // constructors
        internal AgentType(Ruleset rs, String name, Point size)
        {
            _ruleset = rs;
            _name = name;
            _size = size;

        }

        public Agent createAgent(Grid g, Point pos, Direction d)
        {

            if (!Initialized)
            {
                throw new InvalidOperationException("This AgentType cannot be used before it is initialized. Call Engine.Initialize().");
            }

            if (g.Ruleset!=this.Ruleset)
            {
                throw new InvalidOperationException("Cannot add " + Name + " type agent of ruleset " + Ruleset.Name + " to grid of ruleset " + g.Ruleset.Name+".");
            }

            Rectangle bounds =  new Rectangle(pos, GetSize(d));


            return new Agent(g, bounds,this,d);
        }


        // returns the appropriate size for the agent depending on direction.
        internal Point GetSize(Direction d)
        {
            if (d.IsVertical())
            {
                return _size;
            }
            else
            {
                return new Point(_size.Y, _size.X);
            }
        }

       

        // returns whether an agent of type at can be placed at a location.
        public  bool isValidLocation(Grid g, Point location, Direction facing)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("This AgentType cannot be used before it is initialized. Call Engine.Initialize().");
            }

            Rectangle bounds = new Rectangle(location, GetSize(facing));
            if (g.Bounds.Contains(bounds))
            {
                if (g.AgentsWithin(bounds).Count == 0)
                {
                    return true;
                }

            }
         
            return false;
        }



    }
}
