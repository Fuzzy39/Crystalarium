using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium
{
    /// <summary>
    /// An ErrorSplash is what appears when CrystalCore cannot initalize, or maybe other problems later.
    /// Probably temporary? maybe not.
    /// </summary>
    internal class ErrorSplash
    {
        private float i;
        private string errorMessage;
       
        internal ErrorSplash( string errorMessage )        {
            this.i = 0f;
           
            this.errorMessage = errorMessage;
        }


        internal void Draw(SpriteBatch sb, GraphicsDevice gd)
        {
            // make everything a flat color.
            int r = (int)((MathF.Sin(i)+1) * 100 );
            int g = (int)((MathF.Sin(i+2)+1) * 100);
            int b = (int)((MathF.Sin(i+4)+1) * 100);
            gd.Clear(new Color(r, g, b));
    
            // print the error message.
            DrawString(sb, new Vector2(200,200), ":(", 3);
            DrawString(sb, new Vector2(300, 200), errorMessage);

            i += .005f;
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s, float scale)
        {
            sb.DrawString(Textures.testFont, s, pos, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 0);
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s )
        {
            DrawString(sb, pos, s, 1f);
        }

    }
}
