using CrystalCore.Input;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using CrystalCore;
using CrystalCore.Model.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystalarium.Main
{
    internal class CrudeUI
    {

        private CrystalGame game;

        private double frameRate = 60;


        internal Menu currentMenu = null;

        internal Menu RulesetMenu { get; private set; }
        internal Menu SaveMenu { get; private set; }
        internal Menu LoadMenu { get; private set; }

        internal Menu InstructionsMenu { get; private set; }



        public CrudeUI(CrystalGame game)
        {

            this.game = game;


            // well, what is ui structure but a bunch of data definitions and hooks into actual code?
            // this will be expanded on in the future, I bet.

            // all of this is actually hideous
            // horrible
            // I despise looking at this
            Engine Engine = game.Engine;

            RulesetMenu = new Menu("Switch Ruleset?",
                  "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",

                (int i) => { return "Press " + i + " to " + "switch to ruleset '" + Engine.Rulesets[i - 1].Name + "'."; },
                (int i) => { return false; },
                (int i) => { return Engine.Rulesets.Count < i; });

            SaveMenu = new Menu("Save Map",
                 "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
                (int i) =>
                {
                    string path = Path.Combine("Saves", i + ".xml");
                    return "Press " + i + " to " + "save in slot " + i + " (" + (File.Exists(path) ? (new FileInfo(path).Length / 1024 + " KB).") : "Empty).");

                },
                (int i) => { return false; },
                 (int i) => { return false; }
                );


            LoadMenu = new Menu("Load Map",
                 "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
                (int i) =>
                {
                    string path = Path.Combine("Saves", i + ".xml");
                    return "Press " + i + " to " + "load from slot " + i + " (" + (File.Exists(path) ? (new FileInfo(path).Length / 1024 + " KB).") : "Empty).");

                },
                (int i) => { return false; },
                (int i) => { return !File.Exists(Path.Combine("Saves", i + ".xml")); }
                );

            InstructionsMenu = new Menu("Controls",
                "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
             (int i) => // mostly this part, ew
             {
                 Controller c = Engine.Controller;
                 return "These are Crystalarium's Controls. They can be edited in Settings/Controls.xml.\nIf multiple keybinds for a control are defined, only the first is listed here." +

                 "\n\nCamera: Move: " + c.GetAction("CamUp").FirstKeybindAsString() + c.GetAction("CamLeft").FirstKeybindAsString()
                 + c.GetAction("CamDown").FirstKeybindAsString() + c.GetAction("CamRight").FirstKeybindAsString()
                 + ". Zoom: Scrollwheel. Pan: " + c.GetAction("Pan").FirstKeybindAsString()
                 + ". Toggle Debug View: " + c.GetAction("ToggleDebugView").FirstKeybindAsString() +

                 ".\nInteract: Place: " + c.GetAction("PlaceAgent").FirstKeybindAsString() + ". Remove: "
                 + c.GetAction("RemoveAgent").FirstKeybindAsString() + ". Rotate: " + c.GetAction("RotateAgent").FirstKeybindAsString() +
                 ".\nSelect Agents: Previous/Next Agent: " + c.GetAction("PrevAgent").FirstKeybindAsString() + ", " + c.GetAction("NextAgent").FirstKeybindAsString() +
                 ". Select: Number keys. Pipette: " + c.GetAction("Pipette").FirstKeybindAsString() + "." +

                 "\nSimulation: Pause/Unpause: " + c.GetAction("ToggleSim").FirstKeybindAsString() +
                 ". Single Step: " + c.GetAction("SimStep").FirstKeybindAsString() + ". Decrease/Increase Speed: " +
                 c.GetAction("DecreaseSimSpeed").FirstKeybindAsString() + ", " + c.GetAction("IncreaseSimSpeed").FirstKeybindAsString() +

                 ".\nOther: Switch Ruleset: " + c.GetAction("OpenRulesetMenu").FirstKeybindAsString() + ". Save: " + c.GetAction("Save").FirstKeybindAsString() +
                 ". Load: " + c.GetAction("Load").FirstKeybindAsString() + ".\n" +
                 "Copy/Paste: Copy: " + c.GetAction("Copy").FirstKeybindAsString() + ". Cut: " + c.GetAction("Cut").FirstKeybindAsString() +
                 ". Paste: " + c.GetAction("Paste").FirstKeybindAsString()

                 + "\n\nNote: Copy and Cut will freeze the game. Direct your attention to the console window to perform these commands.";



             },
             (int i) => { return i > 1; },
             (int i) => { return false; }
             );


          
        }

        public void Draw(IBatchRenderer rend, GameTime gameTime)
        {

            frameRate += (((1 / gameTime.ElapsedGameTime.TotalSeconds) - frameRate) * 0.1);

            // Draw text on top of the game.

            DrawText( rend);

            if (game.Engine.Controller.Context == "menu")
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


            Engine Engine = game.Engine;
            Map Map = game.Map;

            string info = "Hovering over: " + game.Controls.GetMousePos().X + ", " + game.Controls.GetMousePos().Y;
            if (Engine.Controller.Context == "menu")
            {
                info = "Hovering over: N/A, N/A";
            }

            string rules = "Ruleset: " + game.CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...


            DrawString("FPS: " + Math.Round(frameRate, 1) + " Sim Speed: " + Engine.Sim.ActualStepsPS + " Steps/Second Chunks: "
                + Map.ChunkCount + " Agents: " + Map.AgentCount + " Connections: " + Map.ConnectionCount, new(10, 10), rend);

            DrawString("Placing: " + game.Controls.CurrentType.Name + " (facing " + game.Controls.Rotation + ") \n" + info + "\n" + rules, new(10, 30), rend);

            DrawString("Press " + Engine.Controller.GetAction("Instructions").FirstKeybindAsString() + " For instructions.", new(10, rend.Height - 50), rend);

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

            DrawString("Milestone 7, Build " + CrystalGame.BUILD, new Vector2(10, rend.Height - 25), rend);

            
            return;




        }
    }
}
