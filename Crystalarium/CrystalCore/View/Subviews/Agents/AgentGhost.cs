using CrystalCore.Model.Rulesets;
using CrystalCore.Model;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.View.Configs;

namespace CrystalCore.View.Subviews.Agents
{
    internal class AgentGhost : ViewObject
    {
        public Rectangle Bounds { get; set; } // the tile bounds of this ghost
        public Direction Facing { get; set; } // the direction this ghost is facing

        private AgentViewConfig config; // the type of agent this ghost descends from.



        public AgentGhost(GridView v, AgentViewConfig conf, Point location, Direction facing) : base(v)
        {
            this.config = conf; // we use this template to figure out how to render ourselves.
            Bounds = new Rectangle(location, config.AgentType.GetSize(facing));
            Facing = facing;

        }

        internal override bool Draw(SpriteBatch sb)
        {

            // preform shrinkage in this slightly nasty one liner.
            float shrink = config.Shrinkage;
            RectangleF realBounds = new RectangleF(Bounds).Inflate(-shrink, -shrink);

            // render the ghost.
            renderTarget.Camera.RenderTexture(sb, config.DefaultTexture, realBounds, DetermineColor(), Facing);

            return true;
        }


        private Color DetermineColor()
        {
            Color c;

            // get the color of the agent. if the agent cannot be placed, make it red, instead.
            if (config.AgentType.isValidLocation(renderTarget.Grid, Bounds.Location, Facing))
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
