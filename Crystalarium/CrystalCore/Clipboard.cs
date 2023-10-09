using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CrystalCore
{
    public class Clipboard
    {
        private Ruleset Ruleset { get; set; }
        private List<AgentRepresentation> content;


        public Clipboard()
        {

            content = new List<AgentRepresentation>();
        }


        public void Copy(Map m, Rectangle selection)
        {

            List<Agent> agents = prepareSelection(m, selection);
            foreach (Agent agent in agents)
            {
                content.Add(new AgentRepresentation(agent, selection.Location));
            }
        }

        public void Cut(Map m, Rectangle selection)
        {

            List<Agent> agents = prepareSelection(m, selection);

            foreach (Agent agent in agents)
            {
                content.Add(new AgentRepresentation(agent, selection.Location));
                agent.Destroy();
            }
        }

        private List<Agent> prepareSelection(Map m, Rectangle selection)
        {
            Ruleset = m.Ruleset;
            Rectangle r = new(selection.Location, selection.Size + new Point(1));
            content.Clear();
            List<Agent> toReturn = m.AgentsWithin(r);
            return toReturn;
        }

        public void Paste(Map m, Point location)
        {
            if (Ruleset != m.Ruleset)
            {
                // the selection cannot be pasted.
                return;
            }

            foreach (AgentRepresentation ar in content)
            {
                ar.CreateAgent(m, location);
            }
        }

    }
}
