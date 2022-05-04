using System;
using System.Collections.Generic;
using System.Text;
using Crystalarium.Sim;
using Microsoft.Xna.Framework.Graphics;

namespace Crystalarium.Render.ChunkRender
{
    class Standard : Renderer
    {
        // the standard chunkRenderer.

        public Standard(GridView v, Chunk ch, List<Renderer> others) : base(v, ch, others) { }

        protected override void Render(SpriteBatch sb)
        {
            // just render a grid image for now.
            renderTarget.Camera.RenderTexture(sb, Textures.chunkGrid, renderData.Bounds);

        }

    }
}
