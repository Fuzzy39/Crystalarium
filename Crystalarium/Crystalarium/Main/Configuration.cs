using CrystalCore.Model.Communication;
using CrystalCore.Model.Rulesets;
using CrystalCore.Model.Rulesets.Transformations;
using CrystalCore.Util;
using CrystalCore.View.Configs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Main
{
    /// <summary>
    /// This class sets up the configuration options for Crystalarium's engine.
    /// Namely, it defines rulesets and skinsets.
    /// Eventually, the plan is to read these configuration settings from xml files or similar.
    /// this class will be made of ugly, somewhat repetive code that's just setting parameters and initializing objects.
    /// </summary>
    internal class Configuration
    {

        private CrystalGame game; // The game.

        internal Ruleset BeamRules { get; private set; }
        internal Ruleset TouchRules { get; private set; }

        internal SkinSet DefaultSkin { get; private set; }
        internal SkinSet MiniMapSkin { get; private set; }


        internal Configuration(CrystalGame game)
        {
            this.game = game;
            CreateRulesets();
            CreateDefaultSkins();
            CreateMinimapSkins();

            // let's get this show on the road!
            game.Engine.Initialize();

        }


        private void CreateRulesets()
        {

            // create a ruleset
            BeamRules = game.Engine.addRuleset("Beams");
            BeamRules.PortChannelMode = PortChannelMode.halfDuplex;
            BeamRules.SignalType = SignalType.Beam;

            // define BeamRules
            AgentType t;

            // setup agent types.

            t=BeamRules.CreateType("small", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, true, new PortIdentifier(0, CompassPoint.north)));


            BeamRules.CreateType("flat", new Point(2, 1));
            BeamRules.CreateType("tall", new Point(1, 2));
            BeamRules.CreateType("big", new Point(2, 2));


            game.CurrentRuleset = BeamRules;
         

             // setup TouchRules

            TouchRules = game.Engine.addRuleset("Touch");

            TouchRules.PortChannelMode = PortChannelMode.fullDuplex;
            TouchRules.SignalType = SignalType.Beam;
            TouchRules.BeamMaxLength = 1;
            TouchRules.DiagonalSignalsAllowed = true;


            TouchRules.CreateType("bright", new Point(1, 1));
            TouchRules.CreateType("dark", new Point(1, 1));





        }


        private void CreateDefaultSkins()
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
            AgentViewConfig conf = new AgentViewConfig(baseConfig, BeamRules.GetAgentType("small"));
            conf.Color = Color.White;
            conf.DefaultTexture = Textures.emitter;
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

        private void CreateMinimapSkins()
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


    }

}
