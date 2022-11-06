using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Subviews
{
    /// <summary>
    /// A BeamView Renders Beams.
    /// Not any signal, just beams.
    /// Note that the renderer in its current state stretches its texture considerably.
    /// </summary>
    internal class BeamView : Subview
    {

        private BeamViewConfig config;


        public BeamView(GridView v, Beam b, List<Subview> others, BeamViewConfig config) : base(v, b, others)
        {
            this.config = config;

            if (b.Start.AbsoluteFacing.IsDiagonal() & config.BeamTexture!=null)
            {
                throw new NotImplementedException("Diagonal Beam Rendering is not yet supported");
            }
        }


       

        protected override void Render(SpriteBatch sb)
        {
            if (config.BeamTexture == null)
            {
                return;

            }

            RenderFromA(sb);
            RenderFromB(sb);
           

        }


        private void RenderFromA(SpriteBatch sb)
        {

            Beam beam = (Beam)_renderData;
            Direction facing = (Direction)beam.Start.AbsoluteFacing.ToDirection();

            // if portA is null, then the direction from A would be reversed from the start, which is B.
            if (beam.PortA != beam.Start)
            {
                facing = facing.Opposite();
            }

            bool hasEnd = beam.End != null;
            int value = beam.FromA;
            RenderChannel(sb, facing, hasEnd, value);
        }

        private void RenderFromB(SpriteBatch sb)
        {

            Beam beam = (Beam)_renderData;
            Direction facing = (Direction)beam.Start.AbsoluteFacing.ToDirection();

            // if portA is null, then the direction from A would be reversed from the start, which is B.
            if (beam.PortB != beam.Start)
            {
                facing = facing.Opposite();
            }

            bool hasEnd = beam.End != null;
            int value = beam.FromB;
            RenderChannel(sb, facing, hasEnd, value);
        }


        private void RenderChannel(SpriteBatch sb, Direction facing, bool hasEnd, int value)
        {

            RectangleF renderBounds = new RectangleF(CalcLoc(facing, hasEnd), CalcSize(facing, hasEnd));



            renderBounds = RenderFull(renderBounds, facing);



            renderTarget.Camera.RenderTexture(sb, config.BeamTexture, renderBounds, DetermineColor(value, hasEnd), facing);
        }

        private Color DetermineColor(int value, bool hasEnd)
        {
            if(!hasEnd)
            {
                return Color.Black;
            }

            if(value == 0)
            {
                return Color.DimGray;
            }

            return config.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The size, in tiles, of the beam as it will be rendered.</returns>
        private Vector2 CalcSize(Direction facing, bool hasEnd)
        {
            // get helpful variables.
            Beam beam = (Beam)_renderData;


            Vector2 size = new Vector2(config.BeamWidth, beam.Length);

            if (!hasEnd)
            {
                // we do not want to go out of the grid if we have no target.
                size.Y -= .5f;
            }

            if (facing.IsHorizontal())
            {
                size = new Vector2(size.Y, size.X);
            }

            return size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the location that our rendered bounds will start.</returns>
        private Vector2 CalcLoc(Direction facing, bool hasEnd)
        {
            Beam beam = (Beam)_renderData;
            

            Vector2 loc = new Vector2(0);
            loc.X = (1 - config.BeamWidth) / 2f; // adjust for the width of the beam.

            // if the beam is facing in a positive direction, we always increase the position so that the beam starts in the middle of the tile.
            // otherwise, we only do this if it has an end, so that the starting position is in the middle of a tile.
            // that made no sense, just trust me, it works with this, and doesn't without.
            if (hasEnd || facing == Direction.down || facing == Direction.right)
            {

                loc.Y += .5f;


            }


            if (facing.IsHorizontal())
            {
                loc = new Vector2(loc.Y, loc.X);
            }

            return beam.Bounds.Location.ToVector2() + loc;

        }


        private RectangleF RenderFull(RectangleF start, Direction facing)
        {
            float f = .25f;
            switch (facing)
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
    } //HELICOPTER
}
