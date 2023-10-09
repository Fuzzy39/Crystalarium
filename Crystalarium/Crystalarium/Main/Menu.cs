using CrystalCore.View.Core;
using Microsoft.Xna.Framework;

namespace Crystalarium.Main
{
    delegate string CreateMenuText(int index);
    delegate bool indexDecision(int index);
    internal class Menu : IRenderable
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

        public bool Draw(IRenderer renderer)
        {
            renderer.DrawString(Textures.Consolas, Title, new Point(100), 33, Color.White);
            renderer.DrawString(Textures.Consolas, returnMsg, new Point(120, 150), 22, Color.White);


            int spacing = 1;
            for (int i = 1; i <= 9; i++)
            {
                if (shouldStopEntries(i))
                {
                    break;
                }

                if (shouldSkip(i))
                {
                    continue;
                }
                renderer.DrawString(Textures.Consolas, createMenuText(i), new Vector2(120, 150 + (25 * spacing)), 22, Color.White);
                spacing++;
            }

            return true;

        }



    }
}
