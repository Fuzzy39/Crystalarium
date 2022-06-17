using CrystalCore.Sim;
using CrystalCore.Util;
using CrystalCore.View;
using CrystalCore.View.AgentRender;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using AgentRenderer = CrystalCore.View.AgentRender.AgentView;

namespace CrystalCore.Rulesets
{
    public class AgentType
    {
        /*
         * An AgentType is responsible for creating agents, and defines how a particular agent will behave.
         * It also defines what agentRenderers are used for this agent.
         * 
         */


        private Ruleset _ruleset; // the ruleset this agent type belongs to.


        private AgentViewTemplate _renderConfig; // the way this agent is rendered.

        private string _name; // the human readable name of this agent type.

        private Point _size; // the size of this AgentType, when pointing up.

        // and a list of states. (when we have those)

        // Properties
        public Ruleset Ruleset
        {
            get => Ruleset;
        }

        public AgentViewTemplate RenderConfig
        {
            get => _renderConfig;
        }

        public string Name
        {
            get => _name;
        }

        public Point Size
        {
            get => Size;
        }
         


        // constructors
        internal AgentType(Ruleset rs, String name, Point size)
        {
            _ruleset = rs;
            _name = name;
            _size = size;
            _renderConfig = new AgentViewTemplate();

        }

        public Agent createAgent(Grid g, Point pos, Direction d)
        {

            Rectangle bounds =  new Rectangle(pos, getSize(d));

            return new Agent(g, bounds,this,d);
        }


        // returns the appropriate size for the agent depending on direction.
        private Point getSize(Direction d)
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

        internal AgentRenderer CreateRenderer(SubviewManager m, Agent toRender, List<Subview> others)
        {
            if(toRender.Type!=this)
            {
                throw new ArgumentException(toRender + " is of type " + toRender.Type._name + ", and cannot be rendered as type " + _name);
            }
                
            return _renderConfig.CreateRenderer(m, toRender, others);
        }

       

        // returns whether an agent of type at can be placed at a location.
        public  bool isValidLocation(Grid g, Point location, Direction facing)
        {
            Rectangle bounds = new Rectangle(location, getSize(facing));
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
