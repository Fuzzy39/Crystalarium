using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View.Configs;
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
      


        internal AgentView(GridView v, Agent a, List<Subview> others, AgentViewConfig config) : base(v, a, others)
        {
           
            _ports = null;
            this.config = config;

        }


        protected override void Render(SpriteBatch sb)
        {

            // render the thing if we have been set to.
            if (config.DefaultTexture == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }


            // render the Agent.
            renderTarget.Camera.RenderTexture(sb, config.DefaultTexture, ShrinkBorders(), config.Color, ((Agent)RenderData).Facing);

            // debug port rendering
            if (renderTarget.DoDebugPortRendering)
            {
                if (_ports == null)
                {
                    DebugPortSetup();
                }

                foreach (DebugPort dp in _ports)
                {
                    dp.Draw(sb);

                }
            }

        }

        /// <summary?
        /// This method, made especially for agents, is weird, and maybe should be generalized...
        /// </summary>
        /// <param name="sb"></param>
        internal void DrawBackground(SpriteBatch sb)
        {
            // render the background.
            if (config.Background == null)
            {
                return;
            }

            RectangleF bounds = new RectangleF(RenderData.Bounds);
            Direction facing = Direction.up;

            if (config.DoBackgroundShrinkage)
            {
                bounds = ShrinkBorders();
            }

            if (config.DoBackgroundRotation)
            {
                facing = ((Agent)RenderData).Facing;
            }


            renderTarget.Camera.RenderTexture(sb, config.Background, bounds, config.BackgroundColor, facing);
        }


        private void DebugPortSetup()
        {
            _ports = new List<DebugPort>();

            if (config.Background == null)
            {
                throw new InvalidOperationException("Agent Type " + ((Agent)RenderData).Type.Name + "'s RenderConfig requires a background texture to render debug ports.");
            }

            foreach (Port p in ((PortAgent)RenderData).PortList)
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

