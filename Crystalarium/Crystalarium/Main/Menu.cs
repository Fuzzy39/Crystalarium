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
        private string returnMsg;
        private CreateMenuText createMenuText;
        private indexDecision shouldStopEntries;
        private indexDecision shouldSkip;

        internal Menu(string title, string returnmsg, CreateMenuText createMenuText, indexDecision shouldStopEntries, indexDecision shouldSkip)
        {
            Title = title;
            this.returnMsg = returnmsg;
            this.createMenuText = createMenuText;
            this.shouldStopEntries = shouldStopEntries;
            this.shouldSkip = shouldSkip;
        }

        internal void Draw(SpriteBatch sb)
        {
            Textures.Consolas.Draw(sb, Title, 33, new Vector2(100, 100), Color.White);
            Textures.Consolas.Draw(sb, returnMsg, 22, new Vector2(120, 150), Color.White);

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

                Textures.Consolas.Draw(sb, createMenuText(i), 22, new Vector2(120, 150 + (25 * spacing)), Color.White);
                spacing++;
            }
        }

    }
}
