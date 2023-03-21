    using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Main
{
    /// <summary>
    /// An ErrorSplash is what appears when CrystalCore cannot initalize, or maybe other problems later.
    /// Probably temporary? maybe not.
    /// </summary>
    internal class ErrorSplash
    {
        private float i;
        private string errorMessage;
        Random random;
        private string face;
        private SpriteBatch sb;

        internal ErrorSplash(string errorMessage, SpriteBatch sb)
        {
            this.sb = sb;
            i = 0f;
            string[] faces = { ":(", "X(", ":O",":/" ,":|","!?"};
            random = new Random();
            face = faces[random.Next(faces.Length)];
            
            // a little easter egg, just for people who see it crash a lot (me).
            if(random.NextDouble()<.02)
            {
                face = "Straight from the Chaos Realms!";
            }

            this.errorMessage = errorMessage;
            Console.WriteLine(errorMessage);
        }


        internal void Draw( GraphicsDevice gd)
        {
           

            // make everything a flat color.
            int r = (int)((MathF.Sin(i) + 1) * 100);
            int g = (int)((MathF.Sin(i + 2) + 1) * 100);
            int b = (int)((MathF.Sin(i + 4) + 1) * 100);
            gd.Clear(new Color(r, g, b));

            // print the error message.
            sb.Begin();
            DrawString(sb, new Vector2(50, 50),face, 55f);
            DrawString(sb, new Vector2(50, 130), errorMessage,16.5f);

            i += .005f;

            sb.End();



        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s, float scale)
        {
            Textures.Consolas.Draw(sb, s, scale, pos, Color.White);
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s)
        {
            DrawString(sb, pos, s, 1f);
        }

    }
}
