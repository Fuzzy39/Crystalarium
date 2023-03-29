using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Subviews.Agents
{
    /// <summary>
    /// A Debug port renders a port on an agent very simply, for debugging purposes.
    /// </summary>
    internal class DebugPort : IRenderable
    {

        private Texture2D background;

        private Port _port;

        private AgentView _parent;

        public Port Port
        {
            get => _port;
        }

        public AgentView Parent
        {
            get => _parent;
        }

        public DebugPort(Texture2D background, Port port, AgentView parent)
        {
            this.background = background;
            _port = port;
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
            switch (Port.AbsoluteFacing)
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


            return new RectangleF(bounds.Location + Port.Location.ToVector2(), bounds.Size);

        }

        private Color DetermineColor()
        {
          
            if(Port.HasConnection == false)
            {
                return Color.Magenta;
            }

            if(Port.Value == 0 & Port.TransmittingValue == 0)
            {
                return Color.DimGray;
            }

            if(Port.Value != 0 & Port.TransmittingValue != 0)
            {
                return Color.Purple;
            }

            if(Port.Value!= 0)
            {
                return Color.Blue;
            }

            return Color.Red;


        }

    }
}
