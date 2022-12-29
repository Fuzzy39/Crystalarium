﻿    using CrystalCore.Model.Interface;
using CrystalCore.Model.Language;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
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
            PortID up = new PortID(0, CompassPoint.north);
            PortID down = new PortID(0, CompassPoint.south);
            PortID left = new PortID(0, CompassPoint.west);
            PortID right = new PortID(0, CompassPoint.east);


            // create a ruleset
            CrystalRules = game.Engine.addRuleset("Crystalarium");
           
                
            // define SignalRules
            AgentType t;
            TransformationRule tr;
            Operator greaterThan = new Operator(OperatorType.GreaterThan);
            Operator equals = new Operator(OperatorType.EqualTo);
            Operator xor = new Operator(OperatorType.Xor);
            Operator and = new Operator(OperatorType.And);
            Operator or = new Operator(OperatorType.Or);

            // helpful things?
            Condition isReceiving = new Condition(new ThresholdOperand(1), greaterThan, Zero());
            Condition twoOrMoreSignals = new Condition(new ThresholdOperand(1), greaterThan, new IntOperand(1));
            Condition lessThanTwoSignals = new Condition(new ThresholdOperand(1), new Operator(OperatorType.LessThan), new IntOperand(2));

            // setup agent types.

            // ############### STOPPER #####################
            CrystalRules.CreateType("stopper", new Point(1, 1));

            // ############### Emitter #####################
            t = CrystalRules.CreateType("emitter", new Point(1, 1));

            t.Rules.Add(new TransformationRule());
            // condition: active ports > 0
            t.Rules[0].Requirements = null;
            // transmit on all sides
            t.Rules[0].Transformations.Add(new SignalTransformation(1, up));

            // ############### PRISM #####################
            t = CrystalRules.CreateType("prism", new Point(1, 1));


            // top rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.up);
       
            tr.Transformations.Add(new SignalTransformation(1, down, left, right));


            // right rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.right);
           
            tr.Transformations.Add(new SignalTransformation(1, down, left, up));



            // bottom rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.down);

            tr.Transformations.Add(new SignalTransformation(1, right, left, up));


            // left rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.left);

            tr.Transformations.Add(new SignalTransformation(1, down, right, up));




            //############### MIRROR #####################
            t = CrystalRules.CreateType("mirror", new Point(1, 1));
            // up right

            // shaped like \
            tr = new TransformationRule();
            t.Rules.Add(tr);
            tr.Requirements = new Condition(new PortValueOperand(left), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, down));

            tr = new TransformationRule();
            t.Rules.Add(tr);
            tr.Requirements = new Condition(new PortValueOperand(down), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, left));

            tr = new TransformationRule();
            t.Rules.Add(tr);
            tr.Requirements = new Condition(new PortValueOperand(up), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, right));

            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = new Condition(new PortValueOperand(right), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, up));

            //############### LUMINAL GATE #####################
            t = CrystalRules.CreateType("luminal gate", new Point(1, 1));


            tr = new TransformationRule();
            t.Rules.Add(tr);

           

            // down
            tr.Requirements = new Condition
            (
                new Condition
                (
                    new Condition(new PortValueOperand(left), greaterThan, Zero()),
                    xor,
                    new Condition(new PortValueOperand(right), greaterThan, Zero())
                ),
                and,
                new Condition(new PortValueOperand(up), greaterThan, Zero())

            );

            // transmit 
            tr.Transformations.Add(new SignalTransformation(1,  down));

            // Up
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = new Condition
            (
                new Condition
                (
                    new Condition(new PortValueOperand(left), greaterThan, Zero()),
                    xor,
                    new Condition(new PortValueOperand(right), greaterThan, Zero())
                ),
                and,
                new Condition(new PortValueOperand(down), greaterThan, Zero())
                
            );

            // transmit 
            tr.Transformations.Add(new SignalTransformation(1, up));


            // ############# CHANNEL ####################
            t = CrystalRules.CreateType("channel", new Point(1, 1));
            

            // if we are reciving a particular signal, follow through.
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = new Condition(new PortValueOperand(up), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, down));


            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = new Condition(new PortValueOperand(down), greaterThan, Zero());
            tr.Transformations.Add(new SignalTransformation(1, up));





            game.CurrentRuleset = CrystalRules;




            // setup TouchRules

            BasicRules = game.Engine.addRuleset("Minimal");

            // ###### NOT GATE #######
            t = BasicRules.CreateType("not gate", new Point(1, 1));



            t.Rules.Add(new TransformationRule());
            // not gate
            // Condition (left>0)||(right>0)||(down>0)
            t.Rules[0].Requirements = new Condition
            (
                new Condition
                (
                    new Condition(new PortValueOperand(left), equals, Zero()),
                    and,
                    new Condition(new PortValueOperand(right), equals, Zero())
                ),
                and,
                new Condition(new PortValueOperand(down), equals, Zero())
            );


            // transmit on all sides
            t.Rules[0].Transformations.Add(new SignalTransformation(1,  up));

          

            // ########### STOPPER ##############
            BasicRules.CreateType("stopper", new Point(1, 1));

            // ############ SPLITTER #################
            t = BasicRules.CreateType("splitter", new Point(1, 1));

            // top rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.up);

            tr.Transformations.Add(new SignalTransformation(1, down, left, right));


            // right rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.right);

            tr.Transformations.Add(new SignalTransformation(1, down, left, up));



            // bottom rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.down);

            tr.Transformations.Add(new SignalTransformation(1, right, left, up));


            // left rule
            tr = new TransformationRule();
            t.Rules.Add(tr);

            tr.Requirements = ReceivingOnSide(Direction.left);

            tr.Transformations.Add(new SignalTransformation(1, down, right, up));



            // setup filum
            /* FilumRules = game.Engine.addRuleset("Filum");

             FilumRules.PortChannelMode = PortChannelMode.fullDuplex;
             FilumRules.SignalType = SignalType.Signal;
             FilumRules.SignalMaxLength = 1;
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

            WireRules.RotateLock = true;
            WireRules.SignalMaxLength = 1;
            WireRules.DiagonalSignalsAllowed = true;

            // Wire
            t = WireRules.CreateType("wire", new Point(1, 1));
            AgentType wire = t;
            t.Rules.Add(new TransformationRule());
            // condition: active ports > 0
            t.Rules[0].Requirements = new Condition
                (
                    new Condition(new ThresholdOperand(1), greaterThan, Zero()),
                    and,    
                    new Condition(new ThresholdOperand(1), new Operator(OperatorType.LessThan), new IntOperand(3))
                );


            // Electron tail
            t = WireRules.CreateType("electron tail", new Point(1, 1));
            AgentType electronTail = t;
            t.Rules.Add(new TransformationRule());
            t.Rules[0].Requirements = null;
       

            // Electron Head
            t = WireRules.CreateType("electron head", new Point(1, 1));
            AgentType electronHead = t;
            t.DefaultState.Transformations.Add(new SignalTransformation(1, up, down, left, right,
              new PortID(0, CompassPoint.northwest), new PortID(0, CompassPoint.northeast),
              new PortID(0, CompassPoint.southwest), new PortID(0, CompassPoint.southeast)));

            t.Rules.Add(new TransformationRule());
            t.Rules[0].Requirements = null;
         

            // describe everything's mutations
            wire.Rules[0].Transformations.Add(new MutateTransformation(electronHead));
            electronTail.Rules[0].Transformations.Add(new MutateTransformation(wire));
            electronHead.Rules[0].Transformations.Add(new MutateTransformation(electronTail));

        }

        private IntOperand Zero()
        {
            return new IntOperand(0);
        }

        private Condition ReceivingOnSide(Direction d)
        {
            return new Condition(new PortValueOperand(new PortID(0, d.ToCompassPoint())), new Operator(OperatorType.GreaterThan), Zero());
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


            // Signals skin
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
            beams.SignalConfig.SignalTexture = Textures.pixel;
            beams.SignalConfig.Color = new Color(230, 230, 150);

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
            basic.SignalConfig.SignalTexture = Textures.pixel;
            basic.SignalConfig.Color = new Color(230, 230, 150);

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

            // Signals skin
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
            beams.SignalConfig.SignalTexture = Textures.pixel;
            beams.SignalConfig.Color = new Color(230, 230, 150);

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
            basic.SignalConfig.SignalTexture = Textures.pixel;
            basic.SignalConfig.Color = new Color(230, 230, 150);

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
