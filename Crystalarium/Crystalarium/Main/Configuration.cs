using CrystalCore.Model.Communication;
using CrystalCore.Model.Rulesets;
using CrystalCore.Model.Rulesets.Conditions;
using CrystalCore.Model.Rulesets.Operands;
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

        internal Ruleset CrystalRules { get; private set; }
        internal Ruleset BasicRules { get; private set; }
        internal Ruleset FilumRules { get; private set; }
        internal Ruleset WireRules { get; private set; } // wire rules


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
            PortIdentifier up = new PortIdentifier(0, CompassPoint.north);
            PortIdentifier down = new PortIdentifier(0, CompassPoint.south);
            PortIdentifier left = new PortIdentifier(0, CompassPoint.west);
            PortIdentifier right = new PortIdentifier(0, CompassPoint.east);


            // create a ruleset
            CrystalRules = game.Engine.addRuleset("Crystalarium");
            CrystalRules.PortChannelMode = PortChannelMode.halfDuplex;
            CrystalRules.SignalType = SignalType.Beam;

            // define BeamRules
            AgentType t;
            Operator greaterThan = new Operator(OperatorType.GreaterThan);
            Operator xor = new Operator(OperatorType.Xor);
            Operator and = new Operator(OperatorType.And);
            Operator or = new Operator(OperatorType.Or);

            // setup agent types.

            // ############### STOPPER #####################
            CrystalRules.CreateType("stopper", new Point(1, 1));

            // ############### Emitter #####################
            t = CrystalRules.CreateType("emitter", new Point(1, 1));
        
            t.States.Add(new AgentState());
            // condition: active ports > 0
            t.States[0].Requirements = null;
            // transmit on all sides
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, up));

            // ############### PRISM #####################
            t = CrystalRules.CreateType("prism", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, false, up, down, left, right));

            t.States.Add(new AgentState());
            // condition: active ports > 0
            t.States[0].Requirements = new Condition( new ThresholdOperand( 1), new Operator(OperatorType.GreaterThan), new IntOperand(0));
            // transmit on all sides
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, up, down, left, right));


            //############### MIRROR #####################
            t = CrystalRules.CreateType("mirror", new Point(1, 1));
            // up right
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, false, up, down, left, right));

            t.States.Add(new AgentState());
            // condition: ((left>0) || (down>0)) & ( (up>0) || (right>0) )
            t.States[0].Requirements = new Condition
            (
                new Condition
                (
                    new Condition( new PortValueOperand( left), greaterThan, Zero()),
                    or,
                    new Condition( new PortValueOperand( down), greaterThan, Zero())
                ),
                and,
                new Condition
                (
                    new Condition( new PortValueOperand( up), greaterThan, Zero()),
                    or,
                    new Condition( new PortValueOperand( right), greaterThan, Zero())
                )
             );

            // transmit 
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, up, down, left, right));


            t.States.Add(new AgentState());
            t.States[1].Requirements = new Condition
            (
                new Condition( new PortValueOperand( left), greaterThan, Zero()),
                or,
                new Condition( new PortValueOperand( down), greaterThan, Zero())
            );

            // transmit 
            t.States[1].Transformations.Add(new SignalTransformation(t, 1, true, down, left));
            t.States[1].Transformations.Add(new SignalTransformation(t, 1, false, up, right));


            t.States.Add(new AgentState());
            t.States[2].Requirements = new Condition
            (
                new Condition( new PortValueOperand( right), greaterThan, Zero()),
                or,
                new Condition( new PortValueOperand( up), greaterThan, Zero())
            );

            // transmit 
            t.States[2].Transformations.Add(new SignalTransformation(t, 1, true, right, up));
            t.States[2].Transformations.Add(new SignalTransformation(t, 1, false, down, left));



            //############### LUMINAL GATE #####################
            t = CrystalRules.CreateType("luminal gate", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, false, up, down));

            t.States.Add(new AgentState());

            // condition: ((left>0) ^ (right>0)) & ( (up>0) || (down>0) )
            // why did I make this monstrosity

            t.States[0].Requirements = new Condition
            (
                new Condition
                (
                    new Condition( new PortValueOperand(left), greaterThan, Zero()),
                    xor,
                    new Condition( new PortValueOperand( right), greaterThan, Zero())
                ),
                and,
                new Condition
                (
                    new Condition( new PortValueOperand( up), greaterThan, Zero()),
                    or,
                    new Condition( new PortValueOperand( down), greaterThan, Zero())
                )
            );

            // transmit 
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, up, down));


            // ############# CHANNEL ####################
            t = CrystalRules.CreateType("channel", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, false, up, down));

            t.States.Add(new AgentState());
            // condition: upper port > 0
            t.States[0].Requirements = new Condition( new PortValueOperand( up), new Operator(OperatorType.GreaterThan), new IntOperand(0));
            // transmit below
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, down));
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, false, up));

            t.States.Add(new AgentState());
            // condition: lower port > 0
            t.States[1].Requirements = new Condition( new PortValueOperand( down), new Operator(OperatorType.GreaterThan), new IntOperand(0));
            // transmit above
            t.States[1].Transformations.Add(new SignalTransformation(t, 1, false, down));
            t.States[1].Transformations.Add(new SignalTransformation(t, 1, true, up));


            game.CurrentRuleset = CrystalRules;




            // setup TouchRules

            BasicRules = game.Engine.addRuleset("Minimal");

            BasicRules.PortChannelMode = PortChannelMode.halfDuplex;
            BasicRules.SignalType = SignalType.Beam;

            // ###### NOT GATE #######
            t = BasicRules.CreateType("not gate", new Point(1, 1));

           

            t.States.Add(new AgentState());
            // not gate
            // Condition (left>0)||(right>0)||(down>0)
            t.States[0].Requirements = new Condition
            (
                new Condition
                (
                    new Condition( new PortValueOperand(left), greaterThan, Zero()),
                    or,
                    new Condition( new PortValueOperand(right), greaterThan, Zero())
                ),
                or,
                new Condition( new PortValueOperand(down), greaterThan, Zero())
            );


            // transmit on all sides
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, false, up));

            t.States.Add(new AgentState());
            // condition: active ports > 0
            t.States[1].Requirements = null;
            // transmit on all sides
            t.States[1].Transformations.Add(new SignalTransformation(t, 1, true, up));

            // ########### STOPPER ##############
            BasicRules.CreateType("stopper", new Point(1, 1));

            // ############ SPLITTER #################
            t = BasicRules.CreateType("splitter", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, false, up, down, left, right));

            t.States.Add(new AgentState());
            t.States[0].Requirements = new Condition( new ThresholdOperand( 1), greaterThan, Zero());
            // transmit on all sides
            t.States[0].Transformations.Add(new SignalTransformation(t, 1, true, up, down, left, right));


            // setup filum
            /* FilumRules = game.Engine.addRuleset("Filum");

             FilumRules.PortChannelMode = PortChannelMode.fullDuplex;
             FilumRules.SignalType = SignalType.Beam;
             FilumRules.BeamMaxLength = 1;
             FilumRules.RotateLock = true;

             FilumRules.CreateType("void", new Point(1, 1));
             FilumRules.CreateType("nand", new Point(1, 1));
             FilumRules.CreateType("bridge", new Point(1, 1));
             FilumRules.CreateType("splitter", new Point(1, 1));
             FilumRules.CreateType("wall", new Point(1, 1));
             FilumRules.CreateType("signal", new Point(1, 1));
             FilumRules.CreateType("dying signal", new Point(1, 1));
            */

            // setup wireworld
            WireRules = game.Engine.addRuleset("Wire World");

            WireRules.PortChannelMode = PortChannelMode.fullDuplex;
            WireRules.SignalType = SignalType.Beam;
            WireRules.RotateLock = true;
            WireRules.BeamMaxLength = 1;
            WireRules.DiagonalSignalsAllowed = true;

            t = WireRules.CreateType("wire", new Point(1, 1));
            t.States.Add(new AgentState());
            // condition: active ports > 0
            t.States[0].Requirements = new Condition
                (
                    new Condition( new ThresholdOperand( 1), greaterThan, Zero()),
                    and,
                    new Condition( new ThresholdOperand( 1), new Operator(OperatorType.LessThan), new IntOperand(3))
                );

            // transmit on all sides
            t.States[0].Transformations.Add(new MutateTransformation(t, "electron head") ); 
            //
            t =WireRules.CreateType("electron tail", new Point(1, 1));
            t.States.Add(new AgentState());
            t.States[0].Requirements = null;
            // transmit on all sides
            t.States[0].Transformations.Add(new MutateTransformation(t, "wire"));

       
            t=WireRules.CreateType("electron head", new Point(1, 1));
            t.DefaultState.Transformations.Add(new SignalTransformation(t, 1, true, up, down, left, right,
               new PortIdentifier(0, CompassPoint.northwest), new PortIdentifier(0, CompassPoint.northeast),
               new PortIdentifier(0, CompassPoint.southwest), new PortIdentifier(0, CompassPoint.southeast)));

            t.States.Add(new AgentState());
            t.States[0].Requirements = null;
            // transmit on all sides
            t.States[0].Transformations.Add(new MutateTransformation(t, "electron tail"));
           

        }

        private IntOperand Zero()
        {
            return new IntOperand(0);
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
            baseConfig.Color = Color.White;
            baseConfig.Shrinkage = .05f;


            // Beams skin
            Skin beams = new Skin(CrystalRules, DefaultSkin);
            beams.GridViewBG = Textures.viewboxBG;

            // emitter
            AgentViewConfig conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("emitter"));
            conf.DefaultTexture = Textures.emitter;
            beams.AgentConfigs.Add(conf);

            // stopper
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("stopper"));
            conf.DefaultTexture = Textures.stopper;
            beams.AgentConfigs.Add(conf);

            // prism
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("prism"));
            conf.DefaultTexture = Textures.prism;
            beams.AgentConfigs.Add(conf);

            // mirror
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("mirror"));
            conf.DefaultTexture = Textures.mirror;
            beams.AgentConfigs.Add(conf);

            // luminal gate
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("luminal gate"));
            conf.DefaultTexture = Textures.luminalGate;
            beams.AgentConfigs.Add(conf);

            // channel
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("channel"));
            conf.DefaultTexture = Textures.channel;
            beams.AgentConfigs.Add(conf);

            // beams
            beams.BeamConfig.BeamTexture = Textures.pixel;
            beams.BeamConfig.Color = new Color(230, 230, 150);

            // chunks
            beams.ChunkConfig.ChunkBackground = Textures.chunkGrid;


            // Minimal skin
            Skin basic = new Skin(BasicRules, DefaultSkin);
            basic.GridViewBG = Textures.viewboxBG;

            // bright
            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("not gate"));
            conf.DefaultTexture = Textures.notGate;
            basic.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("stopper"));
            conf.DefaultTexture = Textures.stopper;
            basic.AgentConfigs.Add(conf);

            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("splitter"));
            conf.DefaultTexture = Textures.prism;
            basic.AgentConfigs.Add(conf);

            // beams
            basic.BeamConfig.BeamTexture = Textures.pixel;
            basic.BeamConfig.Color = new Color(230, 230, 150);

            // chunks
            basic.ChunkConfig.ChunkBackground = Textures.chunkGrid;



            // ##### Wire World #####
            Skin wire= new Skin(WireRules, DefaultSkin);
            wire.GridViewBG = Textures.viewboxBG;

            baseConfig = new AgentViewConfig(null);
            baseConfig.DefaultTexture = Textures.pixel;
            baseConfig.Background = Textures.pixel;
            baseConfig.Color = Color.White;

            // bright
            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("wire"));
            conf.Color = new Color(110, 140, 110);
            wire.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("electron head"));
            conf.Color = new Color(0, 255, 0);
            wire.AgentConfigs.Add(conf);

            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("electron tail"));
            conf.Color = new Color(40, 220, 40);
            wire.AgentConfigs.Add(conf);


            // chunks
            wire.ChunkConfig.ChunkBackground = Textures.altChunkGrid;


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
            Skin beams = new Skin(CrystalRules, MiniMapSkin);
            beams.GridViewBG = Textures.viewboxBG;

            // emitter
            AgentViewConfig conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("emitter"));
            conf.Color = new Color(70, 220, 70);
            beams.AgentConfigs.Add(conf);

            //stopper
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("stopper"));
            conf.Color = new Color(180, 180, 180);
            beams.AgentConfigs.Add(conf);

            // prism
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("prism"));
            conf.Color = new Color(50, 100, 200);
            beams.AgentConfigs.Add(conf);

            // mirror
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("mirror"));
            conf.Color = new Color(120, 230, 230);
            beams.AgentConfigs.Add(conf);

            // luminal gate
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("luminal gate"));
            conf.Color = new Color(220, 70, 70);
            beams.AgentConfigs.Add(conf);

            // channel
            conf = new AgentViewConfig(baseConfig, CrystalRules.GetAgentType("channel"));
            conf.Color = new Color(220, 120, 50);
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
            Skin basic = new Skin(BasicRules, MiniMapSkin);
            basic.GridViewBG = Textures.viewboxBG;

            // bright
            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("not gate"));
            conf.Color = new Color(220,70,70);
            basic.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("stopper"));
            conf.Color = new Color(180, 180, 180);
            basic.AgentConfigs.Add(conf);

            // psitter
            conf = new AgentViewConfig(baseConfig, BasicRules.GetAgentType("splitter"));
            conf.Color = new Color(50, 100, 200);
            basic.AgentConfigs.Add(conf);


            // touch
            basic.BeamConfig.BeamTexture = Textures.pixel;
            basic.BeamConfig.Color = new Color(230, 230, 150);

            // chunks
            basic.ChunkConfig = new ChunkViewConfig(beams.ChunkConfig);

            // 
            Skin wire = new Skin(WireRules, MiniMapSkin);
            wire.GridViewBG = Textures.viewboxBG;

            baseConfig = new AgentViewConfig(null);
            baseConfig.DefaultTexture = Textures.pixel;
            baseConfig.Color = Color.White;

            // bright
            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("wire"));
            conf.Color = new Color(110, 140, 110);
            wire.AgentConfigs.Add(conf);

            // dark 
            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("electron head"));
            conf.Color = new Color(0, 255, 0);
            wire.AgentConfigs.Add(conf);

            conf = new AgentViewConfig(baseConfig, WireRules.GetAgentType("electron tail"));
            conf.Color = new Color(40, 220, 40);
            wire.AgentConfigs.Add(conf);


            wire.ChunkConfig = new ChunkViewConfig(beams.ChunkConfig);



        }


    }

}
