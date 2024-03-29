﻿using CrystalCore.Model.Elements;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;


namespace CrystalCore.View.Subviews
{
    internal class ChunkView : Subview
    {
        // the basic idea is that a renderer is highly configurable. It can render a chunk in several ways.
        // RendererTemplate is used to configure chunk renderers on mass.


        private ChunkViewConfig config;


        // Constructor
        internal ChunkView(GridView v, Chunk ch, ChunkViewConfig config) : base(v, ch)
        {
            this.config = config;

        }

        public override bool Draw(IRenderer rend)
        {

            if (!base.Draw(rend))
            {
                return false;
            }

            // check that we are visible on screen.
            if (!renderTarget.Camera.TileBounds.Intersects(_renderData.Bounds))
            {

                return true;

            }

            if (config.ChunkBackground == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }


            rend.Draw(config.ChunkBackground, RenderData.Bounds, determineColor());
             
            return true;

        }


        private Color determineColor()
        {
            // first, check for the base color of this chunk.
            Color toReturn = determineBaseColor();

            // check for situations that demand brightening.

            // brighten checkboard tiles, if needbe.
            if (config.DoCheckerBoardColoring)
            {
                Point pos = RenderData.Map.GetChunkPos((Chunk)RenderData);
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

