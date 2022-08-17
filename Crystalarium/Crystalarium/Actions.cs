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

        internal SkinSet DefaultSkin { get; private set; }
        internal SkinSet MiniMapSkin { get; private set; }

        // used when panning
        private Point panOrigin = new Point();
        private Vector2 panPos = new Vector2();


        internal Actions( Controller c, Crystalarium game)
        {
            
            this.c = c;
            this.game = game;

            Rotation = Direction.up;

            SetupRulesets();
            SetupSkin();
            setupMiniMapSkin();
            SetupController();
        }

        private void SetupRulesets()
        {

            // create a ruleset
            BeamRules = game.Engine.addRuleset("Beams");
            BeamRules.PortChannelMode = PortChannelMode.fullDuplex;
            BeamRules.SignalType = SignalType.Beam;

            // define BeamRules
            AgentType t;

            // setup agent types.

            CurrentType = BeamRules.CreateType("small", new Point(1, 1));
            BeamRules.CreateType("flat", new Point(2, 1));
            BeamRules.CreateType("tall", new Point(1, 2));
            BeamRules.CreateType("big", new Point(2, 2));
        

            CurrentRuleset = BeamRules;


            // setup TouchRules

            TouchRules = game.Engine.addRuleset("Touch");

            TouchRules.PortChannelMode = PortChannelMode.fullDuplex;
            TouchRules.SignalType = SignalType.Beam;
            TouchRules.BeamMaxLength = 1;
            TouchRules.DiagonalSignalsAllowed = true;


            TouchRules.CreateType("bright", new Point(1, 1));
            TouchRules.CreateType("dark", new Point(1, 1));
          

           


        }

        private void SetupSkin()
        {
            // setup SkinSet.
            DefaultSkin = game.Engine.addSkinSet("Default");
            DefaultSkin.ViewCastOverlay = Textures.pixel;


            // set the default settings for agent rendering.
            AgentViewConfig baseConfig = new AgentViewConfig(null);
            baseConfig.Background = Textures.pixel;
            baseConfig.BackgroundColor = new Color(50, 50, 50);
            baseConfig.DefaultTexture = Textures.sampleAgent;
            baseConfig.Color = Color.Magenta;
            baseConfig.Shrinkage = .05f;


            // Beams skin
            Skin beams = new Skin(BeamRules, DefaultSkin);
            beams.GridViewBG = Textures.viewboxBG;

            // small
            AgentViewConfig conf = new AgentViewConfig( baseConfig, BeamRules.GetAgentType("small"));
            conf.Color = Color.Crimson;
            beams.AgentConfigs.Add(conf);

            // flat
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("flat"));
            conf.Color = Color.Gold;
            beams.AgentConfigs.Add(conf);

            // tall
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("tall"));
            conf.Color = Color.LimeGreen;
            beams.AgentConfigs.Add(conf);

            // big 
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("big"));
            conf.Color = Color.DodgerBlue;
            beams.AgentConfigs.Add(conf);

            // beams
            beams.BeamConfig.BeamTexture = Textures.pixel;
            beams.BeamConfig.Color = new Color(230, 230, 150);

            // chunks
            beams.ChunkConfig.ChunkBackground = Textures.chunkGrid;


            // touch skin
            Skin touch = new Skin(TouchRules, DefaultSkin);
            touch.GridViewBG = Textures.viewboxBG;

            // bright
            conf = new AgentViewConfig(baseConfig, TouchRules.GetAgentType("bright"));
            conf.Color = Color.White;
            touch.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, TouchRules.GetAgentType("dark"));
            conf.Color = Color.DimGray;
            touch.AgentConfigs.Add(conf);


            // chunks
            touch.ChunkConfig.ChunkBackground = Textures.chunkGrid;


          
       


        }

        private void setupMiniMapSkin()
        {
            // #### Minimap Skin ###
            MiniMapSkin = game.Engine.addSkinSet("Minimap");
            MiniMapSkin.ViewCastOverlay = Textures.pixel;

            // defaults for agents under this skin.
            AgentViewConfig baseConfig = new AgentViewConfig(null);
            baseConfig.DefaultTexture = Textures.pixel;
            baseConfig.Color = Color.Magenta;

            // Beams skin
            Skin beams = new Skin(BeamRules, MiniMapSkin);
            beams.GridViewBG = Textures.viewboxBG;

            AgentViewConfig conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("small"));
            conf.Color = Color.Crimson;
            beams.AgentConfigs.Add(conf);

            // flat
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("flat"));
            conf.Color = Color.Gold;
            beams.AgentConfigs.Add(conf);

            // tall
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("tall"));
            conf.Color = Color.LimeGreen;
            beams.AgentConfigs.Add(conf);

            // big 
            conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("big"));
            conf.Color = Color.DodgerBlue;
            beams.AgentConfigs.Add(conf);

            // beams
            beams.BeamConfig.BeamTexture = Textures.pixel;
            beams.BeamConfig.Color = new Color(230, 230, 150);

            // chunks
            beams.ChunkConfig.ChunkBackground = Textures.pixel;
            beams.ChunkConfig.DoCheckerBoardColoring = true;
            beams.ChunkConfig.BackgroundColor = new Color(50, 50, 150);
            beams.ChunkConfig.OriginChunkColor = new Color(150, 50, 50);


            // touch skin
            Skin touch = new Skin(TouchRules, MiniMapSkin);
            touch.GridViewBG = Textures.viewboxBG;

            // bright
            conf = new AgentViewConfig(baseConfig, TouchRules.GetAgentType("bright"));
            conf.Color = Color.White;
            touch.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, TouchRules.GetAgentType("dark"));
            conf.Color = Color.DimGray;
            touch.AgentConfigs.Add(conf);


            // chunks
            touch.ChunkConfig = new ChunkViewConfig(beams.ChunkConfig);
        

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
