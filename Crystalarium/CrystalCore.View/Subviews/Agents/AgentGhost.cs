using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.View.Configs;
using CrystalCore.Model.Elements;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;

namespace CrystalCore.View.Subviews.Agents
{
    internal class AgentGhost : IRenderable
    {
        public Rectangle Bounds { get; set; } // the tile bounds of this ghost
        public Direction Facing { get; set; } // the direction this ghost is facing

        private AgentViewConfig config; // the type of agent this ghost descends from.

        private Map map;

        public AgentGhost(Map m, AgentViewConfig conf, Point location, Direction facing)
        {
            this.config = conf; // we use this template to figure out how to render ourselves.
            Bounds = new Rectangle(location, config.AgentType.GetSize(facing));
            Facing = facing;
            map = m;

        }

        public bool Draw(IRenderer renderer)
        {
            // preform shrinkage in this slightly nasty one liner.
            float shrink = config.Shrinkage;
            RectangleF loc = new RectangleF(Bounds).Inflate(-shrink, -shrink);

            float textureFacing = Facing.ToRadians() - config.TextureFacing.ToRadians();

          
            Vector2 size = loc.Size;
            if (Facing.IsHorizontal())
            {
                size = new(size.Y, size.X);
            }
                
            RotatedRect pos = RotatedRect.FromBoundingLocation(loc.Location, size, textureFacing);

            // render the ghost.
            renderer.Draw
            (
                config.DefaultTexture,
                pos,
                DetermineColor()
            );

            return true;
        }
    


        private Color DetermineColor()
        {
            Color c;

            // get the color of the agent. if the agent cannot be placed, make it red, instead.
            if (Entity.IsValidLocation(map, Bounds, Facing))
            {
                // these ought to be exposed better.
                c = config.Color; //Color.Green;

            }
            else
            {
                c = Color.Red;
            }

            // make the color whiteish and transparent.
            return new Color(Color.Lerp(Color.White, c, .5f), .05f);
        }

       
    }
}
