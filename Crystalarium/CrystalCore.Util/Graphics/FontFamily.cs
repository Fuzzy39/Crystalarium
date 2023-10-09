using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.Util.Graphics
{
    /// <summary>
    /// A Font Family is a bunch of spritefonts working together
    /// Hopefully, a font can be rendered of any size.
    /// </summary>
    public class FontFamily
    {

        // sorted by size 
        private List<SpriteFont> fonts;

        public FontFamily(params SpriteFont[] fonts)
        {
            if (fonts == null || fonts.Count() == 0)
            {
                throw new ArgumentException("FontFamily must have at least one font.");
            }

            this.fonts = new List<SpriteFont>(fonts);

            this.fonts.Sort(new Comparison<SpriteFont>((SpriteFont x, SpriteFont y) =>
            {
                return x.LineSpacing - y.LineSpacing;
            }));

        }


        public void Draw(SpriteBatch sb, string s, float height, Vector2 loc, Color color)
        {
            SpriteFont? font = null;
            float fontHeight = 0;


            int i = 0;

            // determine which of our fonts is closest to the desired height.
            foreach (SpriteFont sf in fonts)
            {
                fontHeight = sf.MeasureString(" ").Y;

                if (fontHeight >= height)
                {
                    font = sf;

                    break;

                }
                i++;
            }


            if (font == null)
            {
                font = fonts[^1];
            }



            // compute the scale required for heights to match.
            float scale = height / fontHeight;

            // draw the font
            sb.DrawString(font, s, loc, color, 0, new Vector2(0), scale, SpriteEffects.None, 0f);

        }


    }
}
