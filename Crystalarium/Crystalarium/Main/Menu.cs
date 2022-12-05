using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Main
{
    delegate string CreateMenuText(int index);
    delegate bool indexDecision(int index);
    internal class Menu
    {

        public string Title { get; set; }
        private CreateMenuText createMenuText;
        private indexDecision shouldStopEntries;
        private indexDecision shouldSkip;

        internal Menu(string title, CreateMenuText createMenuText, indexDecision shouldStopEntries, indexDecision shouldSkip)
        {
            Title = title;
            this.createMenuText = createMenuText;
            this.shouldStopEntries = shouldStopEntries;
            this.shouldSkip = shouldSkip;
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.DrawString(Textures.testFont, Title, new Vector2(100, 100), Color.White, 0f, new Vector2(), 1.5f, SpriteEffects.None, 0);
            sb.DrawString(Textures.testFont, "Press P or Esc. to return to game.", new Vector2(120, 150), Color.White);

            int spacing = 1;
            for (int i = 1; i <= 9; i++)
            {
                if (shouldStopEntries(i))
                {
                    break;
                }

                if(shouldSkip(i))
                {
                    continue;
                }

                sb.DrawString(Textures.testFont, "Press " + i + " to "+createMenuText(i)+".", new Vector2(120, 150 + (25 * spacing)), Color.White);
                spacing++;
            }
        }

    }
}
