using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CrystalCrash.Main
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
            string[] randomSplash = { ":(", "X(", "Straight From the Chaos Realms!", ":/", "Hmm. This is awkward.", "And once again, I make a fool of myself.",
            "Same as it ever was. Same as it ever was.", "Did somebody forget a semicolon?", "I, Er, had a bit of an oopsie...", "And it was looking so good!","Aw, crap, this looks complicated.",
                "I didn't realize how unstable computers computing computers would be.", "Maybe just do this in Minecraft?",
                "Well, at least you can relax here.", "Take a deep breath, we'll figure this out.", "Dang. Right in the beanhole.", "Breaking: Bean-Counter blames the Compiler!"
              , "This puts the M. in Crystal M.!", "\"This is such a roblox ripoff, smh\"  - Roblox Kid", "Use this time to draw a picture of Squidward."
            ,"This is an Obamnamation!","Ooh, look, the screen turned gay!", "*Switches your xor gates to xnor gates.*" };
            random = new Random();
            face = randomSplash[random.Next(randomSplash.Length)];


            this.errorMessage = errorMessage;
            Console.WriteLine(errorMessage);
        }


        internal void Draw(GraphicsDevice gd)
        {


            // make everything a flat color.
            int r = (int)((MathF.Sin(i) + 1) * 100);
            int g = (int)((MathF.Sin(i + 2) + 1) * 100);
            int b = (int)((MathF.Sin(i + 4) + 1) * 100);
            gd.Clear(new Color(r, g, b));

            // print the error message.
            sb.Begin();
            DrawString(sb, new Vector2(50, 50), face, .4f);
            DrawString(sb, new Vector2(50, 100), errorMessage, .16f);

            i += .005f;

            sb.End();



        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s, float scale)
        {
            // Textures.Consolas.Draw(sb, s, scale, pos, Color.White);
            sb.DrawString(CrashHandler.Consolas, s, pos, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 0f);
        }

        private void DrawString(SpriteBatch sb, Vector2 pos, string s)
        {
            DrawString(sb, pos, s, 1f);
        }

    }
}
