using CrystalCore.Input;
using CrystalCore.Model.Rulesets;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model.Communication;
using CrystalCore.View.Configs;
using CrystalCore.Model;
using CrystalCore.Model.Grids;

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

      

       

        


        private void SetupController()
        {
            c.Context = "play";

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


            // grow the game.Grid!
            c.addAction("grow up", () => game.Grid.ExpandGrid(Direction.up));
            new Keybind(c, Keystate.OnPress, "grow up", "play", Button.U);
            c.addAction("grow down", () => game.Grid.ExpandGrid(Direction.down));
            new Keybind(c, Keystate.OnPress, "grow down", "play", Button.J);
            c.addAction("grow left", () => game.Grid.ExpandGrid(Direction.left));
            new Keybind(c, Keystate.OnPress, "grow left", "play", Button.H);
            c.addAction("grow right", () => game.Grid.ExpandGrid(Direction.right));
            new Keybind(c, Keystate.OnPress, "grow right", "play", Button.K);

            c.addAction("toggle debug ports", () => game.view.DoDebugPortRendering = !game.view.DoDebugPortRendering);
            new Keybind(c, Keystate.OnPress, "toggle debug ports", "play", Button.O);


            c.addAction("place agent", () =>
            {

                Point clickCoords = GetMousePos();
                Agent toRemove = null;

                // remove all agents on this tile (there should only be one once things are working properly)
                while (true)
                {

                    toRemove = game.Grid.getAgentAtPos(clickCoords);
                    if (toRemove == null)
                    {
                        break;
                    }

                    toRemove.Destroy();
                }

                if (CurrentType.isValidLocation(game.Grid, clickCoords, Rotation))
                {
                    CurrentType.createAgent(game.Grid, clickCoords, Rotation);
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

                    toRemove = game.Grid.getAgentAtPos(clickCoords);
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

                Agent a = game.Grid.getAgentAtPos(clickCoords);
                if (a == null)
                {
                    Rotation = Rotation.Rotate(RotationalDirection.clockwise);
                    return;
                }


                a.Rotate(RotationalDirection.clockwise);
                Rotation = a.Facing;


            });
            new Keybind(c, Keystate.OnPress, "rotate", "play", Button.R);


            c.addAction("pipette", () =>
            {
                Point clickCoords = GetMousePos();

                Agent a = game.Grid.getAgentAtPos(clickCoords);
                if (a == null)
                {
                    return;
                }


                CurrentType = a.Type;
                Rotation = a.Facing;


            });
            new Keybind(c, Keystate.OnPress, "pipette", "play", Button.Tab);


            c.addAction("start pan", () =>
            {
                Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);


                panOrigin = pixelCoords;
                panPos = game.view.Camera.Position;


            });
            new Keybind(c, Keystate.OnPress, "start pan", "play", Button.MouseMiddle) { DisableOnSuperset = false };


            c.addAction("pan", () =>
            {


                Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);
                Vector2 mousePos = game.view.Camera.PixelToTileCoords(pixelCoords);
                Vector2 originPos = game.view.Camera.PixelToTileCoords(panOrigin);

                game.view.Camera.Position = panPos + (originPos - mousePos);




            });
            new Keybind(c, Keystate.Down, "pan", "play", Button.MouseMiddle) { DisableOnSuperset = false };


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


            c.addAction("swap context", () =>
            {
                if (c.Context == "play")
                {
                    c.Context = "menu";

                }
                else
                {
                    c.Context = "play";
                }

                // triggers a reset.
                //game.Grid.Ruleset = game.CurrentRuleset;
                //CurrentType = game.CurrentRuleset.AgentTypes[0];

            });
            new Keybind(c, Keystate.OnPress, "swap context", Button.P);

            c.addAction("ruleset 1", () => SwitchRuleset(0) );
            new Keybind(c, Keystate.OnPress, "ruleset 1","menu", Button.D1);

            c.addAction("ruleset 2", () => SwitchRuleset(1));
            new Keybind(c, Keystate.OnPress, "ruleset 2", "menu", Button.D2);

            c.addAction("ruleset 3", () => SwitchRuleset(2));
            new Keybind(c, Keystate.OnPress, "ruleset 3", "menu", Button.D3);

            c.addAction("ruleset 4", () => SwitchRuleset(3));
            new Keybind(c, Keystate.OnPress, "ruleset 4", "menu", Button.D4);

            c.addAction("ruleset 5", () => SwitchRuleset(4));
            new Keybind(c, Keystate.OnPress, "ruleset 5", "menu", Button.D5);

            c.addAction("ruleset 6", () => SwitchRuleset(5));
            new Keybind(c, Keystate.OnPress, "ruleset 6", "menu", Button.D6);

            c.addAction("ruleset 7", () => SwitchRuleset(6));
            new Keybind(c, Keystate.OnPress, "ruleset 7", "menu", Button.D7);

            c.addAction("ruleset 8", () => SwitchRuleset(7));
            new Keybind(c, Keystate.OnPress, "ruleset 8", "menu", Button.D8);

            c.addAction("ruleset 9", () => SwitchRuleset(8));
            new Keybind(c, Keystate.OnPress, "ruleset 9", "menu", Button.D9);

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
                if(sim.TargetStepsPS>10)
                {
                    sim.TargetStepsPS -= 10;
                }

            });
            new Keybind(c, Keystate.OnPress, "sim slower", "play", Button.LeftControl);

        }

        private void SwitchRuleset(int i)
        {
          
            List<Ruleset> rulesets = game.Engine.Rulesets;
            if(rulesets.Count>i)
            {
                if (rulesets[i] == game.CurrentRuleset)
                {
                    game.Grid.Reset();
                }
                else
                {
                    game.CurrentRuleset = rulesets[i];
                    game.Grid.Ruleset = game.CurrentRuleset;
                }

                CurrentType = game.CurrentRuleset.AgentTypes[0];
                c.Context = "play";
            }
            // do nothing if it's not valid.
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
