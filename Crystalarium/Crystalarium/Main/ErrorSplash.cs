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

        internal ErrorSplash(string errorMessage)
        {
            i = 0f;
            string[] faces = { ":(", "X(", ":O" };
            random = new Random();
            face = faces[random.Next(faces.Length)];
            this.errorMessage = errorMessage;
        }


        internal void Draw(SpriteBatch sb, GraphicsDevice gd)
        {
            // make everything a flat color.
            int r = (int)((MathF.Sin(i) + 1) * 100);
            int g = (int)((MathF.Sin(i + 2) + 1) * 100);
            int b = (int)((MathF.Sin(i + 4) + 1) * 100);
            gd.Clear(new Color(r, g, b));

            // print the error message.
            
            DrawString(sb, new Vector2(50, 50),face, 2.5f);
            DrawString(sb, new Vector2(50, 130), errorMessage,.75f);

            i += .005f;
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s, float scale)
        {
            sb.DrawString(Textures.testFont, s, pos, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 0);
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s)
        {
            DrawString(sb, pos, s, 1f);
        }

    }
}
