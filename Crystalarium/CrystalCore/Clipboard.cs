using CrystalCore.Model.Core;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
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

            if(content.Count == 0) { return; }

            Point min = content[0].Location;
            Point max = content[0].Location + maxSquare(content[0].Type.UpwardsSize);

            foreach(AgentRepresentation ar in content)
            {
                Point newMin = ar.Location;
                Point newMax = ar.Location + maxSquare(ar.Type.UpwardsSize);

                min = new(min.X < newMin.X ? min.X : newMin.X, min.Y < newMin.Y ? min.Y : newMin.Y);
                max = new(max.X < newMax.X ? max.X : newMax.X, max.Y < newMax.Y ? max.Y : newMax.Y);
            }

            Rectangle pasteBounds = Util.MiscUtil.RectFromPoints(min, max);
            pasteBounds.Inflate(1, 1);
            m.Grid.ExpandToFit(pasteBounds);

            foreach (AgentRepresentation ar in content)
            {
                ar.CreateAgent(m, location);
            }
        }


        private Point maxSquare(Point p)
        {
            if(p.X>p.Y)
            {
                return new(p.X, p.X);
            }

            return new(p.Y, p.Y);
        }
    }
}
