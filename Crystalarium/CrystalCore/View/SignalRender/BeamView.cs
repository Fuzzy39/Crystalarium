using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.SignalRender
{
    internal class BeamView : Subview
    {
        /*
         * A BeamView Renders Beams.
         * Not any signal, just beams.
         */

        private Texture2D _beamTexture; // the texture for use as the chunk's background.

        private Color _color; // the default color for a chunk. default is white.

        private float _beamWidth;


        internal float BeamWidth
        {
            get => _beamWidth;

            set
            {

                if (value >= .01f & value<=MaxBeamWidth())
                {
                    _beamWidth = value;
                    return;
                }

                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

      

        public BeamView(GridView v, Beam b, List<Subview> others, Texture2D beamTexture, Color color) : base(v, b, others)
        {
            _beamTexture = beamTexture;
            _color = color;

            BeamWidth = .4f;
        }


        private float MaxBeamWidth()
        {
            if (((Beam)_renderData).Start is HalfPort)
            {
                return .5f;
            }

            return 1f;
        }

        protected override void Render(SpriteBatch sb)
        {
            if (_beamTexture == null)
            {
                return;

            }

            RectangleF renderBounds = RenderHalf();

            // which way does this beam flow?
            CompassPoint absFacing = ((Beam)RenderData).Start.AbsoluteFacing;
            Direction facing = (Direction)absFacing.ToDirection();


            if (((Beam)_renderData).Start is FullPort)
            {
                renderBounds = RenderFull(renderBounds, facing);
              
            }

            renderTarget.Camera.RenderTexture(sb, _beamTexture, renderBounds, _color, facing);
            
        }


        private RectangleF RenderHalf()
        {
            Beam beam = (Beam)_renderData;
            CompassPoint cp = beam.Start.AbsoluteFacing;
            if (cp.IsDiagonal())
            {
                throw new NotImplementedException("Diagonal Beam Rendering is not yet supported");
            }

            Direction d = (Direction)cp.ToDirection();

            RectangleF renderBounds = new RectangleF(beam.Bounds);

            if (d.IsVertical())
            {
                renderBounds.Width = _beamWidth;
                renderBounds.X += (1 - _beamWidth) / 2f;

                //renderBounds.Height += .5f;
               
                renderBounds.Height -= .5f;

                if (beam.End == null & d == Direction.down)
                {

                    renderBounds.Y += .5f;

                }

                if (beam.End!=null)
                {
                    renderBounds.Y += .5f;
                    renderBounds.Height -= .5f;
                }
            }
            else
            {
                renderBounds.Height = _beamWidth;
                renderBounds.Y += (1 - _beamWidth) / 2f;


                renderBounds.Width -= .5f;

                if (beam.End == null & d == Direction.right)
                {

                    renderBounds.X += .5f;

                }

                if (beam.End != null)
                {
                    renderBounds.X += .5f;
                    renderBounds.Width -= .5f;
                }

            }

            return renderBounds;

        }

        private RectangleF RenderFull(RectangleF start, Direction facing)
        {
            float f = .25f;
            switch(facing)
            {
                case Direction.up:
                    start.X += f;
                    break;
                case Direction.down:
                    start.X -= f;
                    break;
                case Direction.left:
                    start.Y -= f;
                    break;
                case Direction.right:
                    start.Y += f;
                    break;
            }
            return start;
        }


        // the bean beam? I'm confused...
    }
}
