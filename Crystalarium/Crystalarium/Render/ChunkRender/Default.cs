using System;
using System.Collections.Generic;
using System.Text;
using Crystalarium.Sim;
using Microsoft.Xna.Framework.Graphics;

namespace Crystalarium.Render.ChunkRender
{
    public class Default: Renderer
    {

        // a very simple renderer, that does very little.

        public Default(GridView v, Chunk ch, List<Renderer> others) : base(v, ch, others) { }

        protected override void Render(SpriteBatch sb)
        {
            // do... things.
            renderTarget.Camera.RenderTexture(sb, Textures.testSquare, renderData.Bounds);

        }
    }
}
