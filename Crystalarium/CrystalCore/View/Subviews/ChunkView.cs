using CrystalCore.Model.Elements;
using CrystalCore.View.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;


namespace CrystalCore.View.Subviews
{
    internal class ChunkView : Subview
    {
        // the basic idea is that a renderer is highly configurable. It can render a chunk in several ways.
        // RendererTemplate is used to configure chunk renderers on mass.


        private ChunkViewConfig config;


        // Constructor
        internal ChunkView(GridView v, Chunk ch, List<Subview> others, ChunkViewConfig config) : base(v, ch, others)
        {
            this.config = config;

        }

        protected override void Render(SpriteBatch sb)
        {

            if (config.ChunkBackground == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }

            renderTarget.Camera.RenderTexture(sb, config.ChunkBackground, RenderData.Bounds, determineColor());


        }


        private Color determineColor()
        {
            // first, check for the base color of this chunk.
            Color toReturn = determineBaseColor();

            // check for situations that demand brightening.

            // brighten checkboard tiles, if needbe.
            if (config.DoCheckerBoardColoring)
            {
                Point pos = RenderData.Grid.GetChunkPos((Chunk)RenderData);
                if ((pos.X + pos.Y) % 2 == 0)
                {
                    brighten(ref toReturn, 1);
                }
            }

            // check if we are being viewed.
            if (renderTarget.ViewCastTarget != null && isRenderedByTarget((Chunk)RenderData))
            {
                brighten(ref toReturn, 2.5);
            }
                
            return toReturn;

        }


        private Color determineBaseColor()
        {
            // Currently, the only situation where the base color changes
            // is if we are the origin chunk. (provided that rule applies in our case)

            if (config.OriginChunkColor != null)
            {
                if (((Chunk)RenderData).Coords.Equals(new Point(0)))
                {
                    return (Color)config.OriginChunkColor;
                }

            }

            return config.BackgroundColor;

        }

        private void brighten(ref Color c, double amount)
        {
            int brightenAmount = config.BrightenAmmount;
            c = new Color(
                (int)(brightenAmount * amount + c.R),
                (int)(brightenAmount * amount + c.G),
                (int)(brightenAmount * amount + c.B));

        }


        // is our viewcast target rendering our chunk?
        private bool isRenderedByTarget(Chunk ch)
        {
            foreach (ChunkView r in renderTarget.ViewCastTarget.Manager.ChunkViews)
            {
                if (r.RenderData == ch)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

