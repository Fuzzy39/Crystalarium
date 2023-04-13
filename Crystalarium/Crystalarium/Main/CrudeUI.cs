using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystalarium.Main
{
    internal class CrudeUI
    {

        private double frameRate = 60;

        public CrudeUI()
        {

        }

        public void Draw(IBatchRenderer rend)
        {

           

            // Draw text on top of the game.

            DrawText( rend);

            if (Engine.Controller.Context == "menu")
            {
                DrawMenu(rend);
            }

            EndDraw(rend);
        }

        private void DrawString(string s, Vector2 pos, IBatchRenderer rend)
        {
           rend.DrawString(Textures.Consolas, s, pos, 22, Color.White);
        }

        // draw info on top of the game.
        private void DrawText( IBatchRenderer rend)
        {

            frameRate += (((1 / gameTime.ElapsedGameTime.TotalSeconds) - frameRate) * 0.1);

            string info = "Hovering over: " + actions.GetMousePos().X + ", " + actions.GetMousePos().Y;
            if (Engine.Controller.Context == "menu")
            {
                info = "Hovering over: N/A, N/A";
            }

            string rules = "Ruleset: " + CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...


            DrawString("FPS: " + Math.Round(frameRate, 1) + " Sim Speed: " + Engine.Sim.ActualStepsPS + " Steps/Second Chunks: "
                + Map.ChunkCount + " Agents: " + Map.AgentCount + " Connections: " + Map.ConnectionCount, new Point(10, 10));

            DrawString("Placing: " + actions.CurrentType.Name + " (facing " + actions.Rotation + ") \n" + info + "\n" + rules, new Point(10, 30));

            DrawString("Press " + Engine.Controller.GetAction("Instructions").FirstKeybindAsString() + " For instructions.", new Point(10, (int)(rend.Height) - 50));

        }

        //draw the crude menu for switching rulesets.
        private void DrawMenu( IBatchRenderer rend)
        {
            rend.Draw(Textures.pixel, new RotatedRect(new(0), new(rend.Width, rend.Height), 0, new()), new Color(0, 0, 0, 180));
            currentMenu.Draw(rend);

        }





        // draw the build number, the most important thing!
        private void EndDraw( IBatchRenderer rend)
        {

            DrawString("Milestone 7, Build " + BUILD, new Vector2(10, rend.Height - 25), rend);

            Engine.EndDraw();
            return;




        }
    }
}
