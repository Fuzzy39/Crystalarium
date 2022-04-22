using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crystalarium.Sim
{
    class Viewport
    {
        /*
         * A viewport renders a grid.
         */

        private List<Viewport> container; // the list of all existing viewports.
        private Rectangle _pixelBounds; // the bounds, in pixels, of the viewport on the game window.
        private Grid _grid; // the grid that this viewport is rendering.

        private Texture2D sideTexture; // the side border texture of this viewport
        private Texture2D cornerTexture; // the corner border texture of this viewport
        private int _borderWidth; // the width, in pixels, of the border of the viewport

        public int BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = (value < 0) ? 0 : value; }
        }
        

        


        // create the viewport
        public Viewport(List<Viewport> viewports, Grid g, Point pos, Point dimensions )
        {
            _grid = g;
            container = viewports;
            _pixelBounds = new Rectangle(pos, dimensions);

            sideTexture = null;
            cornerTexture = null;
        }

        // an alternate viewport constructor, without points.
        public Viewport(List<Viewport> viewports, Grid g, int x, int y, int width, int height)
            : this(viewports, g, new Point(x, y), new Point(width, height)) { }


        public void destroy()
        {
            container.Remove(this);
        }

        public void setTextures(Texture2D sides, Texture2D corners) 
        {
            sideTexture = sides;
            cornerTexture = corners;
        
        }

        public void draw(SpriteBatch sb)
        {
            // we drawin' the thing!

            // the last thing we do is draw the border.

            // draw all 4 corners
            sb.Draw(cornerTexture, _pixelBounds.Location.ToVector2(), new Rectangle(0,0, BorderWidth, BorderWidth), Color.White);
            Rectangle topRight = new Rectangle(0,)

        }



   
    }
}
