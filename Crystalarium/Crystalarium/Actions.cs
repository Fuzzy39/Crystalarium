using CrystalCore.Input;
using CrystalCore.Rulesets;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.View.AgentRender;
using CrystalCore.Model.Communication;

namespace Crystalarium
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

        private Crystalarium game; // The game.

      

        internal Direction Rotation { get; private set; } // the direction that any agents placed down will be facing.

      

        internal AgentType CurrentType { get; set; } // the agent type selected to place.
       
        internal Ruleset CurrentRuleset { get; private set; }
        internal Ruleset BeamRules { get; private set; }
        internal Ruleset TouchRules { get; private set; }

        // used when panning
        private Point panOrigin = new Point();
        private Vector2 panPos = new Vector2();


        internal Actions( Controller c, Crystalarium game)
        {
            
            this.c = c;
            this.game = game;

            Rotation = Direction.up;

            SetupRulesets();
            SetupController();
        }

        private void SetupRulesets()
        {

            // create a ruleset
            BeamRules = new Ruleset("Beams");
            BeamRules.PortChannelMode = PortChannelMode.fullDuplex;
            BeamRules.SignalType = SignalType.Beam;

            // define BeamRules
            AgentType t;

            // set the default settings for agent rendering.
            AgentViewTemplate baseConfig = new AgentViewTemplate();
            baseConfig.AgentBackground = Textures.pixel;
            baseConfig.BackgroundColor = new Color(50, 50, 50);
            baseConfig.DefaultTexture = Textures.sampleAgent;
            baseConfig.Color = Color.Magenta;
            baseConfig.Shrinkage = .05f;

            // setup agent types.


            t = BeamRules.CreateType("small", new Point(1, 1));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.Crimson;

            CurrentType = t;

            t = BeamRules.CreateType("flat", new Point(2, 1));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.Gold;


            t = BeamRules.CreateType("tall", new Point(1, 2));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.LimeGreen;

            t = BeamRules.CreateType("big", new Point(2, 2));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.DodgerBlue;

            BeamRules.BeamRenderConfig.BeamTexture = Textures.pixel;
            BeamRules.BeamRenderConfig.Color = new Color(230, 230, 150);

            CurrentRuleset = BeamRules;


            // setup TouchRules

            TouchRules = new Ruleset("Touch");

            TouchRules.PortChannelMode = PortChannelMode.fullDuplex;
            TouchRules.SignalType = SignalType.Beam;
            TouchRules.BeamMaxLength = 1;
            TouchRules.DiagonalSignalsAllowed = true;


            t = TouchRules.CreateType("bright", new Point(1, 1));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.White;


            t = TouchRules.CreateType("dark", new Point(1, 1));
            t.RenderConfig = baseConfig;
            t.RenderConfig.Color = Color.DimGray;

           ;


        }


        private void SetupController()
        {

            // test code.
            float camSpeed = 1.2f;

            // camera up
            c.addAction("up", () => game.view.Camera.AddVelocity(camSpeed, Direction.up));
            new Keybind(c, Keystate.Down, "up", Button.W);
            new Keybind(c, Keystate.Down, "up", Button.Up);


            // camera down
            c.addAction("down", () =>game.view.Camera.AddVelocity(camSpeed, Direction.down));
            new Keybind(c, Keystate.Down, "down", Button.S);
            new Keybind(c, Keystate.Down, "down", Button.Down);

            // camera left
            c.addAction("left", () =>game.view.Camera.AddVelocity(camSpeed, Direction.left));
            new Keybind(c, Keystate.Down, "left", Button.A);
            new Keybind(c, Keystate.Down, "left", Button.Left);

            // camera right
            c.addAction("right", () =>game.view.Camera.AddVelocity(camSpeed, Direction.right));
            new Keybind(c, Keystate.Down, "right", Button.D);
            new Keybind(c, Keystate.Down, "right", Button.Right);


            // grow the game.Grid!
            c.addAction("grow up", () => game.Grid.ExpandGrid(Direction.up));
            new Keybind(c, Keystate.OnPress, "grow up", Button.U);
            c.addAction("grow down", () => game.Grid.ExpandGrid(Direction.down));
            new Keybind(c, Keystate.OnPress, "grow down", Button.J);
            c.addAction("grow left", () => game.Grid.ExpandGrid(Direction.left));
            new Keybind(c, Keystate.OnPress, "grow left", Button.H);
            c.addAction("grow right", () => game.Grid.ExpandGrid(Direction.right));
            new Keybind(c, Keystate.OnPress, "grow right", Button.K);

            c.addAction("toggle debug ports", () => game.view.DoDebugPortRendering = !game.view.DoDebugPortRendering);
            new Keybind(c, Keystate.OnPress, "toggle debug ports", Button.O);


            c.addAction("place agent", () =>
            {
                Point clickCoords = GetMousePos();

                if (CurrentType.isValidLocation(game.Grid, clickCoords, Rotation))
                {
                    CurrentType.createAgent(game.Grid, clickCoords, Rotation);
                }


            });
            new Keybind(c, Keystate.Down, "place agent", Button.MouseLeft);


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
            new Keybind(c, Keystate.Down, "remove agent", Button.MouseRight);


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
            new Keybind(c, Keystate.OnPress, "rotate", Button.R);



            c.addAction("start pan", () =>
            {
                Point pixelCoords =game.view.LocalizeCoords(Mouse.GetState().Position);


                panOrigin = pixelCoords;
                panPos =game.view.Camera.Position;


            });
            new Keybind(c, Keystate.OnPress, "start pan", Button.MouseMiddle) { DisableOnSuperset = false };


            c.addAction("pan", () =>
            {


                Point pixelCoords =game.view.LocalizeCoords(Mouse.GetState().Position);
                Vector2 mousePos =game.view.Camera.PixelToTileCoords(pixelCoords);
                Vector2 originPos =game.view.Camera.PixelToTileCoords(panOrigin);

               game.view.Camera.Position = panPos + (originPos - mousePos);




            });
            new Keybind(c, Keystate.Down, "pan", Button.MouseMiddle) { DisableOnSuperset = false };


            c.addAction("next agent", () =>
            {
                List<AgentType> types = CurrentRuleset.AgentTypes;

                int i = types.IndexOf(CurrentType);

                i++;
                if (i >= types.Count)
                {
                    i = 0;
                }

                CurrentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "next agent", Button.E);

            c.addAction("prev agent", () =>
            {
                List<AgentType> types = CurrentRuleset.AgentTypes;
                int i = types.IndexOf(CurrentType);

                i--;
                if (i < 0)
                {
                    i = types.Count - 1;
                }

                CurrentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "prev agent", Button.Q);


            c.addAction("swap ruleset", () =>
            { 
                if(CurrentRuleset == BeamRules)
                {
                    CurrentRuleset = TouchRules;
                }
                else
                {
                    CurrentRuleset = BeamRules;
                }

                // triggers a reset.
                game.Grid.Ruleset = CurrentRuleset;
                CurrentType = CurrentRuleset.AgentTypes[0];
                
            });
            new Keybind(c, Keystate.OnPress, "swap ruleset", Button.P);

        }


        // returns the position of the mouse in tilespace relative to the maingame.view.
        internal Point GetMousePos()
        {
            Point pixelCoords =game.view.LocalizeCoords(Mouse.GetState().Position);

            Vector2 clickCoords =game.view.Camera.PixelToTileCoords(pixelCoords);

            clickCoords.Floor();
            return clickCoords.ToPoint();

        }

    }
}
