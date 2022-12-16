using CrystalCore.Input;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.View.Configs;
using CrystalCore.Model;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Elements;
using System.IO;

namespace Crystalarium.Main
{
    internal class Actions
    {

        /*
         * The actions class is (probably) temporary. It's purpose is to set up user interaction with crystalarium.
         * This includes defining controller actions (code for, for example, placing an agent or panning the camera).
         * And, at least as it stands now, it also defines the keybinds for these actions, though it would be nice to maybe
         * Make those editable, as some sort of file the user can edit, or eventually through UI maybe.
         * 
         * I feel like this class won't exactly be good practice.
         */

        private Controller c; // the controller we bind everything to

        private CrystalGame game; // The game.



        internal Direction Rotation { get; private set; } // the direction that any agents placed down will be facing.



        internal AgentType CurrentType { get; private set; } // the agent type selected to place.

      
       
  

        // used when panning
        private Point panOrigin = new Point();
        private Vector2 panPos = new Vector2();


        internal Actions(Controller c, CrystalGame game)
        {
            // it is assumed that actions is setup after configuration.

            this.c = c;
            this.game = game;

            Rotation = Direction.up;

            SetupController();

            CurrentType = game.CurrentRuleset.AgentTypes[0];

           
       
        }

       internal void OnMapReset(object sender, EventArgs e)
        {

            game.CurrentRuleset = game.Map.Ruleset;
            CurrentType = game.CurrentRuleset.AgentTypes[0];
        }

       

        


        private void SetupController()
        {
            c.Context = "play";

            SetupViewInteraction();

            SetupMapInteraction();

            SetupSimInteraction();

            SetupAgentSelection();

            SetupMenuInteraction();
          


        }



        private void SetupViewInteraction()
        {
            // test code.
            float camSpeed = 1.2f;

            // camera up
            c.addAction("up", () => game.view.Camera.AddVelocity(camSpeed, Direction.up));
            new Keybind(c, Keystate.Down, "up", "play", Button.W);


            // camera down
            c.addAction("down", () => game.view.Camera.AddVelocity(camSpeed, Direction.down));
            new Keybind(c, Keystate.Down, "down", "play", Button.S);

            // camera left
            c.addAction("left", () => game.view.Camera.AddVelocity(camSpeed, Direction.left));
            new Keybind(c, Keystate.Down, "left", "play", Button.A);

            // camera right
            c.addAction("right", () => game.view.Camera.AddVelocity(camSpeed, Direction.right));
            new Keybind(c, Keystate.Down, "right", "play", Button.D);

            // panning

            c.addAction("start pan", () =>
            {
                Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);


                panOrigin = pixelCoords;
                panPos = game.view.Camera.Position;


            });
            new Keybind(c, Keystate.OnPress, "start pan", "play", Button.Y) { DisableOnSuperset = false };


            c.addAction("pan", () =>
            {


                Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);
                Vector2 mousePos = game.view.Camera.PixelToTileCoords(pixelCoords);
                Vector2 originPos = game.view.Camera.PixelToTileCoords(panOrigin);

                Vector2 pos = panPos + (originPos - mousePos);
                if(!game.Map.Bounds.Contains(pos))
                {
                    if(pos.Y> game.Map.Bounds.Bottom)
                    {
                        panOrigin.Y = game.view.Camera.TileToPixelCoords(new Vector2( game.Map.Bounds.Bottom)).Y;
                    }
                        
                    if( pos.Y < game.Map.Bounds.Top)
                    {
                        panOrigin.Y = game.view.Camera.TileToPixelCoords(new Vector2(game.Map.Bounds.Top)).Y;
                    }

                    if (pos.X > game.Map.Bounds.Right)
                    {
                        panOrigin.X = game.view.Camera.TileToPixelCoords(new Vector2(game.Map.Bounds.Right)).X;
                    }

                    if (pos.X < game.Map.Bounds.Left)
                    {
                        panOrigin.X = game.view.Camera.TileToPixelCoords(new Vector2(game.Map.Bounds.Left)).X;
                    }
                }

                game.view.Camera.Position = pos;



            });
            new Keybind(c, Keystate.Down, "pan", "play", Button.Y) { DisableOnSuperset = false };

