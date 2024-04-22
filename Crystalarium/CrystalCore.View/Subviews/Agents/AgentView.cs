using CrystalCore.Model.Communication;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;

namespace CrystalCore.View.Subviews.Agents
{
    /// <summary>
    ///  An AgentView is a Subview that renders an agent and its ports.
    /// </summary>
    internal class AgentView : Subview
    {
        private List<AgentViewConfig> configs; // the settings for how this agentview will render itself.
        private AgentViewConfig config;
        private List<DebugPort> _ports; // the ports that this agentview may render. 

        private Agent _agent;

        internal AgentType CurrentType
        {
            get { return config.AgentType; }
        }

        internal AgentView(GridView v, Agent a, List<AgentViewConfig> configs) : base(v, a.Node.Physical)
        {

            _ports = null;
            _agent = a;
            this.configs = configs;
            setCurrentConfig();

        }

        private void setCurrentConfig()
        {
            AgentType t = _agent.Type;
            foreach (AgentViewConfig config in configs)
            {
                if (config.AgentType == t)
                {
                    this.config = config;
                }
            }
        }


        public override bool Draw(IRenderer rend)
        {

            if (!base.Draw(rend))
            {
                return false;
            }

            // check that we are visible on screen.
            if (!renderTarget.Camera.TileBounds.Intersects(_agent.Node.Physical.Bounds))
            {

                return true;

            }

            // render the thing if we have been set to.
            if (config.DefaultTexture == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }


            if (_agent.Type != CurrentType)
            {
                setCurrentConfig();
            }

            // render the Agent.
            Direction facing = _agent.Node.Facing;
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

            return true;

        }

        /// <summary?
        /// This method, made especially for agents, is weird, and maybe should be generalized...
        /// </summary>
        /// <param name="sb"></param>
        internal bool DrawBackground(IRenderer rend)
        {

            if (!base.Draw(rend))
            {
                return false;
            }

            // render the background.
            if (config.Background == null)
            {
                return true;
            }

            RectangleF bounds = new RectangleF(_agent.Node.Physical.Bounds);


            if (config.DoBackgroundShrinkage)
            {
                bounds = ShrinkBorders();
            }



            RotatedRect pos = RotatedRect.FromBoundingLocation(bounds.Location, bounds.Size, 0);
            rend.Draw(config.Background, pos, config.BackgroundColor);
            return true;
        }


        private void DebugPortSetup()
        {
            _ports = new List<DebugPort>();

            if (config.Background == null)
            {
                throw new InvalidOperationException("Agent Type " +_agent.Type.Name + "'s RenderConfig requires a background texture to render debug ports.");
            }

            foreach (Port p in _agent.Node.PortList)
            {
                _ports.Add(new DebugPort(config.Background, p.Descriptor, _agent, this));
            }

        }


        // get the borders of the background in tiles. fair enough.
        private RectangleF ShrinkBorders()
        {


            RectangleF toReturn = new RectangleF(_agent.Node.Physical.Bounds);

            // Perform shrinkage.
            return toReturn.Inflate(-config.Shrinkage, -config.Shrinkage);

        }





    }
}

