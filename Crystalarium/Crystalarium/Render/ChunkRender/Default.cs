using System;
using System.Collections.Generic;
using System.Text;
using Crystalarium.Sim;
using Microsoft.Xna.Framework.Graphics;

namespace Crystalarium.Render.ChunkRender
{
    public class Default: Renderer
    {
        public Default(Viewbox v, Chunk ch) : base(v, ch) { }

        protected override void Render(SpriteBatch sb)
        {
            // do... things.
            renderTarget.RenderTexture(sb, Textures.tile, renderData.Bounds);

        }
    }
}
