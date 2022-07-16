using CrystalCore.Rulesets;
using CrystalCore.Model;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.AgentRender
{
    internal class AgentGhost : ViewObject
    {
        public Rectangle Bounds { get; set; }
        public Direction Facing { get; set; }

        private AgentType type;

        

        public AgentGhost(GridView v, AgentType type, Point location, Direction facing) : base(v)
        {
            this.type = type; // we use this template to figure out how to render ourselves.
            Bounds = new Rectangle(location, type.GetSize(facing));
            Facing = facing;
           


        }

        internal override void Destroy()
        {
            throw new NotImplementedException();
        }

        internal override bool Draw(SpriteBatch sb)
        {

            // preform shrinkage in this slightly nasty one liner.
            float shrink = type.RenderConfig.Shrinkage;
            RectangleF realBounds = new RectangleF(Bounds).Inflate(-shrink, -shrink);

            // render the ghost.
            renderTarget.Camera.RenderTexture(sb, type.RenderConfig.DefaultTexture, realBounds, DetermineColor(), Facing);

            return true;
        }
        

        private Color DetermineColor()
        {
            Color c;

            // get the color of the agent. if the agent cannot be placed, make it red, instead.
            if (type.isValidLocation(renderTarget.Grid, Bounds.Location, Facing))
            {
                // these ought to be exposed better.
                c = type.RenderConfig.Color; //Color.Green;

            }
            else
            {
                c = Color.Red;
            }

            // make the color whiteish and transparent.
            return new Color(Color.Lerp(Color.White, c, .5f), .05f);
        }

        public override bool Equals(object obj)
        {
            if(! (obj is AgentGhost))
                return false;
            AgentGhost objGhost = (AgentGhost)obj;
            if(objGhost == null)
                return false;
            if (this.Bounds != objGhost.Bounds)
                return false;
            if (this.Facing != objGhost.Facing)
                return false;
            //if (this.type != objGhost.type)
                return false;
            return true;
        }
    }
}
