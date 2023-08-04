using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using ChunkRenderer = CrystalCore.View.Subviews.ChunkView;

namespace CrystalCore.View.Subviews.Agents
{
    /// <summary>
    ///  An AgentView is a Subview that renders an agent and its ports.
    /// </summary>
    internal class AgentView : Subview
    {
        private AgentViewConfig config; // the settings for how this agentview will render itself.
        private List<DebugPort> _ports; // the ports that this agentview may render. 
      
        internal AgentType Type
        {
            get { return config.AgentType; }
        }

        internal AgentView(GridView v, Agent a, AgentViewConfig config) : base(v, a)
        {
           
            _ports = null;
            this.config = config;

        }


        protected override void Render(IRenderer rend)
        {

           

            // render the thing if we have been set to.
            if (config.DefaultTexture == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }


            // render the Agent.
            Direction facing = ((Entity)RenderData).Facing;
            float textureFacing = facing.ToRadians() - config.TextureFacing.ToRadians();

            RectangleF loc = ShrinkBorders();
            Vector2 size = loc.Size;
            if (facing.IsHorizontal())
            {
                size = new(size.Y, size.X);
            }

            RotatedRect pos = RotatedRect.FromBoundingLocation(loc.Location, size, textureFacing);

            //pos.RotateAbout( textureFacing- facing.ToRadians(), RenderData.Bounds.Location.ToVector2()+new Vector2(.5f) );

            rend.Draw(config.DefaultTexture, pos, config.Color);

            // debug port rendering
            if (renderTarget.DoDebugRendering)
            {
                if (_ports == null)
                {
                    DebugPortSetup();
                }

                foreach (DebugPort dp in _ports)
                {
                    dp.Draw(rend);

                }
            }

            
        }

        /// <summary?
        /// This method, made especially for agents, is weird, and maybe should be generalized...
        /// </summary>
        /// <param name="sb"></param>
        internal void DrawBackground(IRenderer rend)
        {
            // render the background.
            if (config.Background == null)
            {
                return;
            }

            RectangleF bounds = new RectangleF(RenderData.Bounds);
     

            if (config.DoBackgroundShrinkage)
            {
                bounds = ShrinkBorders();
            }

           

            RotatedRect pos = RotatedRect.FromBoundingLocation(bounds.Location, bounds.Size, 0);
            rend.Draw(config.Background, pos, config.BackgroundColor);
        }


        private void DebugPortSetup()
        {
            _ports = new List<DebugPort>();

            if (config.Background == null)
            {
                throw new InvalidOperationException("Agent Type " + ((Agent)RenderData).Type.Name + "'s RenderConfig requires a background texture to render debug ports.");
            }

            foreach (Port p in ((Agent)RenderData).PortList)
            {
                _ports.Add(new DebugPort(config.Background, p, this));
            }

        }


        // get the borders of the background in tiles. fair enough.
        private RectangleF ShrinkBorders()
        {


            RectangleF toReturn = new RectangleF(RenderData.Bounds);

            // Perform shrinkage.
            return toReturn.Inflate(-config.Shrinkage, -config.Shrinkage);

        }





    }
}

