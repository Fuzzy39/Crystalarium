using CrystalCore.Sim;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using ChunkRenderer = CrystalCore.View.ChunkRender.Renderer;

namespace CrystalCore.View.AgentRender
{
    internal class BasicRenderer : RendererBase
    {
        /*
         * An agent renderer renders agents. Makes sense.
         * AgentView.Renderer and ChunkView.RendererBase should have one superclass, shouldn't they...
         * 
         */

     
        private ChunkRenderer _parent; // the Chunk that is the parent of the agent we are rendering.

        private Texture2D _background; // Our background texture
        private Color _BGcolor; // our background color.

        public ChunkRenderer Parent
        {
            get => _parent;
        }

        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }

        public Color BackgroundColor
        {
            get => _BGcolor;
            set => _BGcolor = value;
        }



        internal BasicRenderer(GridView v, Agent a, List<RendererBase> others) : base(v, a, others)
        {

            _background = null;
            _BGcolor = Color.White;
          
        }

        protected override void Render(SpriteBatch sb)
        {
            // should we die?
            if(_parent == null)
            {
                this.Destroy();
            }

            // render the thing if we have been set to.
            if (_background == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }
           
            renderTarget.Camera.RenderTexture(sb, _background, RenderData.Bounds, _BGcolor, ((Agent)RenderData).Facing.ToRadians());
            
        }



    }
}

