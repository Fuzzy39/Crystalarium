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

        // used when panning
        private Point panOrigin = new Point();
        private Vector2 panPos = new Vector2();


        internal Actions( Controller c, Crystalarium game)
        {
            
            this.c = c;
            this.game = game;

            Rotation = Direction.up;

            SetupController();
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
                List<AgentType> types = game.ruleset.AgentTypes;

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
                List<AgentType> types = game.ruleset.AgentTypes;

                int i = types.IndexOf(CurrentType);

                i--;
                if (i < 0)
                {
                    i = types.Count - 1;
                }

                CurrentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "prev agent", Button.Q);
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
