using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Subviews
{
    /// <summary>
    /// A SignalView Renders Signals.
    /// Not any signal, just beams.
    /// Note that the renderer in its current state stretches its texture considerably.
    /// </summary>
    internal class SignalView : Subview
    {

        private SignalViewConfig config;

        private enum ChannelState
        {
            Unbounded,
            Bounded,
            Active
        }


        public SignalView(GridView v, Connection b, SignalViewConfig config) : base(v, b)
        {
            this.config = config;

            if (b.Start.AbsoluteFacing.IsDiagonal() & config.SignalTexture!=null)
            {
                throw new NotImplementedException("Diagonal Signal Rendering is not yet supported");
            }
        }


       

        protected override void Render(IRenderer rend)
        {
            if (config.SignalTexture == null)
            {
                return;

            }

        
            RenderFromA(rend);
            RenderFromB(rend);

           

        }


        private void RenderFromA(IRenderer rend)
        {

            Connection beam = (Connection)_renderData;
            Direction absfacing = (Direction)beam.Start.AbsoluteFacing.ToDirection();
            Direction facing = absfacing;

            // if portA is null, then the direction from A would be reversed from the start, which is B.
            if (beam.PortA != beam.Start)
            {
                facing = absfacing.Opposite();
            }

            bool hasEnd = beam.End != null;
            int value = beam.FromA;
            RenderChannel(rend, facing, absfacing, hasEnd, value);
        }

        private void RenderFromB(IRenderer rend)
        {

            Connection beam = (Connection)_renderData;
            Direction absfacing = (Direction)beam.Start.AbsoluteFacing.ToDirection();
            Direction facing = absfacing;

            // if portA is null, then the direction from A would be reversed from the start, which is B.
            if (beam.PortB != beam.Start)
            {
                facing = absfacing.Opposite();
            }

            bool hasEnd = beam.End != null;
            int value = beam.FromB;
            RenderChannel(rend, facing, absfacing, hasEnd, value);
        }


        private void RenderChannel(IRenderer rend, Direction facing, Direction absFacing, bool hasEnd, int value)
        {

            ChannelState cs = DetermineState(value, hasEnd);

            // slightly jank.
            if (cs == ChannelState.Active || renderTarget.DoDebugRendering)
            {
                RectangleF renderBounds = new RectangleF(CalcLoc(absFacing, hasEnd), CalcSize(facing, hasEnd));



                renderBounds = RenderFull(renderBounds, facing);
                RotatedRect actualBounds = RotatedRect.FromFootprint(renderBounds, facing);

                rend.Draw(config.SignalTexture, actualBounds, DetermineColor(cs));


            }
        }



        private ChannelState DetermineState(int value, bool hasEnd)
        {
             if (value != 0)
            {
                return ChannelState.Active;
            }
             
            if (!hasEnd)
            {
                return ChannelState.Unbounded;
            }


            return ChannelState.Bounded;
        }

        private Color DetermineColor(ChannelState cs)
        {
            switch (cs)
            {
                case ChannelState.Unbounded:
                    return Color.Black;
                case ChannelState.Bounded:
                    return Color.DimGray;

                case ChannelState.Active:
                    return config.Color;
            }

            return Color.Magenta;
            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The size, in tiles, of the beam as it will be rendered.</returns>
        private Vector2 CalcSize(Direction facing, bool hasEnd)
        {
            // get helpful variables.
            Connection beam = (Connection)_renderData;


            Vector2 size = new Vector2(config.SignalWidth, beam.Length);

         
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
            Connection beam = (Connection)_renderData;
            

            Vector2 loc = new Vector2(0);
            loc.X = (1 - config.SignalWidth) / 2f; // adjust for the width of the beam.

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
