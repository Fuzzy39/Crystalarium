using CrystalCore.Model.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;


namespace CrystalCore.View.ChunkRender
{
    internal class ChunkView : Subview
    {
        // the basic idea is that a renderer is highly configurable. It can render a chunk in several ways.
        // RendererTemplate is used to configure chunk renderers on mass.


        private Texture2D _chunkBG; // the texture for use as the chunk's background.

        private Color _BGColor; // the default color for a chunk. default is white.

        internal int brightenAmount; // the amount of increase in every RGB channel, should a chunk be brightened.
                                     // any brightening events will stack with each other.

        internal bool doCheckerBoardColoring; // if true, every other chunk will be brighter, in a checkerboard pattern.


        private Color? _originChunkColor; // if not null, the chunk with coords 0,0 will have this color.

        private GridView _viewCastTarget; // if not null, chunks viewed by this gridview (assuming it has the same grid) will be brightened.
                                          // this gridview should not be the same as the gridview that this renderer is part of.

       


  

        internal Color? OriginChunkColor
        {
            get => _originChunkColor;
            set => _originChunkColor = value;
        }

        internal GridView ViewCastTarget
        {
            get => _viewCastTarget;
            set
            {
                if (value == null)
                {
                    _viewCastTarget = null;
                    return;
                }

                if (value == renderTarget)
                {

                    throw new InvalidOperationException("A Gridview cannot viewcast itself.");
                }

                if (value.Grid != RenderData.Grid)
                {
                    // hopefully these error messages make sense.
                    throw new InvalidOperationException("Viewcasting requires GridViews that view the same grid.");
                }

                _viewCastTarget = value;


            }
        }


        // Constructor
        internal ChunkView(GridView v, Chunk ch, List<Subview> others, Texture2D chunkBG, Color BGColor) : base(v, ch, others)
        {
            // reasonable defaults.
            _chunkBG = chunkBG;
            _BGColor = BGColor;

            brightenAmount = 30;
            doCheckerBoardColoring = false;


            _originChunkColor = null;


            _viewCastTarget = null;

         

          


        }

        protected override void Render(SpriteBatch sb)
        {


            if (_chunkBG == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }

            renderTarget.Camera.RenderTexture(sb, _chunkBG, RenderData.Bounds, determineColor());
           

         
          

        }


        private Color determineColor()
        {
            // first, check for the base color of this chunk.
            Color toReturn = determineBaseColor();

            // check for situations that demand brightening.

            // brighten checkboard tiles, if needbe.
            if(doCheckerBoardColoring)
            {
                Point pos = RenderData.Grid.getChunkPos((Chunk)RenderData);
                if ((pos.X + pos.Y) % 2 == 0)
                {
                    brighten(ref toReturn, 1);
                }
            }

            // check if we are being viewed.
            if(_viewCastTarget!=null && isRenderedByTarget((Chunk)RenderData))
            {
                brighten(ref toReturn,2.5);
            }

            return toReturn;

        }


        private Color determineBaseColor()
        {
            // Currently, the only situation where the base color changes
            // is if we are the origin chunk. (provided that rule applies in our case)

            if (_originChunkColor != null)
            { 
                if(((Chunk)RenderData).Coords.Equals(new Point(0)))
                {
                    return (Color)_originChunkColor;
                }
           
            }

            return _BGColor;

        }

        private void brighten(ref Color c, double amount)
        {
            c = new Color(
                (int)(brightenAmount*amount + c.R), 
                (int)(brightenAmount*amount + c.G),
                (int)(brightenAmount*amount + c.B));
          
        }


        // is our viewcast target rendering our chunk?
        private bool isRenderedByTarget(Chunk ch)
        {
            foreach (ChunkView r in _viewCastTarget.Manager.ChunkViews)
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