            c.addAction("toggle debug ports", () => game.view.DoDebugRendering = !game.view.DoDebugRendering);
            new Keybind(c, Keystate.OnPress, "toggle debug ports", "play", Button.O);


        }


        private void SetupMapInteraction()
        {
            // grow the game.Grid!
            c.addAction("grow up", () => game.Map.ExpandGrid(Direction.up));
            new Keybind(c, Keystate.OnPress, "grow up", "play", Button.U);
            c.addAction("grow down", () => game.Map.ExpandGrid(Direction.down));
            new Keybind(c, Keystate.OnPress, "grow down", "play", Button.J);
            c.addAction("grow left", () => game.Map.ExpandGrid(Direction.left));
            new Keybind(c, Keystate.OnPress, "grow left", "play", Button.H);
            c.addAction("grow right", () => game.Map.ExpandGrid(Direction.right));
            new Keybind(c, Keystate.OnPress, "grow right", "play", Button.K);

            c.addAction("place agent", () =>
            {

                Point clickCoords = GetMousePos();
                Agent toRemove = null;

                // remove all agents on this tile (there should only be one once things are working properly)
                while (true)
                {

                    toRemove = game.Map.getAgentAtPos(clickCoords);
                    if (toRemove == null)
                    {
                        break;
                    }

                    toRemove.Destroy();
                }

                if (Entity.IsValidLocation(game.Map, new Rectangle(clickCoords, CurrentType.Size), Rotation))
                {
                    new Agent(game.Map, clickCoords, CurrentType, Rotation);
                }


            });
            new Keybind(c, Keystate.Down, "place agent", "play", Button.MouseLeft);


            c.addAction("remove agent", () =>
            {
                Point clickCoords = GetMousePos();
                Agent toRemove = null;

                // remove all agents on this tile (there should only be one once things are working properly)
                while (true)
                {

                    toRemove = game.Map.getAgentAtPos(clickCoords);
                    if (toRemove == null)
                    {
                        break;
                    }

                    toRemove.Destroy();
                }




            });
            new Keybind(c, Keystate.Down, "remove agent", "play", Button.MouseRight);


            c.addAction("rotate", () =>
            {
                Point clickCoords = GetMousePos();

                Agent a = game.Map.getAgentAtPos(clickCoords);
                if (a == null)
                {
                    Rotation = Rotation.Rotate(RotationalDirection.cw);
                    return;
                }


                a.Rotate(RotationalDirection.cw);
                Rotation = a.Facing;


            });
            new Keybind(c, Keystate.OnPress, "rotate", "play", Button.R);

        }



        private void SetupSimInteraction()
        {
            c.addAction("toggle sim", () =>
            {
                game.Engine.Sim.Paused = !game.Engine.Sim.Paused;


            });
            new Keybind(c, Keystate.OnPress, "toggle sim", "play", Button.Space);


            c.addAction("sim step", () =>
            {

                // no need to step if unpaused.
                if (game.Engine.Sim.Paused) { game.Engine.Sim.Step(); }


            });
            new Keybind(c, Keystate.OnPress, "sim step", "play", Button.Z);

            c.addAction("sim faster", () =>
            {

                SimulationManager sim = game.Engine.Sim;
                if (sim.TargetStepsPS < 100)
                {
                    sim.TargetStepsPS += 10;
                }


            });
            new Keybind(c, Keystate.OnPress, "sim faster", "play", Button.LeftShift);

            c.addAction("sim slower", () =>
            {

                // no need to step if unpaused.
                SimulationManager sim = game.Engine.Sim;
                if (sim.TargetStepsPS > 10)
                {
                    sim.TargetStepsPS -= 10;
                }

            });
            new Keybind(c, Keystate.OnPress, "sim slower", "play", Button.LeftControl);
        }

        private void SetupAgentSelection()
        {
            c.addAction("pipette", () =>
            {
                Point clickCoords = GetMousePos();

                Agent a = game.Map.getAgentAtPos(clickCoords);
                if (a == null)
                {
                    return;
                }


                CurrentType = a.Type;
                Rotation = a.Facing;


            });
            new Keybind(c, Keystate.OnPress, "pipette", "play", Button.Tab);





            c.addAction("next agent", () =>
            {
                List<AgentType> types = game.CurrentRuleset.AgentTypes;

                int i = types.IndexOf(CurrentType);

                i++;
                if (i >= types.Count)
                {
                    i = 0;
                }

                CurrentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "next agent", "play", Button.E);

            c.addAction("prev agent", () =>
            {
                List<AgentType> types = game.CurrentRuleset.AgentTypes;
                int i = types.IndexOf(CurrentType);

                i--;
                if (i < 0)
                {
                    i = types.Count - 1;
                }

                CurrentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "prev agent", "play", Button.Q);

            // agents
            c.addAction("agent 1", () => SwitchAgent(0));
            new Keybind(c, Keystate.OnPress, "agent 1", "play", Button.D1);

            c.addAction("agent 2", () => SwitchAgent(1));
            new Keybind(c, Keystate.OnPress, "agent 2", "play", Button.D2);

            c.addAction("agent 3", () => SwitchAgent(2));
            new Keybind(c, Keystate.OnPress, "agent 3", "play", Button.D3);

            c.addAction("agent 4", () => SwitchAgent(3));
            new Keybind(c, Keystate.OnPress, "agent 4", "play", Button.D4);

            c.addAction("agent 5", () => SwitchAgent(4));
            new Keybind(c, Keystate.OnPress, "agent 5", "play", Button.D5);

            c.addAction("agent 6", () => SwitchAgent(5));
            new Keybind(c, Keystate.OnPress, "agent 6", "play", Button.D6);

            c.addAction("agent 7", () => SwitchAgent(6));
            new Keybind(c, Keystate.OnPress, "agent 7", "play", Button.D7);

            c.addAction("agent 8", () => SwitchAgent(7));
            new Keybind(c, Keystate.OnPress, "agent 8", "play", Button.D8);

            c.addAction("agent 9", () => SwitchAgent(8));
            new Keybind(c, Keystate.OnPress, "agent 9", "play", Button.D9);
        }


        private void SetupMenuInteraction()
        {



      

            c.addAction("open ruleset menu", () =>
            {
               if(c.Context.Equals("play"))
               {
                    c.Context = "menu";
                    game.currentMenu = game.RulesetMenu;
               }
               else
               {
                    c.Context = "play";
                    game.currentMenu = null;
               }
               

            });
            new Keybind(c, Keystate.OnPress, "open ruleset menu", Button.P);


            c.addAction("close", () =>
            {

                if(c.Context.Equals("menu"))
                {
                    c.Context = "play";
                    game.currentMenu = null;
                    return;
                }

                game.Exit();

            });
            new Keybind(c, Keystate.OnPress, "close", Button.Escape);


            c.addAction("menu action 1", () => MenuAction(0));
            new Keybind(c, Keystate.OnPress, "menu action 1", "menu", Button.D1);

            c.addAction("menu action 2", () => MenuAction(1));
            new Keybind(c, Keystate.OnPress, "menu action 2", "menu", Button.D2);

            c.addAction("menu action 3", () => MenuAction(2));
            new Keybind(c, Keystate.OnPress, "menu action 3", "menu", Button.D3);

            c.addAction("menu action 4", () => MenuAction(3));
            new Keybind(c, Keystate.OnPress, "menu action 4", "menu", Button.D4);

            c.addAction("menu action 5", () => MenuAction(4));
            new Keybind(c, Keystate.OnPress, "menu action 5", "menu", Button.D5);

            c.addAction("menu action 6", () => MenuAction(5));
            new Keybind(c, Keystate.OnPress, "menu action 6", "menu", Button.D6);

            c.addAction("menu action 7", () => MenuAction(6));
            new Keybind(c, Keystate.OnPress, "menu action 7", "menu", Button.D7);

            c.addAction("menu action 8", () => MenuAction(7));
            new Keybind(c, Keystate.OnPress, "menu action 8", "menu", Button.D8);

            c.addAction("menu action 9", () => MenuAction(8));
            new Keybind(c, Keystate.OnPress, "menu action 9", "menu", Button.D9);








            c.addAction("save", () => 
            {
                
                
                game.currentMenu = game.SaveMenu;
                c.Context = "menu";
            
            });
            new Keybind(c, Keystate.OnPress, "save", "play", Button.LeftControl, Button.S);

            c.addAction("load", () =>
            {

           

                game.currentMenu = game.LoadMenu;
                c.Context = "menu";


            });
            new Keybind(c, Keystate.OnPress, "load", "play", Button.LeftControl, Button.O);
        }



        private void MenuAction(int i)
        {
            if(game.currentMenu==game.RulesetMenu)
            {
                SwitchRuleset(i);
                return;
            }

            string path =Path.Combine("Saves", (i + 1) + ".xml");


            if (game.currentMenu == game.SaveMenu)
            {
                game.Engine.saveManager.Save(path, game.Map);
                c.Context = "play";
                game.currentMenu = null;
                return;
            }

            // must be loading.
            if(!File.Exists(path))
            {
                return;
            }

            try
            {
                game.Engine.saveManager.Load(path, game.Map);

            }
            catch (MapLoadException e)
            {
                game.errorSplash = new ErrorSplash("Crystalrium couldn't load the specified save file.\nReason: " + e.Message);
            }

            c.Context = "play";
            game.currentMenu = null;
        }

        private void SwitchRuleset(int i)
        {
            List<Ruleset> rulesets = game.Engine.Rulesets;
            if (rulesets.Count > i)
            {

                game.CurrentRuleset = rulesets[i];
                game.Map.Ruleset = game.CurrentRuleset;


                CurrentType = game.CurrentRuleset.AgentTypes[0];
                c.Context = "play";
            }
        }

        private void SwitchAgent(int i)
        {
            if(i<game.CurrentRuleset.AgentTypes.Count)
            {
                CurrentType = game.CurrentRuleset.AgentTypes[i];   
            }
        }

        // returns the position of the mouse in tilespace relative to the maingame.view.
        internal Point GetMousePos()
        {
            Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);

            Vector2 clickCoords = game.view.Camera.PixelToTileCoords(pixelCoords);

            clickCoords.Floor();
            return clickCoords.ToPoint();

        }

    }
}
