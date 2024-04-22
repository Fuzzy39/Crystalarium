using CrystalCore.Model.Communication;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Subviews.Agents
{
    /// <summary>
    /// A Debug port renders a port on an agent very simply, for debugging purposes.
    /// </summary>
    internal class DebugPort : IRenderable
    {

        private Texture2D background;

        private Agent _agent;

        private PortDescriptor _portDesc;

        private AgentView _parent;

        public PortDescriptor PortDescriptor
        {
            get => _portDesc;
        }

        public AgentView Parent
        {
            get => _parent;
        }

        public DebugPort(Texture2D background, PortDescriptor port, Agent agent, AgentView parent)
        {
            this.background = background;
            _portDesc = port;
            _agent = agent;
            _parent = parent;
        }


        public bool Draw(IRenderer rend)
        {
            RectangleF bounds = DetermineBounds();
            Color c = DetermineColor();
            rend.Draw(background, bounds, c);
            return true;

        }


        private RectangleF DetermineBounds()
        {
            RectangleF bounds = new RectangleF();

            float width = .7f; // the amount of tile across
            float thickness = .15f; // the amount of tile through

            // a horrible ugly switch statement.
           
            switch (_agent.Node.GetPort(_portDesc).AbsoluteFacing)
            {
                case CompassPoint.north:
                    bounds = new RectangleF((1 - width) / 2f, 0, width, thickness);
                    break;
                case CompassPoint.northeast:
                    bounds = new RectangleF(1 - thickness, 0, thickness, thickness);
                    break;
                case CompassPoint.east:
                    bounds = new RectangleF(1 - thickness, (1 - width) / 2f, thickness, width);
                    break;
                case CompassPoint.southeast:
                    bounds = new RectangleF(1 - thickness, 1 - thickness, thickness, thickness);
                    break;
                case CompassPoint.south:
                    bounds = new RectangleF((1 - width) / 2f, 1 - thickness, width, thickness);
                    break;
                case CompassPoint.southwest:
                    bounds = new RectangleF(0, 1 - thickness, thickness, thickness);
                    break;
                case CompassPoint.west:
                    bounds = new RectangleF(0, (1 - width) / 2f, thickness, width);
                    break;
                case CompassPoint.northwest:
                    bounds = new RectangleF(0, 0, thickness, thickness);
                    break;


            }


            return new RectangleF(bounds.Location + _agent.Node.GetPort(_portDesc).Location.ToVector2(), bounds.Size);

        }

        private Color DetermineColor()
        {

            Port Port = _agent.Node.GetPort(_portDesc);
            if (Port.Connection==null)
            {
                return Color.Magenta;
            }

            if (Port.Input == 0 && Port.Output == 0)
            {
                return Color.DimGray;
            }

            if (Port.Input != 0 && Port.Output != 0)
            {
                return Color.Purple;
            }

            if (Port.Input != 0)
            {
                return Color.Blue;
            }

            return Color.Red;


        }

    }
}
