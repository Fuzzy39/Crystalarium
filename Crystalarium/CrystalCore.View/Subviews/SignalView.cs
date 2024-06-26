﻿using CrystalCore.Model.Communication;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;

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
        private Connection connection;

        private enum ChannelState
        {
            Unbounded,
            Bounded,
            Active
        }


        public SignalView(GridView v, Connection b, SignalViewConfig config) : base(v, b.Physical)
        {
            this.config = config;
            connection = b;


        }




        public override bool Draw(IRenderer rend)
        {

            if (connection.Destroyed)
            {
                return false;
            }

            // check that we are visible on screen.
            if (!renderTarget.Camera.TileBounds.Intersects(connection.Physical.Bounds))
            {

                return true;

            }

            if (config.SignalTexture == null)
            {
                return true;

            }


            RenderFromA(rend);
            RenderFromB(rend);

            return true;



        }


        private void RenderFromA(IRenderer rend)
        {

            Connection beam = connection;
            CompassPoint absfacing = beam.PortA.AbsoluteFacing;
            CompassPoint facing = absfacing;

            // if portA is null, then the direction from A would be reversed from the start, which is B.
            //if (beam.PortA != beam.Start)
            //{
            //    facing = absfacing.Opposite();
            //}

            bool hasEnd = beam.PortB != null;
            int value = beam.FromA;
            RenderChannel(rend, facing, absfacing, hasEnd, value);
        }

        private void RenderFromB(IRenderer rend)
        {

            Connection beam = connection;
            CompassPoint absfacing = beam.PortA.AbsoluteFacing;
            CompassPoint facing = absfacing.Opposite();

            // if portA is null, then the direction from A would be reversed from the start, which is B.
      
            int value = beam.FromB;
            RenderChannel(rend, facing, absfacing, beam.PortB!=null, value);
        }




        private void RenderChannel(IRenderer rend, CompassPoint facing, CompassPoint absFacing, bool hasEnd, int value)
        {
            
            ChannelState cs = DetermineState(value, hasEnd);
            Connection signal = connection;


            if (cs != ChannelState.Active && !renderTarget.DoDebugRendering)
            {
                return;
            }

            bool drawLeft = facing == absFacing;

            // Main goal here is to find the rotatedrect that makes sense for our situation

            float length = signal.Size.X > signal.Size.Y? signal.Size.X : signal.Size.Y;

            if (hasEnd)
            {
                length -= 1f;
            }
            else
            {
                length -= .5f;
            }

            if (facing.IsDiagonal())
            {
                length *= MathF.Sqrt(2);
            }

            Vector2 size = new(length, config.SignalWidth);


            // let's start location with the center of the tile our port comes from.
            Vector2 location = signal.PortA.Location.ToVector2() + new Vector2(.5f);

            float yAxisAngle = absFacing.ToRadians() + (MathF.PI / 2f); // the angle the 
            float dist = .25f; // the edge
            float deltaX = MathF.Cos(yAxisAngle) * dist * (drawLeft ? -1 : 1);
            float deltaY = MathF.Sin(yAxisAngle) * dist * (drawLeft ? -1 : 1);

            location += new Vector2(deltaX, deltaY);


            RotatedRect renderBounds = new(location, size, absFacing.ToRadians(), new(0, .5f));
            rend.Draw(config.SignalTexture, renderBounds, DetermineColor(value, hasEnd));


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



        private Color DetermineColor(int value, bool hasEnd)
        {
            if (value != 0)
            {
                return config.Colors.getColor(value);
            }

            if (!hasEnd)
            {
                return Color.Black;
            }

            return Color.DimGray;
        }


        // the bean beam? I'm confused...
    } //HELICOPTER
}
