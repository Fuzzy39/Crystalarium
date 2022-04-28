using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Render
{
    public class Border
    {

        // the border class represents the borders around a viewbox 
        // for now, just viewboxes.



        // fields
        private Texture2D _sideTexture; // the side border texture of this viewport
        private Texture2D _cornerTexture; // the corner border texture of this viewport
        private int _width; // the width, in pixels, of the border of the viewport
        private Color _color;

        private Viewbox parent;
  


        public int Width
        {
            get => _width;
            set { _width = (value < 0) ? 0 : value;  }
        }

        public Color Color
        {
            get => _color;
            set { _color=value; }
        }


        public void SetTextures(Texture2D side, Texture2D corner)
        {
            _sideTexture = side;
            _cornerTexture = corner;
        }


        public Border(Viewbox parent)
        {
            // border related variables
            _width = 0;
            _sideTexture = null;
            _cornerTexture = null;
            _color = Color.White;

            // params
            this.parent= parent;
            
        }

        public void Draw(SpriteBatch sb)
        {
            drawBorders(sb);
            drawCorners(sb);
        }


        private void drawBorders(SpriteBatch sb)
        {
            if (_sideTexture == null || Width < 1)
            {
                return;
            }

            // I could make this a loop like drawCorners.
            // I'm not sure which is better...

            Point pos;
            Point size;

            // top side.
            pos = new Point(parent.PixelBounds.X, parent.PixelBounds.Y);
            size = new Point(parent.PixelBounds.Width,Width);
            DrawSingleBorder(sb, pos, size);

            // bottom side.
            pos = new Point(parent.PixelBounds.X, parent.PixelBounds.Y + parent.PixelBounds.Height);
            DrawSingleBorder(sb, pos, size);

            // left side.
            pos = new Point(parent.PixelBounds.X, parent.PixelBounds.Y) ;
            size = new Point(Width, parent.PixelBounds.Height );
            DrawSingleBorder(sb, pos, size);

            //right side.
            pos = new Point(parent.PixelBounds.X + parent.PixelBounds.Width-Width, parent.PixelBounds.Y);
            DrawSingleBorder(sb, pos, size);

        }


        // draws one border of the viewport, given appropriate values.
        private void DrawSingleBorder(SpriteBatch sb, Point pos, Point size)
        {
            

            sb.Draw(
                _sideTexture,
                new Rectangle(pos, size),
                null,
                _color,
                0f,
                new Vector2(0, 0),
                new SpriteEffects(),
                1f
            );

        }

        private void drawCorners(SpriteBatch sb)
        {

            // do not draw corners if we don't have corners to draw.
            if (_cornerTexture == null || Width < 1)
            {
                return;
            }

            // draw all 4 corners

            // TODO: rotate sprites here so that corner designs are facing the correct way.

            // this might be stupid, but I'm not repeating code..
            int x = parent.PixelBounds.X;
            for (int xCounter = 0; xCounter < 2; xCounter++, x += parent.PixelBounds.Width - Width)
            {
                int y = parent.PixelBounds.Y;
                for (int yCounter = 0; yCounter < 2; yCounter++, y += parent.PixelBounds.Height - Width)
                {

                    sb.Draw(
                        _cornerTexture,
                        new Rectangle(x, y, Width, Width),
                        _color
                    );

                }
            }



        }

    }
}
