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
                throw new InvalidOperationException("Must supply beamview with texture.");

            }


            if (((Beam)_renderData).Start is HalfPort)
            {
                RenderHalf(sb);
                return;
            }

            throw new NotImplementedException("What a sad fool");
        }


        private void RenderHalf(SpriteBatch sb)
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

                if(beam.End==null)
                {
                    renderBounds.Height += .5f;

                    if (d == Direction.down)
                    {  
                        renderBounds.Y += .5f;
                    }
                }
                else
                {
                    renderBounds.Y += .5f;
                }
            }
            else
            {
                renderBounds.Height = _beamWidth;
                renderBounds.Y += (1 - _beamWidth) / 2f;

                if (beam.End == null)
                {
                    renderBounds.Width += .5f;

                    if (d == Direction.right)
                    {
                        renderBounds.X += .5f;
                    }
                }
                else
                {
                    renderBounds.X += .5f;
                }
               
            }

            renderTarget.Camera.RenderTexture(sb, _beamTexture, renderBounds, _color);

        }


        // the bean beam? I'm confused...
    }
}
